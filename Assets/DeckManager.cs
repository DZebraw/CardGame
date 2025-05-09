using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public Transform deckPanel;//卡组显示区域
    public Transform libraryPanel;//玩家卡牌仓库区域
    public GameObject deckPrefab;
    public GameObject cardPrefab;
    public GameObject dataManager;
    private PlayerData playerData;
    private CardStore cardStore;
    //使卡牌对应id号，这样通过id号就可以找到卡牌
    private Dictionary<int, GameObject> libraryDic = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> deckDic = new Dictionary<int, GameObject>();
    
    private IEnumerator Start()
    {
        // 获取组件引用
        playerData = dataManager.GetComponent<PlayerData>();
        cardStore = dataManager.GetComponent<CardStore>();
    
        // 等待一帧确保其他组件的Start方法已执行
        yield return null;
    
        // 现在可以安全地更新UI
        UpdateLibrary();
        UpdateDeck();
    }

    public void UpdateLibrary()
    {
        for (int i = 0; i < playerData.playerCards.Length; i++)
        {
            if (playerData.playerCards[i] > 0)
            {
                CreateCard(i,CardState.Library);
            }
        }
    }
    
    public void UpdateDeck()
    {
        for (int i = 0; i < playerData.playerDeck.Length; i++)
        {
            if (playerData.playerDeck[i] > 0)
            {
                CreateCard(i,CardState.Deck);
            }
        }
    }

    public void UpdateCard(CardState _state, int _id)
    {
        //在仓库中点击和卡组中点击分别处理
        if (_state == CardState.Deck)
        {
            playerData.playerDeck[_id]--;
            playerData.playerCards[_id]++;
            
            if (!deckDic[_id].GetComponent<CardCounter>().SetCounter(-1))
            {
                deckDic.Remove(_id);
            }
            if (libraryDic.ContainsKey(_id))
            {
                libraryDic[_id].GetComponent<CardCounter>().SetCounter(1);
            }
            else
            {
                CreateCard(_id,CardState.Library);
            }

        }
        else if (_state == CardState.Library)
        {
            playerData.playerDeck[_id]++;
            playerData.playerCards[_id]--;
            
            if (deckDic.ContainsKey(_id))
            {
                deckDic[_id].GetComponent<CardCounter>().SetCounter(1);
            }
            else
            {
                CreateCard(_id,CardState.Deck);
            }
            if (!libraryDic[_id].GetComponent<CardCounter>().SetCounter(-1))
            {
                libraryDic.Remove(_id);
            }
        }
    }

    public void CreateCard(int _id,CardState _cardState)
    {
        Transform targetPanel;
        GameObject targetPrefab;
        var refData = playerData.playerCards;
        Dictionary<int, GameObject> targetDic = libraryDic;
        if (_cardState == CardState.Library)
        {
            targetPanel = libraryPanel;
            targetPrefab = cardPrefab;
        }
        else
        {
            targetPanel = deckPanel;
            targetPrefab = deckPrefab;
            refData = playerData.playerDeck;
            targetDic = deckDic;
        }
        GameObject newCard = Instantiate(targetPrefab, targetPanel);
        newCard.GetComponent<CardCounter>().SetCounter(refData[_id]);
        newCard.GetComponent<CardDisplay>().card = cardStore.cardList[_id];
        targetDic.Add(_id, newCard);
    }
}
