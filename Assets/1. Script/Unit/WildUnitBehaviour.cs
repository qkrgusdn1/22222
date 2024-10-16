using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WildUnitBehaviour : UnitBehaviour
{
    public override void UpdateApproachState()
    {
        if (unit.target != null && unit.zone != null && unit.zoneUnit)
        {
            zoneDistanceToPlayer = Vector3.Distance(unit.zone.transform.position, unit.target.transform.position);
            if (zoneDistanceToPlayer > unit.zone.zoneRange)
            {
                unit.target = null;

                Collider[] cols = Physics.OverlapSphere(unit.zone.transform.position, unit.zone.zoneRange, unit.targetLayer);
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
                        targetPhotonView = closestTarget.GetComponent<PhotonView>();
                        MoveTrunPoint();
                    }
                }
                else
                {
                    MoveTrunPoint();
                }
                return;
            }
            float distanceToTarget = Vector3.Distance(unit.transform.position, unit.target.transform.position);
            if (distanceToTarget < unit.attackRange)
            {
                unit.EnterState(UnitState.Attack);
                return;
            }
            else if (distanceToTarget > unit.targetingRange)
            {
                unit.target = null;
                MoveTrunPoint();
                return;
            }

            if (targetPhotonView != null)
            {
                photonView.RPC("RPCMove", RpcTarget.All, targetPhotonView.ViewID);
                return;
            }
        }
        else if(unit.target == null && unit.zone != null && unit.zoneUnit)
        {
            MoveTrunPoint();
        }
        base.UpdateApproachState();
    }
    
}
