using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingCanvas : MonoBehaviour
{
    int idx;
    public void OnClickedSpawnPointBtn(int idx)
    {
        this.idx = idx;
        for (int i = 0; i < GameMgr.Instance.spawnPoints.Length; i++)
        {
            if (GameMgr.Instance.spawnPoints[i].spawnIdx == idx)
            {
                GameMgr.Instance.character.StartGame(GameMgr.Instance.spawnPoints[i].transform.position);
                break;
            }
        }
    }
}
