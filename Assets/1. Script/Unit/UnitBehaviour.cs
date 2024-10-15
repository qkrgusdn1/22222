using Photon.Pun;
using System;
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
                int targetViewID = targetPhotonView.ViewID;
                photonView.RPC("RPCSetTarget", RpcTarget.All, targetViewID);
                unit.EnterState(UnitState.Approach);
                return;
            }
        }
    }
    
    [PunRPC]
    public void RPCSetTarget(int targetID)
    {
        GameObject target = PhotonView.Find(targetID).gameObject;
        if (target != null)
        {
            unit.target = target;
            Debug.Log($"[RPC] Target set to: {target.name} (ViewID: {targetID})");
        }
        else
        {
            Debug.LogError($"[RPC] Target not found! (ViewID: {targetID})");
        }
    }

    public virtual void UpdateApproachState()
    {
        if (unit.target == null || !unit.target.activeInHierarchy)
        {
            if (unit.zoneUnit)
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
                        PhotonView targetPhotonView = closestTarget.GetComponent<PhotonView>();
                        int targetViewID = targetPhotonView.ViewID;
                        photonView.RPC("RPCSetTarget", RpcTarget.All, targetViewID);
                    }
                }
                else
                {
                    unit.EnterState(UnitState.Idle);
                }
            }
        }
        if (unit.target != null && unit.rangePoint != null)
        {
            distanceToPlayer = Vector3.Distance(unit.rangePoint.transform.position, unit.target.transform.position);
        }
        else if(unit.target == null)
        {
            unit.target = null;
            return;
        }
        if (distanceToPlayer > unit.targetingRange)
        {
            unit.target = null;
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
        Move(unit.target.gameObject);
    }
   
    public void Move(GameObject target)
    {
        Debug.Log(target);
        unit.agent.isStopped = false;
        unit.animator.SetBool("IsRunning", true);
        unit.agent.SetDestination(target.transform.position);

        Vector3 direction = (target.transform.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        unit.body.transform.rotation = Quaternion.Slerp(unit.body.transform.rotation, lookRotation, Time.deltaTime * 100);
    }
    public void MoveTrunPoint()
    {
        unit.agent.isStopped = false;
        Collider[] cols = Physics.OverlapSphere(transform.position, unit.targetingRange, unit.targetLayer);
        if (cols.Length > 0)
        {
            PhotonView targetPhotonView = cols[0].GetComponent<PhotonView>();
            int targetViewID = targetPhotonView.ViewID;
            photonView.RPC("RPCSetTarget", RpcTarget.All, targetViewID);
            distanceToPlayer = Vector3.Distance(unit.rangePoint.transform.position, unit.target.transform.position);
        }
        if (Vector3.Distance(transform.position, unit.turnPoint.position) > 0.5f)
        {
            Move(unit.turnPoint.gameObject);
        }
        else if (Vector3.Distance(transform.position, unit.turnPoint.position) <= 0.5f)
        {
            unit.EnterState(UnitState.Idle);
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
            unit.fKeyImage.gameObject.SetActive(active);
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