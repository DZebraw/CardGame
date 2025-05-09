using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseDisplayer : MonoBehaviour
{
    public Text PhaseText;
    
    void Start()
    {
        BattleManager.Instance.phaseChangeEvent.AddListener(UpdateText);
    }
    
    void UpdateText()
    {
        PhaseText.text = BattleManager.Instance.GamePhase.ToString();
    }
}
