using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//���� ��ɿ� ���� �ݹ� ó�� ���� �� �ְ� MonoBehaviourPunCallbacks Ŭ���� ���
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

    private void Awake()
    {
        instance = this;
    }
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
        //Load Scene �̵� ó�� 
    }

    //���� �� ����Ʈ ���� �� ȣ�� ��
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        Debug.Log("OnRoomListUpdate �κ� ���� �� ����Ʈ " + roomList.Count);
    }

    public void TryToJoinRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom(); //������ �� ���� or �� ����
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Room Name : " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        
    }

    
}
