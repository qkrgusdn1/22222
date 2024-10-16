using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEditor;
using static UnityEngine.UI.CanvasScaler;
using TMPro;
using Photon.Pun;
using System;

public class Unit : MonoBehaviourPunCallbacks, Fighter
{
    public UnitBehaviour[] unitBehaviours;
    public UnitBehaviour curUnitBehaviour;

    public UnitType unitType;

    public NavMeshAgent agent;
    public GameObject target;
    public Rigidbody rb;
    public GameObject rangePoint;

    public GameObject body;
    public Animator animator;
    public float moveSpeed;
    public float targetingRange;
    public float attackRange;
    public float knockDownRange;
    public float zoneDistanceToPlayer;

    public Weapon weapon;

    public float hp;
    public float maxHp;
    public Image hpBar;
    public Image hpBarBg;
    public Image hpBarSecondImage;
    public UnitState unitState;

    public GameObject catchBarBgImage;
    public Image catchBarImage;

    public AnimationEventHandler animationEventHandler;

    public float attackTimer;
    public float maxAttackTimer;
    public bool endAttack;

    public string unitName;

    public int attackAmount;

    public GameObject regularStateBg;

    public LayerMask targetLayer;

    public TMP_Text regularStateText;

    public Sprite thumSprite;

    public Sprite image;
    public Zone zone;

    public UnitBehaviourType unitBehaviourType;
    public GameObject fKeyImage;
    public LayerMask friendlyLayer;

    public bool die;

    public bool zoneUnit;

    public Vector3 turnPoint;

    public Player ownerPlayer;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rangePoint.transform.position, targetingRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(rangePoint.transform.position, knockDownRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(rangePoint.transform.position, attackRange);
    }

    private void Awake()
    {
        InitializeUnit();
        animationEventHandler.finishAttackListener += FinishAttack;
        animationEventHandler.startAttackListener += StartAttack;
        animationEventHandler.endAttackListener += EndAttack;
        animationEventHandler.dieListener += Die;
        rb = GetComponent<Rigidbody>(); 
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    private void InitializeUnit()
    {
        unitBehaviours = GetComponentsInChildren<UnitBehaviour>();
        curUnitBehaviour = GetUnitBehaviour(UnitBehaviourType.Wild);
        attackTimer = maxAttackTimer;
        catchBarImage.fillAmount = 0;
        hpBarBg.gameObject.SetActive(false);
        hp = maxHp;
        EnterState(UnitState.Idle);
        animator.SetBool("IsKnockDown", false);
    }

    public UnitBehaviour GetUnitBehaviour(UnitBehaviourType type)
    {
        unitBehaviourType = type;
        if (type == UnitBehaviourType.Wild)
        {
            targetLayer = LayerMask.GetMask("Friendly");
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            weapon.hitLayerMask = targetLayer;
            
        }
        else if(type == UnitBehaviourType.Reguler)
        {
            turnPoint = transform.position;
            targetLayer = LayerMask.GetMask("Enemy");
            gameObject.layer = LayerMask.NameToLayer("Friendly");
            weapon.hitLayerMask = targetLayer;
        }

        for(int i = 0; i < unitBehaviours.Length; i++)
        {
            if (unitBehaviours[i].unitBehaviourType == type)
            {
                return unitBehaviours[i];
            }
        }
        return null;
    }

    public virtual void EnterState(UnitState state)
    {
        if (unitState == state)
            return;

        string stateName = state.ToString();
        photonView.RPC("RPCEnterState", RpcTarget.All, stateName);
    }
    [PunRPC]
    public void RPCEnterState(string stateName)
    {
        if (Enum.TryParse(stateName, out UnitState state))
        {
            if (unitState == state)
                return;

            unitState = state;
            curUnitBehaviour.EnterState(state);

            if (state == UnitState.Idle)
            {
                target = null;
            }

            if (state == UnitState.Attack)
            {
                AttackStart();
            }
        }
    }
    public void Attack(Fighter target, float damage)
    {
        if (target != null)
            target.TakeDamage(damage, photonView.ViewID);
    }
    public void AttackStart()
    {
        animator.SetBool("IsRunning", false);
        attackTimer = maxAttackTimer;
        photonView.RPC("RPCCrossFadeAttack", RpcTarget.All, attackAmount);
    }
    [PunRPC]
    public void RPCCrossFadeAttack(int attackAmount)
    {
        animator.CrossFade("Attack" + attackAmount, 0.1f);
    }

    public void StartAttack()
    {
        weapon.StartAttack();
    }

    public void EndAttack()
    {
        weapon.EndAttack();
    }

    public void FinishAttack()
    {
        endAttack = true;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        curUnitBehaviour.UpdateState(unitState);

        if (!PhotonNetwork.IsMasterClient)
            return;

        if(unitState == UnitState.Approach && target != null)
        {
            agent.SetDestination(target.transform.position);

            Vector3 direction = (target.transform.position - transform.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            body.transform.rotation = Quaternion.Slerp(body.transform.rotation, lookRotation, Time.deltaTime * 100);
        }else if(unitState == UnitState.Turn && zoneUnit)
        {
            agent.SetDestination(turnPoint);
            Vector3 direction = (turnPoint - transform.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            body.transform.rotation = Quaternion.Slerp(body.transform.rotation, lookRotation, Time.deltaTime * 100);
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



    public void TakeDamage(float damage, int hitterID)
    {
        photonView.RPC("RPCTakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    public void RPCTakeDamage(float damage)
    {
        hpBarBg.gameObject.SetActive(true);
        hp -= damage;

        if (hp / maxHp <= 0.2f)
        {
            EnterState(UnitState.KnockDown);
        }


        hpBar.fillAmount = hp / maxHp;
        StartCoroutine(CoSmoothHpBar(hpBar.fillAmount, 1));
        if (hp <= 0)
        {
            die = true;
            animator.Play("Die");
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
        }
        else
        {
            hp = (float)stream.ReceiveNext();

        }
    }
    public void Catched(Player player)
    {
        ownerPlayer = player;
        hp = maxHp;
        catchBarBgImage.SetActive(false);
        hpBar.fillAmount = 1;
        EnterState(UnitState.Idle);
        animator.SetBool("IsKnockDown", false);
        agent.isStopped = false;

        if (player.photonView.IsMine)
        {
            curUnitBehaviour = GetUnitBehaviour(UnitBehaviourType.Reguler);
            curUnitBehaviour.GetComponent<UnitBehaviour>().PlayerSetting(player);
            hpBar.color = Color.green;

            photonView.RPC("RPCZoneUnit", RpcTarget.All);
            RegularUnitBehaviour regularUnitBehaviour = (RegularUnitBehaviour)curUnitBehaviour;
            regularUnitBehaviour.OnClickedChangeRegularUnitStateBtn(RegularUnitState.Defender.ToString());
        }
    }
    [PunRPC]
    public void RPCZoneUnit()
    {
        zoneUnit = false;
    }

}

public enum UnitState
{
    Idle,
    Approach,
    Attack,
    KnockDown,
    Turn
}

public enum UnitType
{
    oneStar,
    twoStar,
    threeStar
}
