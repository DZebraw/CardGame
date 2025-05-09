using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CardState
{
    Library,
    Deck
}
public class ClickCard : MonoBehaviour,IPointerDownHandler
{
    private DeckManager deckManager;

    public CardState state;

    private void Start()
    {
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
    }

    //点击卡牌之后，将信息传入DeckManager方法中 统一处理
    public void OnPointerDown(PointerEventData eventData)
    {
        int id = this.GetComponent<CardDisplay>().card.id;
        deckManager.UpdateCard(state, id);
    }
}
