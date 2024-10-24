using Cinemachine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
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
    int dieSpawnCount;
    public int maxDieSpawnCount;
    public GameObject selectSpawnCanvas;
    public SelectButton currentSelectButton;
    public CinemachineVirtualCamera virtualCamera;
    public TMP_Text timerText;
    public int minute;
    public int second;
    public GameObject resultPanel;
    public TMP_Text resultText;
    public GameObject diePanel;
    public TMP_Text dieCountText;
    public bool result;
    int zoneAmount;
    private void Start()
    {
        AudioMgr.Instance.waitingMusic.gameObject.SetActive(false);
        AudioMgr.Instance.inGameMusic.gameObject.SetActive(false);
        AudioMgr.Instance.selectMusic.gameObject.SetActive(true);
        AudioMgr.Instance.lobbyMusic.gameObject.SetActive(false);
        GameObject characterObj = PhotonNetwork.Instantiate("Character", Vector3.zero, Quaternion.identity);
        character = characterObj.GetComponent<Character>();
       
        StartCoroutine(CountDown());
    }


    public IEnumerator CoGoResultScene()
    {
        Debug.Log("CoGoResultScene");
        yield return new WaitForSeconds(5);
        PhotonNetwork.LoadLevel("Result");
    }
    public IEnumerator DieCountDown()
    {
        Debug.Log("DieCountDown");
        dieSpawnCount = maxDieSpawnCount;
        while (dieSpawnCount > -1)
        {
            dieCountText.text = dieSpawnCount.ToString();
            yield return new WaitForSeconds(1);
            dieSpawnCount--;
        }
        
        player.animator.Play("Idle");
        diePanel.gameObject.SetActive(false);
        player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        photonView.RPC("RPCSetReSpawnPlayer", RpcTarget.All, player.photonView.ViewID);
    }
    IEnumerator CountDown()
    {
        spawnCount = maxSpawnCount;
        while (spawnCount > -1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPCUpdateCountdown", RpcTarget.All, spawnCount);
            }
            yield return new WaitForSeconds(1);
        }
        if(photonView.IsMine)
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
        StartCoroutine(CoTimer());
    }
    
    IEnumerator CoTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
                
            if (second <= -1)
            {
                second = 59;
                minute--;
            }
            if(minute <= -1)
            {
                break;
            }
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC("RPCUpdateTimer", RpcTarget.All);
        }
        photonView.RPC("RPCResult", RpcTarget.All);
    }
    [PunRPC]
    public void RPCResult()
    {
        for (int i = 0; i < zones.Length; i++)
        {
            if (zones[i].possessions)
            {
                zoneAmount++;
            }
        }
        if (zoneAmount >= 2)
        {
            PhotonMgr.Instance.lose = false;
        }
        else if (zoneAmount < 2)
        {
            PhotonMgr.Instance.lose = true;
        }
        result = true;
        if (PhotonMgr.Instance.lose)
        {
            resultText.text = "ÆÐ¹è";
        }
        else
        {
            resultText.text = "½Â¸®";
        }
        resultPanel.SetActive(true);
        StartCoroutine(CoGoResultScene());
    }
    [PunRPC]
    public void RPCUpdateTimer()
    {
        timerText.text = minute + ":" + second.ToString("00");
        second--;
    }

    [PunRPC]
    public void RPCSetReSpawnPlayer(int playerViewID)
    {
        Player player = PhotonView.Find(playerViewID).GetComponent<Player>();
        player.die = false;
        player.rb.isKinematic = false;
        player.mainCollider.enabled = true;
        player.rollCollider.enabled = false;
        player.hp = player.maxHp;
        player.hpBar.fillAmount = 1;
        player.hpBarSecondImage.fillAmount = 1;
        player.outHpBar.fillAmount = 1;
        player.outHpBarSecondImage.fillAmount = 1;
    }

    [PunRPC]
    public void RPCUpdateCountdown(int remainingTime)
    {
        spawnCountText.text = remainingTime.ToString();
        spawnCount--;
    }

}
