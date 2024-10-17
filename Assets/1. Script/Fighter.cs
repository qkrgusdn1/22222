using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Fighter
{
    GameObject FighterObject
    {
        get;
    }

    void TakeDamage(float damagem, int hitterID);

    void Attack(Fighter target, float damage);
}
