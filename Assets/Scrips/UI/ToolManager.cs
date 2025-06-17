using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolManager : MonoBehaviour
{
    public static ToolManager instance;

    private int actionLeft;
    private static int actionMax = 5;

    public static int ActionMax { get { return actionMax; } set { actionMax = value > 0 ? value : 0; } }
    public int ActionLeft { 
        get 
        { 
            return actionLeft; 
        } 
        set
        {
            actionLeft = value;

            if (actionLeftTMP != null)
                actionLeftTMP.text = actionLeft > 0 ? $"{actionLeft} V" : $"{actionLeft} X";
        }
    }

    [Header("Панели содержания")]
    public GameObject marketPanel;
    public GameObject buildingPanel;
    public GameObject constructPanel;
    public GameObject productPanel;
    public GameObject leaderBoardPanel;
    public GameObject selectedBuildingPanel;
    public GameObject historyPanel;

    [Header("Кнопки управления")]
    public Button marketButton;
    public Button buildingButton;
    public Button constructButton;
    public Button productButton;
    public Button leaderBoardButton;
    public Button hideButton;
    public Button turnEndButton;
    public Button ExitButton;
    public Button HistoryButton;

    [Header("Время деньги действия")]
    public TMP_Text turnCount;
    public TMP_Text fundsPlayer;
    public TMP_Text actionLeftTMP;

    [Header("Игровая информация")]
    public SelectedBuildInfoUI selectedBuildingUI;
    public PlayerBase player;

    BuildingAbstract selectedBuilding;


    private void Start()
    {
        ActionLeft = ActionMax;

        if (ToolManager.instance == null)
            ToolManager.instance = this;

        marketButton.onClick.AddListener(() => ShowView(marketPanel));
        buildingButton.onClick.AddListener(() => ShowView(buildingPanel));
        constructButton.onClick.AddListener(() => ShowView(constructPanel));
        productButton.onClick.AddListener(() => ShowView(productPanel));
        leaderBoardButton.onClick.AddListener(() => ShowView(leaderBoardPanel));
        HistoryButton.onClick.AddListener(() => ShowView(historyPanel));
        turnEndButton.onClick.AddListener(() => EndTurn());

        hideButton.onClick.AddListener(() => HideAllViews());
        selectedBuildingUI.closeButton.onClick.AddListener(HideAllViews);

        ExitButton.onClick.AddListener(() => Application.Quit());
        HideAllViews();
    }

    public void ShowBuildingInfo(BuildingAbstract building)
    {
        selectedBuilding = building;
        HideAllViews();
        selectedBuildingPanel.SetActive(true);
        selectedBuildingUI.Setup(selectedBuilding);
    }

    public void ReloadBuildingWindow()
    {
        HideAllViews();
        selectedBuildingPanel.SetActive(true);
        selectedBuildingUI.Setup(selectedBuilding);
    }

    private void ShowView(GameObject viewToShow)
    {
        bool status = viewToShow.activeSelf;
        DecreaseActions();
        HideAllViews();
        viewToShow.SetActive(!status);
    }

    private void HideAllViews()
    {
        marketPanel.SetActive(false);
        buildingPanel.SetActive(false);
        constructPanel.SetActive(false);
        productPanel.SetActive(false);
        leaderBoardPanel.SetActive(false);
        selectedBuildingPanel.SetActive(false);
        historyPanel.SetActive(false);
    }

    private void EndTurn()
    {
        player.EndTurn();
        actionLeft = actionMax;
        changeUIStatus(true);

        HideAllViews();
        DataLogger.Instance.Clear();
    }

    internal bool IsAnyDialogOpen()
    {
        return selectedBuildingPanel.activeInHierarchy ||
               marketPanel.activeInHierarchy ||
               buildingPanel.activeInHierarchy ||
               constructPanel.activeInHierarchy ||
               productPanel.activeInHierarchy ||
               leaderBoardPanel.activeInHierarchy ||
               historyPanel.activeInHierarchy;
    }

    public void DecreaseActions()
    {
        ActionLeft--;

        if(ActionLeft <= 0 )
        {
            changeUIStatus(false);
        }
    }

    public void changeUIStatus(bool status)
    {
        marketButton.gameObject.SetActive(status);
        buildingButton.gameObject.SetActive(status);
        constructButton.gameObject.SetActive(status);
        productButton.gameObject.SetActive(status);
        leaderBoardButton.gameObject.SetActive(status);
        HistoryButton.gameObject.SetActive(status);
    }
}
