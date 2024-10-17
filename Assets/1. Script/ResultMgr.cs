using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultMgr : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text loadingText;
    public Animator playerAnimator;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (PhotonMgr.Instance.lose)
        {
            resultText.text = "ÆÐ¹è";
            playerAnimator.Play("Die");
        }
        else
        {
            resultText.text = "½Â¸®";
        }
        StartCoroutine(CoLoding());
        StartCoroutine(CoSceneMove());
    }
    IEnumerator CoLoding()
    {
        while (true)
        {
            loadingText.text = "Loading.";
            yield return new WaitForSeconds(0.5f);
            loadingText.text = "Loading..";
            yield return new WaitForSeconds(0.5f);
            loadingText.text = "Loading...";
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator CoSceneMove()
    {
        yield return new WaitForSeconds(5);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Lobby");
    }
}
