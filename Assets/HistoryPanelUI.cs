using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HistoryPanelUI : MonoBehaviour
{
    public TMP_Text text;
    public TurnManager tm;

    public void OnEnable()
    {
        text.text = DataLogger.Instance.Build();
    }

    public void OnDisable()
    {
        text.text = "";
    }
}
