using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WildUnitBehaviour : UnitBehaviour
{
    public override void UpdateApproachState()
    {
        if (targetPhotonView != null && targetPhotonView.IsMine)
        {
            if (unit.target != null && unit.zone != null)
            {
                zoneDistanceToPlayer = Vector3.Distance(unit.zone.transform.position, unit.target.transform.position);
            }
            photonView.RPC("RPCZoneUnitCheckMove", RpcTarget.All);
        }
        base.UpdateApproachState();
    }

    [PunRPC]
    public void RPCZoneUnitCheckMove()
    {
        if (unit.zoneUnit && unit.turnPoint.position != null)
        {
            if (zoneDistanceToPlayer > unit.zone.zoneRange)
            {
                unit.target = null;
                photonView.RPC("RPCMoveTrunPoint", RpcTarget.All);
                targetting = true;
                return;
            }

            targetting = false;
            Collider[] cols = Physics.OverlapSphere(transform.position, unit.targetingRange, unit.targetLayer);

            if (cols.Length <= 0)
            {
                photonView.RPC("RPCMoveTrunPoint", RpcTarget.All);
                return;
            }
        }

        if (unit.target == null && !targetting)
        {
            unit.EnterState(UnitState.Idle);
            return;
        }
    }
    
}
