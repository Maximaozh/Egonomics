using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TradeUI : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private TradeUIItem _itemUIPrefab;
    [SerializeField] private Transform _contentParent;
    [SerializeField] private PlayerUser player;

    [Header("Фильтры")]
    [SerializeField] private TMP_Dropdown _categoryFilter;
    [SerializeField] private TMP_Dropdown _typeFilter;
    [SerializeField] private TMP_InputField _searchInput;

    [SerializeField] private TMP_Text _playerFundsText;

    private List<TradeUIItem> _spawnedItems = new List<TradeUIItem>();
    private TradeItem.TradeItemCategory _selectedCategory;
    private TradeItem.TradeItemType _selectedType;
    private string _searchQuery = "";

    private void Start()
    { 
        InitializeFilters();
        UpdatePlayerFundsText();

        player.Portfolio.OnFundsUpdated += UpdatePlayerFundsText;


        if (EconomyModel.Instance == null)
            return;

        EconomyModel.Instance.OnEconomyUpdated += UpdateUI;
    }

    public void InitializeUI()
    {
        ClearExistingItems();

        foreach (var economicItem in EconomyModel.Instance.goods)
        {
            CreateTradeItem(economicItem.item);
        }

        ApplyFilters();
    }
    private void InitializeFilters()
    {
        _categoryFilter.ClearOptions();
        _categoryFilter.AddOptions(new List<string> { "Все категории" });
        _categoryFilter.AddOptions(System.Enum.GetNames(typeof(TradeItem.TradeItemCategory)).ToList());

        _typeFilter.ClearOptions();
        _typeFilter.AddOptions(new List<string> { "Все типы" });
        _typeFilter.AddOptions(System.Enum.GetNames(typeof(TradeItem.TradeItemType)).ToList());

        _categoryFilter.onValueChanged.AddListener(OnCategoryFilterChanged);
        _typeFilter.onValueChanged.AddListener(OnTypeFilterChanged);
        _searchInput.onValueChanged.AddListener(OnSearchInputChanged);
    }

    private void OnCategoryFilterChanged(int index)
    {
        _selectedCategory = (TradeItem.TradeItemCategory)(index - 1);
        ApplyFilters();
    }

    private void OnTypeFilterChanged(int index)
    {
        _selectedType = (TradeItem.TradeItemType)(index - 1);
        ApplyFilters();
    }

    private void OnSearchInputChanged(string query)
    {
        _searchQuery = query.ToLower();
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        foreach (var item in _spawnedItems)
        {
            bool categoryMatch = _categoryFilter.value == 0 ||
                               item.item.category == _selectedCategory;

            bool typeMatch = _typeFilter.value == 0 ||
                           item.item.type == _selectedType;

            bool searchMatch = string.IsNullOrEmpty(_searchQuery) ||
                             item.item.name.ToLower().Contains(_searchQuery);

            item.gameObject.SetActive(categoryMatch && typeMatch && searchMatch);
        }
    }
    private void UpdatePlayerFundsText()
    {
        if (_playerFundsText != null && player.Portfolio != null)
        {
            _playerFundsText.text = $"$$$: {player.Portfolio.Funds:F2}";
        }
    }

    private void CreateTradeItem(TradeItem item)
    {
        TradeUIItem newItem = Instantiate(_itemUIPrefab, _contentParent);
        newItem.Initialize(item, player.Portfolio);
        _spawnedItems.Add(newItem);
    }

    private void UpdateUI()
    {
        foreach (var item in _spawnedItems)
        {
            item.UpdateUI();
        }

        UpdatePlayerFundsText();
    }

    private void ClearExistingItems()
    {
        foreach (var item in _spawnedItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        _spawnedItems.Clear();
    }

    private void OnDestroy()
    {
        if (EconomyModel.Instance != null)
        {
            player.Portfolio.OnFundsUpdated -= UpdatePlayerFundsText;
            EconomyModel.Instance.OnEconomyUpdated -= UpdateUI;
        }
    }
}

