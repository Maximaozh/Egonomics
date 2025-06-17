using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildDialoguePanel : MonoBehaviour
{
    public TMP_Text buildingNameText;
    public TMP_Text buildingLevelText;

    public Transform productionQueueContent;
    public GameObject productionQueueItemPrefab;
    public ScrollRect productionQueueScrollView;

    public Button addProductionButton;
    public Button closePanelButton;
    public TMP_Dropdown availableProductsDropdown;

    public Button upgradeButton;    

    private BuildingBase currentBuilding;

    void Start()
    {
        addProductionButton.onClick.AddListener(AddProductionOrder);
        closePanelButton.onClick.AddListener(ClosePanel);
    }

    public void SetBuilding(BuildingBase building)
    {
        currentBuilding = building;
        UpdateUI();

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(UpgradeBuilding);

        upgradeButton.interactable = currentBuilding.CanUpgrade();

    }

    private void UpdateUI()
    {
        if (currentBuilding == null) return;

        buildingNameText.text = currentBuilding.buildingName;
        buildingLevelText.text = $"Уровень: {currentBuilding.buildingLevel}";

        //List<TradeItem> availableProducts = currentBuilding.GetAvailableProducts(EconomyModel.Instance.goods.ConvertAll(e => e.item));
        availableProductsDropdown.ClearOptions();

        //List<string> productNames = new List<string>();
        //foreach (var product in availableProducts)
        //{
        //    productNames.Add(product.name);
        //}
        //availableProductsDropdown.AddOptions(productNames);

        UpdateProductionQueueUI();
    }

    public void AddProductionOrder()
    {
        //if (currentBuilding == null) return;

        //List<TradeItem> availableProducts = currentBuilding.GetAvailableProducts(EconomyModel.Instance.goods.ConvertAll(e => e.item));
        //int selectedIndex = availableProductsDropdown.value;

        //if (selectedIndex >= 0 && selectedIndex < availableProducts.Count)
        //{
        //    TradeItem selectedItem = availableProducts[selectedIndex];
        //    currentBuilding.AddProductionOrder(selectedItem, 1);
        //    UpdateProductionQueueUI();
        //}

        //upgradeButton.interactable = currentBuilding.CanUpgrade();
    }

    private void UpdateProductionQueueUI()
    {
        //if (currentBuilding == null) return;

        //foreach (Transform child in productionQueueContent)
        //{
        //    Destroy(child.gameObject);
        //}

        //foreach (var order in currentBuilding.productionQueue)
        //{
        //    GameObject itemGO = Instantiate(productionQueueItemPrefab, productionQueueContent);
        //    ProductionQueueItem item = itemGO.GetComponent<ProductionQueueItem>();

        //    if (item != null)
        //    {
        //        item.SetOrder(order, currentBuilding);
        //    }
        //}
    }
    private void UpgradeBuilding()
    {
        //if (currentBuilding != null && currentBuilding.UpgradeBuilding())
        //{
        //    UpdateUI(); 
        //}
    }
    public void ClosePanel()
    {
        Destroy(gameObject);
    }
}