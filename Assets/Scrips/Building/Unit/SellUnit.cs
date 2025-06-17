using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static MarketManager;

[System.Serializable]
public class SellUnit : BuildingUnit
{
    [Header("Настройка продаж")]
    [SerializeField] private float priceMultiplier = 1.2f;
    [SerializeField] private bool isPrivate = false;
    [SerializeField] private int maxSellCapacity = 100;
    [SerializeField] private int sellCap = 1000;
    [SerializeField] private float priceWeightFactor = 2f;

    [Header("Постоянные заказы")]
    public List<PurchaseUnit> orders = new List<PurchaseUnit>();

    public float PriceMultiplier { get => priceMultiplier; set => priceMultiplier = value; }
    public bool IsPrivate { get => isPrivate; set => isPrivate = value; }
    public int MaxSellCapacity { get => maxSellCapacity; set => maxSellCapacity = value; }
    public int SellCap { get => sellCap; set => sellCap = value; }
    public float PriceWeightFactor { get => priceWeightFactor; set => priceWeightFactor = value; }

    public override void Execute(BuildingUnit outputTarget)
    {
        base.Execute(outputTarget);

        if (ParentBuilding.BuildingType != BuildingType.Shop)
            ProcessAllOrders();
        
    }
    public void AddToMarket()
    {
        if (ParentBuilding.BuildingType == BuildingType.Shop && ParentBuilding != null)
        {
            bool notAdded = MarketManager.Instance.SellUnits.Find(x => this == x) == null ? true : false;

            if (notAdded && !IsPrivate)
                MarketManager.Instance.SellUnits.Add(this);

        }
        else
        {
            bool notAdded = MarketManager.Instance.SellUnitsPlayer.Find(x => this == x) == null ? true : false;

            if (notAdded)
                MarketManager.Instance.SellUnitsPlayer.Add(this);
        }
    }

    public new void Start()
    {
        base.Start();
        AddToMarket();
    }

    public void ChangeProduct(ProductData product)
    {
        this.OutputProduct = product;
        ClearOrders();
    }

    public void ChangeStatus(bool status)
    {
        IsPrivate = status;
        ClearOrders();
    }

    public void ClearOrders()
    {
        var ordersCopy = orders.ToList();
        orders.Clear();

        foreach (var order in ordersCopy)
        {
            if (order != null)
                order.CancelOrder();
        }

        orders.Clear();
    }

   public List<PurchaseUnit> SortOrders()
    {
        return orders
        .Where(order => order.SuggestedMultiplier >= this.PriceMultiplier || order.ParentBuilding.Owner == this.ParentBuilding.Owner) 
        .OrderByDescending(order => order.SuggestedMultiplier) 
        .ToList();
    }

    public int GetOrderPosition(PurchaseUnit order)
    {
        if (order == null || !orders.Contains(order))
            return -1;

        var sortedOrders = SortOrders();

        return sortedOrders.FindIndex(o => o == order);
    }

    public bool AddOrder(PurchaseUnit buyer)
    {
        if (IsPrivate && buyer.ParentBuilding.Owner != this.ParentBuilding.Owner)
            return false;

        orders.Add(buyer);

        return true;
    }

    public bool RemoveOrder(PurchaseUnit buyer)
    {
        return orders.Remove(buyer);
    }



    private void ProcessAllOrders()
    {
        if (OutputProduct == null || !StoredItems.ContainsKey(OutputProduct))
            return;

        int availableAmount = StoredItems[OutputProduct].Amount;
        if (availableAmount <= 0) return;

        var prioritizedOrders = SortOrdersWithPriority();

        DistributeStockToOrders(prioritizedOrders, availableAmount);
    }

    private List<PurchaseUnit> SortOrdersWithPriority()
    {
        return orders
            .Where(order => order != null && order.OutputProduct == this.OutputProduct)
            .OrderByDescending(order => order.ParentBuilding.Owner == this.ParentBuilding.Owner) // Сначала внутренние заказы
            .ThenByDescending(order => CalculateOrderWeight(order)) // Затем по весу заказа
            .ToList();
    }

    private float CalculateOrderWeight(PurchaseUnit order)
    {
        float baseWeight = order.SuggestedMultiplier;

        if (order.ParentBuilding.Owner == this.ParentBuilding.Owner)
        {
            baseWeight *= 1.5f; 
        }

        return Mathf.Pow(baseWeight, PriceWeightFactor);
    }

    private void DistributeStockToOrders(List<PurchaseUnit> prioritizedOrders, int totalAvailable)
    {
        if (prioritizedOrders.Count == 0) return;

        float totalWeight = prioritizedOrders.Sum(order => CalculateOrderWeight(order));
        if (totalWeight <= 0) return;

        int remainingAmount = Mathf.Min(totalAvailable, MaxSellCapacity);

        foreach (var order in prioritizedOrders)
        {
            if (remainingAmount <= 0) break;

            float orderWeight = CalculateOrderWeight(order);
            float weightRatio = orderWeight / totalWeight;

            int allocatedAmount = Mathf.FloorToInt(remainingAmount * weightRatio);

            allocatedAmount = Mathf.Max(1, allocatedAmount);
            allocatedAmount = Mathf.Min(allocatedAmount, remainingAmount);

            int actualAmount = CalculateTransferAmount(order, allocatedAmount, remainingAmount);
            if (actualAmount <= 0) continue;

            ExecuteOrder(order, actualAmount);
            remainingAmount -= actualAmount;

            totalWeight -= orderWeight;
        }
    }

    private int CalculateTransferAmount(PurchaseUnit order, int desiredAmount, int availableAmount)
    {
        if (order == null || order.OutputProduct != this.OutputProduct)
            return 0;

        int transferAmount = Mathf.Min(desiredAmount, availableAmount);

        int buyerFreeSpace = order.MaxStorageCapacity - order.GetCurrentItemAmount(OutputProduct);
        transferAmount = Mathf.Min(transferAmount, buyerFreeSpace);

        if (order.ParentBuilding.Owner != this.ParentBuilding.Owner)
        {
            float itemPrice = OutputProduct.CalculateResultPrice() * order.SuggestedMultiplier;
            int buyerAffordable = Mathf.FloorToInt(order.ParentBuilding.Owner.Portfolio.funds / itemPrice);
            transferAmount = Mathf.Min(transferAmount, buyerAffordable);
        }

        transferAmount = Mathf.Min(transferAmount, OutputCapacity);

        return transferAmount > 0 ? transferAmount : 0;
    }



    private void ExecuteOrder(PurchaseUnit order, int amount)
    {
        if (OutputProduct == null)
            return;

        var transferAmout = Mathf.Min(amount, StoredItems[OutputProduct].Amount);

        if (order.ParentBuilding.Owner == this.ParentBuilding.Owner)
        {
            int buyerAffordable = Mathf.FloorToInt(order.ParentBuilding.Owner.Portfolio.funds / OutputProduct.CalculateResultPrice() * order.SuggestedMultiplier);
            transferAmout = Mathf.Min(buyerAffordable, transferAmout);
        }
        
        var productToTransfer = new ProductInstance(
            StoredItems[OutputProduct].Data,
            StoredItems[OutputProduct].Quality,
            StoredItems[OutputProduct].Brand,
            transferAmout
        );

        order.AddProductInstance(productToTransfer);

        StoredItems[OutputProduct].Amount -= amount;

        float totalPrice = amount * order.OutputProduct.CalculateResultPrice() * order.SuggestedMultiplier;

        if (order.ParentBuilding.Owner != this.ParentBuilding.Owner)
        {
            order.ParentBuilding.Owner.Portfolio.funds -= totalPrice;
            ParentBuilding.Owner.Portfolio.funds += totalPrice;
        }

        if(transferAmout > 0)
        {
            float revenue = transferAmout * totalPrice;
            MarketManager.Instance.LastSalesRecords.Add(new SaleRecord(
                OutputProduct,
                this,
                transferAmout,
                revenue,
                1));
        }
    }
        
    public float GetCurrentPrice()
    {
        return OutputProduct.CalculateResultPrice() * PriceMultiplier;
    }
}