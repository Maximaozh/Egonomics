using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionItem : MonoBehaviour
{
    [Header("UI элементы")]
    public TMP_Text outputTMP;
    public TMP_Text inputTMP;

    public Button upButton;
    public Button downButton;
    public Button deleteButton;

    [Header("Данные")]
    public BuildingUnit output;
    public BuildingUnit input;

    [Header("Иконки")]
    public Image outputIco;
    public Image inputIco;
    public Sprite ErrorSprite;

    public void Start()
    {
        upButton.onClick.AddListener(() => upConnection());
        downButton.onClick.AddListener(()=>downConnetiction());
        deleteButton.onClick.AddListener(() => removeConnection());
    }

    public void Setup(BuildingUnit output, BuildingUnit input)
    {
        this.output = output;
        this.input = input;

        outputTMP.text = output.UnitName;
        inputTMP.text = input.UnitName;

        outputIco.sprite = ErrorSprite;
        inputIco.sprite = ErrorSprite;

        if (output.OutputProduct != null)
            outputIco.sprite = output.OutputProduct.Ico;

        if (input.OutputProduct != null)
            inputIco.sprite = input.OutputProduct.Ico;
    }

    public void upConnection()
    {
        if (output == null || input == null)
            return;

        var building = output.ParentBuilding;
        var connect = building.connections.Find(x => x.target == input && x.source == output);
        var index = building.connections.IndexOf(connect);

        if (connect == null || index <= 0)
            return;

        building.connections.RemoveAt(index);
        building.connections.Insert(index - 1, connect);

        transform.SetSiblingIndex(index - 1);
    }


    public void downConnetiction()
    {
        if (output == null || input == null)
            return;

        var building = output.ParentBuilding;
        var connect = building.connections.Find(x => x.target == input && x.source == output);
        var index = building.connections.IndexOf(connect);

        if (connect == null || index >= building.connections.Count - 1)
            return;

        building.connections.RemoveAt(index);
        building.connections.Insert(index + 1, connect);

        transform.SetSiblingIndex(index + 1);
    }

    public void removeConnection()
    {
        if (output == null || input == null)
            return;
        var building = output.ParentBuilding;
        var connect = building.connections.Find(x => x.target == input && x.source == output);

        if (connect != null)
        {
            building.connections.Remove(connect);
            Destroy(this.gameObject);
        }
    }
}
