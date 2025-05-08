using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class OpenPackage : MonoBehaviour
{
    public GameObject cardPrefab;
    private CardStore cardStore;
    public GameObject cardPool;
    private List<GameObject> cards = new List<GameObject>();
    public PlayerData playerData;

    void Start()
    {
        cardStore = GetComponent<CardStore>();
    }

    void Update()
    {
        
    }

    public void OnClickOpen()
    {
        if (playerData.playerCoins < 2)
        {
            return;
        }
        else
        {
            playerData.playerCoins -= 2;
        }
        
        ClearPool();
        for (int i = 0; i < 5; i++)
        {
            GameObject newCard = Instantiate(cardPrefab,cardPool.transform);

            newCard.GetComponent<CardDisplay>().card = cardStore.RandomCard();
            
            cards.Add(newCard);
        }
        SaveCardDate();
        playerData.SavePlayerData();
    }

    public void ClearPool()
    {
        foreach (var card in cards)
        {
            Destroy(card);
        }
        cards.Clear();
    }

    public void SaveCardDate()
    {
        foreach (var card in cards)
        {
            int id = card.GetComponent<CardDisplay>().card.id;
            Debug.Log("卡牌id="+id);
            playerData.playerCards[id] += 1;
        }
    }
    
}
