using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedBuildInfoUI : MonoBehaviour
{
    [Header("UI элементы")]
    public Button closeButton;
    public TMP_Text buildName;
    public TMP_Text buildType;
    public TMP_Text maintaince;
    public Toggle unitConnectionToggle;

    public Button ClearButton;
    public Button CopyButton;
    public Button PasteButton;

    private BuildingAbstract currentBuilding;

    [Header("Префабы")]
    public Transform gridContent;
    public Transform infoContent;
    public GameObject gridUnititem;

    public void Start()
    {
        ClearButton.onClick.AddListener(HandleClearButton);
        ClearButton.onClick.AddListener(RefreshUI);

        CopyButton.onClick.AddListener(HandleCopyButton);

        PasteButton.onClick.AddListener(HandlePasteButton);
        PasteButton.onClick.AddListener(RefreshUI);
    }

    private void OnEnable()
    {
        if (currentBuilding != null)
        {
            currentBuilding.ActiveUnits.CollectionChanged += OnActiveUnitsChanged;
        }
    }

    private void OnDisable()
    {
        if (currentBuilding != null)
        {
            currentBuilding.ActiveUnits.CollectionChanged -= OnActiveUnitsChanged;
        }
    }

    private void OnActiveUnitsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RefreshUI();
    }

    public void Setup(BuildingAbstract building)
    {
        foreach (Transform child in infoContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in gridContent)
        {
            Destroy(child.gameObject);
        }


        currentBuilding = building;
        UpdateBuildingInfo();
        UpdateUnitGrid();
    }

    private void UpdateBuildingInfo()
    {
        buildName.text = currentBuilding.BuildingName;
        buildType.text = currentBuilding.BuildingType.ToString();
        maintaince.text = $"СО: {currentBuilding.MaintenanceCost}";
    }

    private void UpdateUnitGrid()
    {

        foreach (Transform child in gridContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var unit in currentBuilding.ActiveUnits)
        {
            var slot = Instantiate(gridUnititem, gridContent);
            var comp = slot.GetComponent<GridUnititemSlot>();
            unit.GUIContent = infoContent;
            comp.setup(unit);
            comp.button.onClick.AddListener(() => ClearContent(infoContent));
            comp.AddListners();
        }
    }
    public void RefreshUI()
    {
        if (currentBuilding == null) return;

        ToolManager.instance.ReloadBuildingWindow();
    }


    public void ClearContent(Transform content)
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }

    public void HandleCopyButton()
    {
        CopyBuffer.CopyBuilding(currentBuilding);
    }
    public void HandlePasteButton()
    {
        CopyBuffer.PasteBuilding(ref currentBuilding);
    }

    public void HandleClearButton()
    {
        for (int i = 0; i < currentBuilding.ActiveUnits.Count; i++)
            currentBuilding.DeleteUnit(i);
    }

}
