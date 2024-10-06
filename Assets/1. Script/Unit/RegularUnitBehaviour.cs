using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegularUnitBehaviour : UnitBehaviour
{

    public RegularUnitState regularUnitState;

    public float stopRange;

    string regularStateName;


    private void Start()
    {
        regularUnitState = RegularUnitState.Defender;
        regularStateName = "사수";
        unit.regularStateText.text = "현재 아군 상태 : " + regularStateName;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(unit.rangePoint.transform.position, stopRange);
    }



    private void Update()
    {
        if(player != null && regularUnitState != RegularUnitState.Defender)
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

    public void OnClickedChangeRegularUnitStateBtn(string regularStateBtnName)
    {
        if (Enum.TryParse(regularStateBtnName, out RegularUnitState state))
        {
            regularUnitState = state;
        }

        if(regularUnitState == RegularUnitState.Follow)
        {
            unit.EnterState(UnitState.Approach);
            regularStateName = "따라오기";
        }
        else if(regularUnitState == RegularUnitState.Guard)
        {
            unit.EnterState(UnitState.Approach);
            regularStateName = "경호";
        }
        else if(regularUnitState == RegularUnitState.Defender)
        {
            unit.EnterState(UnitState.Idle);
            regularStateName = "사수";
        }

        unit.regularStateText.text = "현재 아군 상태 : " + regularStateName;
    }

    public override void UpdateApproachState()
    {

        if (regularUnitState == RegularUnitState.Follow || regularUnitState == RegularUnitState.Guard && unit.target == null)
        {
            unit.animator.SetBool("IsRunning", true);
            unit.agent.SetDestination(player.transform.position);

            Vector3 direction = (player.transform.position - transform.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            unit.body.transform.rotation = Quaternion.Slerp(unit.body.transform.rotation, lookRotation, Time.deltaTime * 100);
            return;
        }
        else if (regularUnitState == RegularUnitState.Defender && unit.target == null)
        {
            unit.EnterState(UnitState.Idle);
            return;
        }

        base.UpdateApproachState();
    }

    
}

public enum RegularUnitState
{
    Guard,
    Defender,
    Follow
}
