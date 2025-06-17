using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public BuildingUnit info; // Функциональная часть
    public GameObject GUIPrefab; //  GUI элемент
    public Transform GUIContent; //  GUI содержаие

    public virtual void ShowUI(string extra)
    {
        GameObject GUI = Instantiate(GUIPrefab, GUIContent);
        var GUIComponent = GUI.GetComponent<UnitGUI>();
        GUIComponent.Configure(this, extra);    
    }
}
