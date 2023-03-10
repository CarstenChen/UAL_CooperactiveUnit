using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;


public class PlayerController : MonoBehaviour
{
    public float targetAngle;
    public delegate void ScreamEvent(float a);
    public static event ScreamEvent MonsterSlowDownEvent;

    [Header("Movement Setting")]
    [SerializeField] protected static CinemachineFreeLook TPSCamera;
    [SerializeField] protected static CinemachineVirtualCamera faceCamera;
    [SerializeField] protected float moveSpeed;  //最大速度
    [SerializeField] protected float maxRotateSpeed;  //旋转的最大速度
    [SerializeField] protected float gravity = 10f;  //重力
    [SerializeField] protected float initialJumpSpeed = 0f;  //起跳初速度

    [Header("Scream Setting")]
    public float minMonsterSpeedDecreaseRate = 0.5f;
    public float maxMonsterSpeedDecreaseRate = 0.8f;
    public GameObject screamEffect;

    [NonSerialized] public Animator animator;
    public PlayerInput playerInput;
    protected CharacterController playerController;

    protected float maxSpeedRef = 1f;  //虚拟运动进程区间最大值
    protected float targetSpeedRef;  //实际运动进程区间（需要插值到的）值
    protected float curSpeedRef;  //当前运动进程区间值
    protected float groundAcceleration = 4f;
    protected float groundDeceleration = 5f;

    protected Quaternion targetRotation;  //角色forward转向输入方向的四元数
    protected float shortestDeltaRotDegree;  //当前旋转的delta角（最小角度）
    protected float curRotateSpeed;  //当前速度下的旋转速度

    protected bool isGrounded = true;
    protected bool isReadyToJump;
    protected float curVerticalSpeed; //当前垂直速度速度

    protected float idleTimeout = 5f;  //多久开始考虑idle动画
    protected float idleTimer;
    protected AnimatorInfo animatorCache;
    protected AnimatorInfo animatorCacheExtraLayer;

    protected bool isAttacking;

    protected bool canScream = false;

    protected PlayerScreenEffects playerScreenEffects;

    protected AIMonsterController monster;

    protected bool onSpawn;

    protected bool IsPressingMoveKey
    {
        get { return !Mathf.Approximately(playerInput.MoveInput.sqrMagnitude, 0f); }
    }

    // Start is called before the first frame update
    void Awake()
    {
        //初始化
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<CharacterController>();
        animatorCache = new AnimatorInfo(animator);
        animatorCacheExtraLayer = new AnimatorInfo(animator);
        playerScreenEffects = GetComponent<PlayerScreenEffects>();
        faceCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        TPSCamera = GameObject.Find("TPS FreeLook").GetComponent<CinemachineFreeLook>();
        //SceneManager.activeSceneChanged += Spawn;

        ObjectPool.instance.PreLoadGameObject(screamEffect, 10);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //存储动画状态
        CacheAnimatorState();
        CacheExtraAnimatorState();
        //播放标签为“BlockInput”的动画时，禁止输入
        //UpdateInputBlock();

        //后续尖叫可能用得上
        //DealWithScreamAttackAnimation();
        if (canScream)
            NewScream();

        CalculateHorizontalMovement();
        CalculateVerticalMovement();
        SetMoveAnimation();

        CalculateRotation();
        if (IsPressingMoveKey)
            CharacterRotate();

        TimeoutToIdle();
    }

    //这个方法会覆盖被选为Animate Physics的Animator Controller的root motion，进而由此函数控制移动
    private void OnAnimatorMove()
    {
        if (FinalSceneAIDirector.Instance != null)
        {
            if (FinalSceneAIDirector.Instance.autoWriting)
            {
                return;
            }
        }

        if (onSpawn) return;
        Vector3 movement = Vector3.zero;

        if (isGrounded)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, -Vector3.up);

            if (Physics.Raycast(ray, out hit, 1f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                ////用动画计算移动距离的一种方式
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

        //四元数相乘为叠加
        playerController.transform.rotation *= animator.deltaRotation;

        //加入垂直速度
        movement += curVerticalSpeed * Vector3.up * Time.deltaTime;

        //移动
        playerController.Move(movement);
        //Debug.Log(movement);

        //注意使用这种方法判断是否在地面必须要先Move()
        isGrounded = playerController.isGrounded;

        //设置跳跃动画
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

        //如果输入量超过1才标准化
        if (moveInput.sqrMagnitude > 1)
            moveInput.Normalize();

        targetSpeedRef = moveInput.magnitude;

        //如果停止按动则减速
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
            //长按跳的更高
            if (playerInput.JumpInput && curVerticalSpeed > 0f)
            {
                curVerticalSpeed -= gravity * Time.deltaTime;
            }
            else if (Mathf.Approximately(curVerticalSpeed, 0f))
            {
                curVerticalSpeed = 0f;
            }
            else
                //airborne状态
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

        //计算摄像机当前朝向的实际方向
        Vector3 TPSCamForward = Quaternion.Euler(0, TPSCamera.m_XAxis.Value, 0) * Vector3.forward;

        //计算转角四元数
        Quaternion desiredRotation;

        if (Mathf.Approximately(Vector3.Dot(moveInputDir, Vector3.forward), -1)/*互为反向向量*/)
        {
            //如果不这么做，当摄像机旋转90度时，前后方向会相反
            desiredRotation = Quaternion.LookRotation(-TPSCamForward);
        }
        else
        {
            //当前输入方向与Vector3.forward的四元数就是其与摄像机正前方的四元数
            Quaternion TPSCamForward2DesiredDir = Quaternion.FromToRotation(Vector3.forward, moveInputDir);
            //计算把正前方向转向该方向的四元数
            desiredRotation = Quaternion.LookRotation(TPSCamForward2DesiredDir * TPSCamForward);
        }

        targetRotation = desiredRotation;


        //计算当前转角和目标转角提供delta角
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

    //缓存动画状态
    void CacheAnimatorState()
    {
        //上一帧动画信息记录
        animatorCache.previousCurrentStateInfo = animatorCache.currentStateInfo;
        animatorCache.previousIsAnimatorTransitioning = animatorCache.isAnimatorTransitioning;
        animatorCache.previousNextStateInfo = animatorCache.nextStateInfo;

        //当前帧动画信息更新
        animatorCache.currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animatorCache.isAnimatorTransitioning = animator.IsInTransition(0);
        animatorCache.nextStateInfo = animator.GetNextAnimatorStateInfo(0);
    }

    void CacheExtraAnimatorState()
    {
        //上一帧动画信息记录
        animatorCacheExtraLayer.previousCurrentStateInfo = animatorCache.currentStateInfo;
        animatorCacheExtraLayer.previousIsAnimatorTransitioning = animatorCache.isAnimatorTransitioning;
        animatorCacheExtraLayer.previousNextStateInfo = animatorCache.nextStateInfo;

        //当前帧动画信息更新
        animatorCacheExtraLayer.currentStateInfo = animator.GetCurrentAnimatorStateInfo(1);
        animatorCacheExtraLayer.isAnimatorTransitioning = animator.IsInTransition(1);
        animatorCacheExtraLayer.nextStateInfo = animator.GetNextAnimatorStateInfo(1);
    }

    void UpdateInputBlock()
    {
        //播放标签为“BlockInput”的动画时，禁止输入
        bool currentInputBlock = !animatorCache.isAnimatorTransitioning && animatorCache.currentStateInfo.tagHash == Animator.StringToHash("BlockInput");

        currentInputBlock |= animatorCache.nextStateInfo.tagHash == Animator.StringToHash("BlockInput");

        PlayerInput.inputBlock = currentInputBlock;
    }


    void DealWithScreamAttackAnimation()
    {
        ////动画归一化时间（0-1），在（0-1）上的repeat
        //animator.SetFloat("MeleeStateTime", Mathf.Repeat(animatorCache.currentStateInfo.normalizedTime, 1f));


        ////每一帧都reset，这样可以保证每次点击都触发一次trigger
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
        if (playerInput.ScreamInput && !playerInput.hasDealAttack && !playerScreenEffects.ringLocked)
        {
            float offset = Mathf.Abs(playerScreenEffects.effectScaleValue - playerScreenEffects.ringScaleValue);

            if (!AIDirector.Instance.monsterGuideFinished)
            {
                if (offset < 0.08f)
                {
                    MonsterSlowDownEvent(maxMonsterSpeedDecreaseRate);
                    monster.hitTimes++;

                    playerInput.hasDealAttack = true;

                    AIDirector.Instance.guideScreamCount++;

                    playerScreenEffects.DealWithRingDisplay();

                    screamEffect.SetActive(false);
                    screamEffect.SetActive(true);

                    animator.SetBool("Scream", true);
                    StartCoroutine(ResetScreamAnim());

                    AIDirector.Instance.playerScreamOnce = true;
                }
                else
                {
                    playerInput.hasDealAttack = true;
                }


                if (!AIDirector.Instance.monsterGuideFinished)
                {
                    AIDirector.Instance.bulletTime = false;
                }
            }
            else
            {
                if (offset < 0.05f)
                {
                    if (offset < 0.025f)
                    {
                        MonsterSlowDownEvent(minMonsterSpeedDecreaseRate);
                        monster.hitTimes++;

                        Debug.Log("Perffect, Monster Slow Down");
                        playerInput.hasDealAttack = true;

                        AIDirector.Instance.playerSuccessToScream++;
                        AIDirector.Instance.CalculateDifficulty();
                    }
                    else
                    {
                        MonsterSlowDownEvent(maxMonsterSpeedDecreaseRate);
                        monster.hitTimes++;

                        Debug.Log("Good, Monster Slow Down");
                        playerInput.hasDealAttack = true;

                        AIDirector.Instance.playerSuccessToScream++;
                        AIDirector.Instance.CalculateDifficulty();
                    }

                    playerScreenEffects.DealWithRingDisplay();
                    screamEffect.SetActive(false);
                    screamEffect.SetActive(true);

                    animator.SetBool("Scream", true);
                    StartCoroutine(ResetScreamAnim());

                    AIDirector.Instance.playerScreamOnce = true;
                }
                else
                {
                    Debug.Log("Failed");
                    playerInput.hasDealAttack = true;

                    AIDirector.Instance.RandomDecreaseHitTimes();
                    AIDirector.Instance.playerFailToScream++;
                    AIDirector.Instance.CalculateDifficulty();
                }
            }

        }
    }
    private void NewScream()
    {
        if (playerInput.ScreamInput && !playerInput.hasDealAttack&&!playerScreenEffects.ringLocked && AIDirector.Instance.onScreamRange && AIDirector.Instance.onCatchingState)
        {
            if (!AIDirector.Instance.hasFinishedGuide)
            {
                MonsterSlowDownEvent(maxMonsterSpeedDecreaseRate);

                playerInput.hasDealAttack = true;

                monster.hitTimes++;
                AIDirector.Instance.guideScreamCount++;
                if (AIDirector.Instance.guideScreamCount >= 6)
                {
                    monster.hitTimes += 100;
                }

                playerScreenEffects.DealWithRingDisplay();

                GameObject newSoundWave = ObjectPool.instance.GetGameObject(screamEffect, transform.position, Quaternion.identity, ObjectPool.instance.poolRoot);
                ObjectPool.instance.SetGameObject(newSoundWave, 1f);

                if (!animator.GetBool("Scream"))
                {
                    AIDirector.Instance.playerScreamOnce = true;
                }


                animator.SetBool("Scream", true);
                StartCoroutine(ResetScreamAnim());


                if (!AIDirector.Instance.monsterGuideFinished)
                {
                    AIDirector.Instance.bulletTime = false;
                }
            }
            else
            {
                int random = UnityEngine.Random.Range(0, 100);
                if(random == 99)
                {
                    monster.hitTimes+=1000;
                }
                else if (random < 80)
                {
                    MonsterSlowDownEvent(minMonsterSpeedDecreaseRate);
                    monster.hitTimes++;

                    Debug.Log("Perffect, Monster Slow Down");
                    playerInput.hasDealAttack = true;
                }
                else
                {
                    MonsterSlowDownEvent(maxMonsterSpeedDecreaseRate);
                    monster.hitTimes++;

                    Debug.Log("Good, Monster Slow Down");
                    playerInput.hasDealAttack = true;
                }

                playerScreenEffects.DealWithRingDisplay();
                GameObject newSoundWave= ObjectPool.instance.GetGameObject(screamEffect, 
                    transform.position, Quaternion.identity, ObjectPool.instance.poolRoot);
                ObjectPool.instance.SetGameObject(newSoundWave, 1f);

                if (!animator.GetBool("Scream"))
                {
                    AIDirector.Instance.playerScreamOnce = true;
                }

                animator.SetBool("Scream", true);
                StartCoroutine(ResetScreamAnim());
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

    public void Spawn(Scene arg0, Scene arg1)
    {
        if (this != null)
        {
            transform.position = GameObject.Find("Spawner").transform.position;
            transform.rotation = Quaternion.identity;
            onSpawn = true;
            StartCoroutine(ResetMovement());
        }
    }

    IEnumerator ResetMovement()
    {
        yield return new WaitForSeconds(1f);
        onSpawn = false;
    }

    IEnumerator ResetScreamAnim()
    {
        yield return null;
        if (animatorCacheExtraLayer.currentStateInfo.IsName("Scream") && animatorCacheExtraLayer.currentStateInfo.normalizedTime >= 1)
        {
            animator.SetBool("Scream", false);
        }
        else
        {
            StartCoroutine(ResetScreamAnim());
        }

    }

    public static void ChangeToFaceCamera()
    {
        TPSCamera.GetComponent<CinemachineInputProvider>().enabled = false;
        PlayerInput.inputBlock = true;
        faceCamera.Priority = 50;
    }

    public static void ChangeToFPSCamera()
    {
        faceCamera.Priority = 5;
        PlayerInput.inputBlock = false;
        TPSCamera.GetComponent<CinemachineInputProvider>().enabled = true;
    }
}
