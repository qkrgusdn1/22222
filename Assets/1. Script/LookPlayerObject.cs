using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookPlayerObject : MonoBehaviour
{
    private void Update()
    {
        Vector3 direction = (Camera.main.transform.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 100);
    }
}
