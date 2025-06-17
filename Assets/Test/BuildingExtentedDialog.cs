using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingExtentedDialog : MonoBehaviour
{
    public BuildingAbstract buildingData;

    public int playerID = 0;


    private void OnMouseDown()
    {
        try
        {
            if (ToolManager.instance.IsAnyDialogOpen()) return;

            if (buildingData == null || buildingData.Owner == null)
                return;

            // В редакторе можно выбирать любые здания
#if UNITY_EDITOR
            ToolManager.instance.ShowBuildingInfo(buildingData);
            ToolManager.instance.DecreaseActions();
            // В релизе — только свои
#else
                if (buildingData.Owner.playerId == playerID && ToolManager.instance.ActionLeft > 0)
                {
                    ToolManager.instance.ShowBuildingInfo(buildingData);
                    ToolManager.instance.DecreaseActions();
                }
#endif
        }
        catch (Exception ex)
        {
           Debug.Log("Ошибка нажатия на здание." + ex.Message);
        }
        
    }
}
