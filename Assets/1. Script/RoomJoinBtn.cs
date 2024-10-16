using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomJoinBtn : MonoBehaviour
{
    public TMP_Text tile;

    RoomInfo info;
    public void SetRoomInfo(RoomInfo roomInfo)
    {
        info = roomInfo;
        tile.text = roomInfo.Name;
    }

    public void OnClickedJoin()
    {
        if (string.IsNullOrEmpty(LobbyMgr.Instance.nickNameInputField.text))
        {
            return;
        }
        PhotonMgr.Instance.nickName = LobbyMgr.Instance.nickNameInputField.text;
        LobbyMgr.Instance.loadingPanel.SetActive(true);
        PhotonMgr.Instance.TryToJoinRoom(info.Name, () =>
        {
            Debug.Log("방 접속 실패");
            LobbyMgr.Instance.loadingPanel.SetActive(false);
            //LobbyMgr.Instance.failRoomPanel.SetActive(true);
            GetComponentInChildren<FindRoomPanel>().CreateRoom(PhotonMgr.Instance.curRoomInfos);
        } );
    }
}
