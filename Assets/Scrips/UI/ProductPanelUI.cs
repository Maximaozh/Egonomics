using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductPanelUI : MonoBehaviour
{
    public GameObject productRecipePrefab;
    public Transform content;

    private void OnEnable()
    {
        FillData();
    }

    private void OnDisable()
    {
    }

    private void FillData()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        if (MarketManager.Instance == null || MarketManager.Instance.GameProductsDatabase == null)
            return;

        foreach (var item in MarketManager.Instance.GameProductsDatabase)
        {
            GameObject newItem = Instantiate(productRecipePrefab, content);
            newItem.GetComponent<CraftRecipeUI>().Configure(item);
        }
    }
}
