using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefaultUnitUI : MonoBehaviour
{
    [Header("��������� Unit")]
    public Unit unit;
    public BuildingUnit bUnit;

    [Header("�������� UI")]
    public TMP_Text unitName;
    public TMP_Text maintaince;
    public TMP_Text workerLevel;
    public TMP_Text unitLevel;
    public TMP_Text maximumCapacity;
    public TMP_Text output;
    public TMP_Dropdown items;

    public Button clearStorageButton;

    public void Setup()
    {
        bUnit = unit.info;

        unitName.text = bUnit.UnitName;
        maintaince.text = bUnit.MaintenanceCost.ToString() + " ��";
        workerLevel.text = bUnit.WorkersLevel + " ��";
        unitLevel.text = bUnit.UnitLevel + " ��";
        maximumCapacity.text = bUnit.MaxStorageCapacity + " ��";

        foreach (var item in bUnit.StoredItems)
        {
            items.options.Add(new TMP_Dropdown.OptionData() { text = $"{item.Key.ItemName}: �{item.Value.Quality} �{item.Value.Brand} #{item.Value.Amount}" });
        }

        clearStorageButton.onClick.AddListener(() => ClearStorage());
    }

    public void ClearStorage()
    {
        items.ClearOptions();
        bUnit.StoredItems.Clear();
    }
}
