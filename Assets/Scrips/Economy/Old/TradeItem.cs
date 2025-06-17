using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TradeItem
{
    public string itemID; // ���������� ID
    public string name;   // ��� �������� ���������
    
    public int level;     // ������� �������� ������

    public TradeItemCategory category;          // ��������� ������
    public TradeItemType type;                  // ��� ������
    
    public List<TradeItemRequired> requireds;   // ������, �� ���� ��������
    
    public float basePrice;                     // ������� ��������� ������
    public float calculatedPrice;               // ������� ��������� ������

    public float brandRating; // ��������� ������� ������ ������
    public float qualityRating; // ��������� �������� ������

    public struct TradeItemRequired // ����� ������� � ��������
    {
        public TradeItem item;  // �������� �������
        public int quantity;    // ������� ������������ ��� ������������
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
