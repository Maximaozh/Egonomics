using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProductInstance
{
    private ProductData data;      // Ссылка на ScriptableObject
    private float quality = 1.0f;  // 0.5 - 1.5 (50%-150%)
    private float brand;           // Название бренда
    private int amount = 0;        // Количество товара

    public ProductData Data { get => data; set => data = value; }
    public float Quality { get => quality; set => quality = value; }
    public float Brand { get => brand; set => brand = value; }
    public int Amount { get => amount; set => amount = value; }

    public float CalculateAppeal(float playerPrice)
    {
        float brandFactor = Mathf.Clamp01(Brand / 100f);
        float qualityFactor = Mathf.Clamp01(Quality / 100f);
        float priceFactor = 1f;
        if (playerPrice > 0)
        {
            float priceRatio = Data.CalculateResultPrice() / playerPrice;
            priceFactor = Mathf.Exp(-Mathf.Pow(priceRatio - 1f, 2f));
        }
        float appeal = brandFactor +
                      qualityFactor +
                      priceFactor;

        return appeal;
    }

    public ProductInstance(ProductData data, float quality, float brand, int amount)
    {
        this.Data = data;
        this.Quality = quality;
        this.Brand = brand;
        this.Amount = amount;
    }

    public float GetPrice()
    {
        return Data.CalculateResultPrice() * Quality;
    }
}
