using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LodingMgr : MonoBehaviour
{
    private void Update()
    {
        if (PhotonNetwork.InLobby)
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}
