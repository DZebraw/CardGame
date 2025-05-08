using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Text nameText;
    public Text attackText;
    public Text healthText;
    public Text effectText;

    public Image backgroundImage;

    public Card card;

    void Start()
    {
        ShowCard();
    }

    public void ShowCard()
    {
        nameText.text = card.cardName;
        
        //如果是某类型卡，转化为某类型，然后赋值
        if (card is MonsterCard)
        {
            MonsterCard monster = card as MonsterCard;
            attackText.text = monster.attack.ToString();
            healthText.text = monster.healthPoint.ToString();
            
            effectText.gameObject.SetActive(false);
        }
        else if(card is SpellCard)
        {
            SpellCard spell = card as SpellCard;
            effectText.text = spell.effect;

            attackText.gameObject.SetActive(false);
            healthText.gameObject.SetActive(false);
        }
    }
}
