using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinManager : MonoBehaviour
{
    
    [Header("Îáúåêòû")]
    [SerializeField] private GameObject mainGUI;
    [SerializeField] private GameObject winnerGUI;

    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private Button leaveButton;

    [SerializeField] private TurnManager turnManager;

    [SerializeField] private int turnLimit = 50;

    [SerializeField] private bool isContiniue = true;

    public GameObject MainGUI { get => mainGUI; set => mainGUI = value; }
    public GameObject WinnerGUI { get => winnerGUI; set => winnerGUI = value; }
    public TMP_Text WinnerText { get => winnerText; set => winnerText = value; }
    public Button LeaveButton { get => leaveButton; set => leaveButton = value; }
    public TurnManager TurnManager { get => turnManager; set => turnManager = value; }
    public int TurnLimit { get => turnLimit; set => turnLimit = value; }

    public void LateUpdate()
    {
        if (TurnManager.TurnCount >= TurnLimit && isContiniue) 
        {
            MainGUI.SetActive(false);
            WinnerGUI.SetActive(true);

            var winner = TurnManager.Players.OrderByDescending(x => x.Portfolio.funds).FirstOrDefault();

            WinnerText.text = winner.playerName + " ÏÎÁÅÄÈË!\nÑ÷¸ò:" + winner.Portfolio.funds;

            LeaveButton.onClick.AddListener(() => Application.Quit());
            turnManager.isContinious = false;

            if(Simulation.isSimulation)
            {
                Debug.Log(Simulation.simCount);
                Simulation.simCount--;
                if (Simulation.simCount <= 0)
                {
                #if UNITY_EDITOR
                    EditorApplication.ExitPlaymode();
#else
                    Application.Quit();
#endif
                }
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            }
        }
    }

}
