using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductionQueueItem : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text countText;
    public TMP_Text requiresToCraft;
    public Button removeButton;
    public TMP_InputField quantityInputField;

    //private BuildBase.ProductionOrder order;
    private BuildingBase building;

    //public void SetOrder(BuildBase.ProductionOrder order, BuildBase building)
    //{
    //    this.order = order;
    //    this.building = building;

    //    itemNameText.text = order.targetItem.name;
    //    UpdateAmountText();

    //    quantityInputField.text = order.targetAmount.ToString(); 
    //    quantityInputField.onEndEdit.AddListener(OnQuantityChanged); 

    //    removeButton.onClick.AddListener(RemoveOrder);

    //    UpdateRequiredItemsText();
    //}

    //private void OnQuantityChanged(string quantityText)
    //{
    //    if (int.TryParse(quantityText, out int quantity))
    //    {
    //        quantity = Mathf.Clamp(quantity, 1, building.productionLimit);

    //        order.targetAmount = quantity;
    //        UpdateAmountText();
    //        quantityInputField.text = quantity.ToString();
    //    }
    //    else
    //    {
    //        quantityInputField.text = order.targetAmount.ToString();
    //    }

    //}

    //private void UpdateRequiredItemsText()
    //{
    //    string requiredItemsString = "";

    //    foreach (var required in order.targetItem.requireds)
    //    {
    //        requiredItemsString += $"{required.item.name}: {required.quantity}, ";
    //    }
    //    if (requiredItemsString.Length > 2)
    //    {
    //        requiredItemsString = requiredItemsString.Substring(0, requiredItemsString.Length - 2);
    //    }

    //    requiresToCraft.text = requiredItemsString;
    //}

    //private void UpdateAmountText()
    //{
    //    countText.text = $"{order.producedAmount}/{order.targetAmount}";
    //}

    //private void RemoveOrder()
    //{
    //    building.RemoveProductionOrder(building.productionQueue.IndexOf(order));
    //    Destroy(gameObject);
    //}
}
