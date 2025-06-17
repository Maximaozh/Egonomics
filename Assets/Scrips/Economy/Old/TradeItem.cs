using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TradeItem
{
    public string itemID; // Уникальный ID
    public string name;   // Это название продукции
    
    public int level;     // Уровень качества товара

    public TradeItemCategory category;          // Категория товара
    public TradeItemType type;                  // Вид товара
    
    public List<TradeItemRequired> requireds;   // Список, из чего создаётся
    
    public float basePrice;                     // Базовая стоимость товара
    public float calculatedPrice;               // Базовая стоимость товара

    public float brandRating; // Описывает рейтинг бренда товара
    public float qualityRating; // Описывает качество товара

    public struct TradeItemRequired // Класс товаров в создании
    {
        public TradeItem item;  // Исходный предмет
        public int quantity;    // Сколько используется при производстве
    }

    public enum TradeItemCategory
    {
        Necessity,
        Basic,
        Advanced,
        Luxury
    }

    public enum TradeItemType
    {
        Resource,
        Production,
        Technology,
        Other
    }

    public TradeItem(string itemID, string name, int level, TradeItemCategory category, TradeItemType type, List<TradeItemRequired> requireds, float basePrice)
    {
        this.itemID = itemID;
        this.name = name;
        this.level = level;
        this.category = category;
        this.type = type;
        this.requireds = requireds;
        this.basePrice = basePrice;
        this.calculatedPrice = 0;
    }

    public void CalculateResultPrice()
    {
        float totalPrice = basePrice; 

        if (requireds != null)
        {
            foreach (var required in requireds)
            {
                if (required.item != null)
                {
                    totalPrice += required.item.calculatedPrice * required.quantity;
                }
            }
        }

        calculatedPrice = totalPrice;
    }


    public override string ToString()
    {
        string requiredsInfo = "";
        if (requireds != null)
        {
            foreach (var required in requireds)
            {
                if (required.item != null)
                {
                    requiredsInfo += $"Required: {required.item.name} x{required.quantity}\n";
                }
                else
                {
                    requiredsInfo += $"Required: Unknown Item x{required.quantity}\n";
                }
            }
        }

        string iconInfo = "Icon: Present";
        return $"Item ID: {itemID}\n" +
               $"Name: {name}\n" +
               $"Level: {level}\n" +
               $"{iconInfo}\n" +
               $"Category: {category}\n" +
               $"Type: {type}\n" +
               $"Base Price: {basePrice}\n" +
               requiredsInfo;
    }
}
