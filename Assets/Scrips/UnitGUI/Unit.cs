using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public BuildingUnit info; // �������������� �����
    public GameObject GUIPrefab; //  GUI �������
    public Transform GUIContent; //  GUI ���������

    public virtual void ShowUI(string extra)
    {
        GameObject GUI = Instantiate(GUIPrefab, GUIContent);
        var GUIComponent = GUI.GetComponent<UnitGUI>();
        GUIComponent.Configure(this, extra);    
    }
}
