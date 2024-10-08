using UnityEngine;

public class Zone : MonoBehaviour
{
    public Transform[] spawnPoints;
    public int spawnAmount;

    public Unit[] units;

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
            unit.spawnPoint = spawnPoints[i];
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
