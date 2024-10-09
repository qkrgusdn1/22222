using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WildUnitBehaviour : UnitBehaviour
{
    public override void UpdateApproachState()
    {
        if (unit.target != null && unit.zone != null)
        {
            zoneDistanceToPlayer = Vector3.Distance(unit.zone.transform.position, unit.target.transform.position);
        }

        if (unit.zoneUnit && unit.turnPoint.position != null)
        {
            if (zoneDistanceToPlayer > unit.zone.zoneRange)
            {
                unit.target = null;
                MoveTrunPoint();
                targetting = true;
                return;
            }

            targetting = false;
            Collider[] cols = Physics.OverlapSphere(transform.position, unit.targetingRange, unit.targetLayer);

            if (cols.Length <= 0)
            {
                MoveTrunPoint();
                return;
            }
        }

        if (unit.target == null && !targetting)
        {
            unit.EnterState(UnitState.Idle);
            return;
        }

        base.UpdateApproachState();
    }
    
}
