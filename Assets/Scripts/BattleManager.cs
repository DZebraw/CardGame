using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GamePhase
{
    gameStart,
    playerDraw,//抽卡阶段
    playerAction,
    enemyDraw,
    enemyAction
}

public class BattleManager : MonoBehaviour
{
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

    //游戏流程
    //开始游戏：加载数据，卡组洗牌，初始手牌
    //回合结束，游戏阶段

    private void Start()
    {
        GameStart();
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

        GamePhase = GamePhase.playerDraw;
    }

    //读取数据
    public void ReadDeck()
    {
        //加载玩家卡组
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

    //玩家抽牌和敌人抽牌由抽牌按钮调用
    public void OnPlayerDraw()
    {
        if (GamePhase == GamePhase.playerDraw)
        {
            DrawCard(0,1);
            GamePhase = GamePhase.playerAction;
        }
    }
    public void OnEnemyDraw()
    {
        if (GamePhase == GamePhase.enemyDraw)
        {
            DrawCard(1,1);
            GamePhase = GamePhase.enemyAction;
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
        if (GamePhase == GamePhase.playerAction)
        {
            GamePhase = GamePhase.enemyDraw;
        }
        else if (GamePhase == GamePhase.enemyAction)
        {
            GamePhase = GamePhase.playerDraw;
        }
    }
}
