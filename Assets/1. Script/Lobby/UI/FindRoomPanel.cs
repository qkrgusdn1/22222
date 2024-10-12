using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Photon.Realtime;

public class FindRoomPanel : MonoBehaviour
{
    public RoomJoinBtn roomJoinBtnPrefab;
    public RectTransform parentTr;
    List<RoomJoinBtn> roomJoinBtns = new List<RoomJoinBtn>();
    public TMP_InputField filterTagInputField;

    private void OnEnable()
    {
        CreateRoom(PhotonMgr.Instance.curRoomInfos);
        StartCoroutine(CoRefreshRoom());
    }

    public void CreateRoom(List<RoomInfo> roomInfos)
    {
        int roomCount = roomInfos.Count;

        for (int i = 0; i < roomJoinBtns.Count; i++)
        {
            if (i < roomCount)
            {
                roomJoinBtns[i].SetRoomInfo(roomInfos[i]);
                roomJoinBtns[i].gameObject.SetActive(true);
            }
            else
            {
                roomJoinBtns[i].gameObject.SetActive(false);
            }
        }

        for (int i = roomJoinBtns.Count; i < roomCount; i++)
        {
            RoomJoinBtn roomJoinBtn = Instantiate(roomJoinBtnPrefab, parentTr);
            roomJoinBtn.SetRoomInfo(roomInfos[i]);
            roomJoinBtns.Add(roomJoinBtn);
        }
    }

    IEnumerator CoRefreshRoom()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            CreateRoom(PhotonMgr.Instance.curRoomInfos);
        }
    }

    public void OnClickedFilterBtn()
    {
        if (string.IsNullOrEmpty(filterTagInputField.text))
        {
            StartCoroutine(CoRefreshRoom());
            return;
        }
        StopCoroutine(CoRefreshRoom());
        List<RoomInfo> filteredInfos = PhotonMgr.Instance.curRoomInfos.Where(e =>
        {
            return e.Name.Contains(filterTagInputField.text);
        }).ToList();
        CreateRoom(filteredInfos);
    }
}
