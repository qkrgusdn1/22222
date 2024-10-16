using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public TMP_Text lodingText;
    public float lodingCount;
    private void Start()
    {
        StartCoroutine(CoLoding());
    }

    IEnumerator CoLoding()
    {
        while (true)
        {
            lodingText.text = "Loading.";
            yield return new WaitForSeconds(lodingCount);
            lodingText.text = "Loading..";
            yield return new WaitForSeconds(lodingCount);
            lodingText.text = "Loading...";
            yield return new WaitForSeconds(lodingCount);
        }
    }
}
