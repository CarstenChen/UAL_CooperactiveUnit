using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public float targetAngle;
    public delegate void ScreamEvent(float a);
    public static event ScreamEvent MonsterSlowDownEvent;

    [Header("Movement Setting")]
    [SerializeField] protected CinemachineFreeLook TPSCamera;
    [SerializeField] protected float moveSpeed;  //����ٶ�
    [SerializeField] protected float maxRotateSpeed;  //��ת������ٶ�
    [SerializeField] protected float gravity = 10f;  //����
    [SerializeField] protected float initialJumpSpeed = 0f;  //�������ٶ�

    [Header("Cream Setting")]
    public float minMonsterSpeedDecreaseRate = 0.5f;
    public float maxMonsterSpeedDecreaseRate = 0.8f;

    protected Animator animator;
    protected PlayerInput playerInput;
    protected CharacterController playerController;

    protected float maxSpeedRef = 1f;  //�����˶������������ֵ
    protected float targetSpeedRef;  //ʵ���˶��������䣨��Ҫ��ֵ���ģ�ֵ
    protected float curSpeedRef;  //��ǰ�˶���������ֵ
    protected float groundAcceleration = 4f;
    protected float groundDeceleration = 5f;

    protected Quaternion targetRotation;  //��ɫforwardת�����뷽�����Ԫ��
    protected float shortestDeltaRotDegree;  //��ǰ��ת��delta�ǣ���С�Ƕȣ�
    protected float curRotateSpeed;  //��ǰ�ٶ��µ���ת�ٶ�

    protected bool isGrounded = true;
    protected bool isReadyToJump;
    protected float curVerticalSpeed; //��ǰ��ֱ�ٶ��ٶ�

    protected float idleTimeout = 5f;  //��ÿ�ʼ����idle����
    protected float idleTimer;
    protected AnimatorInfo animatorCache;

    protected bool isAttacking;

    protected bool canScream = false;

    protected PlayerScreenEffects playerScreenEffects;

    protected AIMonsterController monster;

    protected bool IsPressingMoveKey
    {
        get { return !Mathf.Approximately(playerInput.MoveInput.sqrMagnitude, 0f); }
    }

    // Start is called before the first frame update
    void Awake()
    {
        //��ʼ��
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<CharacterController>();
        animatorCache = new AnimatorInfo(animator);
        playerScreenEffects = GetComponent<PlayerScreenEffects>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //�洢����״̬
        CacheAnimatorState();
        //���ű�ǩΪ��BlockInput���Ķ���ʱ����ֹ����
        UpdateInputBlock();

        //������п����õ���
        //DealWithScreamAttackAnimation();
        if(canScream)
        Scream();

        CalculateHorizontalMovement();
        CalculateVerticalMovement();
        SetMoveAnimation();

        CalculateRotation();
        if (IsPressingMoveKey)
            CharacterRotate();

        TimeoutToIdle();
    }

    //��������Ḳ�Ǳ�ѡΪAnimate Physics��Animator Controller��root motion�������ɴ˺��������ƶ�
    private void OnAnimatorMove()
    {
        Vector3 movement = Vector3.zero;

        if (isGrounded)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, -Vector3.up);

            if (Physics.Raycast(ray, out hit, 1f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                ////�ö��������ƶ������һ�ַ�ʽ
                //movement = Vector3.ProjectOnPlane(animator.deltaPosition, hit.normal);
                movement = moveSpeed * curSpeedRef * transform.forward * Time.deltaTime;
            }
            else
            {
                movement = moveSpeed * curSpeedRef * transform.forward * Time.deltaTime;
                //movement = animator.deltaPosition;
            }
        }
        else
        {
            movement = moveSpeed * curSpeedRef * transform.forward * Time.deltaTime;
        }

        //��Ԫ�����Ϊ����
        playerController.transform.rotation *= animator.deltaRotation;

        //���봹ֱ�ٶ�
        movement += curVerticalSpeed * Vector3.up * Time.deltaTime;

        //�ƶ�
        playerController.Move(movement);
        //Debug.Log(movement);

        //ע��ʹ�����ַ����ж��Ƿ��ڵ������Ҫ��Move()
        isGrounded = playerController.isGrounded;

        //������Ծ����
        if (isGrounded) { }
        //Debug.Log("isGrounded");
        else
        {
            animator.SetFloat("VerticalSpeed", curVerticalSpeed);
        }

        animator.SetBool("IsGrounded", isGrounded);
    }

    void CalculateHorizontalMovement()
    {
        Vector2 moveInput = playerInput.MoveInput;

        //�������������1�ű�׼��
        if (moveInput.sqrMagnitude > 1)
            moveInput.Normalize();

        targetSpeedRef = moveInput.magnitude;

        //���ֹͣ���������
        float a = IsPressingMoveKey ? groundAcceleration : groundDeceleration;

        //Vt = Vo +at
        curSpeedRef = Mathf.MoveTowards(curSpeedRef, targetSpeedRef, Time.deltaTime * a);
    }

    void CalculateVerticalMovement()
    {
        if (!playerInput.JumpInput && isGrounded)
            isReadyToJump = true;

        if (isGrounded)
        {
            curVerticalSpeed = -2f;

            if (playerInput.JumpInput && isReadyToJump)
            {
                curVerticalSpeed = initialJumpSpeed;
                isGrounded = false;
                isReadyToJump = false;
            }
        }
        else
        {
            //�������ĸ���
            if (playerInput.JumpInput && curVerticalSpeed > 0f)
            {
                curVerticalSpeed -= gravity * Time.deltaTime;
            }
            else if (Mathf.Approximately(curVerticalSpeed, 0f))
            {
                curVerticalSpeed = 0f;
            }
            else
                //airborne״̬
                curVerticalSpeed -= 2 * gravity * Time.deltaTime;
        }
    }

    void SetMoveAnimation()
    {
        animator.SetFloat("ForwardSpeed", curSpeedRef);
    }

    void CalculateRotation()
    {
        Vector3 moveInputDir = new Vector3(playerInput.MoveInput.x, 0, playerInput.MoveInput.y).normalized;

        //�����������ǰ�����ʵ�ʷ���
        Vector3 TPSCamForward = Quaternion.Euler(0, TPSCamera.m_XAxis.Value, 0) * Vector3.forward;

        //����ת����Ԫ��
        Quaternion desiredRotation;

        if (Mathf.Approximately(Vector3.Dot(moveInputDir, Vector3.forward), -1)/*��Ϊ��������*/)
        {
            //�������ô�������������ת90��ʱ��ǰ������෴
            desiredRotation = Quaternion.LookRotation(-TPSCamForward);
        }
        else
        {
            //��ǰ���뷽����Vector3.forward����Ԫ�����������������ǰ������Ԫ��
            Quaternion TPSCamForward2DesiredDir = Quaternion.FromToRotation(Vector3.forward, moveInputDir);
            //�������ǰ����ת��÷������Ԫ��
            desiredRotation = Quaternion.LookRotation(TPSCamForward2DesiredDir * TPSCamForward);
        }

        targetRotation = desiredRotation;


        //���㵱ǰת�Ǻ�Ŀ��ת���ṩdelta��
        Vector3 desiredDir = targetRotation * Vector3.forward;
        float currentAngle = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
        targetAngle = Mathf.Atan2(desiredDir.x, desiredDir.z) * Mathf.Rad2Deg;
        shortestDeltaRotDegree = Mathf.DeltaAngle(currentAngle, targetAngle);
    }

    void CharacterRotate()
    {
        animator.SetFloat("DeltaDeg2Rag", shortestDeltaRotDegree * Mathf.Deg2Rad);

        curRotateSpeed = maxRotateSpeed * curSpeedRef / maxSpeedRef;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, curRotateSpeed * Time.deltaTime);
    }

    void TimeoutToIdle()
    {
        bool inputDetected = IsPressingMoveKey || playerInput.JumpInput || playerInput.ScreamInput;

        if (isGrounded && !inputDetected)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTimeout)
            {
                idleTimer = 0f;
                animator.SetTrigger("IdleTrigger");
            }
        }
        else
        {
            idleTimer = 0f;
            animator.ResetTrigger("IdleTrigger");
        }

        animator.SetBool("HasInput", inputDetected);
    }

    //���涯��״̬
    void CacheAnimatorState()
    {
        //��һ֡������Ϣ��¼
        animatorCache.previousCurrentStateInfo = animatorCache.currentStateInfo;
        animatorCache.previousIsAnimatorTransitioning = animatorCache.isAnimatorTransitioning;
        animatorCache.previousNextStateInfo = animatorCache.nextStateInfo;

        //��ǰ֡������Ϣ����
        animatorCache.currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animatorCache.isAnimatorTransitioning = animator.IsInTransition(0);
        animatorCache.nextStateInfo = animator.GetNextAnimatorStateInfo(0);
    }

    void UpdateInputBlock()
    {
        //���ű�ǩΪ��BlockInput���Ķ���ʱ����ֹ����
        bool currentInputBlock = !animatorCache.isAnimatorTransitioning && animatorCache.currentStateInfo.tagHash == Animator.StringToHash("BlockInput");

        currentInputBlock |= animatorCache.nextStateInfo.tagHash == Animator.StringToHash("BlockInput");

        playerInput.inputBlock = currentInputBlock;
    }


    void DealWithScreamAttackAnimation()
    {
        ////������һ��ʱ�䣨0-1�����ڣ�0-1���ϵ�repeat
        //animator.SetFloat("MeleeStateTime", Mathf.Repeat(animatorCache.currentStateInfo.normalizedTime, 1f));


        ////ÿһ֡��reset���������Ա�֤ÿ�ε��������һ��trigger
        //animator.ResetTrigger("MeleeAttackTrigger");

        //if (playerInput.AttackInput)
        //{
        //    animator.SetTrigger("MeleeAttackTrigger");
        //}
    }

    bool IsWeaponAnimationOnPlay()
    {
        bool isWeaponEquipped;

        isWeaponEquipped = animatorCache.nextStateInfo.tagHash == Animator.StringToHash("WeaponEquippedAnim") || animatorCache.currentStateInfo.tagHash == Animator.StringToHash("WeaponEquippedAnim");

        return isWeaponEquipped;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            monster = other.GetComponent<AIMonsterController>();

            canScream = true;
            //Debug.Log("Can scream now");

        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.tag == "Monster")
    //    {
    //        monster = other.GetComponent<AIMonsterController>();
    //        if (monster == null)
    //            return;

    //        if (playerInput.ScreamInput)
    //        {
    //            float offset = Mathf.Abs(playerScreenEffects.vignetteScaleValue - playerScreenEffects.ringScaleValue);

    //            if (offset < 0.05f)
    //            {
    //                if (offset < 0.025f)
    //                {
    //                    MonsterSlowDownEvent(minMonsterSpeedDecreaseRate);
    //                    Debug.Log("Perffect, Monster Slow Down");

    //                    monster.hitTimes++;
    //                }
    //                else
    //                {
    //                    MonsterSlowDownEvent(maxMonsterSpeedDecreaseRate);
    //                    Debug.Log("Good, Monster Slow Down");

    //                    monster.hitTimes++;
    //                }
    //                playerScreenEffects.DealWithRingDisplay();
    //            }
    //            else
    //            {
    //                Debug.Log("Failed");

    //                if (monster.hitTimes > 0)
    //                {
    //                    monster.hitTimes--;
    //                }
    //            }
    //        }
    //    }
    //}

    private void Scream()
    {
        if (playerInput.ScreamInput&&!playerInput.hasDealAttack)
        {
            float offset = Mathf.Abs(playerScreenEffects.vignetteScaleValue - playerScreenEffects.ringScaleValue);

            if (offset < 0.05f)
            {
                if (offset < 0.025f)
                {
                    MonsterSlowDownEvent(minMonsterSpeedDecreaseRate);
                    Debug.Log("Perffect, Monster Slow Down");
                    playerInput.hasDealAttack = true;
                    monster.hitTimes++;
                }
                else
                {
                    MonsterSlowDownEvent(maxMonsterSpeedDecreaseRate);
                    Debug.Log("Good, Monster Slow Down");
                    playerInput.hasDealAttack = true;
                    monster.hitTimes++;
                }
                playerScreenEffects.DealWithRingDisplay();
            }
            else
            {
                Debug.Log("Failed");
                playerInput.hasDealAttack = true;
                if (monster.hitTimes > 0)
                {
                    monster.hitTimes--;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Monster")
        {
            canScream = false;
            //Debug.Log("Can scream now");

        }
    }
}
