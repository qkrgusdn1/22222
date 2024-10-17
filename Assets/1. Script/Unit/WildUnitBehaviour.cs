using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WildUnitBehaviour : UnitBehaviour
{
    public override void UpdateApproachState()
    {
        if(unit.curUnitBehaviour.unitBehaviourType == UnitBehaviourType.Wild)
        {
            if (unit.zoneUnit && unit.target != null)
            {
                float dis = Vector3.Distance(unit.zone.transform.position, unit.target.transform.position);
                if (dis > unit.zone.zoneRange)
                {
                    unit.EnterState(UnitState.Turn);
                    return;
                }
            }
            base.UpdateApproachState();
        }
        
    }

    public override void UpdateTurnState()
    {
        if (unit.curUnitBehaviour.unitBehaviourType == UnitBehaviourType.Wild)
        {
            if (unit.zoneUnit)
            {
                if (unit.target == null)
                {
                    if (Vector3.Distance(unit.transform.position, unit.turnPoint) <= 0.5f)
                    {
                        unit.target = null;
                        unit.EnterState(UnitState.Idle);
                        return;
                    }
                    return;
                }
                float dis = Vector3.Distance(unit.zone.transform.position, unit.target.transform.position);
                if (dis > unit.zone.zoneRange)
                {
                    if (Vector3.Distance(unit.transform.position, unit.turnPoint) <= 0.5f)
                    {
                        unit.target = null;
                        unit.EnterState(UnitState.Idle);
                        return;
                    }
                    return;
                }
                if (unit.target != null && Vector3.Distance(unit.transform.position, unit.target.transform.position) <= unit.targetingRange)
                {
                    Collider[] cols = Physics.OverlapSphere(transform.position, unit.targetingRange, unit.targetLayer);

                    if (cols.Length > 0)
                    {
                        GameObject closestTarget = null;
                        float closestDistance = Mathf.Infinity;

                        foreach (Collider col in cols)
                        {
                            float distance = Vector3.Distance(transform.position, col.transform.position);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestTarget = col.gameObject;
                            }
                        }

                        if (closestTarget != null)
                        {
                            unit.targetPhotonView = closestTarget.GetComponent<PhotonView>();
                            int targetViewID = unit.targetPhotonView.ViewID;
                            photonView.RPC("RPCSetTarget", RpcTarget.All, targetViewID);
                            unit.EnterState(UnitState.Approach);
                            return;
                        }
                    }

                    unit.EnterState(UnitState.Approach);
                    return;
                }
            }
        }
    }

}
