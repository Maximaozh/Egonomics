using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildExpandedUI : MonoBehaviour
{
    [SerializeField] public GameObject buildButtonPrefab;
    [SerializeField] public Transform buildButtonParent;
    [SerializeField] private PlayerBase player;

    private BuildingAbstract selectedBuild;
    public BuildingAbstract SelectedBuild { get { return selectedBuild; } set { selectedBuild = value; } }


    private void OnEnable()
    {
        InitializeUI();
    }

    private void OnDisable()
    {
    }

    public void InitializeUI()
    {
        foreach (Transform child in buildButtonParent)
        {
            Destroy(child.gameObject);
        }


        if (BuildManagerExpanded.Instance == null || BuildManagerExpanded.Instance.availableBuilds == null)
            return;

        foreach (var build in BuildManagerExpanded.Instance.availableBuilds)
        {
            GameObject buttonGO = Instantiate(buildButtonPrefab, buildButtonParent);
            BuildExpandedItemUI button = buttonGO.GetComponent<BuildExpandedItemUI>();

            if (button != null)
            {
                button.Initialize(build, this, player);
            }
        }
    }


    public void SetSelectedBuild(BuildingAbstract build)
    {
        selectedBuild = build;
        //Debug.Log($"Выбрана постройка в UI: {build?.buildingName}");
    }

}
