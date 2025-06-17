using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MarketItem", menuName = "Inventory/MarketItem")]
public class MarketItem : ScriptableObject
{
    public ProductData product; // ������� �������� ������

    [SerializeField] private float necessity; // ������������� ������ ��� ���������, ��� - 100% �� ���������� - 40%
    [SerializeField] private int supply; // ������� ����������� �� ������ �����
    [SerializeField] private int demand; // ������� ������ �� ������ �����

    [SerializeField] private int defaultDemand;

    public float Necessity { get => necessity; set => necessity = value; }
    public int Supply { get => supply; set => supply = value; }
    public int Demand { get => demand; set => demand = value; }

    public void ResetToDefaults()
    {
        Supply = 0;
        Demand = defaultDemand;
    }

    private void OnEnable()
    {
        if (defaultDemand == 0)
        {
            defaultDemand = Demand;
        }

        ResetToDefaults();
    }

    public MarketItem(ProductData item, float necessity, int supply, int demand)
    {
        this.product = item;
        this.Necessity = necessity;
        this.Supply = supply;
        this.Demand = demand;
    }
}
