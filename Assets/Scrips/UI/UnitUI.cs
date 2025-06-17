using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitUI : MonoBehaviour
{
    [Header("Место прикрепления")]
    public Transform content;

    [Header("Конфигруационные настройки")]
    public BuildingUnit unit;
    public virtual void Configure(BuildingUnit unit)
    {
        this.unit = unit;
    }
}
