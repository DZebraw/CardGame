using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerData : MonoBehaviour
{
    public CardStore cardStore;
    public int playerCoins;
    public int[] playerCards;//玩家仓库卡
    public int[] playerDeck;//玩家卡组卡
    public TextAsset playerData;

    private void Start()
    {
        cardStore.LoadCardData();
        LoadPlayerData();
    }

    public void LoadPlayerData()
    {
        //根据已有卡牌固定数组长度
        playerCards = new int[cardStore.cardList.Count];
        playerDeck = new int[cardStore.cardList.Count];
        
        string[] dataRow = playerData.text.Split('\n');
        foreach (var row in dataRow)
        {
            string[] rowArray = row.Split(',');
            if (rowArray[0] == "#")
            {
                continue;
            }
            else if (rowArray[0] == "coins")
            {
                playerCoins = int.Parse(rowArray[1]);
            }
            else if (rowArray[0] == "card")
            {
                int id = int.Parse(rowArray[1]);
                int num = int.Parse(rowArray[2]);
                //载入玩家数据
                playerCards[id] = num;
            }
            else if (rowArray[0] == "deck")
            {
                int id = int.Parse(rowArray[1]);
                int num = int.Parse(rowArray[2]);
                //载入卡组
                playerDeck[id] = num;
            }
        }
    }

    public void SavePlayerData()
    {
        string path = Application.dataPath + "/Datas/playerData.csv";
        
        List<string> datas = new List<string>();
        datas.Add("coins," + playerCoins.ToString());
        //保存所拥有的卡
        for (int i = 0; i < playerCards.Length; i++)
        {
            //如果对应卡牌数量不为0,添加卡牌信息 
            if (playerCards[i] != 0)
            {
                datas.Add("card," + i.ToString() + "," + playerCards[i].ToString());
            }
        }
        //保存卡组
        for (int i = 0; i < playerDeck.Length; i++)
        {
            if (playerDeck[i] != 0)
            {
                datas.Add("deck," + i.ToString() + "," + playerDeck[i].ToString());
            }
        }
        
        //保存数据
        File.WriteAllLines(path, datas);
        Debug.Log(datas);
    }
}
