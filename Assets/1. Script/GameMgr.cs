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

    public Inventory inventory;
    public Player player;
    public float maxOneStarTimer;
    public float maxTwoStarTimer;
    public float maxThreeStarTimer;
}
