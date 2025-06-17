using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MarketItemUI : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text itemPrice;
    public TMP_Text supplyDemandText;

    public Slider supply;
    public Slider demand;

    public float totalSupply;
    public float totalDemand;

    public float supplyCap;
    public float demandCap;

    public GameObject playerItemPrefab;
    public Transform content;

    public Image icon;

    public void Setup(MarketItem item)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        itemName.text = item.product.ItemName;
        itemPrice.text = $"Цена: {item.product.CalculateResultPrice()}";
        supplyDemandText.text = $"Спрос: {item.Demand} | Предложение: {item.Supply}";
        icon.sprite = item.product.Ico;

        var salesByPlayer = MarketManager.Instance.LastSalesRecords
            .Where(s => s.Product == item.product)
            .GroupBy(s => s.Seller.ParentBuilding.Owner)
            .Select(g => new {
                Player = g.Key,
                TotalSold = g.Sum(s => s.AmountSold),
                TotalRevenue = g.Sum(s => s.TotalRevenue),
                Appeal = g.Average(s => s.Appeal)
            })
            .OrderByDescending(x => x.TotalSold) // Сортируем по продажам
            .ToList();

        totalSupply = salesByPlayer.Sum(x => x.TotalSold);
        totalDemand = item.Demand;

        demandCap = MarketManager.Instance.BaseDemand;
        supplyCap = item.Demand;

        demand.value = (totalDemand / demandCap);
        supply.value = (totalSupply / supplyCap);

        foreach (var sale in salesByPlayer)
        {
            string player = sale.Player.playerName;
            int sold = sale.TotalSold;
            int cap = item.Demand;
            float appeal = sale.Appeal;

            GameObject newItem = Instantiate(playerItemPrefab, content);
            newItem.GetComponent<PlayerMarketItemUI>().Setup(player, cap, sold, appeal);
        }

        supply.enabled = false;
        demand.enabled = false;

    }
}

