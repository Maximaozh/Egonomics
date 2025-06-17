using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PlayerManager
{
    public enum Strategy
    { 
        production,
        reseller,
        combine
    }

    [Serializable]
    public class PlayerData 
   {
        public string playerName;           // ���
        public bool isAi;                   // ������� �� �� 

        // ���� ��
        public float lowerRC;               // ���� �������� -
        public float higherRC;              // ���� �������� +
        public float technologyChance;      // ���� ����������
        public float aggersiveValue;      // ������������� +
        public int actionsPerturnCount = 5;   // ���������� �������� � ���
        public int turnWaitCount = 3;         // ����� �����������, ���
        public Strategy strategy;           // ����� �����������, ���
        public int money;         // ������
    }

    public const int minPlayerCount = 1;
    public const int maxPlayerCount = 20;

    private static int playerCount = 1;

    public static bool isSimulation = false;

    public static bool loaded = false;
    public static int aiSupport = 500;
    public static int turnCount = 30;
    public static int PlayerCount {
        get
        {
            return playerCount;
        }
        set
        {
            if (value < 1 || value > maxPlayerCount)
                return;

            playerCount = value;

            if(playerCount > Players.Count)
            {
                Players.Add(AddAiDefault($"{GetRandomName()}[{value}]"));
            } else if (playerCount < Players.Count)
            {
                Players.RemoveAt(Players.Count - 1);
            }
        }
    }

    private static List<PlayerData> players;
    public static List<PlayerData> Players { get => players; set => players = value; }

    static PlayerManager()
    {
        LoadAgents();
    }

    public static PlayerData AddAiDefault(string name)
    {
        PlayerData ai = new PlayerData();
        ai.playerName = name;
        ai.isAi = true;

        ai.lowerRC = 0f;               
        ai.higherRC = 1f; ;             
        ai.technologyChance = 0.5f;      
        ai.aggersiveValue = 0.5f;      
        ai.actionsPerturnCount = 5;  
        ai.turnWaitCount = 2;       
        ai.strategy = Strategy.combine; 
        ai.money = 500; 

        return ai;
    }
    private static string SavePath =>
     Path.Combine(Application.persistentDataPath, "ai_config.json");

    public static void SaveAgents()
    {
        try
        {
            PlayerSaveData saveData = new PlayerSaveData
            {
                players = Players,
                splayerCount = playerCount,
                saiSupport = aiSupport,
                sturnCount = turnCount
            };

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"������ ��������� �: {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"������ ����������: {e.Message}");
        }
    }

    public static void LoadAgents()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(json);

                Players = saveData.players ?? new List<PlayerData>();
                playerCount = saveData.splayerCount;
                turnCount = saveData.sturnCount;
                aiSupport = saveData.saiSupport;

            }
            else
            {
                Players = new List<PlayerData>();
                PlayerData userPlayer = new PlayerData
                {
                    playerName = "�����",
                    isAi = false
                };
                Players.Add(userPlayer);
                playerCount = 1;
            }

            loaded = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"������ ��������: {e.Message}");
            Players = new List<PlayerData>();
            loaded = false;
        }
    }

    [Serializable]
    private class PlayerSaveData
    {
        public List<PlayerData> players;
        public int splayerCount;
        public int sturnCount;
        public int saiSupport;
    }

    // ������ ��� � ��������� �� ����
    private static List<string> gameNames = new List<string>
    {
        "�������",         
        "������",          
        "�������",         
        "�������",         
        "��������",        
        "����'����",       
        "����",            
        "�����",           
        "����� �����",     
        "������",          
        "�����",           
        "����",            
        "����",            
        "������",          
        "�����",           
        "��������",        
        "�����",           
        "����",            
        "������",          
        "�������",         
        "��������",        
        "�������",         
        "�����",           
        "������ ���",      
        "������",          
        "������",          
        "��������",        
        "�������",         
        "������",          
        "������"           
    };

    public static string GetRandomName()
    {
        if (gameNames.Count == 0)
        {
            Debug.LogWarning("������ ��� ����!");
            return "����������";
        }

        int randomIndex = UnityEngine.Random.Range(0, gameNames.Count);
        return gameNames[randomIndex];
    }


}
