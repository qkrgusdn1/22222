using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Fighter
{
    void TakeDamage(float damage);

    void Attack(Fighter target, float damage);
}
