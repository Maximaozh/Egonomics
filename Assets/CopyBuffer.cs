using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BuildingUnit;

public class CopyBuffer : MonoBehaviour
{
    private static BuildingAbstract source;

    public static void CopyBuilding(BuildingAbstract copyBuilding)
    {
        source = copyBuilding;
    }

    public static void PasteBuilding(ref BuildingAbstract target)
    {
        if (source == null || target == null || target == source)
            return;

        if (target.BuildingType != source.BuildingType)
            return;

        for (int i = 0; i < target.ActiveUnits.Count; i++)
            target.DeleteUnit(i);

        for(int i = 0; i < target.ActiveUnits.Count; i++)
        {
            var sourceUnit = source.ActiveUnits[i];
            var targetUnit = target.ActiveUnits[i];

            Unit template           = target.AvaiableUnits.FirstOrDefault(b => b.info.Type == sourceUnit.info.Type);
            Unit templateInstance   = Instantiate(template);

            template.info.OutputProduct = sourceUnit.info.OutputProduct;

            target.ReplaceUnitByExisting(targetUnit, templateInstance);
        }

        for (int i = 0; i < source.connections.Count; i++)
        {
            var su = source.connections[i].source;
            int su_index = source.ActiveUnits.Select(x => x.info).ToList().IndexOf(su);

            var tu = source.connections[i].target;
            int tu_index = source.ActiveUnits.Select(x => x.info).ToList().IndexOf(tu);

            var sut = target.ActiveUnits[su_index];
            var tut = target.ActiveUnits[tu_index];

            target.AddConnection(sut.info, tut.info);
        }
    }
}
