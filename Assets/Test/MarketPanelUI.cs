using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketPanelUI : MonoBehaviour
{
    public GameObject marketItemPrefab;
    public Transform content;

    private void OnEnable()
    {
        FillMarketData();
    }

    private void OnDisable()
    {
    }

    private void FillMarketData()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        if (MarketManager.Instance == null || MarketManager.Instance.MarketItems == null)
            return;

        foreach (var item in MarketManager.Instance.MarketItems)
        {            
            GameObject newItem = Instantiate(marketItemPrefab, content);
            newItem.GetComponent<MarketItemUI>().Setup(item);
        }
    }
}
