using UnityEngine;


public class UserBuild : MonoBehaviour
{
    private BuildManager buildManager;
    public BuildUI buildCanvasUI;
    private PlayerBase player;

    public Vector3 offset;

    void Start()
    {
        buildManager = BuildManager.Instance;
        player = GetComponent<PlayerBase>(); 
    }
    void Update()
    {
        if (buildCanvasUI.SelectedBuild != null && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos) + offset;

            Vector3Int cellPosition = buildManager.grid.WorldToCell(worldPosition);
            cellPosition.z = 0;

            //Debug.Log($"World: {worldPosition} | Cell: {cellPosition}");

            bool success = buildManager.TryBuild(buildCanvasUI.SelectedBuild, cellPosition, player);

            if (success)
            {
                //Debug.Log($"Построено: {buildCanvasUI.SelectedBuild.buildingName}");
            }
            else
            {
                //Debug.Log("Не удалось построить здание!");
            }

            buildCanvasUI.SetSelectedBuild(null);
        }
    }
}
