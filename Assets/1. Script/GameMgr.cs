using Cinemachine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviourPunCallbacks
{
    private static GameMgr instance;
    public static GameMgr Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        instance = this;
    }
    public Character character;
    public SpawnPoint[] spawnPoints;
    public Inventory inventory;
    public Player player;
    public float maxOneStarTimer;
    public float maxTwoStarTimer;
    public float maxThreeStarTimer;
    public GameObject trunPointGroup;
    public Zone[] zones;
    public int spawnPointIdx;
    public TMP_Text spawnCountText;
    int spawnCount;
    public int maxSpawnCount;
    public GameObject selectSpawnCanvas;
    public SelectButton currentSelectButton;
    public CinemachineVirtualCamera virtualCamera;
    private void Start()
    {
        AudioMgr.Instance.waitingMusic.gameObject.SetActive(false);
        AudioMgr.Instance.inGameMusic.gameObject.SetActive(false);
        AudioMgr.Instance.selectMusic.gameObject.SetActive(true);
        AudioMgr.Instance.lobbyMusic.gameObject.SetActive(false);
        GameObject characterObj = PhotonNetwork.Instantiate("Character", Vector3.zero, Quaternion.identity);
        character = characterObj.GetComponent<Character>();
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        spawnCount = maxSpawnCount;
        while (spawnCount > -1)
        {
            photonView.RPC("RPCUpdateCountdown", RpcTarget.All, spawnCount);
            yield return new WaitForSeconds(1);
            spawnCount--;
        }
        photonView.RPC("RPCStartGame", RpcTarget.All);
    }
    [PunRPC]
    public void RPCStartGame()
    {
        AudioMgr.Instance.waitingMusic.gameObject.SetActive(false);
        AudioMgr.Instance.inGameMusic.gameObject.SetActive(true);
        AudioMgr.Instance.selectMusic.gameObject.SetActive(false);
        AudioMgr.Instance.lobbyMusic.gameObject.SetActive(false);
        selectSpawnCanvas.SetActive(false);
        character.StartGame(spawnPoints[spawnPointIdx].transform.position);
    }

    [PunRPC]
    public void RPCUpdateCountdown(int remainingTime)
    {
        spawnCountText.text = remainingTime.ToString();
    }

}
