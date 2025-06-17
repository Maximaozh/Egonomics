using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GridUnititemSlot : MonoBehaviour
{
    public Unit unitHolder;
    public TMP_Text unitText;

    public Button button;
    public Image icon;
    public void setup(Unit unit)
    {
        this.unitHolder = unit;
        button = this.GetComponent<Button>();
        unitText.text = unitHolder.info.UnitName;
        
        if(unitHolder.info.OutputProduct != null)
            icon.sprite = unitHolder.info.OutputProduct.Ico;
    }    

    public void AddListners()
    {
        button.onClick.AddListener(() => unitHolder.ShowUI(""));
    }
}
