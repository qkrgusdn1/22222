using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBehaviour : MonoBehaviour
{
    public Unit unit;
    public UnitBehaviourType unitBehaviourType;
    public void InitUnit(Unit unit)
    {
        this.unit = unit;
    }
    public abstract void EnterState(UnitState unitState);
    public abstract void UpdateState(UnitState state);
}
public enum UnitBehaviourType
{
    Wild,
    Reguler
}
