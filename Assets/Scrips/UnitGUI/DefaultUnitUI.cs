using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefaultUnitUI : MonoBehaviour
{
    [Header("Выбранный Unit")]
    public Unit unit;
    public BuildingUnit bUnit;

    [Header("Элементы UI")]
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
        maintaince.text = bUnit.MaintenanceCost.ToString() + " со";
        workerLevel.text = bUnit.WorkersLevel + " ур";
        unitLevel.text = bUnit.UnitLevel + " уд";
        maximumCapacity.text = bUnit.MaxStorageCapacity + " мв";

        foreach (var item in bUnit.StoredItems)
        {
            items.options.Add(new TMP_Dropdown.OptionData() { text = $"{item.Key.ItemName}: К{item.Value.Quality} Б{item.Value.Brand} #{item.Value.Amount}" });
        }

        clearStorageButton.onClick.AddListener(() => ClearStorage());
    }

    public void ClearStorage()
    {
        items.ClearOptions();
        bUnit.StoredItems.Clear();
    }
}
