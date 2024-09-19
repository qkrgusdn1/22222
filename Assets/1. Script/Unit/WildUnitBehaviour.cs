using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildUnitBehaviour : UnitBehaviour
{
    public override void EnterState(UnitState unitState)
    {
        if (unitState == UnitState.Idle)
        {
            unit.agent.isStopped = true;
        }
        if (unitState == UnitState.Approach)
        {
            //*******�ڵ� ������*******
            unit.agent.isStopped = false;
        }
        else if (unitState == UnitState.Attack)
        {
            unit.animator.Play("Attack");
            unit.agent.isStopped = true;
            unit.endAttack = false;
        }
        else if (unitState == UnitState.KnockDown)
        {
            unit.animator.SetBool("IsRunning", false);
            unit.animator.SetBool("IsKnockDown", true);
        }
    }

    public override void UpdateState(UnitState state)
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
            UpdateKnockDownState();
            return;
        }

        if (unit.rangePoint != null)
        {
            float distanceToPlayer = Vector3.Distance(unit.rangePoint.transform.position, unit.target.transform.position);

            if (distanceToPlayer <= unit.targetingRange)
            {
                if (distanceToPlayer > unit.stoppingDistance)
                {
                    unit.animator.SetBool("IsRunning", true);
                    unit.agent.isStopped = false;
                }
                else
                {
                    unit.animator.SetBool("IsRunning", false);
                    unit.agent.isStopped = true;
                    unit.agent.velocity = new Vector3(0, unit.rb.velocity.y, 0);
                    unit.rb.velocity = new Vector3(0, unit.rb.velocity.y, 0);
                }
            }
            else
            {
                unit.animator.SetBool("IsRunning", false);
                unit.agent.isStopped = true;
                unit.agent.velocity = new Vector3(0, unit.rb.velocity.y, 0);
                unit.rb.velocity = new Vector3(0, unit.rb.velocity.y, 0);
            }
        }
    }

    void UpdateIdleState() //Idle �ൿ - � Ȱ�� �ʿ�?
    {
        float disTarget = Vector3.Distance(unit.target.transform.position, transform.position);
        if (disTarget <= unit.targetingRange)
        {
            EnterState(UnitState.Approach);
            return;
        }
    }

    void UpdateApproachState()
    {
        //� �ڵ尡 �ʿ��Ѱ���?

        //Ÿ���� null�϶� , �ֶ� -> Idle
        //Ÿ���� ���� ���� ������ ������ �� -> Attack

        if (unit.target == null)
        {
            EnterState(UnitState.Idle);
            return;
        }

        //Ÿ���� �ָ� Idle�� ��ȯ
        float disTarget = Vector3.Distance(unit.target.transform.position, transform.position);
        if (disTarget > unit.targetingRange)
        {
            EnterState(UnitState.Idle);
            return;
        }
        else if (disTarget <= unit.attackRange)
        {
            EnterState(UnitState.Attack);
        }

        //******* ������ �ڵ� ********
        unit.agent.SetDestination(unit.target.transform.position);

        //Ÿ���� �ٶ󺸴� �ڵ�
        Vector3 direction = (unit.target.transform.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        unit.body.transform.rotation = Quaternion.Slerp(unit.body.transform.rotation, lookRotation, Time.deltaTime * 100);
    }


    void UpdateAttackState()
    {
        if (unit.endAttack)
        {
            unit.attackTimer -= Time.deltaTime;

            if (unit.attackTimer <= 0)
            {
                EnterState(UnitState.Idle);
            }
        }
    }

    void UpdateKnockDownState()
    {
        Collider[] colliders = Physics.OverlapSphere(unit.rangePoint.transform.position, unit.knockDownRange, unit.targetLayer);

        bool isPlayerInRange = false;

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                isPlayerInRange = true;
                break;
            }
        }
        if (isPlayerInRange)
        {
            if (!unit.friendly)
                unit.catchBarBgImage.SetActive(true);
        }
        else
        {
            unit.catchBarImage.fillAmount = 0;
            unit.catchBarBgImage.SetActive(false);
        }
    }
}
