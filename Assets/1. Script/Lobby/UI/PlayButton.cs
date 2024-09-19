using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickedPlayBtn);
    }
    void OnClickedPlayBtn()
    {
        PhotonMgr.Instance.TryToJoinRoom();
    }
}
