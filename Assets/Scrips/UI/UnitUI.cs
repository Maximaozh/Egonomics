using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitUI : MonoBehaviour
{
    [Header("����� ������������")]
    public Transform content;

    [Header("���������������� ���������")]
    public BuildingUnit unit;
    public virtual void Configure(BuildingUnit unit)
    {
        this.unit = unit;
    }
}
