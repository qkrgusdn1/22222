using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WildUnitBehaviour : UnitBehaviour
{
    public override void UpdateApproachState()
    {
        if (unit.target == null)
        {
            unit.EnterState(UnitState.Idle);
            return;
        }
        base.UpdateApproachState();
    }
}
