using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDialogue : MonoBehaviour
{
    public BuildingBase buildingData; // ������ �� ������ ������
    public  GameObject buildingPanelPrefab; // ������ ������ ���������� �������
    private GameObject currentPanel; // ������� �������� ������
    private GameObject pivot;

    public string CanvasName;
    public int playerID = 0; // ID ������, �������� ������ ������������ ������

    private void OnMouseDown()
    {
        if (buildingData != null && buildingData.owner != null && buildingData.owner.playerId == playerID)
        {
            if (currentPanel != null)
            {
                Destroy(currentPanel);
            }

            pivot = GameObject.Find(CanvasName).transform.gameObject;

            currentPanel = Instantiate(buildingPanelPrefab, pivot.transform);

            BuildDialoguePanel buildingPanel = currentPanel.GetComponent<BuildDialoguePanel>();

            if (buildingPanel != null && buildingData != null)
            {
                buildingPanel.SetBuilding(buildingData);
            }

            RectTransform panelRectTransform = currentPanel.GetComponent<RectTransform>();
            if (panelRectTransform != null)
            {
                panelRectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                //Debug.LogError("RectTransform �� ������.");
            }
        }
        else
        {
            //Debug.Log("��� ������ �� ����������� ���.");
        }
    }

    private void OnDestroy()
    {
        if (currentPanel != null)
        {
            Destroy(currentPanel);
        }
    }
}