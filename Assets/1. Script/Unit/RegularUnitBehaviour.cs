using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class RegularUnitBehaviour : UnitBehaviour
{
    public float stopRange;

    public RegularUnitRole curRole;
    public RegularUnitRole[] regularUnitRoles;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(unit.rangePoint.transform.position, stopRange);
    }

    private void Start()
    {
        for(int i = 0; i< regularUnitRoles.Length; i++)
        {
            regularUnitRoles[i].regularUnitBehaviour = this;
        }
    }

    private void Update()
    {
        if (unit.unitBehaviourType == UnitBehaviourType.Wild)
            return;

        if (unit.die)
            return;

        if (unit.unitState == UnitState.KnockDown)
            return;

        if(unit.ownerPlayer != null && !unit.ownerPlayer.photonView.IsMine)
        {
            return;
        }

        if(curRole != null)
            curRole.UpdateRole();
    }

    public void OnClickedChangeRegularUnitStateBtn(string regularStateBtnName)
    {
        photonView.RPC("RPCOnClickedChangeRegularUnitStateBtn", RpcTarget.All, regularStateBtnName);
    }
    [PunRPC]
    public void RPCOnClickedChangeRegularUnitStateBtn(string regularStateBtnName)
    {
        if (Enum.TryParse(regularStateBtnName, out RegularUnitRoleType role))
        {
            for(int i = 0; i < regularUnitRoles.Length; i++)
            {
                if (regularUnitRoles[i].type == role)
                {
                    curRole = regularUnitRoles[i];
                    break;
                }
            }
        }

        curRole.EnterRole();

        unit.regularStateText.text = "현재 아군 상태 : " + curRole.type;
    }



    public override void UpdateAttackState()
    {
        
    }

    public override void UpdateIdleState()
    {
        
    }

    public override void UpdateTurnState()
    {
       
    }

    public override void UpdateApproachState()
    {
    
    }


}

public enum RegularUnitRoleType
{
    Guard,
    Defender,
    Follow,
}
