using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviourPunCallbacks
{
    public void StartGame(Vector3 spawnPoint)
    {
        foreach(Zone zone in GameMgr.Instance.zones)
        {
            zone.StartSpwan();
        }
        PhotonNetwork.Instantiate("Player", spawnPoint, Quaternion.identity);
    }
}
