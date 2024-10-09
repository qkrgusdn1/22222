using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, Fighter
{
    public float moveSpeed;
    public Transform bodyTr;
    public Animator animator;

    protected Vector3 dir;
    public float cameraRotationX;
    public float cameraRotationY;
    string mouseXAxisName = "Mouse X";
    string mouseYAxisName = "Mouse Y";
    public Transform cameraPointTr;
    public float mouseHorSensitivity = 2;
    public float mouseVerSensitivity = 2;
    public float rotationVelocity;
    string horAxisName = "Horizontal";
    string verAxisName = "Vertical";
    private float targetRotation = 0.0f;
    Vector3 cameraDirection;
    Vector3 inputDirection;
    AnimationEventHandler animationEventHandler;
    Inventory inventory;

    public Collider mainCollider;
    public Collider rollCollider;

    public float hp;
    public float maxHp;
    public Image hpBar;
    public Image hpBarSecondImage;

    public FriendlyBtn friendlyBtnPrefab;
    public List<FriendlyBtn> friendlyBtns = new List<FriendlyBtn>();


    [Tooltip("작을 수록 빠르게 회전함")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    public float controlPower;

    float jumpForce;
    public float maxJumpForce;
    bool isJumping;
    public bool isGrounded;
    public LayerMask groundLayer;
    public LayerMask itemLayer;
    public LayerMask enemyLayer;
    public LayerMask friendlyLayer;
    Rigidbody rb;
    public float checkPickUpRange;

    public GameObject hand;

    public GameObject inventoryBg;
    [HideInInspector]
    public bool IsStop;
    bool noFinshAttack;
    bool onSlope;

    public float rollSpeed;
    private bool isRoll;
    public float rollDuration;
    private float rollTime = 0f;
    private Vector3 rollDirection;

    public int attackAmount;
    const int MAXATTACKAMOUNT = 3;
    float stopAttackTime;
    public float maxStopAttackTime;


    public Unit catchTarget;

    float catchTimer;
    public float maxCatchTimer;

    public bool activeStateBg;

    public List<Unit> friendlyUnits = new List<Unit>();

    Unit currentUnit;

    private void Awake()
    {
        GameMgr.Instance.player = this;
        animationEventHandler = bodyTr.GetComponent<AnimationEventHandler>();
        inventory = GetComponentInChildren<Inventory>();
        animationEventHandler.startAttackListener += StartAttack;
        animationEventHandler.endAttackListener += EndAttack;
        animationEventHandler.finishAttackListener += FinishAttack;
        animationEventHandler.startRollListener += StartRoll;
        animationEventHandler.endRollListener += EndRoll;
        mainCollider.enabled = true;
        rollCollider.enabled = false;
        GameMgr.Instance.inventory = inventory;
    }
    private void Start()
    {
        hp = maxHp;
        hpBar.fillAmount = 1;
        hpBarSecondImage.fillAmount = 1;
        jumpForce = maxJumpForce;
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    public void Attack(Fighter target, float damage)
    {
        if (target != null)
            target.TakeDamage(damage);
    }
    public void StartAttack()
    {
        inventory.currentWeapon.StartAttack();
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;

        hpBar.fillAmount = hp / maxHp;
        StartCoroutine(CoSmoothHpBar(hpBar.fillAmount, 1));
        if (hp <= 0)
        {
            animator.Play("Die");
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
    public void EndAttack()
    {
        inventory.currentWeapon.EndAttack();
    }

    public void FinishAttack()
    {
        noFinshAttack = false;
        IsStop = false;
        inventory.currentWeapon.hittedList.Clear();
        attackAmount++;
        if (attackAmount >= MAXATTACKAMOUNT)
        {
            attackAmount = 0;
        }
        stopAttackTime = maxStopAttackTime;
    }

    public void StartRoll()
    {
        mainCollider.enabled = false;
        rollCollider.enabled = true;
    }

    public void EndRoll()
    {
        mainCollider.enabled = true;
        rollCollider.enabled = false;
    }

    private void Update()
    {

        if (stopAttackTime > 0 && !noFinshAttack)
        {
            stopAttackTime -= Time.deltaTime;
            if (stopAttackTime <= 0)
            {
                attackAmount = 0;
            }

        }
        if (!IsStop)
        {
            SetInputDirection();
            Move();
        }
        else
        {
            animator.SetBool("IsRunning", false);
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        if (Input.GetMouseButtonDown(0) && !isRoll && !IsStop)
        {
            if (inventory.currentWeapon != null && !noFinshAttack)
            {
                noFinshAttack = true;
                IsStop = true;
                Debug.Log("Attack" + attackAmount);
                animator.SetTrigger("Attack" + attackAmount);
            }
        }
        ChecknearRegularUnit();
        if (Input.GetKeyDown(KeyCode.F))
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, checkPickUpRange, itemLayer);
            Debug.Log(cols.Length);
            if (cols.Length >= 1)
            {
                foreach (Collider col in cols)
                {
                    col.GetComponent<Item>().GetItem();
                    break;
                }
            }
            Debug.Log(cols.Length);
            
        }
        if (Input.GetKey(KeyCode.F))
        {
            if (catchTarget != null)
            {
                Dictionary<UnitType, float> unitTimers = new Dictionary<UnitType, float>
                {
                    { UnitType.oneStar, GameMgr.Instance.maxOneStarTimer },
                    { UnitType.twoStar, GameMgr.Instance.maxTwoStarTimer },
                    { UnitType.threeStar, GameMgr.Instance.maxThreeStarTimer }
                };

                if (catchTimer >= unitTimers[catchTarget.unitType])
                {
                    catchTarget.catchBarImage.fillAmount = 1;
                    Catch(catchTarget);
                }
                else
                {
                    catchTarget.catchBarImage.fillAmount = catchTimer / unitTimers[catchTarget.unitType];
                }
                catchTimer += Time.deltaTime;

            }
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            if (catchTarget != null)
            {
                catchTarget.catchBarImage.fillAmount = 0;
            }
            catchTarget = null;
            catchTimer = 0;
        }

        if (Input.GetKeyDown(KeyCode.E) && !activeStateBg)
        {
            Inventory weaponInventory = GetComponentInChildren<Inventory>();
            if (!inventoryBg.activeSelf)
            {
                inventoryBg.SetActive(true);
                inventory.friendlyImageBasic.gameObject.SetActive(true);
                inventory.friendlyImage.gameObject.SetActive(false);
                IsStop = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                animator.SetBool("IsRunning", false);
            }
            else if (inventoryBg.activeSelf)
            {
                inventoryBg.SetActive(false);
                IsStop = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                weaponInventory.selectImage.gameObject.SetActive(false);
                weaponInventory.selectDescription.gameObject.SetActive(false);
                for (int i = 0; i < weaponInventory.buttons.Count; i++)
                {
                    weaponInventory.buttons[i].gameObject.SetActive(false);
                }

            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !isRoll && !IsStop)
        {
            RollStart();
        }

        if (isRoll)
        {
            Roll();
        }



        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping && !isRoll)
        {
            isJumping = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void ChecknearRegularUnit()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, checkPickUpRange, enemyLayer | friendlyLayer);

        float minDistance = float.MaxValue;
        int targetIdx = -1;
        for (int i = 0; i < cols.Length; i++)
        {
            Unit unit = cols[i].GetComponent<Unit>();

            if (unit == null)
                continue;
            if (unit.unitBehaviourType == UnitBehaviourType.Wild)
            {
                if (unit.unitState != UnitState.KnockDown)
                    continue;
            }

            float distance = Vector3.Distance(cols[i].transform.position, transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                targetIdx = i;
            }
        }
        if (targetIdx == -1)
        {
            if (currentUnit != null)
            {
                currentUnit.curUnitBehaviour.UpdateFKeyImage(false);
                if (catchTarget != null)
                {
                    catchTarget.catchBarImage.fillAmount = 0;
                }
                catchTarget = null;
                catchTimer = 0;
            }
            currentUnit = null;
            return;
        }
            

        
        if (currentUnit != null)
                currentUnit.curUnitBehaviour.UpdateFKeyImage(false);    
        currentUnit = cols[targetIdx].GetComponent<Unit>();
        currentUnit.curUnitBehaviour.UpdateFKeyImage(true);

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (currentUnit.unitState == UnitState.KnockDown && currentUnit.curUnitBehaviour.unitBehaviourType == UnitBehaviourType.Wild)
            {
                catchTarget = currentUnit;
                catchTimer = 0;
            }
            else if (currentUnit.curUnitBehaviour.unitBehaviourType == UnitBehaviourType.Reguler)
            {
                if (activeStateBg)
                {
                    currentUnit.regularStateBg.SetActive(false);
                    activeStateBg = false;
                    IsStop = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    currentUnit.regularStateBg.SetActive(true);
                    OnRegularUnitState();
                }
            }
        }


    }
    
    public void OnRegularUnitState()
    {
        activeStateBg = true;
        IsStop = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        inventoryBg.SetActive(false);
        GameMgr.Instance.inventory.selectImage.gameObject.SetActive(false);
        GameMgr.Instance.inventory.selectDescription.gameObject.SetActive(false);
        for (int i = 0; i < GameMgr.Instance.inventory.buttons.Count; i++)
        {
            GameMgr.Instance.inventory.buttons[i].gameObject.SetActive(false);
        }

    }

    void Catch(Unit unit)
    {
        friendlyUnits.Add(unit);
        unit.curUnitBehaviour = unit.GetUnitBehaviour(UnitBehaviourType.Reguler);
        unit.curUnitBehaviour.GetComponent<UnitBehaviour>().PlayerSetting(this);
        unit.hpBar.color = Color.green;
        unit.hp = catchTarget.maxHp;
        unit.catchBarBgImage.SetActive(false);
        unit.hpBar.fillAmount = 1;
        unit.EnterState(UnitState.Idle);
        unit.animator.SetBool("IsKnockDown", false);
        unit.agent.isStopped = false;
        FriendlyBtn friendlyBtn = Instantiate(friendlyBtnPrefab, inventory.friendlyBtnGroup.transform);
        friendlyBtns.Add(friendlyBtn);
        friendlyBtn.SetFriendlyBtn(inventory, unit);
        unit.unitState = UnitState.Idle;
        unit.zoneUnit = false;
        unit.GetComponent<RegularUnitBehaviour>().regularUnitState = RegularUnitState.Defender;
        catchTarget = null;
    }

    private void OnCollisionStay(Collision collision)
    {
        onSlope = true;
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
                {
                    isGrounded = true;
                    isJumping = false;
                    onSlope = false;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = false;
        }
    }


    private void LateUpdate()
    {
        if (!IsStop)
        {
            Rotation();
        }
    }

    public void Rotation()
    {

        cameraRotationY += Input.GetAxis(mouseXAxisName) * mouseHorSensitivity;
        cameraRotationX += Input.GetAxis(mouseYAxisName) * -1 * mouseVerSensitivity;

        if (cameraRotationX < -80)
            cameraRotationX = -80;
        if (cameraRotationX > 85)
            cameraRotationX = 85;

        cameraPointTr.localEulerAngles = new Vector3(cameraRotationX, cameraRotationY, 0);
    }

    void RollStart()
    {
        if (inputDirection.magnitude == 0)
            return;

        isRoll = true;

        rollTime = rollDuration;
        rollDirection = cameraDirection;

        animator.SetTrigger("Roll");
    }

    void Roll()
    {
        if (rollTime > 0)
        {
            rb.velocity = new Vector3(rollDirection.normalized.x * rollSpeed, rb.velocity.y, rollDirection.normalized.z * rollSpeed);
            rollTime -= Time.deltaTime;
        }
        else
        {
            isRoll = false;
        }

    }



    void Move()
    {
        if (onSlope) return;

        if (isRoll) return;

        bool isRunning = inputDirection.magnitude > 0;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        float currentMoveSpeed = isSprinting ? moveSpeed * 1.3f : moveSpeed;

        if (isRunning)
        {
            Vector3 normalDirection = new Vector3(inputDirection.x, 0.0f, inputDirection.z).normalized;
            float _targetRotation = Mathf.Atan2(normalDirection.x, normalDirection.z) * Mathf.Rad2Deg + cameraPointTr.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(bodyTr.eulerAngles.y, _targetRotation, ref rotationVelocity, RotationSmoothTime);
            bodyTr.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 normalizedCameraDirection = cameraDirection.normalized;
        rb.velocity = new Vector3(currentMoveSpeed * normalizedCameraDirection.x * controlPower, rb.velocity.y, currentMoveSpeed * normalizedCameraDirection.z * controlPower);

        animator.SetBool("IsRunning", isRunning);
    }


    void SetInputDirection() //입력 방향 설정
    {
        if (Input.GetMouseButton(0))
            return;

        float x = Input.GetAxis(horAxisName);
        float z = Input.GetAxis(verAxisName);

        Vector3 inputDirection = new Vector3(x, 0, z);
        Vector3 normalDirection = new Vector3(x, 0.0f, z).normalized;

        if (inputDirection.magnitude > 0)
        {
            //x,z,방향에 대한 벡터에 대한 카메라의 현재 수평 회전(y 축 회전)을 기준으로 하여 그 대상 방향으로 회전해야 할 각도(_targetRotation)
            targetRotation = Mathf.Atan2(normalDirection.x, normalDirection.z) * Mathf.Rad2Deg + cameraPointTr.eulerAngles.y;
        }
        else
        {
            targetRotation = 0;
        }

        //카메라를 기준으로 입력했을 때 나아가야될 방향을 설정하는 코드
        Vector3 cameraDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        this.inputDirection = inputDirection;
        this.cameraDirection = cameraDirection;
        controlPower = inputDirection.magnitude;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkPickUpRange);
    }
}
