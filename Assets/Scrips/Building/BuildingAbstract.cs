using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static BuildingUnit;

public  class BuildingAbstract : MonoBehaviour
{
    [Header("Общая информация")]
    [SerializeField] private string buildingName;         // Название здание
    [SerializeField] private BuildingType buildingType;   // Тип здания
    [SerializeField] private PlayerBase owner;            // Игрок-владелец этого здания

    [Header("Департаменты")]
    [SerializeField] private ObservableCollection<Unit> activeUnits;      // Построенные в этом здании unit
    [SerializeField] private List<Unit> avaiableUnits;                    // Доступные для строительства unit

    [Header("Характеристика здания")]
    [SerializeField] private int buildingLevel;           // Уровень здания
    [SerializeField] private int purchaseCost;            // Стоимость строительства
    [SerializeField] private int maintenanceCost;         // Затраты на использование
    [SerializeField] private string description;          // Описание

    [Header("Дизайн зданий")]
    [SerializeField] public Sprite buildingSprite;       // Оно отображается в выборе строения
    [SerializeField] public TileBase buildingTile;       // Оно отображается на карте

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
            //Debug.LogWarning($"Связь между {source} и {target} уже существует в обратном направлении!");
            return false;
        }

        if (HasConnection(source, target))
        {
            //Debug.LogWarning($"Связь между {source} и {target} уже существует!");
            return false;
        }

        if(connections.Any((x)=>x.source == source))
        {
            //Debug.LogWarning($"{source} уже соединён");
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
            Debug.LogWarning($"Связь между {source} и {target} не существует в обратном направлении!");
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

        // Пробегаемся по связям
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
            //Debug.LogError("Нельзя передавать null юниты!");
            return false;
        }

        if (!ActiveUnits.Contains(oldUnit))
        {
            //Debug.LogError("Старый юнит не найден в списке активных юнитов!");
            return false;
        }

        if (!AvaiableUnits.Exists(u => u.info.UnitName == newUnit.info.UnitName))
        {
            //Debug.LogError($"Юнит {newUnit.info.unitName} не доступен для строительства в этом здании!");
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
        //Debug.Log($"Юнит {oldUnit.info.unitName} успешно заменен на {newUnit.info.unitName}");
        return true;
    }

    public void DeleteUnit(int slot)
    {
        if (slot > activeUnits.Count || slot < 0)
            return;

        Unit unit = this.ActiveUnits[slot];

        if (unit == null)
            Debug.LogError("Удаление юнита был обнаружен пустой");

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