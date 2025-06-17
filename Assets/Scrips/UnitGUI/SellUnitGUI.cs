using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellUnitGUI : UnitGUI
{
    [Header("Ёлементы UI —пециализированные")]
    public TMP_Dropdown avaiableList;
    public TMP_Text output;
    public Button selectButton;

    public Toggle privateUnitToggle;

    public Slider priceSlider;
    public TMP_Text priceTMP;

    public override void Configure(Unit unit, string extra)
    {
        base.Configure(unit, extra);
        configureSpecial();

        var sellUnit = (SellUnit)unit.info;


        if (sellUnit == null)
            return;
        
        priceTMP.text = sellUnit.PriceMultiplier.ToString("0.00");
        priceSlider.value = sellUnit.PriceMultiplier;
        privateUnitToggle.isOn = sellUnit.IsPrivate;
    }

    public new void Start()
    {
        selectButton.onClick.AddListener(() => { SelectResource(); RefreshFullUI();});
        priceSlider.onValueChanged.AddListener((x) => PriceSliderChange(x));
        privateUnitToggle.onValueChanged.AddListener((x) => PrivateToggleChanged(x));
    }


    public void configureSpecial()
    {
        avaiableList.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        SellUnit ps = (SellUnit)unit.info;

        if(ps.OutputProduct != null)
            output.text = ps.OutputProduct.ItemName;

        foreach (var resource in MarketManager.Instance.GameProductsDatabase)
        {
            options.Add(new TMP_Dropdown.OptionData()
            {
                text = $"{resource.ItemName}",
                image = resource.Ico
            });
        }

        avaiableList.AddOptions(options);

        if (ps.OutputProduct != null)
        {
            int selectedIndex = MarketManager.Instance.GameProductsDatabase.FindIndex(r => r == ps.OutputProduct);
            if (selectedIndex >= 0)
            {
                avaiableList.value = selectedIndex;
            }
        }
    }

    public void PriceSliderChange(float sliderValue)
    {        
        if (sliderValue > 2.0f || sliderValue < 0)
            return;

        var sellUnit = (SellUnit)unit.info;

        sellUnit.PriceMultiplier = sliderValue;

        priceTMP.text = sliderValue.ToString("0.00");

        Configure(this.unit, "");
    }

    public void PrivateToggleChanged(bool status)
    {
        var sellUnit = (SellUnit)unit.info;
        sellUnit.IsPrivate = status;
        Configure(this.unit, "");
    }


    public void SelectResource()
    {
        SellUnit ps = (SellUnit)unit.info;
        int selectedIndex = avaiableList.value;
        ps.OutputProduct = MarketManager.Instance.GameProductsDatabase[selectedIndex];
        ps.orders.Clear();
        Configure(this.unit, "");
    }
}
