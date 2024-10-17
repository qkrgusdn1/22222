using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRegularUnitRole : RegularUnitRole
{
    public override void EnterRole()
    {
        unit.turnPoint = unit.transform.position;
    }

    public override void UpdateRole()
    {
        if (unit.unitState == UnitState.Idle)
        {
            float distanceToOwnerPlayer = Vector3.Distance(unit.rangePoint.transform.position, unit.ownerPlayer.transform.position);
            if(distanceToOwnerPlayer > unit.targetingRange)
            {
                unit.EnterState(UnitState.Approach);
            }
        }
        else if (unit.unitState == UnitState.Approach)
        {
            unit.target = unit.ownerPlayer.gameObject;
            float distanceToOwnerPlayer = Vector3.Distance(unit.rangePoint.transform.position, unit.ownerPlayer.transform.position);
            if (distanceToOwnerPlayer < unit.targetingRange)
            {
                unit.EnterState(UnitState.Idle);
                return;
            }
        }
    }

    
}
