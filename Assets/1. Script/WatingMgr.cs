using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WatingMgr : MonoBehaviourPunCallbacks
{
    public TMP_Text waitingText;
    int countDown = 10;
    public GameObject otherPlayerModel;

    public TMP_Text playerNickName;
    public TMP_Text otherplayerNickName;
    Coroutine waitingCoroutine;

    private void Start()
    {
        AudioMgr.Instance.waitingMusic.gameObject.SetActive(true);
        AudioMgr.Instance.inGameMusic.gameObject.SetActive(false);
        AudioMgr.Instance.selectMusic.gameObject.SetActive(false);
        AudioMgr.Instance.lobbyMusic.gameObject.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = PhotonMgr.Instance.nickName;
        playerNickName.text = PhotonNetwork.PlayerList[0].NickName;
        photonView.RPC("RPCEnteredPlayer", RpcTarget.All);
        if (PhotonNetwork.IsMasterClient)
            waitingCoroutine = StartCoroutine(CoWaitingLodding());
    }

    [PunRPC]
    public void RPCNickNameSetting(string otherNickName)
    {
        otherplayerNickName.text = otherNickName;
    }

    [PunRPC]
    public void RPCEnteredPlayer()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            otherPlayerModel.SetActive(true);
            if (waitingCoroutine != null)
            {
                StopCoroutine(waitingCoroutine);
            }
            string otherNickName = PhotonNetwork.PlayerList[1].NickName;
            photonView.RPC("RPCNickNameSetting", RpcTarget.All, otherNickName);
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                StartCoroutine(Countdown());
            }
            
        }
    }
    IEnumerator CoWaitingLodding()
    {
        while (true)
        {
            waitingText.text = "플레이어 대기 중.";
            yield return new WaitForSeconds(1);
            waitingText.text = "플레이어 대기 중..";
            yield return new WaitForSeconds(1);
            waitingText.text = "플레이어 대기 중...";
            yield return new WaitForSeconds(1);
        }
    }
    private IEnumerator Countdown()
    {
        while (countDown > -1)
        {
            photonView.RPC("RPCUpdateCountdown", RpcTarget.All, countDown);
            yield return new WaitForSeconds(1);
            countDown--;
        }

        photonView.RPC("RPCLoadGameScene", RpcTarget.All);
    }

    [PunRPC]
    public void RPCUpdateCountdown(int remainingTime)
    {
        waitingText.text = remainingTime.ToString();
    }
    [PunRPC]
    public void RPCLoadGameScene()
    {
        PhotonNetwork.LoadLevel("InGame");
    }
}
