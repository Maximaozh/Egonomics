using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceUnit : BuildingUnit
{
    [Header("Ресурсы для генерации")]
    public List<ProductData> avaiableResources;

    public new void Start()
    {
        base.Start();

        if (avaiableResources == null)
            return;

        if(OutputProduct == null)
            OutputProduct = avaiableResources[0];
    }
    public override void Execute(BuildingUnit outputTarget)
    {
        base.Execute(outputTarget);
        Produce();
        if (outputTarget != null)
            Transfer(outputTarget);
    }

    public void Produce()
    {
        if (OutputProduct == null)
            return;

        float quality = (WorkersLevel + UnitLevel);
        float brand = 1;
        int produceAmount = (int)((BasePower + BasePower * (Mathf.Min(WorkersLevel, MaxReasonableLevel))) / OutputProduct.BasePrice);

        ProductInstance created = new ProductInstance(OutputProduct, quality, brand, produceAmount);
        AddProductInstance(created);
    }
}
