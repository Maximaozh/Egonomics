using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static MarketManager;

public class DataLogger : MonoBehaviour
{
    public static DataLogger Instance;

    [SerializeField] private string dataFilePath;

    private StringBuilder logInfo = new StringBuilder();

    private StringBuilder LogInfo { get => logInfo; set => logInfo = value; }

    public bool Log(string text)
    {
        logInfo.AppendLine(text);
        return true;
    }

    public void Clear()
    {
        logInfo.Clear();
    }

    public string Build()
    {
        return logInfo.ToString();
    }

    public void SaveFile()
    {
        File.AppendAllText(dataFilePath, $"{TurnManager.Instance.TurnCount};" + Build() +"\n");
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        Instance = this;

    }


    void Start()
    {
        if (Instance == null)
            Instance = this;

    }
}
