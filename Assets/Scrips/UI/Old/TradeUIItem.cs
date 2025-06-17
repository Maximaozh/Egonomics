using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeUIItem : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameTMP;
    [SerializeField] private TMP_Text itemPriceTMP;
    [SerializeField] private TMP_Text itemQuantityTMP;
    [SerializeField] private TMP_InputField quantityInput;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private TMP_Text transactionInfoTMP;

    public TradeItem item;
    public FinancialPortfolio portfolio;

    public void Initialize(TradeItem tradeItem, FinancialPortfolio playerPortfolio)
    {
        item = tradeItem;
        portfolio = playerPortfolio;

        buyButton.onClick.AddListener(OnBuyClicked);
        sellButton.onClick.AddListener(OnSellClicked);
        quantityInput.onValueChanged.AddListener(OnQuantityChanged);

        transactionInfoTMP.text = "";

        UpdateUI();
    }

    public void UpdateUI()
    {
        var economicItem = EconomyModel.Instance.goods.Find(e => e.item == item);
        itemNameTMP.text = economicItem.item.name;
        
        itemPriceTMP.text = $"Купить: {economicItem.purchasePrice:F2} | Продать: {economicItem.salePrice:F2}";
    }

    public void OnBuyClicked()
    {
        if (int.TryParse(quantityInput.text, out int quantity))
        {
        }
    }

    public void OnSellClicked()
    {
        if (int.TryParse(quantityInput.text, out int quantity))
        {
        }
    }
    private void OnQuantityChanged(string quantityText)
    {
        if (int.TryParse(quantityText, out int quantity))
        {
            var economicItem = EconomyModel.Instance.goods.Find(e => e.item == item);

            if (quantity > 0)
            {
                float buyCost = economicItem.purchasePrice * quantity;
                float sellEarnings = economicItem.salePrice * quantity;

                transactionInfoTMP.text = $"Купить: -{buyCost:F2} | Продать: +{sellEarnings:F2}";
            }
            else
            {
                transactionInfoTMP.text = "";
            }
        }
        else
        {
            transactionInfoTMP.text = "";
        }
    }

}
