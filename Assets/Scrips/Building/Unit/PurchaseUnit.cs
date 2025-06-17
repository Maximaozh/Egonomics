using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurchaseUnit : BuildingUnit
{
    [Header("Настройки закупки")]
    [SerializeField] private SellUnit sellerUnit;
    [SerializeField] private float suggestedMultiplier = 1f;
    [SerializeField] private bool fillFromMarket = false;
    [SerializeField] private string sellerString;

    [SerializeField] private float marketMultiplier = 1.2f;
    public SellUnit SellerUnit { get => sellerUnit; set => sellerUnit = value; }
    public float SuggestedMultiplier { get => suggestedMultiplier; set => suggestedMultiplier = value; }
    public bool FillFromMarket { get => fillFromMarket; set => fillFromMarket = value; }
    public string SellerString { get => SellerUnit.UnitName;}

    public override void Execute(BuildingUnit outputTarget)
    {
        if (OutputProduct == null)
            return;

        if (FillFromMarket)
        {
            MarketFill();
            DataLogger.Instance.Log($"{this.ParentBuilding.Owner.playerName}[{this.UnitName}] закупил [{OutputProduct.ItemName}] с рынка по {OutputProduct.CalculateResultPrice()* marketMultiplier}");
        }

        if (outputTarget != null)
        {
            Transfer(outputTarget);
        }
    }

    public void MarketFill()
    {
        if (ParentBuilding == null || ParentBuilding.Owner == null) return;

        int maxAmountByStorage = MaxStorageCapacity - GetCurrentItemAmount(OutputProduct);
        if (maxAmountByStorage <= 0) return;

        float itemPrice = OutputProduct.CalculateResultPrice();

        // Учитываем множитель здания
        if (ParentBuilding.BuildingType != BuildingType.Shop)
            itemPrice *= marketMultiplier;
        else
            itemPrice *= (marketMultiplier * 1.5f);

        // Ограничиваем максимальную трату 30% текущих средств
        float maxSpend = ParentBuilding.Owner.Portfolio.funds * 0.3f;
        int maxAffordable = Mathf.FloorToInt(maxSpend / itemPrice);

        if (maxAffordable <= 0) return;

        int amountToBuy = Mathf.Min(
            maxAmountByStorage,
            maxAffordable,
            OutputCapacity
        );

        if (amountToBuy <= 0) return;

        float totalCost = amountToBuy * itemPrice;

        // Дополнительная проверка перед списанием
        if (ParentBuilding.Owner.Portfolio.funds >= totalCost)
        {
            var marketProduct = new ProductInstance(
                OutputProduct,
                1.0f,
                1.0f,
                amountToBuy
            );

            AddProductInstance(marketProduct);
            ParentBuilding.Owner.Portfolio.funds -= totalCost;
        }
    }

    public void CancelOrder()
    {
        if(SellerUnit != null)
            SellerUnit.RemoveOrder(this);

        SellerUnit = null;
    }

    public void SetOrder(SellUnit seller, ProductData data, float multiplier)
    {
        if (SellerUnit != null)
            SellerUnit.RemoveOrder(this);

        SellerUnit = seller;
        OutputProduct = data;
        SuggestedMultiplier = multiplier;

        if (SellerUnit != null)
            SellerUnit.AddOrder(this);
    }

    public void ChangeOrderMultiplier(float multiplier)
    {
        SuggestedMultiplier = multiplier;
    }
}