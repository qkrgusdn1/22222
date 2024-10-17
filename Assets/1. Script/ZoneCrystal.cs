using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoneCrystal : MonoBehaviourPunCallbacks, Fighter
{
    public float hp;
    public float maxHp;
    public Image hpBar;
    public Image hpBarSecondImage;
    Coroutine smoothHpBar;
    Zone zone;

    public GameObject FighterObject => gameObject;

    public void Start()
    {
        hp = maxHp;
        hpBar.fillAmount = 1;
        hpBarSecondImage.fillAmount = 1;
        zone = GetComponentInParent<Zone>();
    }

    public void TakeDamage(float damage, int hitterViewID)
    {
        Debug.Log("hitterViewID = " + hitterViewID);
        photonView.RPC("RPCTakeDamage", RpcTarget.All, damage, hitterViewID);
    }
    [PunRPC]
    public void RPCHpZero(int viewID)
    {
        Player player = PhotonNetwork.GetPhotonView(viewID).GetComponent<Player>();
        
        hp = maxHp;
        hpBar.fillAmount = 1;
        hpBarSecondImage.fillAmount = 1;
        if (GameMgr.Instance.player != player)
        {
            for (int i = 0; i < zone.units.Count; i++)
            {
                if (zone.units[i] != null)
                {
                    zone.units[i].hp = zone.units[i].maxHp;
                    zone.units[i].hpBar.fillAmount = 1;
                    zone.units[i].EnterState(UnitState.Idle);
                }
            }
        }
        else
        {
            for (int i = 0; i < zone.units.Count; i++)
            {
                if(zone.units[i] != null)
                {
                    player.Catch(zone.units[i]);
                    gameObject.layer = LayerMask.NameToLayer("Friendly");
                    hpBar.color = Color.green;
                }
            }
        }
        zone.units.Clear();
    }

    [PunRPC]
    public void RPCTakeDamage(float damage, int viewID)
    {
        hp -= damage;
        hpBar.fillAmount = hp / maxHp;
        if (smoothHpBar != null)
        {
            StopCoroutine(smoothHpBar);
            smoothHpBar = null;
        }
        smoothHpBar = StartCoroutine(CoSmoothHpBar(hpBar.fillAmount, 1));
        if (hp <= 0)
        {
            if (smoothHpBar != null)
            {
                StopCoroutine(smoothHpBar);
                smoothHpBar = null;
            }
        }
    }
    private IEnumerator CoSmoothHpBar(float targetFillAmount, float duration)
    {
        float elapsedTime = 0f;
        float startFillAmount = hpBarSecondImage.fillAmount;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            hpBarSecondImage.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / duration);
            yield return null;
        }

        hpBarSecondImage.fillAmount = targetFillAmount;
    }

    public void Attack(Fighter target, float damage)
    {

    }

}
