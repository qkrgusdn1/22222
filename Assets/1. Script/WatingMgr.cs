using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WatingMgr : MonoBehaviourPunCallbacks
{
    public TMP_Text waitingText;
    bool start;
    int countDown = 10;

    private void Start()
    {
        photonView.RPC("RPCEnteredPlayer", RpcTarget.All);
        StartCoroutine(WaitingLodding());
    }

    [PunRPC]
    public void RPCEnteredPlayer()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            start = true;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    IEnumerator WaitingLodding()
    {
        while (!start)
        {
            waitingText.text = "플레이어 대기 중.";
            yield return new WaitForSeconds(1);
            waitingText.text = "플레이어 대기 중..";
            yield return new WaitForSeconds(1);
            waitingText.text = "플레이어 대기 중...";
            yield return new WaitForSeconds(1);
        }

        while (start)
        {
            waitingText.text = countDown.ToString();
            if (countDown == 0)
            {
                PhotonNetwork.LoadLevel("InGame");
                break;
            }
            yield return new WaitForSeconds(1);
            countDown--;
        }
    }
}
