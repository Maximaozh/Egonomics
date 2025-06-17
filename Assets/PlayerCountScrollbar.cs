using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCountScrollbar : MonoBehaviour
{
    [Header("UI элементы")]
    public Slider sb;
    public TMP_Text playerCountTMP;
    public GameObject agentUIPrefab;
    public GameObject contentPlayers;


    public Slider ah;
    public TMP_Text ahTMP;


    public Slider tc;
    public TMP_Text tcTMP;


    private int targetPlayerCount;
    private bool isChanging = false;
    private float lastChangeTime;
    private const float changeDelay = 0.1f;


    public void Start()
    {
        sb.minValue = PlayerManager.minPlayerCount;
        sb.maxValue = PlayerManager.maxPlayerCount;
        sb.value = PlayerManager.PlayerCount;
        playerCountTMP.text = PlayerManager.PlayerCount.ToString();

        ah.minValue = 500;
        ah.maxValue = 5000;
        ah.value = PlayerManager.aiSupport;
        ahTMP.text = PlayerManager.aiSupport.ToString();


        tc.minValue = 10;
        tc.maxValue = 100;
        tc.value = PlayerManager.turnCount;
        tcTMP.text = PlayerManager.turnCount.ToString();

        targetPlayerCount = PlayerManager.PlayerCount;


        if (PlayerManager.loaded == true)            
            for (int i = 0; i < PlayerManager.PlayerCount; i++)
            {
                AddAgentGO(i);
            }

        if(contentPlayers.transform.childCount > 0)
        {
            var gop = contentPlayers.transform.GetChild(0);

            for(int i = 1; i < gop.transform.childCount; i++)
            {
                var gopc = gop.transform.GetChild(i);
                gopc.gameObject.SetActive(false);
            }

            gop.GetChild(gop.childCount - 4).gameObject.SetActive(true);
            gop.GetChild(gop.childCount - 2).gameObject.SetActive(true);

        }

        sb.onValueChanged.AddListener(x => HandleSlider(x));
        ah.onValueChanged.AddListener(x => HandleSupport(x));
        tc.onValueChanged.AddListener(x => HandleTurn(x));
    }




    private void HandleSupport(float value)
    {
        int intValue = Mathf.FloorToInt(value);
        ahTMP.text = intValue.ToString("0");
        PlayerManager.aiSupport = intValue;
    }
    private void HandleTurn(float value)
    {
        int intValue = Mathf.FloorToInt(value);
        tcTMP.text = intValue.ToString("0");
        PlayerManager.turnCount = intValue;
    }

    private void HandleSlider(float value)
    {
        int newTarget = Mathf.CeilToInt(value);
        if (newTarget != targetPlayerCount)
        {
            targetPlayerCount = newTarget;
            lastChangeTime = Time.time;

            if (!isChanging)
            {
                StartCoroutine(ProcessPlayerCountChange());
            }
        }
    }

    private IEnumerator ProcessPlayerCountChange()
    {
        isChanging = true;

        while (Time.time - lastChangeTime < changeDelay)
        {
            yield return null;
        }
        while (PlayerManager.PlayerCount != targetPlayerCount)
        {
            int direction = targetPlayerCount > PlayerManager.PlayerCount ? 1 : -1;
            int newCount = PlayerManager.PlayerCount + direction;

            PlayerManager.PlayerCount = newCount;
            playerCountTMP.text = newCount.ToString();
            HandleAgentChild(newCount - direction, newCount);

            yield return new WaitForEndOfFrame();
        }

        isChanging = false;
    }
    private void HandleAgentChild(int oldPC, int newPC)
    {
        if (newPC > oldPC)
        {
            AddAgentGO(newPC - 1);
        }
        else if (newPC < oldPC)
        {
            int childCount = contentPlayers.transform.childCount;
            if (childCount > 1) // т.к. 0 == игрок
            {
                var child = contentPlayers.gameObject.transform.GetChild(childCount - 1);
                Destroy(child.transform.gameObject);
            }
        }
    }

    public void AddAgentGO(int agentIndex)
    {
        if (agentIndex >= PlayerManager.PlayerCount || agentIndex < 0)
            return;

        var agent = Instantiate(agentUIPrefab, contentPlayers.transform);
        var agentUI = agent.GetComponent<AgentUI>();
        agentUI.Configurate(PlayerManager.Players[agentIndex]);

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPlayers.GetComponent<RectTransform>());
    }
}
