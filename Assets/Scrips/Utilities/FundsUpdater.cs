using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FundsUpdater : MonoBehaviour
{
    public TMP_Text fundsTMP;
    public PlayerBase player;
    public float lastFunds;

    void LateUpdate()
    {
        if (fundsTMP == null || player == null || player.Portfolio == null)
            return;

        if (lastFunds == player.Portfolio.funds)
            return;

        fundsTMP.text = player.Portfolio.funds.ToString();
        lastFunds = player.Portfolio.funds;
    }
}
