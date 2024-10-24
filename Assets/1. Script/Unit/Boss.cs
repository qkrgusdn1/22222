using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Unit
{
    public int maxAttackAmount;
    public BossAttackType currentPatternAttack;

    public override void FinishAttack()
    {
        if(target != null)
        {
            float distanceToPlayer = Vector3.Distance(rangePoint.transform.position, target.transform.position);
            if (distanceToPlayer > attackRange)
            {
                weapon.canDamage = false;
                attackAmount = 0;
                attackTimer = 0;
                base.FinishAttack();
                EnterState(UnitState.Approach);
                return;
            }
        }
        
        if (attackAmount >= maxAttackAmount || target == null)
        {
            weapon.canDamage = false;
            attackAmount = 0;
            attackTimer = 0;
            base.FinishAttack();
            return;
        }
        else
        {
            weapon.canDamage = false;
            endAttack = false;
            AttackStart();
            return;
        }
    }

    public override void AttackStart()
    {
        if(attackAmount == 0)
        {
            BossAttackType[] attackTypes = (BossAttackType[])System.Enum.GetValues(typeof(BossAttackType));
            currentPatternAttack = attackTypes[Random.Range(0, attackTypes.Length)];
            if (currentPatternAttack == BossAttackType.one)
            {
                maxAttackAmount = 1;
                attackName = "Attack";
            }
            else if (currentPatternAttack == BossAttackType.three)
            {
                maxAttackAmount = 3;
                attackName = "Attack";
            }
        }
        
        base.AttackStart();
    }
}
public enum BossAttackType
{
    one,
    three
}
