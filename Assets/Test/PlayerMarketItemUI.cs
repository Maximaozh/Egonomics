using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMarketItemUI : MonoBehaviour
{
    public TMP_Text playerName;
    public TMP_Text supplyText;

    public Slider slider;

    public float supplyCap;
    public float supplyPlayer;
    public float appeal;

    public void Setup(string playerName, float supplyCap, float supplyPlayer, float appeal)
    {
        this.playerName.text = playerName;
        this.supplyCap = supplyCap;
        this.supplyPlayer = supplyPlayer;

        slider.enabled = false;
        slider.value = (supplyPlayer / supplyCap);

        supplyText.text = "Предложил: " + supplyPlayer + " (" + (supplyPlayer / supplyCap * 100).ToString("0.00") + '%' + 
                          ") \nПривлекательность: " + appeal.ToString("0.00");
    }
}
