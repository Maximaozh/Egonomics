using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    public class SaleRecord
    {
        [SerializeField] private ProductData product;
        [SerializeField] private SellUnit seller;
        [SerializeField] private int amountSold;
        [SerializeField] private float totalRevenue;
        [SerializeField] private float appeal;
        [SerializeField] private List<SaleRecord> lastPlayerSalesRecords; 

        public ProductData Product { get => product; set => product = value; }
        public SellUnit Seller { get => seller; set => seller = value; }
        public int AmountSold { get => amountSold; set => amountSold = value; }
        public float TotalRevenue { get => totalRevenue; set => totalRevenue = value; }
        public float Appeal { get => appeal; set => appeal = value; }

        public SaleRecord(ProductData product, SellUnit seller, int amountSold, float totalRevenue, float appeal)
        {
            this.Product = product;
            this.Seller = seller;
            this.AmountSold = amountSold;
            this.TotalRevenue = totalRevenue;
            this.Appeal = appeal;
        }
    }

    [SerializeField] private int baseDemand = 2000;
    [SerializeField] private int baseSupply = 2000;

    public static MarketManager Instance { get; private set; }
    public int BaseDemand { get => baseDemand; set => baseDemand = value; }
    public int BaseSupply { get => baseSupply; set => baseSupply = value; }
    public List<PurchaseUnit> PurchaseUnits { get => purchaseUnits; set => purchaseUnits = value; }
    public List<SellUnit> SellUnits { get => sellUnits; set => sellUnits = value; }
    public List<SellUnit> SellUnitsPlayer { get => sellUnitsPlayer; set => sellUnitsPlayer = value; }
    public List<MarketItem> MarketItems { get => marketItems; set => marketItems = value; }
    public List<ProductData> GameProductsDatabase { get => gameProductsDatabase; set => gameProductsDatabase = value; }
    public List<SaleRecord> LastSalesRecords { get => lastSalesRecords; set => lastSalesRecords = value; }
    public List<SaleRecord> LastPlayerSalesRecords { get => lastPlayerSalesRecords; set => lastPlayerSalesRecords = value; }

    [SerializeField] private List<PurchaseUnit> purchaseUnits; // Все департаменты закупок
    [SerializeField] private List<SellUnit> sellUnits;         // Все департаменты продаж в розницу
    [SerializeField] private List<SellUnit> sellUnitsPlayer;   // Все департаменты продаж между игроками или их unit


    [SerializeField] private List<MarketItem> marketItems;

    [SerializeField] private List<ProductData> gameProductsDatabase;

    [SerializeField] private List<SaleRecord> lastSalesRecords;
    [SerializeField] private List<SaleRecord> lastPlayerSalesRecords;


    // Реализация продажи товара согласно приоритетам
    public void ExecuteSellerProcess()
    {
        //Debug.Log("Экономическая симуляция");
        LastSalesRecords.Clear();


        if (SellUnits == null)
            return;

        foreach (MarketItem marketItem in MarketItems)
        {
            ProductData product = marketItem.product;
            int remainingDemand = marketItem.Demand;

            List<SellUnit> sellersWithProduct = SellUnits
          .Where(sellUnit => sellUnit != null &&
                 sellUnit.StoredItems != null &&
                 sellUnit.StoredItems.ContainsKey(product) &&
                 sellUnit.StoredItems[product] != null &&
                 sellUnit.StoredItems[product].Amount > 0)
          .ToList();

            if (sellersWithProduct.Count == 0) continue;

            int totalSupply = sellersWithProduct.Sum(seller => seller.StoredItems[product].Amount);
            marketItem.Supply = totalSupply;

            sellersWithProduct.Sort((a, b) =>
                {
                    var firstItem = a.StoredItems[product];
                    var secondItem = b.StoredItems[product];

                    float firstAppeal = firstItem.CalculateAppeal(a.GetCurrentPrice());
                    float secondAppeal = secondItem.CalculateAppeal(b.GetCurrentPrice());

                    return secondAppeal.CompareTo(firstAppeal);
                }
            );

            // Продаём товар, пока есть спрос и предложение
            foreach (SellUnit seller in sellersWithProduct)
            {
                float sellerAppeal = seller.StoredItems[product].CalculateAppeal(seller.GetCurrentPrice());
                
                // Минимальный порог привлекательности товара для клиентаа
                if (sellerAppeal < 0.1)
                    break;

                if (remainingDemand <= 0) break;

                int availableAmount = Mathf.Min(seller.StoredItems[product].Amount, seller.MaxSellCapacity);
                int soldAmount = Mathf.Min(availableAmount, remainingDemand);
                
                //Debug.Log($"{seller.parentBuilding.owner.playerName} sell {product.itemName} for {soldAmount} remaining {seller.storedItems[product].amount}");
                
                if(soldAmount > 0)
                {
                    float revenue = soldAmount * seller.GetCurrentPrice();
                    LastSalesRecords.Add(new SaleRecord(
                        product,
                        seller,
                        soldAmount,
                        revenue,
                        sellerAppeal));

                    seller.StoredItems[product].Amount -= soldAmount;
                    seller.ParentBuilding.Owner.Portfolio.funds += soldAmount * seller.GetCurrentPrice();
                    remainingDemand -= soldAmount;
                    seller.Normalize();
                    DataLogger.Instance.Log($"{seller.ParentBuilding.Owner.playerName}[{seller.UnitName}] продал на рынок [{seller.OutputProduct.ItemName}] по {revenue} $$$");
                }

            }

            //marketItem.Demand = Convert.ToInt32(UnityEngine.Random.Range(0.8f,1.2f) * marketItem.Demand);
        }
    }

    public List<SellUnit> GetValidSellersFor(ProductData product, PlayerBase buyer)
    {
        return SellUnitsPlayer
                       .Distinct()
                       .Where(seller => seller.OutputProduct == product 
                                        &&
                                       (!seller.IsPrivate || (seller.IsPrivate && seller.ParentBuilding.Owner == buyer))
                                        &&
                                       (seller.ParentBuilding.Owner.Portfolio.funds > -2000 || seller.ParentBuilding.Owner == buyer))
                       .ToList();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        Instance = this;

    }

    void Start()
    {
        if(Instance == null)
            Instance = this;

        LastSalesRecords = new List<SaleRecord>();
        LastPlayerSalesRecords = new List<SaleRecord>();

    }
}
