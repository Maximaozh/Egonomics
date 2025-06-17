using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseUnitGUI : UnitGUI
{
    [Header("UI специализированные")]
    public TMP_Dropdown avaiableProducts;
    public TMP_Dropdown avaiableSellers;

    public TMP_Text position;

    public Slider priceMultiplier;

    public Toggle BuyFromExternalMarket;

    public Button confirmButton;

    public TMP_Text sellerTMP;

    public TMP_Text selectedProductTMP;
    public TMP_Text priceMultiplierTMP;


    public Button Reset;

    public override void Configure(Unit unit, string extra)
    {
        base.Configure(unit, extra);
        ConfigureSpecial();
    }

    public new void Start()
    {
        avaiableProducts.onValueChanged.AddListener((x) => OnProductValueChanged(x));
        avaiableSellers.onValueChanged.AddListener((x) => OnSellerValueChanged(x));
        priceMultiplier.onValueChanged.AddListener((x) => PriceSliderChanged(x));
        BuyFromExternalMarket.onValueChanged.AddListener((x) => MarketToggleChanged(x));
        confirmButton.onClick.AddListener(() => { OnConfirmButtonClicked(); RefreshFullUI(); });
    }

    public void OnConfirmButtonClicked()
    {
        PurchaseUnit ps = (PurchaseUnit)unit.info;
        
        if (ps == null)
            return;

        var sellers = MarketManager.Instance.GetValidSellersFor(ps.OutputProduct, ps.ParentBuilding.Owner);
        var desiredProduct = MarketManager.Instance.GameProductsDatabase[avaiableProducts.value];

        ps.CancelOrder();

        SellUnit seller = null;
        if (sellers.Count > 0)
            seller = sellers[avaiableSellers.value];


        if (seller == null || BuyFromExternalMarket.isOn)
            sellerTMP.text = "Рынок";
        else
            sellerTMP.text = seller.UnitName;

        ps.SetOrder(seller, desiredProduct, ps.SuggestedMultiplier);
        Configure(this.unit,"");
    }

    public void ConfigureSpecial()
    {
        avaiableSellers.ClearOptions();
        avaiableProducts.ClearOptions();
        
        PurchaseUnit ps = (PurchaseUnit)unit.info;

        priceMultiplierTMP.text = ps.SuggestedMultiplier.ToString("0.00");

        if(ps.OutputProduct != null)
            selectedProductTMP.text = ps.OutputProduct.ItemName;

        priceMultiplier.value = ps.SuggestedMultiplier;

        List<TMP_Dropdown.OptionData> optionsProducts = new List<TMP_Dropdown.OptionData>();

        foreach (var product in MarketManager.Instance.GameProductsDatabase)
        {
            optionsProducts.Add(new TMP_Dropdown.OptionData()
            {
                text = $"{product.ItemName}",
                image = product.Ico
            });
        }

        avaiableProducts.AddOptions(optionsProducts);

        if(ps.OutputProduct != null)
            avaiableProducts.value = MarketManager.Instance.GameProductsDatabase.FindIndex(x => x == ps.OutputProduct);

        if (ps.SellerUnit != null)
        {
            avaiableSellers.value = MarketManager.Instance.GetValidSellersFor(ps.OutputProduct, ps.ParentBuilding.Owner).IndexOf(ps.SellerUnit);
            position.text = ps.SellerUnit.GetOrderPosition(ps).ToString();
            sellerTMP.text = ps.SellerUnit.UnitName;
        }


        if (ps.FillFromMarket)
            sellerTMP.text = "Рынок";
        
        BuyFromExternalMarket.isOn = ps.FillFromMarket;
        OnProductValueChanged(avaiableProducts.value);
    }

    public void OnProductValueChanged(int index)
    {
        avaiableSellers.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        PurchaseUnit ps = (PurchaseUnit)unit.info;

        ps.OutputProduct = MarketManager.Instance.GameProductsDatabase[index];

        foreach (var seller in MarketManager.Instance.GetValidSellersFor(ps.OutputProduct, ps.ParentBuilding.Owner))
        {
            ProductInstance resource;
                var tryToGetResource = seller.TryGetProductInstance(ps.OutputProduct, out resource);

            if (tryToGetResource == false)
            {
                options.Add(new TMP_Dropdown.OptionData()
                {
                    text = $"{seller.UnitName}[{seller.ParentBuilding.Owner.playerName}] -: М:{seller.PriceMultiplier.ToString("0.00")} Ц:{(ps.OutputProduct.CalculateResultPrice()* seller.PriceMultiplier).ToString("0.00")} шт."
                });
            } else
            {
                options.Add(new TMP_Dropdown.OptionData()
                {
                    text = $"{seller.UnitName}[{seller.ParentBuilding.Owner.playerName}] +: К:{resource.Quality} Б:{resource.Brand} Х:{resource.Amount} М:{seller.PriceMultiplier.ToString("0.00")} Ц:{(seller.OutputProduct.CalculateResultPrice() * seller.PriceMultiplier).ToString("0.00")}"
                });
            }

        }

        ps.OutputProduct = MarketManager.Instance.GameProductsDatabase[index];
        selectedProductTMP.text = ps.OutputProduct.ItemName;
        avaiableSellers.AddOptions(options);
    }

    public void OnSellerValueChanged(int index)
    {
        PurchaseUnit ps = (PurchaseUnit)unit.info;
        var sellers = MarketManager.Instance.GetValidSellersFor(ps.OutputProduct, ps.ParentBuilding.Owner);

        ps.CancelOrder();
        ps.SetOrder(sellers[index], ps.OutputProduct,ps.SuggestedMultiplier);

        sellerTMP.text = sellers[index].UnitName;
        
        if (BuyFromExternalMarket.isOn)
            sellerTMP.text = "Рынок";

        position.text = sellers[index].GetOrderPosition(ps).ToString();
    }

    public void PriceSliderChanged(float sliderValue)
    {
        if (sliderValue > 2.0f || sliderValue < 0)
            return;

        var ps = (PurchaseUnit)unit.info;

        if (BuyFromExternalMarket.isOn)
            return;

        ps.SuggestedMultiplier = sliderValue;

        var sellers = MarketManager.Instance.GetValidSellersFor(ps.OutputProduct, ps.ParentBuilding.Owner);
                
        if (sellers != null && sellers.Count > 0 && ps.SellerUnit != null)
            position.text = ps.SellerUnit.GetOrderPosition(ps).ToString();

        priceMultiplierTMP.text = ps.SuggestedMultiplier.ToString("0.00");

        Configure(this.unit, "");
    }

    public void MarketToggleChanged(bool value)
    {
       

        var ps = (PurchaseUnit)unit.info;
        ps.FillFromMarket = value;


        Configure(this.unit, "");
    }
}
