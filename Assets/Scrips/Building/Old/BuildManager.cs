
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    public List<BuildingBase> availableBuilds;
    public Tilemap groundTilemap;
    public Tilemap buildingTilemap;
    public Grid grid;
    public TileBase defaultGroundTile;
    public List<BuildingBase> AvailableBuilds => availableBuilds;

    private Dictionary<string, Color> playerColors = new Dictionary<string, Color>();
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
            DontDestroyOnLoad(gameObject);
        }
    }
    public bool TryBuild(BuildingBase build, Vector3Int position, PlayerBase player)
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

    public bool CanBuild(BuildingBase build, Vector3Int position, PlayerBase player)
    {
        if (player.Portfolio.Funds < build.cost)
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

    public void Build(BuildingBase build, Vector3Int position, PlayerBase player)
    {
        player.Portfolio.Funds -= build.cost;

        Color playerColor = GetPlayerColor(player);

        TileBase currentTile = groundTilemap.GetTile(position);

        if (currentTile == null)
        {
            groundTilemap.SetTile(position, defaultGroundTile);
        }

        groundTilemap.SetTileFlags(position, TileFlags.None);
        groundTilemap.SetColor(position, playerColor);

        buildingTilemap.SetTile(position, build.buildingTile);
;
        BuildingBase buildingInstance = Instantiate(build, grid.CellToWorld(position), Quaternion.identity);
        buildingInstance.owner = player;
        buildingInstance.transform.SetParent(this.transform);

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
        int hash = name.GetHashCode();

        float r = Mathf.Abs((hash * 0.1f) % 1);
        float g = Mathf.Abs((hash * 0.2f) % 1);
        float b = Mathf.Abs((hash * 0.3f) % 1);

        return new Color(r, g, b);
    }
   
}
