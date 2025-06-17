using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildUIItem : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text title;
    
    public BuildingBase buildPrefab;
    public PlayerBase playerBase;
    public BuildUI buildUI;

    public void Initialize(BuildingBase build, BuildUI buildUI, PlayerBase player)
    {
        this.buildPrefab = build;
        this.buildUI = buildUI;
        this.playerBase = player;
        
        //Debug.Log(buildPrefab);
        //Debug.Log(buildUI);
        //Debug.Log(player);

        title.text = build.buildingName;
        button.image.sprite = build.buildingSprite;

        button.onClick.AddListener(OnClick);

        if (player.Portfolio.Funds < build.cost)
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
