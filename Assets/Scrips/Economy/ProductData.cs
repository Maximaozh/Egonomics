using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
[CreateAssetMenu(fileName = "Economy", menuName = "Inventory/Product")]
public class ProductData : ScriptableObject
{
    [SerializeField] private int itemID; // Уникальный ID
    [SerializeField] private string itemName;   // Это название продукции

    [SerializeField] private int level;     // Уровень качества товара

    [SerializeField] private ProductType type;                        // Вид товара

    [SerializeField] private List<ProductRequirements> requireds;     // Список, из чего создаётся

    [SerializeField] private float basePrice;                         // Базовая стоимость товара

    [SerializeField] private Sprite ico;                               // Изображение объекта

    public int ItemID { get => itemID; set => itemID = value; }
    public string ItemName { get => itemName; set => itemName = value; }
    public int Level { get => level; set => level = value; }
    public ProductType Type { get => type; set => type = value; }
    public List<ProductRequirements> Requireds { get => requireds; set => requireds = value; }
    public float BasePrice { get => basePrice; set => basePrice = value; }
    public Sprite Ico { get => ico; set => ico = value; }

    [System.Serializable]
    public struct ProductRequirements // Класс товаров в создании
    {
        public ProductData product;  // Исходный предмет
        public int quantity;    // Сколько используется при производстве

        public ProductRequirements(ProductData item, int count)
        {
            product = item;
            quantity = count;
        }
    }

    public enum ProductType
    {
        Resource,
        Production,
        Technology,
        Other
    }

    public ProductData(int itemID, string name, int level, ProductType type, List<ProductRequirements> requireds, float basePrice)
    {
        this.ItemID = itemID;
        this.ItemName = name;
        this.Level = level;
        this.Type = type;
        this.Requireds = requireds;
        this.BasePrice = basePrice;
    }

    public float CalculateResultPrice()
    {
        float totalPrice = BasePrice;

        if (Requireds != null)
        {
            foreach (var required in Requireds)
            {
                if (required.product != null)
                {
                    totalPrice += required.product.CalculateResultPrice() * required.quantity;
                }
            }
        }

        var mi = MarketManager.Instance.MarketItems.Where(x => x.product == this).FirstOrDefault();
        if(mi != null)
        {
            int demand = mi.Demand;
            int supply = mi.Supply;
            float necessity = mi.Necessity;

            totalPrice = totalPrice * (1 + Mathf.Clamp(0.1f * (demand * necessity) / (supply  + 0.005f),0, 0.1f));
        }

        return totalPrice;
    }

   

    public override string ToString()
    {
        string requiredsInfo = "";
        if (Requireds != null)
        {
            foreach (var required in Requireds)
            {
                if (required.product != null)
                {
                    requiredsInfo += $"Required: {required.product.ItemName} x {required.quantity}\n";
                }
                else
                {
                    requiredsInfo += $"Required: Unknown Item x{required.quantity}\n";
                }
            }
        }

        string iconInfo = "Icon: Present";
        return $"Item ID: {ItemID}\n" +
               $"Name: {ItemName}\n" +
               $"Level: {Level}\n" +
               $"{iconInfo}\n" +
               $"Type: {Type}\n" +
               $"Base Price: {BasePrice}\n" +
               requiredsInfo;
    }
}
