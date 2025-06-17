using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftUnit : BuildingUnit
{

    public new void Start()
    {
        base.Start();
    }

    public override void Execute(BuildingUnit outputTarget)
    {
        base.Execute(outputTarget);

        if (OutputProduct == null)
            return;

        int max = CalculateMaxCraftAmount();

        if(max > 0)
            CraftProducts(max);

        if (outputTarget != null)
            TransferSelectedProduct(OutputProduct,outputTarget);
    }

    private int CalculateMaxCraftAmount()
    {
        if (OutputProduct.Requireds == null || OutputProduct.Requireds.Count == 0)
            return 0;

        int maxAmount = int.MaxValue;

        foreach (var requirement in OutputProduct.Requireds)
        {
            if (!StoredItems.ContainsKey(requirement.product) || StoredItems[requirement.product].Amount <= 0)
                return 0;

            int resourceAmount = StoredItems[requirement.product].Amount;

            int posible = Mathf.FloorToInt(resourceAmount / requirement.quantity);

            if (posible < maxAmount)
                maxAmount = posible;
        }
        //Debug.Log("max = " + maxAmount);
        return maxAmount > 0 ? maxAmount : 0;
    }


    private void CraftProducts(int amountToCraft)
    {
        float totalQuality = 0f;
        float totalBrand = 0f;
        int totalItemsUsed = 0;

        foreach (var requirement in OutputProduct.Requireds)
        {
            var requiredAmount = Mathf.FloorToInt(requirement.quantity * amountToCraft);
            var productInstance = StoredItems[requirement.product];

            totalQuality += productInstance.Quality * requiredAmount;
            totalBrand  += productInstance.Brand * requiredAmount;
            totalItemsUsed += requiredAmount;

            RemoveProduct(requirement.product, requiredAmount);
        }

        float resultQuality = (totalQuality / totalItemsUsed) + WorkersLevel;
        float resultBrand = (totalBrand / totalItemsUsed) + WorkersLevel;

        var craftedProduct = new ProductInstance(
            OutputProduct,
            resultQuality,
            resultBrand,
            amountToCraft
        );

        AddProductInstance(craftedProduct);
    }
}
