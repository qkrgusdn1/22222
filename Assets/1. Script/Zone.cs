using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;
    public int spawnAmount;

    public string[] unitPrefabNames;
    public List<Unit> units = new List<Unit>();
    public float zoneRange;
    public GameObject canvas;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, zoneRange);
    }
    private void Update()
    {
        if (GameMgr.Instance.player != null)
        {
            float distance = Vector3.Distance(transform.position, GameMgr.Instance.player.transform.position);
            if (distance <= zoneRange)
            {
                canvas.SetActive(true);
            }
            else
            {
                canvas.SetActive(false);
            }
        }
    }
    public void StartSpwan()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                Transform temp = spawnPoints[i];
                int randomIndex = Random.Range(i, spawnPoints.Length);
                spawnPoints[i] = spawnPoints[randomIndex];
                spawnPoints[randomIndex] = temp;
            }
            for (int i = 0; i < spawnAmount; i++)
            {
                GameObject unitObject = PhotonNetwork.InstantiateRoomObject(unitPrefabNames[Random.Range(0, unitPrefabNames.Length)], spawnPoints[i].position, Quaternion.identity);
                int unitViewID = unitObject.GetComponent<PhotonView>().ViewID;
                int unitPositionViewID = spawnPoints[i].GetComponent<PhotonView>().ViewID;
                photonView.RPC("RPCUnitPositions", RpcTarget.All, unitViewID, unitPositionViewID);
                
            }
        }
    }
    [PunRPC]
    public void RPCUnitPositions(int unitViewID, int unitPositionViewID)
    {
        GameObject unitObj = PhotonView.Find(unitViewID).gameObject;
        Transform unitPosition = PhotonView.Find(unitPositionViewID).transform;
        unitObj.transform.SetParent(unitPosition);
        Unit unit = unitObj.GetComponent<Unit>();
        units.Add(unit);
        unit.zoneUnit = true;
        unit.turnPoint = unitPosition.position;
        unit.zone = this;
    }
}
