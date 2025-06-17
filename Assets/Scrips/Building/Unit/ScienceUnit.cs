using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScienceUnit : BuildingUnit
{
    [SerializeField] private float upgradeDifficulty = 5;

    public float UpgradeDifficulty { get => upgradeDifficulty; set => upgradeDifficulty = value; }

    public override void Execute(BuildingUnit outputTarget)
    {
        base.Execute(outputTarget);
        Study();
    }
    
    public void Study()
    {
        var units = ParentBuilding.ActiveUnits.Where(x => x != this);

        var modifier = Mathf.RoundToInt((float)(BasePower) / UpgradeDifficulty);

        foreach (var unit in units)
        {
            unit.info.ChangeWorkerLevel(modifier);
        }
    }
}
