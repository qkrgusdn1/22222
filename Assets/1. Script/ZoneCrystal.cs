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

    public void Start()
    {
        hp = maxHp;
        hpBar.fillAmount = 1;
        hpBarSecondImage.fillAmount = 1;
        zone = GetComponentInParent<Zone>();
    }

    public void TakeDamage(float damage)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPCTakeDamage", RpcTarget.All, damage);
            photonView.RPC("RPCHpZero", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }
    [PunRPC]
    public void RPCHpZero(int selectingPlayerId)
    {
        if (hp > 0)
            return;
        hp = maxHp;
        hpBar.fillAmount = 1;
        hpBarSecondImage.fillAmount = 1;
        if (selectingPlayerId != PhotonNetwork.LocalPlayer.ActorNumber)
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
                    gameObject.layer = LayerMask.NameToLayer("Friendly");
                    hpBar.color = Color.green;
                    zone.units[i].curUnitBehaviour = zone.units[i].GetUnitBehaviour(UnitBehaviourType.Reguler);
                    zone.units[i].curUnitBehaviour.GetComponent<UnitBehaviour>().PlayerSetting(GameMgr.Instance.player);
                    zone.units[i].hpBar.color = Color.green;
                    zone.units[i].hp = zone.units[i].maxHp;
                    zone.units[i].catchBarBgImage.SetActive(false);
                    zone.units[i].hpBar.fillAmount = 1;
                    zone.units[i].EnterState(UnitState.Idle);
                    zone.units[i].animator.SetBool("IsKnockDown", false);
                    zone.units[i].agent.isStopped = false;
                    FriendlyBtn friendlyBtn = Instantiate(GameMgr.Instance.player.friendlyBtnPrefab, GameMgr.Instance.inventory.friendlyBtnGroup.transform);
                    GameMgr.Instance.player.friendlyBtns.Add(friendlyBtn);
                    friendlyBtn.SetFriendlyBtn(GameMgr.Instance.inventory, zone.units[i]);
                    zone.units[i].unitState = UnitState.Idle;
                    zone.units[i].zoneUnit = false;
                    zone.units[i].GetComponent<RegularUnitBehaviour>().regularUnitState = RegularUnitState.Defender;
                }
            }
        }
    }

    [PunRPC]
    public void RPCTakeDamage(float damage)
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
