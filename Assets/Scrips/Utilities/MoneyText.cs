using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyText : MonoBehaviour
{
    public TMP_Text fundsTMP;
    public PlayerBase player;
    void LateUpdate()
    {
        if (fundsTMP is not null && player is not null && player.Portfolio is not null)
            fundsTMP.text = player.Portfolio.funds.ToString();
    }
}
