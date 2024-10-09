using UnityEngine;

public class Zone : MonoBehaviour
{
    public Transform[] spawnPoints;
    public int spawnAmount;

    public Unit[] units;

    public float zoneRange;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, zoneRange);
    }

    private void Start()
    {
        StartSpwan();
    }

    public void StartSpwan()
    {
        MixSpawnPoints();
        for (int i = 0; i < spawnAmount; i++)
        {
            //나중에 포톤 인스테이트로 바꾸기
            Unit unit = Instantiate(units[Random.Range(0, units.Length)], spawnPoints[i].position, Quaternion.identity, spawnPoints[i]);
            unit.zoneUnit = true;
            unit.turnPoint = spawnPoints[i];
            unit.zone = this;
        }
    }
    private void MixSpawnPoints()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform temp = spawnPoints[i];
            int randomIndex = Random.Range(i, spawnPoints.Length);
            spawnPoints[i] = spawnPoints[randomIndex];
            spawnPoints[randomIndex] = temp;
        }
    }
}
