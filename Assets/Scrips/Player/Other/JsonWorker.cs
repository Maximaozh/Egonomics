
using System.Collections.Generic;
using UnityEngine;
using static TradeItem;
public static class JsonWorker
{
    //public static string SaveToJson(List<TradeItem> items)
    //{
    //    TradeItemList wrapper = new TradeItemList();
    //    wrapper.items = items.ConvertAll(item => ConvertToSerializable(item));
    //    return JsonUtility.ToJson(wrapper, prettyPrint: true);
    //}
    private static TradeItemSerializable ConvertToSerializable(TradeItem item)
    {
        return new TradeItemSerializable
        {
            itemID = item.itemID,
            name = item.name,
            level = item.level,
            category = item.category,
            type = item.type,
            requireds = item.requireds?.ConvertAll(r => new TradeItemRequiredSerializable
            {
                itemID = r.item?.itemID, // Сохраняем только ID
                quantity = r.quantity
            }),
            basePrice = item.basePrice
        };
    }

    public static List<TradeItem> LoadFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("trade_items");
        string json = jsonFile.text;


        TradeItemList wrapper = JsonUtility.FromJson<TradeItemList>(json);


        Dictionary<string, TradeItem> itemDatabase = new Dictionary<string, TradeItem>();

        foreach (var data in wrapper.items)
        {
            TradeItem item = new TradeItem(
                data.itemID,
                data.name,
                data.level,
                data.category,
                data.type,
                null, 
                (int)data.basePrice
            );

            itemDatabase[data.itemID] = item;
        }

        foreach (var data in wrapper.items)
        {
            TradeItem item = itemDatabase[data.itemID];

            if (data.requireds != null)
            {
                item.requireds = data.requireds.ConvertAll(r =>
                    new TradeItem.TradeItemRequired
                    {
                        item = itemDatabase.TryGetValue(r.itemID, out TradeItem requiredItem)
                            ? requiredItem
                            : null,
                        quantity = r.quantity
                    });
            }
        }

        return new List<TradeItem>(itemDatabase.Values);
    }

}

[System.Serializable]
public class TradeItemRequiredSerializable
{
    public string itemID;
    public int quantity;
}

[System.Serializable]
public class TradeItemSerializable
{
    public string itemID;
    public string name;
    public int level;
    public TradeItemCategory category;
    public TradeItemType type;
    public List<TradeItemRequiredSerializable> requireds;
    public float basePrice;
}

[System.Serializable]
public class TradeItemList
{
    public List<TradeItemSerializable> items = new List<TradeItemSerializable>();
}