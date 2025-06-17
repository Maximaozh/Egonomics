using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanelUI : MonoBehaviour
{
    [Header("Настройка")]
    public BuildingAbstract selectedBuilding;
    public PlayerBase player;
    [SerializeField] public int leftForTurn = 2;
    [SerializeField] public int buildLimit = 2;

    [Header("UI элементы")]
    public TMP_Dropdown buildingsDropdown;
    public TMP_Dropdown sourceDropdown;
    public TMP_Dropdown targetDropdown;

    public Button addConnectionButton;
    public Transform connectionsContent;

    public GameObject connectionPrefab;

    public void OnEnable()
    {
        var buildings = player.Portfolio.Buildings;

        if (buildings == null)
            return;

        if (buildings.Count == 0)
            return;

        selectedBuilding = player.Portfolio.Buildings[0];

        buildingsDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (var building in buildings)
        {
            options.Add(new TMP_Dropdown.OptionData()
            {
                text = $"{building.BuildingName}"
            });
        }

        LoadConnections();
        buildingsDropdown.AddOptions(options);
        SelectOutputChange(0);
        SelectInputChange(0);
    }

    public void OnDisable()
    {
        selectedBuilding = null;
        
        foreach(Transform child in connectionsContent)
        {
            Destroy(child.gameObject);
        }

        buildingsDropdown.ClearOptions();
        sourceDropdown.ClearOptions();
        targetDropdown.ClearOptions();
    }

    public void Start()
    {
        addConnectionButton.onClick.AddListener(() => AddConnection());
        buildingsDropdown.onValueChanged.AddListener((x) => { ClearContent(); SelectBuilding(x); });
        //outputDropdown.onValueChanged.AddListener(x => SelectOutputChange(x));
        //inputDropdown.onValueChanged.AddListener(x => SelectInputChange(x));
    }


    public void AddConnection()
    {
        if (selectedBuilding == null)
        {
            //Debug.LogWarning("Не выбрано здание для создания соединения");
            return;
        }

        int outputIndex = sourceDropdown.value;
        int inputIndex = targetDropdown.value;

        if (outputIndex < 0 || outputIndex >= selectedBuilding.ActiveUnits.Count ||
            inputIndex < 0 || inputIndex >= selectedBuilding.ActiveUnits.Count)
        {
            //Debug.LogWarning("Неверно выбраны юниты для соединения");
            return;
        }

        BuildingUnit sourceUnit = selectedBuilding.ActiveUnits[outputIndex].info;
        BuildingUnit targetUnit = selectedBuilding.ActiveUnits[inputIndex].info;

        if (sourceUnit == targetUnit)
        {
            //Debug.LogWarning("Нельзя создать связь юнита с самим собой");
            return;
        }

        if (selectedBuilding.connections.Any(c => c.source == sourceUnit))
        {
            //Debug.LogWarning($"У юнита {targetUnit.unitName} уже есть входная связь");
            return;
        }

        if (selectedBuilding.connections.Any(c => c.source == targetUnit && c.target == sourceUnit))
        {
            //Debug.LogWarning("Нельзя создать обратную связь");
            return;
        }

        bool connectionAdded = selectedBuilding.AddConnection(sourceUnit, targetUnit);

        if (connectionAdded)
        {
            //Debug.Log($"Соединение создано: {sourceUnit.unitName} -> {targetUnit.unitName}");

            LoadConnections();

            SelectInputChange(outputIndex);
        }
        else
        {
            //Debug.LogWarning("Не удалось создать соединение");
        }
    }

    public void ClearContent()
    {
        foreach(Transform child in connectionsContent)
        {
            Destroy(child.gameObject);
        }
    }

    public void SelectBuilding(int index)
    {
        if (index < 0 || index >= player.Portfolio.Buildings.Count)
            return;

        selectedBuilding = player.Portfolio.Buildings[index];

        if (selectedBuilding == null)
            return;

        sourceDropdown.ClearOptions();
        targetDropdown.ClearOptions();


        var units = selectedBuilding.ActiveUnits;
        List<TMP_Dropdown.OptionData> optionsSource = GetAllUnitOptions(units);

        sourceDropdown.AddOptions(optionsSource);
        targetDropdown.AddOptions(optionsSource);
        LoadConnections();
    }

    public void SelectOutputChange(int index)
    {
        if (index < 0 || index >= selectedBuilding.ActiveUnits.Count)
            return;

        var selectedUnit = selectedBuilding.ActiveUnits[index];

        sourceDropdown.ClearOptions();

        if (selectedBuilding == null)
            return;

        var units = selectedBuilding.ActiveUnits;

        List<TMP_Dropdown.OptionData> options = GetAllUnitOptions(units);

        sourceDropdown.AddOptions(options);
    }

    public void SelectInputChange(int index)
    {
        if (index < 0 || index >= selectedBuilding.ActiveUnits.Count)
            return;

        var selectedUnit = selectedBuilding.ActiveUnits[index];

        targetDropdown.ClearOptions();

        if (selectedBuilding == null)
            return;

        var units = selectedBuilding.ActiveUnits;

        List<TMP_Dropdown.OptionData> options = GetAllUnitOptions(units);

        targetDropdown.AddOptions(options);
    }

    public List<TMP_Dropdown.OptionData> GetAllUnitOptions(ObservableCollection<Unit> units)
    {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (var unit in units)
        {
            string text = unit.info.UnitName;
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();

            if (unit.info.OutputProduct != null)
                data.image = unit.info.OutputProduct.Ico;

            data.text = text;


            options.Add(data);
        }

        return options;
    }

    public void LoadConnections()
    {
        foreach (Transform child in connectionsContent)
        {
            Destroy(child.gameObject);
        }

        var connections = selectedBuilding.connections;

        foreach (var connect in connections)
        {
            var connectGO = Instantiate(connectionPrefab, connectionsContent);
            var GUI = connectGO.GetComponent<ConnectionItem>();
            GUI.Setup(connect.source, connect.target);

            connectGO.transform.SetSiblingIndex(connections.IndexOf(connect));
        }
    }
}
