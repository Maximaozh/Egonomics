using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildExpandedItemUI : MonoBehaviour
{

    [SerializeField] private Button button;
    [SerializeField] private TMP_Text title;

    public BuildingAbstract buildPrefab;
    public PlayerBase playerBase;
    public BuildExpandedUI buildUI;

    public void Initialize(BuildingAbstract build, BuildExpandedUI buildUI, PlayerBase player)
    {
        this.buildPrefab = build;
        this.buildUI = buildUI;
        this.playerBase = player;

        //Debug.Log(buildPrefab);
        //Debug.Log(buildUI);
        //Debug.Log(player);

        title.text = build.BuildingName;
        button.image.sprite = build.buildingSprite;

        button.onClick.AddListener(OnClick);

        if (player.Portfolio.Funds < build.PurchaseCost)
        {
            button.interactable = false;
        }
    }

    void OnClick()
    {
        buildUI.SetSelectedBuild(buildPrefab);
        buildUI.gameObject.SetActive(false);
    }
}
