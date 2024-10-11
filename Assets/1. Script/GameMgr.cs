using Photon.Pun;
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
    public float spawnCount;
    public float maxSpawnCount;
    bool gameStart;
    private void Start()
    {
        GameObject obj = PhotonNetwork.Instantiate("Character", Vector3.zero, Quaternion.identity);
        photonView.RPC("RPCCountDown", RpcTarget.All);
        character = obj.GetComponent<Character>();
    }
    

    [PunRPC]
    public void RPCCountDown()
    {
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        spawnCount = maxSpawnCount;
        while (true)
        {
            yield return null;
            if (gameStart)
            {
                character.StartGame(spawnPoints[spawnPointIdx].transform.position);
                break;
            }
                
            if (spawnCount >= 0)
            {
                spawnCount -= Time.deltaTime;
                spawnCountText.text = spawnCount.ToString("F0");
            }
            else
            {
                for(int i = 0; i < spawnPointIdx; i++)
                {

                }
            }
        }

    }
    
}
