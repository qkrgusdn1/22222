using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SelectSpawnCanvas : MonoBehaviour
{
    public TMP_Text playerNickName;
    public TMP_Text otherplayerNickName;
    private void Start()
    {
        playerNickName.text = PhotonNetwork.PlayerList[0].NickName;
        otherplayerNickName.text = PhotonNetwork.PlayerList[1].NickName;
    }
    public void OnClickedGameStartBtn()
    {
        GameMgr.Instance.character.StartGame(GameMgr.Instance.spawnPoints[GameMgr.Instance.spawnPointIdx].transform.position);
    }
}
