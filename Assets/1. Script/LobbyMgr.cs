using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyMgr : MonoBehaviour
{
    private static LobbyMgr instance;

    public static LobbyMgr Instance
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

    public TMP_InputField nickNameInputField;
    public GameObject loadingPanel;
    public GameObject failRoomPanel;

    public void OnClikedExitBtn()
    {
        Application.Quit();
    }
}
