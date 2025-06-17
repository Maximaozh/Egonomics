using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProduceUnitGUI : UnitGUI
{
    [Header("Ёлементы UI —пециализированные")]
    public TMP_Dropdown avaiableList;
    public TMP_Text output;
    public Button selectButton;

    public override void Configure(Unit unit, string extra)
    {
        base.Configure(unit, extra);
        configureSpecial();

    }
    public new void Start()
    {
        selectButton.onClick.AddListener(() => { SelectResource(); RefreshFullUI(); });
    }


    public void configureSpecial()
    {
        avaiableList.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        ProduceUnit ps = (ProduceUnit)unit.info;
        output.text = ps.OutputProduct.ItemName;

        foreach (var resource in ps.avaiableResources)
        {
            options.Add(new TMP_Dropdown.OptionData()
            {
                text = $"{resource.ItemName} => —ложность: ({resource.BasePrice})", image=resource.Ico
            });
        }

        avaiableList.AddOptions(options);

        if (ps.OutputProduct != null)
        {
            int selectedIndex = ps.avaiableResources.FindIndex(r => r == ps.OutputProduct);
            if (selectedIndex >= 0)
            {
                avaiableList.value = selectedIndex;
            }
        }
    }

    public void SelectResource()
    {
        ProduceUnit ps = (ProduceUnit)unit.info;
        int selectedIndex = avaiableList.value;
        ps.OutputProduct = ps.avaiableResources[selectedIndex];
        //Debug.Log("–есурс назначен " + ps.outputProduct.itemName);
        Configure(this.unit, "");
    }
}
