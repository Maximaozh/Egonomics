using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildUI : MonoBehaviour
{
    [SerializeField] public GameObject buildButtonPrefab;
    [SerializeField] public Transform buildButtonParent;
    [SerializeField] private PlayerBase player;

    private BuildingBase selectedBuild;
    public BuildingBase SelectedBuild { get { return selectedBuild; } set { selectedBuild = value; } }

    private BuildManager buildManager;
    void Start()
    {
        buildManager = BuildManager.Instance;
        InitializeUI();
    }

    public void InitializeUI()
    {
        foreach (Transform child in buildButtonParent)
        {
            Destroy(child.gameObject);
        }


        if (buildManager == null || buildManager.availableBuilds == null)
            return;

        foreach (var build in buildManager.availableBuilds)
        {
            GameObject buttonGO = Instantiate(buildButtonPrefab, buildButtonParent);
            BuildUIItem button = buttonGO.GetComponent<BuildUIItem>();


            if (button != null)
            {
                button.Initialize(build, this, player);
            }
        }
    }


    public void SetSelectedBuild(BuildingBase build)
    {
        selectedBuild = build;
        //Debug.Log($"Выбрана постройка в UI: {build?.buildingName}");
    }
}