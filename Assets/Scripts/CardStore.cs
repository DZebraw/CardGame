using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class CardStore : MonoBehaviour
{
    public TextAsset cardData;
    public List<Card> cardList = new List<Card>();

    void Start()
    {
        //LoadCardData();
    }

    //通过表格所给卡牌数据将所有卡牌类型加入到列表cardList中
    public void LoadCardData()
    {
        //按照回车来分割（分割每一行）
        string[] dataRow = cardData.text.Split('\n');
        foreach (var row in dataRow)
        {
            string[] rowArray = row.Split(',');
            
            if (rowArray[0] == "#")
            {
                continue;
            }
            else if (rowArray[0] == "monster")
            {
                //新建怪兽卡
                int id = int.Parse(rowArray[1]);
                string name = rowArray[2];
                int atk = int.Parse(rowArray[3]);
                int health = int.Parse(rowArray[4]);
                MonsterCard monsterCard = new MonsterCard(id, name, atk, health);
                cardList.Add(monsterCard);
            }
            else if (rowArray[0] == "spell")
            {
                //新建魔法卡
                int id = int.Parse(rowArray[1]);
                string name = rowArray[2];
                string effect = rowArray[3];
                SpellCard spellCard = new SpellCard(id, name, effect);
                cardList.Add(spellCard);
            }
        }
    }

    public Card RandomCard()
    {
        Card card = cardList[Random.Range(0, cardList.Count)];
        return card;
    }
}
