using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[System.Serializable]
public class FinancialPortfolio : MonoBehaviour
{
    [SerializeField] public List<BuildingAbstract> Buildings = new List<BuildingAbstract>(); // ������ ������ ������

    public delegate void FundsUpdated();
    public delegate void ItemsUpdated();
    public event FundsUpdated OnFundsUpdated;

    [SerializeField]  public float initialfunds;
    [SerializeField] public float funds;  // ���������� ������ � ������


    private void Start()
    {
        initialfunds = funds;
    }
    public float Funds { 
        get { return funds; }
        set
        {
            if (funds != value)
            {
                funds = value;
                // �������� �������, ���� �������� ����������
                OnFundsUpdated?.Invoke();
            }
        }
    }  
}
