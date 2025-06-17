using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitGUI : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform content;
    public SelectedBuildInfoUI buildTool;

    [Header("Элементы UI Общие")]
    public TMP_Text unitName;
    public TMP_Text maintaince;
    public TMP_Text workerLevel;
    public TMP_Text unitLevel;
    public TMP_Text maximumCapacity;
    public TMP_Dropdown items;
    public Button clearStorageButton;


    [Header("Конфигруационные настройки")]
    public Unit unit;
    public virtual void Configure(Unit unit, string extra)
    {
        this.unit = unit;
        unitName.text = unit.info.ExtraInfo;
        maintaince.text = unit.info.MaintenanceCost.ToString() + " со";
        workerLevel.text = unit.info.WorkersLevel + " ур";
        unitLevel.text = unit.info.UnitLevel + " уд";
        maximumCapacity.text = unit.info.MaxStorageCapacity + " мв";

        foreach (var item in unit.info.StoredItems)
        {
            Sprite sprite = item.Value.Data.Ico;
            items.options.Add(new TMP_Dropdown.OptionData() { text = $"{item.Key.ItemName}: К{item.Value.Quality} Б{item.Value.Brand} #{item.Value.Amount}", image=sprite});
        }

        if (buildTool == null)
        {
            buildTool = GetComponentInParent<SelectedBuildInfoUI>();
            if (buildTool == null)
            {
                buildTool = FindObjectOfType<SelectedBuildInfoUI>();
            }
        }
    }
    public void Start()
    {
        clearStorageButton.onClick.AddListener(() => { ClearStorage(); RefreshFullUI(); });
    }

    public void ClearStorage()
    {
        items.ClearOptions();
        unit.info.StoredItems.Clear();
    }

    protected void RefreshFullUI()
    {
        if (buildTool != null)
        {
            buildTool.RefreshUI();
        }
        else
        {
            //Debug.LogWarning("SelectedBuildInfoUI утерян вовеки веков!");
        }
    }
}
