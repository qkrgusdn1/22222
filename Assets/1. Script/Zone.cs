using Photon.Pun;
using UnityEngine;

public class Zone : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;
    public int spawnAmount;

    public string[] unitPrefabNames;

    public float zoneRange;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, zoneRange);
    }
    public void StartSpwan()
    {
        photonView.RPC("RPCMixSpawnPoints", RpcTarget.All);
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject unitObject = PhotonNetwork.Instantiate(unitPrefabNames[Random.Range(0, unitPrefabNames.Length)], spawnPoints[i].position, Quaternion.identity);
            Unit unit = unitObject.GetComponent<Unit>();
            unitObject.transform.SetParent(spawnPoints[i]);
            unit.zoneUnit = true;
            unit.turnPoint = spawnPoints[i];
            unit.zone = this;
        }
    }
    [PunRPC]
    public void RPCMixSpawnPoints()
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
