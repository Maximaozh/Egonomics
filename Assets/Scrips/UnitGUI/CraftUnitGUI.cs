using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftUnitGUI : UnitGUI
{
    [Header("Ёлементы UI —пециализированные")]
    public TMP_Dropdown avaiableList;
    public Button buildButton;
    public TMP_Text selectedProduct;
    public SelectedBuildInfoUI selectedBuildingPanel;
    public override void Configure(Unit unit, string extra)
    {
        base.Configure(unit, extra);
        configureSpecial();

        var ps = (CraftUnit)unit.info;

        if (ps.OutputProduct != null)
            selectedProduct.text = ps.OutputProduct.ItemName;
    }


    public new void Start()
    {
        buildButton.onClick.AddListener(() => { SelectProduct(); RefreshFullUI(); });
    }

    public void configureSpecial()
    {
        avaiableList.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (var avaiable in MarketManager.Instance.GameProductsDatabase.Where(x => x.Requireds != null && x.Requireds.Count != 0).ToList())
        {
            options.Add(new TMP_Dropdown.OptionData()
            {
                text = $"{avaiable.ItemName}", image = avaiable.Ico
            });
        }

        avaiableList.AddOptions(options);
    }

    public void SelectProduct()
    {
        var ps = (CraftUnit)unit.info;

        if (ps == null)
            return;

        var list = MarketManager.Instance.GameProductsDatabase.Where(x => x.Requireds != null && x.Requireds.Count != 0).ToList();

        if (list == null || list.Count == 0)
            return;

        if (avaiableList.options.Count == 0)
            return;

        ps.OutputProduct = list[avaiableList.value];

        Configure(this.unit,"");
    }

}
