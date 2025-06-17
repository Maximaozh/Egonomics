using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[System.Serializable]
public class BuildingBase : MonoBehaviour
{
    public string buildingName; // Имя здания
    public int buildingLevel;   // Уровень здания
    public TradeItem.TradeItemType buildingItemType; // Тип производимых товаров
    public Sprite buildingSprite;   // Спрайт здания
    public TileBase buildingTile;   // Тайлик здания
    public int cost;                // Стоимость строительства
    public string description;      // Описание
    public PlayerBase owner;        // Кто построил здание



























    //public int productionLimit = 10;
    //public List<ProductionOrder> productionQueue = new List<ProductionOrder>();


    //public int maxLevel = 4;
    //public int upgradeCostMultiplier = 2;
    //public int upgradeProductionBonus = 5; // Бонус к лимиту производства за уровень

    //[System.Serializable]
    //public class ProductionOrder
    //{
    //    public TradeItem targetItem;
    //    public int targetAmount;
    //    public int producedAmount;
    //}

    //public bool AddProductionOrder(TradeItem item, int amount)
    //{
    //    if (!CanProduceItem(item))
    //        return false;

    //    if (productionQueue.Exists(order => order.targetItem == item))
    //    {
    //        //Debug.LogWarning($"Товар {item.name} уже есть в очереди производства.");
    //        return false;
    //    }

    //    productionQueue.Add(new ProductionOrder
    //    {
    //        targetItem = item,
    //        targetAmount = Mathf.Min(amount, productionLimit),
    //        producedAmount = owner.Portfolio.GetItemCount(item)
    //    });
    //    return true;
    //}

    //public void RemoveProductionOrder(int index)
    //{
    //    if (index >= 0 && index < productionQueue.Count)
    //    {
    //        productionQueue.RemoveAt(index);
    //    }
    //}

    //public void ProcessProduction()
    //{
    //    foreach (var order in productionQueue)
    //    {
    //        int possibleProduction = CalculateMaxProduction(order.targetItem, order.targetAmount);

    //        if (possibleProduction > 0)
    //        {
    //            ProduceItems(order.targetItem, possibleProduction);
    //            order.producedAmount = owner.Portfolio.GetItemCount(order.targetItem);
    //        }
    //    }
    //}
    //public bool CanProduceItem(TradeItem item)
    //{
    //    return item.type == buildingItemType &&
    //           item.level >= buildingLevel;
    //}

    //private int CalculateMaxProduction(TradeItem item, int amount)
    //{
    //    int maxPossible = amount;

    //    foreach (var req in item.requireds)
    //    {
    //        if (owner.Portfolio.GetItemCount(req.item) < req.quantity)
    //            return 0;

    //        maxPossible = Mathf.Min(maxPossible,
    //            owner.Portfolio.GetItemCount(req.item) / req.quantity);
    //    }

    //    return Mathf.Min(maxPossible, productionLimit);
    //}

    //private void ProduceItems(TradeItem item, int amount)
    //{
    //    foreach (var req in item.requireds)
    //    {
    //        owner.Portfolio.RemoveItem(req.item, req.quantity * amount);
    //    }

    //    owner.Portfolio.AddItem(item, amount);
    //}
    //public List<TradeItem> GetAvailableProducts(List<TradeItem> allItems)
    //{
    //    List<TradeItem> available = new List<TradeItem>();
    //    foreach (var item in allItems)
    //    {
    //        if (CanProduceItem(item)) available.Add(item);
    //    }
    //    return available;
    //}
    //public bool UpgradeBuilding()
    //{
    //    if (buildingLevel <= 1)
    //    {
    //        //Debug.Log($"Здание {buildingName} уже достигло максимального уровня!");
    //        return false;
    //    }

    //    int upgradeCost = cost * (maxLevel - buildingLevel + 1) * upgradeCostMultiplier;

    //    if (owner.Portfolio.Funds < upgradeCost)
    //    {
    //        //Debug.Log($"Недостаточно средств для улучшения {buildingName}!");
    //        return false;
    //    }

    //    owner.Portfolio.Funds -= upgradeCost;
    //    buildingLevel--;
    //    productionLimit += upgradeProductionBonus;

    //    //Debug.Log($"{buildingName} улучшено до уровня {buildingLevel}. Новый лимит производства: {productionLimit}");
    //    return true;
    //}
    public bool CanUpgrade()
    {
        return buildingLevel > 1 && owner.Portfolio.Funds >= cost * (buildingLevel + 1);
    }
}
