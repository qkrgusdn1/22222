using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRegularUnitRole : RegularUnitRole
{
    public override void UpdateRole()
    {
        if (unit.unitState == UnitState.Idle)
        {
            unit.animator.SetBool("IsRunning", false);
            float distanceToOwnerPlayer = Vector3.Distance(unit.rangePoint.transform.position, unit.ownerPlayer.transform.position);
            if(distanceToOwnerPlayer > unit.attackRange)
            {
                regularUnitBehaviour.SetNewTarget(unit.ownerPlayer.gameObject);
                unit.EnterState(UnitState.Approach);
            }
        }
        else if (unit.unitState == UnitState.Approach)
        {
            unit.animator.SetBool("IsRunning", true);
            float distanceToOwnerPlayer = Vector3.Distance(unit.rangePoint.transform.position, unit.ownerPlayer.transform.position);
            if (distanceToOwnerPlayer < unit.attackRange)
            {
                unit.EnterState(UnitState.Idle);
                return;
            }
        }
    }

    
}
