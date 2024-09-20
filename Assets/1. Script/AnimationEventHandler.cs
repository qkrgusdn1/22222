using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public Action startAttackListener;
    public Action endAttackListener;
    public Action finishAttackListener;
    public Action startRollListener;
    public Action endRollListener;
    public Action dieListener;
    public void StartAttack()
    {
        startAttackListener?.Invoke();
    }
    public void EndAttack()
    {
        endAttackListener?.Invoke();
    }
    public void FinishAttack()
    {
        finishAttackListener?.Invoke();
    }
    public void StratRoll()
    {
        startRollListener?.Invoke();
    }
    public void EndRoll()
    {
        endRollListener?.Invoke();
    }
    public void Die()
    {
        dieListener?.Invoke();
    }
}
