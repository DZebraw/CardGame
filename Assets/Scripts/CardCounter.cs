using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCounter : MonoBehaviour
{
    public Text CounterText;
    private int counter = 0;

    public bool SetCounter(int _value)
    {
        counter += _value;
        OnCounterChannge();
        if (counter == 0)
        {
            Destroy(gameObject);
            return false;
        }

        return true;
    }

    private void OnCounterChannge()
    {
        CounterText.text = counter.ToString();
    }
}
