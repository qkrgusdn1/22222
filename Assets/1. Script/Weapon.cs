using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string key;
    public Vector3 atkRange;
    public bool canDamage;
    public GameObject rangePoint;
    public List<GameObject> hittedList = new List<GameObject>();
    Player owner;
    public float damage;
    public LayerMask hitLayerMask;
    public void StartAttack()
    {
        canDamage = true;
    }

    public void EndAttack()
    {
        canDamage = false;
    }
    private void Start()
    {
        owner = GetComponentInParent<Player>();
    }

    public void Update()
    {
        if (!canDamage)
            return;
        Collider[] colliders = Physics.OverlapBox(rangePoint.transform.position, atkRange / 2, transform.rotation, hitLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (!hittedList.Contains(colliders[i].gameObject))
            {
                hittedList.Add(colliders[i].gameObject);
                owner.Attack(colliders[i].gameObject.GetComponent<Unit>(), damage);
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (rangePoint == null)
            return;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(rangePoint.transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, atkRange);
    }
}
