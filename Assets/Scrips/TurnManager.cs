using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using static EconomyModel;
using static FinancialPortfolio;
using static PlayerManager;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private WinManager winManager;
    public static TurnManager Instance { get; private set; }

    [SerializeField] private List<PlayerBase> players = new List<PlayerBase>();
    [SerializeField] private int userPlayerSlot = 0;
    [SerializeField] private int currentPlayerIndex = 0;

    [SerializeField] private int turnCount = 0;
    [SerializeField] private TMP_Text turnTMP;

    [SerializeField] public bool IsRoundOver { get; private set; }
    public List<PlayerBase> Players { get => players; set => players = value; }
    public int UserPlayerSlot { get => userPlayerSlot; set => userPlayerSlot = value; }
    public int CurrentPlayerIndex { get => currentPlayerIndex; set => currentPlayerIndex = value; }
    public int TurnCount { get => turnCount; set => turnCount = value; }
    public TMP_Text TurnTMP { get => turnTMP; set => turnTMP = value; }

    public event System.Action<int> OnTurnChanged;

    [SerializeField] private string dataFilePath;

    public bool simulation;
    public bool isContinious;

    void Start()
    {
        LoadPlayersFromPlayerManager();
        

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        dataFilePath = Path.Combine(Application.persistentDataPath, $"game_data_{timestamp}.csv");
        Debug.Log(dataFilePath);
        if (!File.Exists(dataFilePath))
        {
            File.WriteAllText(dataFilePath, "Turn;PlayerID;Funds\n");
        }

        if (players.Count == 0)
        {
            return;
        }

        if (userPlayerSlot < 0 || userPlayerSlot >= players.Count)
        {
            userPlayerSlot = 0;
        }


        StartTurn();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void StartTurn()
    {
        PlayerBase currentPlayer = Players[CurrentPlayerIndex];
        currentPlayer.TakeTurn();
    }

    public void EndTurn()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
        OnTurnChanged?.Invoke(CurrentPlayerIndex);
        CheckIfRoundIsOver();
        StartTurn();

    }

    private void CheckIfRoundIsOver()
    {
        if (CurrentPlayerIndex == 0)
        {
            IsRoundOver = true;

            foreach (var player in Players)
            {
                foreach (var building in player.Portfolio.Buildings)
                {
                    building.Process();
                }
            }
            MarketManager.Instance.ExecuteSellerProcess();

            TurnCount += 1;
            if (TurnTMP is not null)
                TurnTMP.text = TurnCount.ToString() + " ход";

            if (TurnCount % 5 == 0)
            {
                string logData = "";
                foreach (var pl in Players)
                {
                    string line = $"{TurnCount};{pl.playerId};{pl.Portfolio.funds}";
                    logData += line + "\n";

                    File.AppendAllText(dataFilePath, line + "\n");
                }
            }

            ToolManager.instance.ActionLeft = ToolManager.ActionMax;

        }
        else
        {
            IsRoundOver = false;
        }
    }



    private void LoadPlayersFromPlayerManager()
    {
        if (!PlayerManager.loaded)
            PlayerManager.LoadAgents();

        players.Clear();
        winManager.TurnLimit = PlayerManager.turnCount + 1;

        PlayerBase mainPlayer = FindObjectOfType<PlayerUser>();
        if (mainPlayer == null)
        {
            return;
        }

        if(Simulation.isSimulation == false)
        {
            players.Add(mainPlayer);
            mainPlayer.playerId = 0;
            mainPlayer.playerName = PlayerManager.Players[0].playerName;
            mainPlayer.Portfolio.funds = PlayerManager.Players[0].money * 100;
            mainPlayer.Portfolio.initialfunds = PlayerManager.Players[0].money * 100;
            ToolManager.ActionMax = PlayerManager.Players[0].actionsPerturnCount;
        }

        for (int i = 1; i < PlayerManager.PlayerCount; i++)
        {
            if (i >= PlayerManager.Players.Count)
            {
                continue;
            }

            PlayerManager.PlayerData playerData = PlayerManager.Players[i];

            GameObject aiPlayerObj = new GameObject($"FSM_Player{i}");
            PlayerFiniteState aiPlayer = aiPlayerObj.AddComponent<PlayerFiniteState>();
            FinancialPortfolio aiPortfolio = aiPlayerObj.AddComponent<FinancialPortfolio>();

            aiPlayer.playerId = i;
            aiPlayer.PlayerName = playerData.playerName;
            aiPlayer.LowerResouceLevel = playerData.lowerRC;
            aiPlayer.HigherResourceLevel = playerData.higherRC;
            aiPlayer.TechnologyChance = playerData.technologyChance;
            aiPlayer.Agressive = playerData.aggersiveValue;
            aiPlayer.ActionsPerTurnMaximum = playerData.actionsPerturnCount;
            aiPlayer.SleepWaitingMaximum = playerData.turnWaitCount;
            aiPlayer.Portfolio = aiPortfolio;
            aiPlayer.advantage = PlayerManager.aiSupport;

            switch (playerData.strategy)
            {
                case PlayerManager.Strategy.production:
                    aiPlayer.SelectedStrategy = PlayerFiniteState.AIStrategy.Produce;
                    break;
                case PlayerManager.Strategy.reseller:
                    aiPlayer.SelectedStrategy = PlayerFiniteState.AIStrategy.Reseller;
                    break;
                case PlayerManager.Strategy.combine:
                    aiPlayer.SelectedStrategy = PlayerFiniteState.AIStrategy.ProduceAndSell;
                    break;
            }

            aiPlayer.Portfolio.funds = playerData.money * 100;
            aiPlayer.Portfolio.initialfunds = playerData.money * 100;

            players.Add(aiPlayer);
        }
    }
}