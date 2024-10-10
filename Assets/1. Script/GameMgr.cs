using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
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

    private void Start()
    {
        GameObject obj = PhotonNetwork.Instantiate("Character", Vector3.zero, Quaternion.identity);
        character = obj.GetComponent<Character>();
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
}
