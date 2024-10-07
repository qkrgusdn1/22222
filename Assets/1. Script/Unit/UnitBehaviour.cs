using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBehaviour : MonoBehaviour
{
    public Unit unit;
    public UnitBehaviourType unitBehaviourType;
    public float distanceToPlayer;
    public Player player;

    public void PlayerSetting(Player player)
    {
        this.player = player;
    }
    public virtual void InitUnit(Unit unit)
    {
        this.unit = unit;
    }
    public virtual void EnterState(UnitState unitState)
    {
        Debug.Log($"Entering state: {unitState}");
        if (unitState == UnitState.Idle)
        {
            unit.agent.isStopped = true;
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
        Debug.Log($"UpdateState: {state}");
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

        if (unit.rangePoint != null && unit.target != null)
        {
            distanceToPlayer = Vector3.Distance(unit.rangePoint.transform.position, unit.target.transform.position);
            if(distanceToPlayer >= unit.targetingRange)
            {
                unit.EnterState(UnitState.Idle);
                unit.agent.isStopped = true;
                unit.agent.velocity = new Vector3(0, unit.rb.velocity.y, 0);
                unit.rb.velocity = new Vector3(0, unit.rb.velocity.y, 0);
            }
        }
    }

    public virtual void UpdateIdleState() //Idle 행동 - 어떤 활동 필요?
    {
        unit.animator.SetBool("IsRunning", false);
        Debug.Log("UpdateIdleState");
        Collider[] cols = Physics.OverlapSphere(transform.position, unit.targetingRange, unit.targetLayer);

        if (cols.Length <= 0)
            return;
        unit.target = cols[0].gameObject;
        unit.EnterState(UnitState.Approach);
        
    }

    public virtual void UpdateApproachState()
    {
        Debug.Log("UpdateApproachState");
        //어떤 코드가 필요한가요?

        //타겟이 null일때 , 멀때 -> Idle
        //타겟이 공격 범위 안으로 들어왔을 때 -> Attack
        distanceToPlayer = Vector3.Distance(unit.rangePoint.transform.position, unit.target.transform.position);
        //타겟이 멀면 Idle로 전환
        if (distanceToPlayer > unit.targetingRange)
        {
            unit.target = null;
            unit.EnterState(UnitState.Idle);
            return;
        }
        else if (distanceToPlayer < unit.attackRange)
        {
            Debug.Log("AttackTime");
            unit.endAttack = false;
            unit.EnterState(UnitState.Attack);
            return;
        }
        unit.animator.SetBool("IsRunning", true);
        //******* 수정된 코드 ********
        unit.agent.SetDestination(unit.target.transform.position);

        //타겟을 바라보는 코드
        Vector3 direction = (unit.target.transform.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        unit.body.transform.rotation = Quaternion.Slerp(unit.body.transform.rotation, lookRotation, Time.deltaTime * 100);
    }


    public virtual void UpdateAttackState()
    {
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
        unit.agent.isStopped = true;
        unit.agent.velocity = new Vector3(0, unit.rb.velocity.y, 0);
        unit.rb.velocity = new Vector3(0, unit.rb.velocity.y, 0);
        if (unit.attackTimer <= 0)
        {
            unit.EnterState(UnitState.Idle);
            return;
        }

    }

    public virtual void UpdateFKeyImage(bool active)
    {
        if(unitBehaviourType == UnitBehaviourType.Reguler)
        {
            unit.fKeyImage.gameObject.SetActive(active);
        }
        else if(unitBehaviourType == UnitBehaviourType.Wild)
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
