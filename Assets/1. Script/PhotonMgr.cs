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
    //������ ���� ���� �õ�
    public void TryToJoinServer()
    {
        Debug.Log("���� ���� �õ�");

        //������ ������ ����Ǿ��ִ��� Ȯ��
        if (!PhotonNetwork.IsConnected) //�����Ϳ� ���ӵƴ��� Ȯ���ϴ� �ڵ�
        {
            Debug.Log("������ �ȵǾ� �ֽ��ϴ�.");
            PhotonNetwork.ConnectUsingSettings();//Settings�� ������ �������� ���� �õ�
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
    //������ ���� ���ӵǸ� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        //���� ������ ������ ���� Ȯ��
        Debug.Log("PhotonNetwork.CloudRegion : " + PhotonNetwork.CloudRegion);
        Debug.Log("���� ���� �Ϸ�");
        if (!PhotonNetwork.InLobby) //�κ� ���� �ƴϸ�
        {
            Debug.Log("�κ�� ���� �õ��մϴ�.");
            PhotonNetwork.JoinLobby();
        }
        else //�̹� �κ� ���̸�
        {
            OnJoinedLobby();

        }
    }

    //�κ� ��ġ������ �ʿ��� ó�� - ������ �� �ִ� �� ����ƮPhotonNetwork �����ݴϴ�. 
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("�κ� ���� �Ϸ�");
        PhotonNetwork.LoadLevel("Lobby");
        //Load Scene �̵� ó�� 
    }

    public List<RoomInfo> curRoomInfos;

    //���� �� ����Ʈ ���� �� ȣ�� ��
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
