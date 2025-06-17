using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static BuildingUnit;

public  class BuildingAbstract : MonoBehaviour
{
    [Header("����� ����������")]
    [SerializeField] private string buildingName;         // �������� ������
    [SerializeField] private BuildingType buildingType;   // ��� ������
    [SerializeField] private PlayerBase owner;            // �����-�������� ����� ������

    [Header("������������")]
    [SerializeField] private ObservableCollection<Unit> activeUnits;      // ����������� � ���� ������ unit
    [SerializeField] private List<Unit> avaiableUnits;                    // ��������� ��� ������������� unit

    [Header("�������������� ������")]
    [SerializeField] private int buildingLevel;           // ������� ������
    [SerializeField] private int purchaseCost;            // ��������� �������������
    [SerializeField] private int maintenanceCost;         // ������� �� �������������
    [SerializeField] private string description;          // ��������

    [Header("������ ������")]
    [SerializeField] public Sprite buildingSprite;       // ��� ������������ � ������ ��������
    [SerializeField] public TileBase buildingTile;       // ��� ������������ �� �����

    public class ConnectedUnits
    {
        public BuildingUnit source;
        public BuildingUnit target;

        public override bool Equals(object obj)
        {
            return obj is ConnectedUnits other &&
                   ((source == other.source && target == other.target) ||
                    (source == other.target && target == other.source));
        }

        public override int GetHashCode()
        {
            return source.GetHashCode() ^ target.GetHashCode();
        }
    }

    public List<ConnectedUnits> connections;

    public string BuildingName { get => buildingName; set => buildingName = value; }
    public BuildingType BuildingType { get => buildingType; set => buildingType = value; }
    public PlayerBase Owner { get => owner; set => owner = value; }
    public ObservableCollection<Unit> ActiveUnits { get => activeUnits; set => activeUnits = value; }
    public List<Unit> AvaiableUnits { get => avaiableUnits; set => avaiableUnits = value; }
    public int BuildingLevel { get => buildingLevel; set => buildingLevel = value; }
    public int PurchaseCost { get => purchaseCost; set => purchaseCost = value; }
    public int MaintenanceCost { get => maintenanceCost; set => maintenanceCost = value; }
    public string Description { get => description; set => description = value; }

    public bool AddConnection(BuildingUnit source, BuildingUnit target)
    {
        if (HasConnection(target, source))
        {
            //Debug.LogWarning($"����� ����� {source} � {target} ��� ���������� � �������� �����������!");
            return false;
        }

        if (HasConnection(source, target))
        {
            //Debug.LogWarning($"����� ����� {source} � {target} ��� ����������!");
            return false;
        }

        if(connections.Any((x)=>x.source == source))
        {
            //Debug.LogWarning($"{source} ��� �������");
            return false;
        }

        var newConnection = new ConnectedUnits { source = source, target = target };
        connections.Add(newConnection);
        return true;
    }

    public bool RemoveConnection(BuildingUnit source, BuildingUnit target)
    {
        if (!HasConnection(source, target))
        {
            Debug.LogWarning($"����� ����� {source} � {target} �� ���������� � �������� �����������!");
            return false;
        }
        var connection = connections.Where(x => x.target == target && x.source == source).FirstOrDefault();
        connections.Remove(connection);
        return true;
    }

    public bool HasConnection(BuildingUnit unit1, BuildingUnit unit2)
    {
        return connections.Exists(c =>
            (c.source == unit1 && c.target == unit2) ||
            (c.source == unit2 && c.target == unit1));
    }
    public List<ConnectedUnits> GetConnections()
    {
        return new List<ConnectedUnits>(connections);
    }

    public void Start()
    {
        if (ActiveUnits == null || ActiveUnits.Count == 0)
            LoadDefault();
    }

    public void LoadDefault()
    {
        connections = new List<ConnectedUnits>();
        ActiveUnits = new ObservableCollection<Unit>();
        if (AvaiableUnits.Count <= 0)
            return;

        for (int x = 0; x < 9; x++)
        {
            Unit newUnit = Instantiate(AvaiableUnits[0]);
            PlaceUnit(newUnit);
        }
    }

    public void Process()
    {
        Owner.Portfolio.funds -= MaintenanceCost;

        var executedUnits = new List<BuildingUnit>();

        // ����������� �� ������
        foreach (var connect in connections)
        {
            connect.source.Execute(connect.target);
            executedUnits.Add(connect.source);
        }

        foreach(var unit in ActiveUnits)
        {
            if (!executedUnits.Contains(unit.info))
                unit.info.Execute(null);
        }
        
    }

    public bool PlaceUnit(Unit unit)
    {
        if (unit == null)
            return false;

        if (Owner.Portfolio.funds < unit.info.PurchaseCost)
            return false;

        ActiveUnits.Add(unit);
        unit.info.ParentBuilding = this;

        Owner.Portfolio.funds -= unit.info.PurchaseCost;

        return true;
    }
    public bool ReplaceUnitByExisting(Unit oldUnit, Unit newUnit)
    {

        if (Owner.Portfolio.funds < newUnit.info.PurchaseCost)
            return false;

        if (oldUnit == null || newUnit == null)
        {
            //Debug.LogError("������ ���������� null �����!");
            return false;
        }

        if (!ActiveUnits.Contains(oldUnit))
        {
            //Debug.LogError("������ ���� �� ������ � ������ �������� ������!");
            return false;
        }

        if (!AvaiableUnits.Exists(u => u.info.UnitName == newUnit.info.UnitName))
        {
            //Debug.LogError($"���� {newUnit.info.unitName} �� �������� ��� ������������� � ���� ������!");
            return false;
        }

        int index = ActiveUnits.IndexOf(oldUnit);

        ActiveUnits.RemoveAt(index);
        Destroy(oldUnit.gameObject);

        newUnit.info.ParentBuilding = this;
        ActiveUnits.Insert(index, newUnit);
        Owner.Portfolio.funds -= newUnit.info.PurchaseCost;

        //Debug.Log($"was {oldUnit.info.ParentBuilding.buildingName} get to {oldUnit.info.ParentBuilding.buildingName}");
        //Debug.Log($"was {oldUnit.info.ParentBuilding.owner.playerName} get to {oldUnit.info.ParentBuilding.owner.playerName}");
        //Debug.Log($"���� {oldUnit.info.unitName} ������� ������� �� {newUnit.info.unitName}");
        return true;
    }

    public void DeleteUnit(int slot)
    {
        if (slot > activeUnits.Count || slot < 0)
            return;

        Unit unit = this.ActiveUnits[slot];

        if (unit == null)
            Debug.LogError("�������� ����� ��� ��������� ������");

        var outputConnections = connections.Where(x => x.source == unit.info).ToList();
        var inputConnections = connections.Where(x => x.target == unit.info).ToList();

        foreach (var oc in outputConnections)
            RemoveConnection(oc.source, oc.target);

        foreach (var ic in inputConnections)
            RemoveConnection(ic.source, ic.target);

        if (unit.info.Type == BuildingUnit.UnitType.sell)
        {
            SellUnit su = (SellUnit)unit.info;

            su.ClearOrders();

            if (this.buildingType == BuildingType.Shop)
                MarketManager.Instance.SellUnits.Remove(su);
            else
                MarketManager.Instance.SellUnitsPlayer.Remove(su);

        } else if (unit.info.Type == BuildingUnit.UnitType.purchase)
        {
            PurchaseUnit pu = (PurchaseUnit)unit.info;
            pu.CancelOrder();
        }


        Unit slotTempalte = this.AvaiableUnits.FirstOrDefault(b => b.info.Type == UnitType.slot);
        Unit slotSelected = Instantiate(slotTempalte);

        ReplaceUnitByExisting(unit, slotSelected);
    }
}

public enum BuildingType
{
    Produce,
    Craft,
    Shop
}