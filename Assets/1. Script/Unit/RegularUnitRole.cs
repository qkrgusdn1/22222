using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularUnitRole : MonoBehaviourPunCallbacks
{
    public float distanceToPlayer;
    public RegularUnitRoleType type;
    public Unit unit;
    public UnitBehaviour regularUnitBehaviour;
    private void Awake()
    {
        unit = GetComponentInParent<Unit>();
    }
    public virtual void EnterRole()
    {

    }
    public virtual void UpdateRole()
    {

    }

  
}
