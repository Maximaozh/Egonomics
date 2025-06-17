using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[SerializeField]
public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject tradeCanvas;
    [SerializeField] private TradeUI tradeUI;
    [SerializeField] private BuildUI buildUI;
    [SerializeField] private GameObject buildCanvas;
    [SerializeField] private PlayerUser player;

    [SerializeField] private Button TradeButton;
    [SerializeField] private Button BuildButton;
    [SerializeField] private Button PortfolioInfoButton;
    [SerializeField] private Button EndTurnButton;
    [SerializeField] private Button ExitButton;
    [SerializeField] private TMP_Text FundsTMP;


    [SerializeField] private GameObject PortfolioInfoPanel;
    [SerializeField] private TMP_Text PortfolioInfoTMP;
    [SerializeField] List<PlayerBase> players;

    void Start()
    {
        //TradeButton.onClick.AddListener(ToggleTradeUI);
        //BuildButton.onClick.AddListener(ToggleBuildUI);
        //EndTurnButton.onClick.AddListener(TurnEnd);
        //ExitButton.onClick.AddListener(ExitApplication);

        //player.Portfolio.OnFundsUpdated += UpdateFundsDisplay;

        //PortfolioInfoPanel.SetActive(false);
        //PortfolioInfoButton.onClick.AddListener(DisplayPortfolioValues);

        //tradeCanvas.SetActive(false);
        //buildCanvas.SetActive(false);
        //UpdateFundsDisplay();
    }
    void DisplayPortfolioValues()
    {
        if(PortfolioInfoPanel.activeInHierarchy == true)
        {
            PortfolioInfoPanel.SetActive(false);
            return;
        }
        
        PortfolioInfoPanel.SetActive(true); 

        string displayText = "";

        foreach (PlayerBase player in players)
        {
            float totalValue = CalculatePortfolioValue(player.Portfolio);
            displayText += $"{player.PlayerName} - {totalValue:F2}\n"; 
        }

        PortfolioInfoTMP.text = displayText;
    }

    float CalculatePortfolioValue(FinancialPortfolio portfolio)
    {
        float totalValue = portfolio.Funds;

        return totalValue;
    }
    public void ToggleTradeUI()
    {
        bool isActive = !tradeCanvas.activeSelf;
        tradeCanvas.SetActive(isActive);

        if (isActive)
            tradeUI.InitializeUI();
    }
    public void ToggleBuildUI()
    {
        bool isActive = !buildCanvas.activeSelf;
        buildCanvas.SetActive(isActive);

        if (isActive)
            buildUI.InitializeUI();
    }
    public void TurnEnd()
    {
        player.EndTurn();
    }
    private void UpdateFundsDisplay()
    {
        FundsTMP.text = $"$: {player.Portfolio.Funds}";
    }

    
    public void ExitApplication()
    {
        Application.Quit();
    }
}
