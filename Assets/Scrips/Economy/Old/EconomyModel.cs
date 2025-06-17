using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class EconomyModel : MonoBehaviour
{
    public static EconomyModel Instance { get; private set; }
    
    [System.Serializable]
    public class EconomicTradeItem
    {
        public TradeItem item;
        public int quantity;

        public float purchasePrice;
        public float salePrice;
        public float demand;
        public float equilibrium;
    }

    public struct GoodsSaveData
    {
        public List<EconomicTradeItem> items;
    }

    public List<EconomicTradeItem> goods;       // описание всех товаров, доступных в городе
    public float baseDemandFactor = 1.0f;       // Базовый фактор, неизменяемый
    public float eventDemandFactor = 1.0f;      // Изменяемый в процессе факторы
    public float equilibriumInfluence = 0.5f;   // Влияние близости к точке 

    public event System.Action OnEconomyUpdated;
    private List<EconomicTradeItem> pendingUpdates = new List<EconomicTradeItem>();

    public void QueuePriceUpdate(EconomicTradeItem item)
    {
        if (!pendingUpdates.Contains(item))
        {
            pendingUpdates.Add(item);
        }
    }
    public EconomicTradeItem GetItem(TradeItem item)
    {
        return goods.Find(e => e.item == item);
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        Instance = this;

        InitializeEconomy();
    }
    public void InitializeEconomy() // Первоначальный старт экономики
    {
        foreach (var economicItem in goods)
        {
            economicItem.demand = CalculateBaseDemand(economicItem.item);
            UpdateEconomy();
        }
    }
    public void UpdateEconomy()
    {
        foreach (var economicItem in goods)
            UpdatePrices(economicItem);
    }

    public float CalculateBaseDemand(TradeItem item)
    {
        float demand = baseDemandFactor;

        switch(item.category)
        {
            case TradeItem.TradeItemCategory.Necessity: demand *= 1.0f; break;
            case TradeItem.TradeItemCategory.Basic:     demand *= 1.5f; break;
            case TradeItem.TradeItemCategory.Advanced:  demand *= 1.2f; break;
            case TradeItem.TradeItemCategory.Luxury:    demand *= 1.0f; break;
            default: break;
        }

        switch (item.type)
        {
            case TradeItem.TradeItemType.Production:    demand *= 1.2f; break;
            case TradeItem.TradeItemType.Resource:      demand *= 0.8f; break;
            case TradeItem.TradeItemType.Other:         demand *= 1.0f; break;
            case TradeItem.TradeItemType.Technology:    demand *= 0.8f; break;
            default: break;
        }

        return demand;
    }

    public void UpdatePrices(EconomicTradeItem economicItem)
    {
        float equilibriumDelta = Mathf.Abs(economicItem.quantity - economicItem.equilibrium);
        float equilibriumFactor = 1 - Mathf.Clamp01(equilibriumDelta / economicItem.equilibrium) * equilibriumInfluence;

        float demandFactor = economicItem.demand;
        float supplyFactor = economicItem.quantity;
        float priceFactor = demandFactor / (supplyFactor + 1);

        float basePrice = economicItem.item.calculatedPrice * priceFactor;

        float randomiser = Random.Range(0.8f, 1.5f);


        economicItem.purchasePrice = Mathf.Max(1, basePrice * equilibriumFactor);    
        economicItem.salePrice = Mathf.Max(1, basePrice * 0.8f * equilibriumFactor);

        economicItem.item.basePrice = Random.Range(0.8f, 1.2f);
    }

    public void UpdateDemand(EconomicTradeItem economicItem)
    {
        economicItem.demand += Random.Range(-0.1f, 0.1f); 
        if (economicItem.demand < 0)
            economicItem.demand = 0;
    }

    public void AddItemToEconomy(TradeItem item, int initialQuantity)
    {
        if (goods.Exists(e => e.item == item))
            return;

        EconomicTradeItem newEconomicItem = new EconomicTradeItem
        {
            item = item,
            quantity = initialQuantity,
            purchasePrice = 0,
            salePrice = 0,
            demand = CalculateBaseDemand(item),
            equilibrium = (int)Mathf.Pow(2,item.level)
        };

        goods.Add(newEconomicItem);

        UpdatePrices(newEconomicItem);
        OnEconomyUpdated?.Invoke();
    }

    //void Start()
    //{
    //    TextAsset jsonFile = Resources.Load<TextAsset>("trade_items");
    //    List<TradeItem> loadedItems = JsonWorker.LoadFromJson();

    //    foreach (var item in loadedItems)
    //    {
    //        item.CalculateResultPrice();
    //        AddItemToEconomy(item, (int)Mathf.Pow(2, (int)Mathf.Pow(2, item.level)));
    //    }
    //}

    public void RecalculateAllPrices()
    {
        foreach (var item in pendingUpdates)
        {
            UpdatePrices(item);
            item.item.CalculateResultPrice();
        }
        pendingUpdates.Clear();
        OnEconomyUpdated?.Invoke();
    }

    public bool BuyFromMarket(TradeItem item, int quantity)
    {
        var economicItem = goods.Find(e => e.item == item);
        if (economicItem == null || economicItem.quantity < quantity)
            return false;

        economicItem.quantity -= quantity;
        QueuePriceUpdate(economicItem);
        return true;
    }
    public void SellToMarket(TradeItem item, int quantity)
    {
        var economicItem = goods.Find(e => e.item == item);
        if (economicItem != null)
        {
            economicItem.quantity += quantity;
            QueuePriceUpdate(economicItem); 
        }
    }

}
