using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

public enum GamePhase
{
    gameStart,
    playerDraw,//抽卡阶段
    playerAction,
    enemyDraw,
    enemyAction
}

public class BattleManager : MonoSingleton<BattleManager>
{
    public static BattleManager Instance;
    
    public PlayerData playerData;
    public PlayerData enemyData;//数据

    public List<Card> playerDeckList = new List<Card>();
    public List<Card> enemyDeckList = new List<Card>();//卡组
    
    public GameObject cardPrefab;//卡牌，专门用于战斗的卡牌预制件

    public Transform playerHand;
    public Transform enemyHand;//手牌

    public GameObject[] playerBlocks;
    public GameObject[] enemyBlocks;//格子

    public GameObject playerIcon;
    public GameObject enemyIcon;//头像

    public GamePhase GamePhase = GamePhase.gameStart;

    public UnityEvent phaseChangeEvent = new UnityEvent();//每当回合发生变化则触发

    public int[] summonCountMax = new int[2];//0:player,1:enemy
    private int[] summonCounter = new int[2];//召唤次数
    private GameObject waitingMonster;
    private int waitingPlayer;

    public GameObject arrowPrefab;
    private GameObject arrow;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameStart();
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            waitingMonster = null;
            DestroyArrow();
            CloseBlock();
        }
    }

    public void GameStart()
    {
        //读取数据
        ReadDeck();
        //卡组洗牌
        ShuffletDeck(0);
        ShuffletDeck(1);
        //玩家抽卡5，敌人抽卡5
        DrawCard(0,3);
        DrawCard(1,3);

        NextPhase();
        
        summonCounter = summonCountMax;
    }

    //读取数据，加载玩家卡组于playerDeckList、enemyDeckList
    public void ReadDeck()
    {
        for (int i = 0; i < playerData.playerDeck.Length; i++)
        {
            if (playerData.playerDeck[i] != 0)
            {
                //得到每种卡的数量
                int count = playerData.playerDeck[i];
                for (int j = 0; j < count; j++)
                {
                    //下面的这种是引用类型，不能使用，如果在战斗中改变了会同时改变CardList种数据即原始数据，这是不对的
                    //playerDeckList.Add(playerData.cardStore.cardList[i]); 
                    playerDeckList.Add(playerData.cardStore.CopyCard(i));
                }
            }
        }
        //加载敌人卡组
        for (int i = 0; i < enemyData.playerDeck.Length; i++)
        {
            if (enemyData.playerDeck[i] != 0)
            {
                int count = enemyData.playerDeck[i];
                for (int j = 0; j < count; j++)
                {
                    enemyDeckList.Add(enemyData.cardStore.CopyCard(i));
                }
            }
        }
    }
    
    //洗牌
    public void ShuffletDeck(int _player)//0:player,1:enemy
    {
        List<Card> shuffletDeck = new List<Card>();
        
        if (_player == 0)
        {
            shuffletDeck = playerDeckList;
        }
        else if (_player == 1)
        {
            shuffletDeck = enemyDeckList;
        }

        //洗牌算法
        for (int i = 0; i < shuffletDeck.Count; i++)
        {
            int rad = Random.Range(0, shuffletDeck.Count);
            Card temp = shuffletDeck[i];
            shuffletDeck[i] = shuffletDeck[rad];
            shuffletDeck[rad] = temp;
        }
    }

    //玩家抽牌和敌人抽牌由抽牌 按钮调用
    public void OnPlayerDraw()
    {
        if (GamePhase == GamePhase.playerDraw)
        {
            DrawCard(0,1);
            NextPhase();
        }
    }
    public void OnEnemyDraw()
    {
        if (GamePhase == GamePhase.enemyDraw)
        {
            DrawCard(1,1);
            NextPhase();
        }
    }

    //抽卡:谁，抽几张
    public void DrawCard(int _player, int _count)
    {
        List<Card> drawDeck = new List<Card>();//从哪抽
        Transform hand = transform;//抽到谁手里
        
        if (_player == 0)
        {
            drawDeck = playerDeckList;
            hand = playerHand;
        }
        else if (_player == 1)
        {
            drawDeck = enemyDeckList;
            hand = enemyHand;
        }

        for (int i = 0; i < _count; i++)
        {
            GameObject card = Instantiate(cardPrefab, hand);
            card.GetComponent<CardDisplay>().card = drawDeck[0];
            card.GetComponent<BattleCard>().playerID = _player;
            drawDeck.RemoveAt(0);
        }
    }

    //由回合结束按钮调用
    public void OnClickTurnEnd()
    {
        TurnEnd();
    }
    
    public void TurnEnd()
    {
        if (GamePhase == GamePhase.playerAction || GamePhase == GamePhase.enemyAction)
        {
            NextPhase();
        }
    }

    public void NextPhase()
    {
        //如果到了最后一个enemyaction，则回到第二个playerdraw，而不是第一个：gamestart
        if ((int)GamePhase == System.Enum.GetNames(GamePhase.GetType()).Length - 1)
        {
            GamePhase = GamePhase.playerDraw;
        }
        else
        {
            GamePhase += 1;
        }
        phaseChangeEvent.Invoke();
    }

    /// <summary>
    /// 发出召唤请求
    /// </summary>
    /// <param name="_player">玩家编号</param>
    /// <param name="_monster">怪兽卡</param>
    public void SummonRequst(int _player,GameObject _monster)
    {
        GameObject[] blocks;
        bool hasEmptyBlock = false;
        if (_player == 0 && GamePhase == GamePhase.playerAction)
        {
            blocks = playerBlocks;
        }
        else if(_player == 1 && GamePhase == GamePhase.enemyAction)
        {
            blocks = enemyBlocks;
        }
        else
        {
            return;
        }
        
        if (summonCounter[_player] > 0)
        {
            foreach (var block in blocks)
            {
                //如果是空的则说明格子没有卡
                if (block.GetComponent<Block>().card == null)
                {
                    //等待召唤显示
                    block.GetComponent<Block>().summonBlock.SetActive(true);//高亮显示
                    hasEmptyBlock = true;
                }
            }
        }
        if (hasEmptyBlock)
        {
            waitingMonster = _monster;
            waitingPlayer = _player;
            CreateArrow(_monster.transform,arrowPrefab);
        }
    }

    /// <summary>
    /// 召唤确认 
    /// </summary>
    /// <param name="_block"></param>
    public void SummonConfirm(Transform _block)
    {
        Summon(waitingPlayer,waitingMonster,_block);
        CloseBlock();
        DestroyArrow();
    }

    public void Summon(int _player, GameObject _monster ,Transform _block)
    {
        _monster.transform.SetParent(_block);
        _monster.transform.localPosition = Vector3.zero;
        _monster.GetComponent<BattleCard>().state = BattleCardState.inBlock;
        _block.GetComponent<Block>().card = _monster;
        summonCounter[_player]--;
    }

    public void CreateArrow(Transform _startPoint,GameObject _prefab)
    {
        DestroyArrow();
        arrow = Instantiate(_prefab,GameObject.Find("Canvas").transform);
        arrow.GetComponent<Arrow>().SetStartPoint(new Vector2(_startPoint.position.x,_startPoint.position.y));
    }
    public void DestroyArrow()
    {
        Destroy(arrow);
    }

    public void CloseBlock()
    {
        GameObject[] blocks;
        if (waitingPlayer == 0)
        {
            blocks = playerBlocks;
        }
        else
        {
            blocks = enemyBlocks;
        }
        foreach (var block in blocks)
        {
            block.GetComponent<Block>().summonBlock.SetActive(false);//关闭高亮显示
        }
    }
}
