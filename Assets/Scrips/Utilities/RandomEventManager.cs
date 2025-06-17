using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    public static RandomEventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void TriggerRandomEvent()
    {
        List<RandomEvent> events = new List<RandomEvent>()
        {
            new EconomicBoom(),
            new EconomicCrisis(),
            new NaturalDisaster(),
            new TradeAgreement()
        };

        RandomEvent randomEvent = events[Random.Range(0, events.Count)];

        randomEvent.Activate();
    }
}

public abstract class RandomEvent
{
    public abstract void Activate();
}

public class EconomicBoom : RandomEvent
{
    public override void Activate()
    {
        //Debug.Log("Экономический бум! Цены на все товары увеличиваются на 10%.");
        foreach (var item in EconomyModel.Instance.goods)
        {
            item.purchasePrice *= 1.1f;
            item.salePrice *= 1.1f;
        }
    }
}

public class EconomicCrisis : RandomEvent
{
    public override void Activate()
    {
        //Debug.Log("Экономический кризис! Цены на все товары уменьшаются на 10%.");
        foreach (var item in EconomyModel.Instance.goods)
        {
            item.purchasePrice *= 0.9f;
            item.salePrice *= 0.9f;
        }
    }
}

public class NaturalDisaster : RandomEvent
{
    public override void Activate()
    {
        //Debug.Log("Природная катастрофа! Количество ресурсов уменьшается на 20%.");
        foreach (var item in EconomyModel.Instance.goods)
        {
            if (item.item.type == TradeItem.TradeItemType.Resource)
            {
                item.quantity = (int)(item.quantity*0.8f);
            }
        }
    }
}

public class TradeAgreement : RandomEvent
{
    public override void Activate()
    {
        //Debug.Log("Торговое соглашение! Цены на все товары уменьшаются на 5%.");
        foreach (var item in EconomyModel.Instance.goods)
        {
            item.purchasePrice *= 0.95f;
            item.salePrice *= 0.95f;
        }
    }
}
