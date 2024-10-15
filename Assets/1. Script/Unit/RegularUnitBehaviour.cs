using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegularUnitBehaviour : UnitBehaviour
{

    public RegularUnitState regularUnitState;

    public float stopRange;

    public string regularStateName;

    private void Start()
    {
        regularStateName = "사수";
        unit.regularStateText.text = "현재 유닛 상태 " + regularStateName;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(unit.rangePoint.transform.position, stopRange);
    }


   
    private void Update()
    {
        if (unit.die)
            return;

        if(unit.unitState == UnitState.KnockDown)
        {
            regularUnitState = RegularUnitState.KnockDown;
        }

        if(player != null && regularUnitState != RegularUnitState.Defender && regularUnitState != RegularUnitState.KnockDown)
        {
            float playerdis = Vector3.Distance(unit.rangePoint.transform.position, player.transform.position);
            if (playerdis <= stopRange)
            {
                unit.EnterState(UnitState.Idle);
                return;
            }
            else if (playerdis > stopRange)
            {
                unit.EnterState(UnitState.Approach);
            }
        }
    }

    //?? ????? ?? ???? ???? ??? ?? ???? ??
    public void OnClickedChangeRegularUnitStateBtn(string regularStateBtnName)
    {
        photonView.RPC("RPCOnClickedChangeRegularUnitStateBtn", RpcTarget.All, regularStateBtnName);
    }
    [PunRPC]
    public void RPCOnClickedChangeRegularUnitStateBtn(string regularStateBtnName)
    {
        if (regularUnitState == RegularUnitState.KnockDown)
        {
            regularStateName = "기절";
            return;
        }

        if (Enum.TryParse(regularStateBtnName, out RegularUnitState state))
        {
            regularUnitState = state;
        }
        if (regularUnitState == RegularUnitState.Follow)
        {
            unit.EnterState(UnitState.Approach);
            regularStateName = "따라오기";
        }
        else if (regularUnitState == RegularUnitState.Guard)
        {
            unit.EnterState(UnitState.Approach);
            regularStateName = "경호";
        }
        else if (regularUnitState == RegularUnitState.Defender)
        {
            unit.EnterState(UnitState.Idle);
            unit.turnPointObj.transform.position = transform.position;
            unit.turnPoint = unit.turnPointObj.transform;

            regularStateName = "사수";
        }

        unit.regularStateText.text = "현재 유닛 상태 : " + regularStateName;
    }

    public override void UpdateAttackState()
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
            if (Vector3.Distance(transform.position, unit.turnPoint.position) > 0.5f && unit.target == null && regularUnitState == RegularUnitState.Defender)
            {
                unit.agent.isStopped = false;
                unit.EnterState(UnitState.Approach);
                return;
            }
            else
            {
                unit.EnterState(UnitState.Idle);
                return;
            }
        }
    }

    public override void UpdateApproachState()
    {
        if(regularUnitState == RegularUnitState.KnockDown)
        {
            return;
        }
        if(regularUnitState == RegularUnitState.Follow)
        {
            Debug.Log("Follow");
            unit.animator.SetBool("IsRunning", true);
            unit.agent.SetDestination(player.transform.position);

            Vector3 direction = (player.transform.position - transform.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            unit.body.transform.rotation = Quaternion.Slerp(unit.body.transform.rotation, lookRotation, Time.deltaTime * 100);
            return;
        }
        if (regularUnitState == RegularUnitState.Guard && unit.target == null)
        {
            Debug.Log("Guard");
            unit.animator.SetBool("IsRunning", true);
            unit.agent.SetDestination(player.transform.position);

            Vector3 direction = (player.transform.position - transform.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            unit.body.transform.rotation = Quaternion.Slerp(unit.body.transform.rotation, lookRotation, Time.deltaTime * 100);
            return;
        }
        else if (regularUnitState == RegularUnitState.Defender)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, unit.targetingRange, unit.targetLayer);
            Debug.Log("Defender");
            if (cols.Length <= 0)
            {
                Debug.Log("MoveTrunPoint");
                MoveTrunPoint();
                return;
            }
            
        }

        base.UpdateApproachState();
    }


}

public enum RegularUnitState
{
    Guard,
    Defender,
    Follow,
    KnockDown,
    None
}
