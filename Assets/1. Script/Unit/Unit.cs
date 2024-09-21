using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEditor;

public class Unit : MonoBehaviour
{
    public UnitBehaviour[] unitBehaviours;
    public UnitBehaviour curUnitBehaviour;

    public UnitType unitType;

    public bool friendly;

    public NavMeshAgent agent;
    public Player target;
    public Rigidbody rb;
    public GameObject rangePoint;

    public GameObject body;
    public Animator animator;
    public float moveSpeed;
    public float targetingRange;
    public float attackRange;
    public float knockDownRange;

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

    public int attackAmount;

    public LayerMask targetLayer;

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
        for(int i = 0; i < unitBehaviours.Length; i++)
        {
            unitBehaviours[i].InitUnit(this);
        }
        animationEventHandler.endAttackListener += EndAttack;
        animationEventHandler.dieListener += Die;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    public UnitBehaviour GetUnitBehaviour(UnitBehaviourType type)
    {
        if(type == UnitBehaviourType.Wild)
        {
            targetLayer = LayerMask.NameToLayer("Friendly");
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
        else if(type == UnitBehaviourType.Reguler)
        {
            targetLayer = LayerMask.NameToLayer("Enemy");
            gameObject.layer = LayerMask.NameToLayer("Friendly");
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

    private void OnEnable()
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

    public virtual void EnterState(UnitState state)
    {
        
        if (unitState == state)
            return;
        
        unitState = state;

        curUnitBehaviour.EnterState(state);
       
    }


    
    //Attack 애니메이션이 끝나면 호출되는 함수!
    public void EndAttack()
    {
        endAttack = true;
        attackTimer = 0;
    }

   

    void Die()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        curUnitBehaviour.UpdateState(unitState);
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

    public void TakeDamage(float damage)
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
            animator.Play("Die");
        }
    }

}

public enum UnitState
{
    Idle,
    Approach,
    Attack,
    KnockDown
}

public enum UnitType
{
    oneStar,
    twoStar,
    threeStar
}
