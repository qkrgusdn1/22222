using Photon.Pun;
using System;
using UnityEditor;
using UnityEngine;

public abstract class UnitBehaviour : MonoBehaviourPunCallbacks
{
    public Unit unit;
    public UnitBehaviourType unitBehaviourType;
    public float distanceToPlayer;
    public float zoneDistanceToPlayer;
    public Player player;
    public bool targetting;
    public PhotonView targetPhotonView;
    bool isTargetChange;
    private void Awake()
    {
        unit = GetComponentInParent<Unit>();
    }
    public void PlayerSetting(Player player)
    {
        this.player = player;
    }

    public virtual void EnterState(UnitState unitState)
    {
        if (unitState == UnitState.Idle)
        {
            SetNewTarget(null);
            unit.animator.SetBool("IsKnockDown", false);
            unit.agent.isStopped = true;
            unit.agent.velocity = new Vector3(0, unit.rb.velocity.y, 0);
            unit.rb.velocity = new Vector3(0, unit.rb.velocity.y, 0);
        }
        if (unitState == UnitState.Approach)
        {
            unit.agent.isStopped = false;
        }
        else if (unitState == UnitState.Attack)
        {
            unit.agent.isStopped = true;
        }
        else if (unitState == UnitState.KnockDown)
        {
            unit.animator.SetBool("IsRunning", false);
            unit.animator.SetBool("IsKnockDown", true);
        }else if(unitState == UnitState.Turn)
        {
            unit.agent.isStopped = false;
            unit.animator.SetBool("IsRunning", true);
        }
    }

    public virtual void UpdateState(UnitState state)
    {
        if (unit.unitState == UnitState.Idle)
        {
            UpdateIdleState();
        }
        else if (unit.unitState == UnitState.Approach)
        {
            UpdateApproachState();
        }
        else if (unit.unitState == UnitState.Attack)
        {
            UpdateAttackState();
        }
        else if (unit.unitState == UnitState.KnockDown)
        {
            return;
        }
        else if (unit.unitState == UnitState.Turn)
        {
            UpdateTurnState();
        }
    }

    public virtual void UpdateTurnState()
    {

    }

    public virtual void UpdateIdleState()
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
                SetNewTarget(closestTarget);
                unit.EnterState(UnitState.Approach);
                return;
            }
        }
    }
    public void SetNewTarget(GameObject newTarget)
    {
        if (!isTargetChange && (targetPhotonView == null || targetPhotonView.gameObject != newTarget))
        {
            targetPhotonView = newTarget?.GetComponent<PhotonView>();
            if (targetPhotonView != null)
            {
                photonView.RPC("RPCSetTarget", RpcTarget.All, targetPhotonView.ViewID);
            }
            else
            {
                photonView.RPC("RPCSetTarget", RpcTarget.All, -1);
            }
        }
    }
    [PunRPC]
    public void RPCSetTarget(int targetID)
    {
        isTargetChange = true;

        if (targetID == -1)
        {
            unit.target = null;
        }
        else
        {
            GameObject target = PhotonView.Find(targetID)?.gameObject;
            if (target != null)
            {
                unit.target = target;
            }
        }

        isTargetChange = false;
    }

    public virtual void UpdateApproachState()
    {
        unit.agent.isStopped = false;
        unit.animator.SetBool("IsRunning", true);

        if (unit.target != null)
        {
            distanceToPlayer = Vector3.Distance(unit.rangePoint.transform.position, unit.target.transform.position);
            if (distanceToPlayer > unit.targetingRange)
            {
                SetNewTarget(null);
                if (!unit.zoneUnit)
                {
                    unit.EnterState(UnitState.Idle);
                }
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
            if (!unit.zoneUnit)
            {
                unit.EnterState(UnitState.Idle);
            }
            else
            {
                unit.EnterState(UnitState.Turn);
            }
                
        }
    }

    public virtual void UpdateAttackState()
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
            //if (unit.turnPoint != null&& Vector3.Distance(transform.position, unit.turnPoint.position) > 0.5f && unit.target == null && unit.zoneUnit)
            //{
            //    unit.agent.isStopped = false;
            //    unit.EnterState(UnitState.Approach);
            //    return;
            //}
            //else if(!unit.zoneUnit)
            //{
                
            //}
            
        }
    }

    public virtual void UpdateFKeyImage(bool active)
    {
        if (unitBehaviourType == UnitBehaviourType.Reguler)
        {
            if(unit.ownerPlayer.photonView.ViewID == GameMgr.Instance.player.photonView.ViewID)
            {
                unit.fKeyImage.gameObject.SetActive(active);
            }
            else
            {
                if(unit.unitState == UnitState.KnockDown)
                {
                    unit.catchBarImage.fillAmount = 0;
                    unit.catchBarBgImage.SetActive(active);
                }
            }
        }
        else if (unitBehaviourType == UnitBehaviourType.Wild)
        {
            unit.catchBarImage.fillAmount = 0;
            unit.catchBarBgImage.SetActive(active);
        }

        if (unit.catchBarBgImage.activeSelf)
        {

        }
        unit.catchBarImage.fillAmount = 0;
    }
}
public enum UnitBehaviourType
{
    Wild,
    Reguler
}