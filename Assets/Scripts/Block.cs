using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour,IPointerDownHandler
{
    public GameObject card;
    public GameObject summonBlock;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (summonBlock.activeInHierarchy)
        {
            BattleManager.Instance.SummonConfirm(transform);
        }
    }
}
