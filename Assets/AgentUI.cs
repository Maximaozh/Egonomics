using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using static PlayerManager;

public class AgentUI : MonoBehaviour
{
    private  PlayerManager.PlayerData  playerData;


    [Header("Поле имени")]
    public TMP_InputField nameIF;

    [Header("Слайдеры")]
    public Slider LRC;
    public Slider HRC;
    public Slider TC;
    public Slider Aggression;
    public Slider APT;
    public Slider WPT;
    public Slider Money;

    [Header("Tекст")]
    public TMP_Text LRCTMP;
    public TMP_Text HRCTMP;
    public TMP_Text TCTMP;
    public TMP_Text AggressionTMP;
    public TMP_Text APTTMP;
    public TMP_Text WPTTMP;
    public TMP_Text MoneyTMP;

    [Header("Стратегия")]
    public TMP_Dropdown strategyDDB;


    public void Configurate(PlayerManager.PlayerData player)
    {
        LRC.minValue = 0;
        LRC.maxValue = 1;
        LRC.onValueChanged.AddListener(x => {
            HandleSliderChange(x, LRC, LRCTMP, 0);
            ChangeColourTMP(LRCTMP, x);
        });

        HRC.minValue = 0;
        HRC.maxValue = 1;
        HRC.onValueChanged.AddListener(x => {
            HandleSliderChange(x, HRC, HRCTMP, 1);
            ChangeColourTMP(HRCTMP, x);
        });

        TC.minValue = 0;
        TC.maxValue = 1;
        TC.onValueChanged.AddListener(x => {
            HandleSliderChange(x, TC, TCTMP, 2);
            ChangeColourTMP(TCTMP, x);
        });

        Aggression.minValue = 0;
        Aggression.maxValue = 1;
        Aggression.onValueChanged.AddListener(x => {
            HandleSliderChange(x, Aggression, AggressionTMP, 3);
            ChangeColourTMP(AggressionTMP, x);
        });

        APT.minValue = 1;
        APT.maxValue = 20;
        APT.onValueChanged.AddListener(x => {
            HandleSliderChange(x, APT, APTTMP, 4);
            ChangeColourTMP(APTTMP, x / APT.maxValue);
        });

        WPT.minValue = 1;
        WPT.maxValue = 20;
        WPT.onValueChanged.AddListener(x => {
            HandleSliderChange(x, WPT, WPTTMP, 5);
            ChangeColourTMP(WPTTMP, x / WPT.maxValue);
        });

        Money.minValue = 100;
        Money.maxValue = 999;
        Money.onValueChanged.AddListener(x => {
            HandleSliderChange(x, Money, MoneyTMP, 6);
            ChangeColourTMP(MoneyTMP, (x - Money.minValue) / (Money.maxValue - Money.minValue));
        });

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        options.Add(new TMP_Dropdown.OptionData("Производство"));
        options.Add(new TMP_Dropdown.OptionData("Перепродажи"));
        options.Add(new TMP_Dropdown.OptionData("Универсал"));

        strategyDDB.AddOptions(options);

        playerData = player;
        nameIF.text = playerData.playerName;

        SetSliderAndText(LRC, LRCTMP, playerData.lowerRC, 0);
        SetSliderAndText(HRC, HRCTMP, playerData.higherRC, 1);
        SetSliderAndText(TC, TCTMP, playerData.technologyChance, 2);
        SetSliderAndText(Aggression, AggressionTMP, playerData.aggersiveValue, 3);
        SetSliderAndText(APT, APTTMP, playerData.actionsPerturnCount, 4);
        SetSliderAndText(WPT, WPTTMP, playerData.turnWaitCount, 5);
        SetSliderAndText(Money, MoneyTMP, playerData.money, 6);

        switch (playerData.strategy)
        {
            case Strategy.production:
                strategyDDB.value = 0;
                break;
            case Strategy.reseller:
                strategyDDB.value = 1;
                break;
            case Strategy.combine:
                strategyDDB.value = 2;
                break;
        }
        strategyDDB.Select();
        strategyDDB.RefreshShownValue();
        HandleDropdownboxChange(((int)playerData.strategy));

        nameIF.onValueChanged.AddListener(x => HandleNameChanged(x));
        strategyDDB.onValueChanged.AddListener(x => HandleDropdownboxChange(x));
    }


    private float GetNormalizedValue(Slider slider, float value, int fieldNum)
    {
        if (fieldNum <= 3)
            return value;

        return (value - slider.minValue) / (slider.maxValue - slider.minValue);
    }

    private void SetSliderAndText(Slider slider, TMP_Text text, float value, int fieldNum)
    {
        slider.value = value;

        if (fieldNum == 4 || fieldNum == 5 || fieldNum == 6)
            text.text = Mathf.FloorToInt(value).ToString();
        else
            text.text = value.ToString("0.##");

        float normalizedValue = GetNormalizedValue(slider, value, fieldNum);
        ChangeColourTMP(text, normalizedValue);
    }
    private void HandleNameChanged(string value)
    {
        playerData.playerName = value;
    }

    private void HandleDropdownboxChange(int value)
    {
        switch (value)
        {
            case 0:
                playerData.strategy = PlayerManager.Strategy.production;
                break;
            case 1:
                playerData.strategy = PlayerManager.Strategy.reseller;
                break;
            case 2:
                playerData.strategy = PlayerManager.Strategy.combine;
                break;

            default:
                value = -1;
                break;
        }
    }

    private void ChangeColourTMP(TMP_Text content, float value)
    {
        Color newColor;
        if (value <= 0.25f)
        {
            float t = value / 0.25f;
            newColor = Color.Lerp(Color.white, Color.yellow, t);
        }
        else if (value <= 0.5f)
        {
            float t = (value - 0.25f) / 0.25f;
            newColor = Color.Lerp(Color.yellow, Color.cyan, t);
        }
        else if (value <= 0.75f)
        {
            float t = (value - 0.5f) / 0.25f;
            newColor = Color.Lerp(Color.cyan, new Color(0.8f, 0.6f, 1f), t);
        }
        else
        {
            newColor = new Color(0.8f, 0.6f, 1f);
        }

        content.color = newColor;
    }

    private void HandleSliderChange(float value, Slider slider, TMP_Text content, int fieldNum)
    {
        // Только обновляем текст и данные, цвет меняется в onValueChanged
        if (fieldNum == 4 || fieldNum == 5 || fieldNum == 6)
            content.text = Mathf.FloorToInt(value).ToString();
        else
            content.text = value.ToString("0.##");

        // Обновляем данные игрока...
        switch (fieldNum)
        {
            case 0: playerData.lowerRC = (float)Math.Round(value, 2); break;
            case 1: playerData.higherRC = (float)Math.Round(value, 2); break;
            case 2: playerData.technologyChance = (float)Math.Round(value, 2); break;
            case 3: playerData.aggersiveValue = (float)Math.Round(value, 2); break;
            case 4: playerData.actionsPerturnCount = Mathf.FloorToInt(value); break;
            case 5: playerData.turnWaitCount = Mathf.FloorToInt(value); break;
            case 6: playerData.money = Mathf.FloorToInt(value); break;
        }
    }
}
