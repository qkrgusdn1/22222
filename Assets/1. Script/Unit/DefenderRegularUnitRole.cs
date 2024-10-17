using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderRegularUnitRole : RegularUnitRole
{
    public override void EnterRole()
    {
        unit.turnPoint = unit.transform.position;
    }

    public override void UpdateRole()
    {
        if(unit.unitState == UnitState.Idle)
        {
            unit.animator.SetBool("IsRunning", false);
            Collider[] cols = Physics.OverlapSphere(transform.position, unit.targetingRange, unit.targetLayer);
            if (cols.Length > 0)
            {
                GameObject closestTarget = null;
                float closestDistance = Mathf.Infinity;

                foreach (Collider col in cols)
                {
                    PhotonView targetPhotonView = col.GetComponent<PhotonView>();

                    if (targetPhotonView != null)
                    {
                        float distance = Vector3.Distance(transform.position, col.transform.position);

                        if (targetPhotonView.Owner != PhotonNetwork.LocalPlayer || targetPhotonView.IsMine || PhotonNetwork.IsMasterClient)
                        {
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestTarget = col.gameObject;
                            }
                        }
                    }
                }

                if (closestTarget != null)
                {
                    regularUnitBehaviour.SetNewTarget(closestTarget);
                    unit.EnterState(UnitState.Approach);
                    return;
                }
            }

        }
        else if(unit.unitState == UnitState.Approach)
        {
            unit.agent.isStopped = false;
            unit.animator.SetBool("IsRunning", true);

            if (unit.target != null)
            {
                distanceToPlayer = Vector3.Distance(unit.rangePoint.transform.position, unit.target.transform.position);
                if (distanceToPlayer > unit.targetingRange)
                {
                    unit.target = null;
                    unit.EnterState(UnitState.Idle);
                    return;
                }
                else if (distanceToPlayer < unit.attackRange)
                {
                    unit.endAttack = false;
                    unit.EnterState(UnitState.Attack);
                    return;
                }
            }
            else
            {
                unit.EnterState(UnitState.Turn);
            }
        }
        else if(unit.unitState == UnitState.Attack)
        {
            if (unit.hp / unit.maxHp <= 0.2f)
            {
                unit.EnterState(UnitState.KnockDown);
            }
            if (distanceToPlayer > unit.attackRange)
            {
                if (unit.endAttack)
                {
                    unit.attackTimer = 0;
                    unit.EnterState(UnitState.Approach);
                    unit.agent.isStopped = false;
                }
                return;
            }
            unit.attackTimer -= Time.deltaTime;

            if (unit.attackTimer <= 0)
            {
                unit.EnterState(UnitState.Idle);
                return;
            }
        }
        else if(unit.unitState == UnitState.Turn)
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
        }
    }
}
