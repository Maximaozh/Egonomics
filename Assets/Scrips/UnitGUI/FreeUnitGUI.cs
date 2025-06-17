using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FreeUnitGUI : UnitGUI
{
    
    [Header("Элементы UI Специализированные")]
    public TMP_Dropdown avaiableList;
    public Button buildButton;
    public  override void Configure(Unit unit, string extra)
    {
        base.Configure(unit, extra);
        configureSpecial();

        buildButton.onClick.AddListener(() => { Build();  RefreshFullUI(); });
    }
    
    

    public void configureSpecial()
    {
        avaiableList.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (var availableUnit in unit.info.ParentBuilding.AvaiableUnits)
        {
            options.Add(new TMP_Dropdown.OptionData()
            {
                text = $"{availableUnit.info.UnitName} (Цена: {availableUnit.info.PurchaseCost})"
            });
        }

        avaiableList.AddOptions(options);
    }

    public void Build()
    {
        if (unit.info.ParentBuilding == null || avaiableList.options.Count == 0)
            return;

        int selectedIndex = avaiableList.value;

        if (selectedIndex < 0 || selectedIndex >= unit.info.ParentBuilding.AvaiableUnits.Count)
            return;

        Unit unitToBuild = unit.info.ParentBuilding.AvaiableUnits[selectedIndex];

        if (unit.info.ParentBuilding.Owner.Portfolio.funds < unitToBuild.info.PurchaseCost)
            return;

        Unit newUnit = Instantiate(unitToBuild);

        bool success = unit.info.ParentBuilding.ReplaceUnitByExisting(unit, newUnit);

        if (success)
        {
            Configure(newUnit, newUnit.info.UnitName);
            //Debug.Log($"Здание {newUnit.info.unitName} успешно построено!");
        }
        else
        {
            Destroy(newUnit.gameObject);
            //Debug.LogError("Не удалось построить здание!");
        }
    }

}
