using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviourPunCallbacks
{
    public Sprite selectSprite;
    public Sprite unableToSelectSprite;
    public Sprite normalSprite;          
    public Image image;
    public bool selected;
    public int idx;
    bool other;

    public void OnClickedSelectBtn()
    {
        if (!other)
        {
            if (GameMgr.Instance.currentSelectButton != null)
            {
                GameMgr.Instance.currentSelectButton.photonView.RPC("RPCChangeNormalRPC", RpcTarget.All);
            }
            GameMgr.Instance.spawnPointIdx = idx;
            GameMgr.Instance.currentSelectButton = this;
            photonView.RPC("RPCUpdateButtonSpriteRPC", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }
       
    }

    [PunRPC]
    public void RPCChangeNormalRPC()
    {
        image.sprite = normalSprite;
        other = false;
    }

    [PunRPC]
    public void RPCUpdateButtonSpriteRPC(int selectingPlayerId)
    {
        if (selectingPlayerId != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            other = true;
            image.sprite = unableToSelectSprite;
        }
        else
        {
            image.sprite = selectSprite;
        }
    }
}
