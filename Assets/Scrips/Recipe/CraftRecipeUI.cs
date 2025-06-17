using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftRecipeUI : MonoBehaviour
{
    [Header("������� ����������")]
    public GameObject craftImage;
    public GameObject detailedInfo;

    [Header("UI ��������")]
    public TMP_Text itemNameTMP;
    public Image resultProductImageTop;
    public Image resultProductImageBottom;

    [Header("�������� ����������")]
    public Transform craftImageContent;
    public Transform detailedInfoContent;

    [Header("�������")]
    public ProductData product;

    public void Configure(ProductData data)
    {
        product = data;

        if (product == null)
            return;

        itemNameTMP.text = product.ItemName;
        resultProductImageBottom.sprite = product.Ico;
        resultProductImageTop.sprite = product.Ico;

        if(product.Requireds == null || product.Requireds.Count == 0)
        {
            detailedInfoContent.gameObject.SetActive(false);
            craftImageContent.gameObject.SetActive(false);
            itemNameTMP.text += " (������ ������� ��� ������)";
        } else
            foreach (var required in product.Requireds)
            {
                GameObject ci = Instantiate(craftImage, craftImageContent);
                ci.transform.SetAsFirstSibling();
                var imageScript = ci.GetComponent<IconRecipeCountUI>();
                imageScript.Configure(required.quantity, required.product.Ico);

                GameObject di = Instantiate(detailedInfo, detailedInfoContent);
                var detailScript = di.GetComponent<ItemRecipeUI>();
                detailScript.Configure(required.product.ItemName, required.quantity, required.product.Ico);
            }
    }
}
