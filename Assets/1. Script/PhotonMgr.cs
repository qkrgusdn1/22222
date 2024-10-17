using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.SceneManagement;

public class PhotonMgr : MonoBehaviourPunCallbacks
{
    private static PhotonMgr instance;
    
    public static PhotonMgr Instance
    {
        get
        {
            return instance;
        }
    }
    public bool IsReadyToJoinRoom()
    {
        return PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby;
    }

    private void Awake()
    {
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 15;
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public bool result;
    public bool lose;
    public string nickName;
    

    private void Start()
    {
        TryToJoinServer();
    }
    //마스터 서버 접속 시도
    public void TryToJoinServer()
    {
        Debug.Log("서버 연결 시도");

        //마스터 서버에 연결되어있는지 확인
        if (!PhotonNetwork.IsConnected) //마스터에 접속됐는지 확인하는 코드
        {
            Debug.Log("연결이 안되어 있습니다.");
            PhotonNetwork.ConnectUsingSettings();//Settings에 설정된 내용으로 연결 시도
        }
        else
        {
            OnConnectedToMaster();
        }

    }
    public void CreateRoom(string roomTitle, int maxPlayer)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayer;
        PhotonNetwork.CreateRoom(roomTitle, roomOptions);
    }
    //마스터 서버 접속되면 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        //서버 접속한 마스터 서버 확인
        Debug.Log("PhotonNetwork.CloudRegion : " + PhotonNetwork.CloudRegion);
        Debug.Log("서버 접속 완료");
        if (!PhotonNetwork.InLobby) //로비 안이 아니면
        {
            Debug.Log("로비로 접속 시도합니다.");
            PhotonNetwork.JoinLobby();
        }
        else //이미 로비 안이면
        {
            OnJoinedLobby();

        }
    }

    //로비 위치에서의 필요한 처리 - 참여할 수 있는 룸 리스트PhotonNetwork 보여줍니다. 
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("로비 접속 완료");
        PhotonNetwork.LoadLevel("Lobby");
        //Load Scene 이동 처리 
    }

    public List<RoomInfo> curRoomInfos;

    //서버 룸 리스트 갱신 시 호출 됨
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        curRoomInfos = roomList;
    }

    Action failCallBack;
    public void TryToJoinRoom(string name = null, Action failCallBack = null)
    {
        this.failCallBack = failCallBack;
        if (string.IsNullOrEmpty(name))
            PhotonNetwork.JoinRandomOrCreateRoom();
        else
        {
            PhotonNetwork.JoinRoom(name);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        failCallBack?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Room Name : " + PhotonNetwork.CurrentRoom.Name);
        SceneManager.LoadScene("Wating");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        
    }

    
}
