using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBase : MonoBehaviour
{
    [SerializeField] private FinancialPortfolio portfolio;
    [SerializeField] public string playerName;
    [SerializeField] public int playerId;

    public FinancialPortfolio Portfolio { get { return portfolio; } set { portfolio = value; } }  // ��������� ������
    public string PlayerName { get { return playerName; } set { playerName = value; } }// ��� ������
    public virtual void TakeTurn() // ����� ����� ����������� �� ��, ��� ������ ������ � ���� ���
    {
        ////Debug.Log($"player {PlayerName} is active");
    }
    
    protected virtual void Start()
    {
        if (Portfolio == null)
        {
            //Debug.LogError($"�� ��������� ��������� ��� ������ {PlayerName}!");
        }
    }
    public virtual void EndTurn() 
    {
        ////Debug.Log($"player {PlayerName} has eneded turn");
        TurnManager.Instance.EndTurn();
    }
}
