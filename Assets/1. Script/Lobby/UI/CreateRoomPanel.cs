using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomPanel : MonoBehaviour
{
    public TMP_InputField roomTitleInputField;
    public void OnClickedCreateRoomBtn()
    {
        if (string.IsNullOrEmpty(LobbyMgr.Instance.nickNameInputField.text))
        {
            return;
        }
        if (string.IsNullOrEmpty(roomTitleInputField.text))
        {
            return;
        }
        PhotonMgr.Instance.nickName = LobbyMgr.Instance.nickNameInputField.text;
        LobbyMgr.Instance.loadingPanel.SetActive(true);
        PhotonMgr.Instance.CreateRoom(roomTitleInputField.text, 2);
    }
}
