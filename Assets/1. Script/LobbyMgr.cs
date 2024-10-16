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

    private void Start()
    {
        AudioMgr.Instance.waitingMusic.gameObject.SetActive(false);
        AudioMgr.Instance.inGameMusic.gameObject.SetActive(false);
        AudioMgr.Instance.selectMusic.gameObject.SetActive(false);
        AudioMgr.Instance.lobbyMusic.gameObject.SetActive(true);
    }

    public TMP_InputField nickNameInputField;
    public GameObject loadingPanel;
    public GameObject failRoomPanel;

    public void OnClikedExitBtn()
    {
        Application.Quit();
    }
}
