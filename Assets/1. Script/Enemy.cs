using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;

    public bool friendly;

    public NavMeshAgent agent;
    public Player target;
    Rigidbody rb;
    public GameObject rangePoint;

    public GameObject body;
    public Animator animator;
    public float moveSpeed;
    public float targetingRange;
    public float attackRange;
    public float knockDownRange;
    public float stoppingDistance;

    public float hp;
    public float maxHp;
    public Image hpBar;
    public Image hpBarBg;

    public EnemyState enemyState;

    public GameObject catchBarBgImage;
    public Image catchBarImage;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rangePoint.transform.position, targetingRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(rangePoint.transform.position, stoppingDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(rangePoint.transform.position, knockDownRange);
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(rangePoint.transform.position, attackRange);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    private void OnEnable()
    {
        catchBarImage.fillAmount = 0;
        hpBarBg.gameObject.SetActive(false);
        hp = maxHp;
        EnterState(EnemyState.Idle);
        animator.SetBool("IsKnockDown", false);
    }

    public void EnterState(EnemyState state)
    {
        if (enemyState == state)
            return;

        enemyState = state;
        if (enemyState == EnemyState.Idle)
        {
            agent.isStopped = true;
        }
        if (enemyState == EnemyState.Approach)
        {
            //*******코드 삭제됨*******
            agent.isStopped = false;
        }
        else if (enemyState == EnemyState.Attack)
        {
            animator.Play("Attack");
            agent.isStopped = true;
            endAttack = false;
        }else if(enemyState == EnemyState.KnockDown)
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsKnockDown", true);
        }
    }


    float attackTimer;
    bool endAttack;
    //Attack 애니메이션이 끝나면 호출되는 함수!
    public void EndAttack()
    {
        endAttack = true;
        attackTimer = 0;
    }

    public LayerMask targetLayer;
    void UpdateIdleState() //Idle 행동 - 어떤 활동 필요?
    {
        if (target == null)
        {
            EnterState(EnemyState.Idle);
            return;
        }

        //?????? ???? Idle?? ????
        float disTarget = Vector3.Distance(target.transform.position, transform.position);
        if (disTarget > targetingRange)
        {
            EnterState(EnemyState.Idle);
            return;
        }
        else if (disTarget <= attackRange)
        {
            EnterState(EnemyState.Attack);
        }
        Vector3 direction = (target.transform.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, lookRotation, Time.deltaTime * 100);
        agent.SetDestination(target.transform.position);
    }

    void UpdateApproachState()
    {
        //어떤 코드가 필요한가요?

        //타겟이 null일때 , 멀때 -> Idle
        //타겟이 공격 범위 안으로 들어왔을 때 -> Attack

        if (target == null)
        {
            EnterState(EnemyState.Idle);
            return;
        }

        //타겟이 멀면 Idle로 전환
        float disTarget = Vector3.Distance(target.transform.position, transform.position);
        if (disTarget > targetingRange)
        {
            EnterState(EnemyState.Idle);
            return;
        }
        else if (disTarget <= attackRange)
        {
            EnterState(EnemyState.Attack);
        }

        //******* 수정된 코드 ********
        agent.SetDestination(target.transform.position);

        //타겟을 바라보는 코드
        Vector3 direction = (target.transform.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, lookRotation, Time.deltaTime * 100);
    }


    void UpdateAttackState()
    {
        if (endAttack)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                EnterState(EnemyState.Idle);
            }
        }
    }

    void UpdateKnockDownState()
    {
        Collider[] colliders = Physics.OverlapSphere(rangePoint.transform.position, knockDownRange, targetLayer);

        bool isPlayerInRange = false;

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            { 
                isPlayerInRange = true;
                break;
            }
        }
        if (isPlayerInRange)
        {
            if(!friendly)
                catchBarBgImage.SetActive(true);
        }
        else
        {
            catchBarImage.fillAmount = 0;
            catchBarBgImage.SetActive(false);
        }
    }

    private void Update()
    {
        if (enemyState == EnemyState.Idle)
        {
            UpdateIdleState();
        }
        else if (enemyState == EnemyState.Approach)
        {
            UpdateApproachState();
        }
        else if (enemyState == EnemyState.Attack)
        {
            UpdateAttackState();
        }
        else if(enemyState == EnemyState.KnockDown)
        {
            UpdateKnockDownState();
            return;
        }

        if (rangePoint != null)
        {
            float distanceToPlayer = Vector3.Distance(rangePoint.transform.position, target.transform.position);

            if (distanceToPlayer <= targetingRange)
            {
                if (distanceToPlayer > stoppingDistance)
                {
                    animator.SetBool("IsRunning", true);
                    agent.isStopped = false;
                }
                else
                {
                    animator.SetBool("IsRunning", false);
                    agent.isStopped = true;
                    agent.velocity = new Vector3(0, rb.velocity.y, 0);
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                }
            }
            else
            {
                animator.SetBool("IsRunning", false);
                agent.isStopped = true;
                agent.velocity = new Vector3(0, rb.velocity.y, 0);
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        hpBarBg.gameObject.SetActive(true);
        hp -= damage;

        if (hp / maxHp <= 0.2f)
        {
            EnterState(EnemyState.KnockDown);
        }


        hpBar.fillAmount = hp / maxHp;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

}

public enum EnemyState
{
    Idle,
    Approach,
    Attack,
    KnockDown
}

public enum EnemyType
{
    oneStar,
    twoStar,
    threeStar
}
