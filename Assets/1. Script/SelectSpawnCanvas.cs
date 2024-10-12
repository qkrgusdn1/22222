using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSpawnCanvas : MonoBehaviour
{
    public void OnClickedSpawnPointBtn(int idx)
    {
        GameMgr.Instance.spawnPointIdx = idx;
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
