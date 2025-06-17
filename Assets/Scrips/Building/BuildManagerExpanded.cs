using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildManagerExpanded : MonoBehaviour
{
    public static BuildManagerExpanded Instance { get; private set; }

    [SerializeField] public Tilemap groundTilemap;
    [SerializeField] public Tilemap buildingTilemap;
    [SerializeField] public Grid grid;
    [SerializeField] public TileBase defaultGroundTile;

    public List<BuildingAbstract> availableBuilds;

    private Dictionary<string, Color> playerColors = new Dictionary<string, Color>();

    private static readonly System.Random random = new System.Random();
    private static readonly List<string> randomNames = new List<string>
    {
        "Эдельвейс", "Гранд", "Орион", "Альфа", "Бета", "Гамма", "Дельта", "Сигма",
        "Небоскрёб", "Башня", "Высота", "Зенит", "Горизонт", "Параллель", "Меридиан",
        "Империя", "Корона", "Сфера", "Куб", "Пирамида", "Овал", "Арка", "Фасад",
        "Капитолий", "Акрополь", "Пантеон", "Колизей", "Олимп", "Парнас", "Атлант",
        "Титан", "Гигант", "Икар", "Прометей", "Феникс", "Грифон", "Пегас", "Кентавр",
        "Сфинкс", "Кварц", "Топаз", "Рубин", "Сапфир", "Изумруд", "Алмаз", "Платина",
        "Восход", "Закат", "Полдень", "Полночь", "Созвездие", "Галактика", "Туманность"
    };

    public List<Vector3Int> GetAvailableBuildPositions()
    {
        List<Vector3Int> availablePositions = new List<Vector3Int>();

        BoundsInt bounds = groundTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                if (groundTilemap.HasTile(cellPosition) && !buildingTilemap.HasTile(cellPosition))
                {
                    availablePositions.Add(cellPosition);
                }
            }
        }
        return availablePositions;
    }
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public bool TryBuild(BuildingAbstract build, Vector3Int position, PlayerBase player)
    {
        if (CanBuild(build, position, player))
        {
            Build(build, position, player);
            return true;
        }
        else
        {
            //Debug.Log("Нельзя построить здесь!");
            return false;
        }
    }

    public bool CanBuild(BuildingAbstract build, Vector3Int position, PlayerBase player)
    {
        if (player.Portfolio.Funds < build.PurchaseCost)
        {
            //Debug.Log("Недостаточно средств!");
            return false;
        }

        TileBase existingBuilding = buildingTilemap.GetTile(position);

        if (!groundTilemap.HasTile(position) || buildingTilemap.HasTile(position))
        {
            //Debug.Log("Недоступная позиция строительства");
            return false;
        }


        return true;
    }

    public void Build(BuildingAbstract build, Vector3Int position, PlayerBase player)
    {
        if (build == null || position == null || player == null)
            return;

        player.Portfolio.Funds -= build.PurchaseCost;

        Color playerColor = GetPlayerColor(player);

        TileBase currentTile = groundTilemap.GetTile(position);

        if (currentTile == null)
        {
            groundTilemap.SetTile(position, defaultGroundTile);
        }

        groundTilemap.SetTileFlags(position, TileFlags.None);
        groundTilemap.SetColor(position, playerColor);

        buildingTilemap.SetTile(position, build.buildingTile);
        
        BuildingAbstract buildingInstance = Instantiate(build, grid.CellToWorld(position), Quaternion.identity);
        buildingInstance.Owner = player;
        buildingInstance.LoadDefault();
        buildingInstance.BuildingName = GetRandomName(buildingInstance.BuildingName);
        buildingInstance.transform.SetParent(this.transform);

        player.Portfolio.Buildings.Add(buildingInstance);

        //Debug.Log($"{player.PlayerName} построил {build.buildingName}!");
    }

    public Color GetPlayerColor(PlayerBase player)
    {
        if (playerColors.ContainsKey(player.PlayerName))
        {
            return playerColors[player.PlayerName];
        }
        else
        {
            Color newColor = GenerateColorFromString(player.PlayerName);
            playerColors[player.PlayerName] = newColor;
            return newColor;
        }
    }

    public Color GenerateColorFromString(string name)
    {
        if (string.IsNullOrEmpty(name))
            return Color.gray;

        int hash = name.GetHashCode();
        Random.InitState(hash);  

        float hue = Mathf.Repeat(hash / 1000f, 1f);
        float saturation = 0.7f + Mathf.Abs((hash * 0.0001f) % 0.3f); 
        float value = 0.8f + Mathf.Abs((hash * 0.0002f) % 0.2f);     

        // Конвертируем HSV в RGB
        Color color = Color.HSVToRGB(hue, saturation, value);
        return color;
    }
    
    public string GetRandomName(string origin)
    { 
        string randomName = randomNames[random.Next(randomNames.Count)];
        int randomNumber = random.Next(100, 1000);

        return $"Зд. {origin} им. {randomName}  ул. {randomNumber}";
    }

}
