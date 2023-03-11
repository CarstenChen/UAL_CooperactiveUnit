using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

[Serializable]
public class Parameter
{
    [Header("AI Settings")]
    public Transform chaseTarget;
    public float raidDistanceBehindPlayer;
    public float raidAngleBehindPlayer = 60f;
    public float fastDeccelaration = 100f;
    public float normalAcceleration = 10f;
    public float normalChaseSpeed = 10f;
    public float fastChaseSpeed = 15f;
    public float speedUpRate =0.2f;
    public float minSpeed;
    public float[] coolDown;
    public int dizzyHitTimes = 3;
    public float catchDistance = 2f;

    [Header("Animation Settings")]
    public Animator animator;
    public AnimatorInfo animatorCache;

    [Header("Effect Settings")]
    public GameObject raidFlashEffect;
    public GameObject dizzyObj;
    public GameObject slowDownEffect;
    public GameObject[] slowDownEffectPoints;

    [Header("Mesh Settings")]
    public GameObject bodyMesh;

    [NonSerialized] public float currentChaseSpeed;
}

public class AIMonsterController : MonoBehaviour
{
    [Header("Monster Settings")]
    public Parameter param;
    public LayerMask adjustNormalLayer;
    public Waypoints[] routes;

    [NonSerialized] public NavMeshAgent agent;
    [NonSerialized] public bool readyToRaid;
    [NonSerialized] public bool readyToChase;
    [NonSerialized] public bool routeChanged;
    [NonSerialized] public bool playerInSight;
    [NonSerialized] public bool playerInSphereTrigger;
    [NonSerialized] public bool playerHeard;
    [NonSerialized] public bool playerFirstFound;
    [NonSerialized] public Waypoints currentPatrolRoute;
    [NonSerialized] public int hitTimes = 0;
    [NonSerialized] public float currentCoolDown;
    [NonSerialized] public int dizzyTimes = 0;
    [NonSerialized] public bool attackOver = false;

    //state data
    protected Dictionary<StateType, State> states = new Dictionary<StateType, State>();
    //patrol route data
    protected Waypoints previousPatrolRoute;
    //player info
    protected bool lastPlayerInSight;
    protected bool lastPlayerInSphereTrigger;
    //slowdown debuff (override)
    protected Coroutine currentSlowDownCoroutine;

    [Header("Guide Relevant Settings")]
    public GameObject appearanceTimeline;
    public GameObject firstDizzyTimeline;
    protected bool blockAI;


    public enum StateType
    {
        Idle, Chase, Attack, Patrol, Raid, Dizzy
    }

    protected State currentState;

    public void Awake()
    {

        agent = GetComponent<NavMeshAgent>();
        param.animator = GetComponent<Animator>();
        param.animatorCache = new AnimatorInfo(param.animator);

        currentPatrolRoute = routes[0];
        previousPatrolRoute = routes[0];

        PlayerController.MonsterSlowDownEvent += SlowDownOnAttacked;
        AIDirector.MonsterSpeedUpEvent += SwitchChaseSpeed;

    }

    private void Start()
    {

        RegisterState();
        //以Idle状态进入
        SwitchToState(StateType.Idle);

        InvokeRepeating("AdjustPatrolRoutes", 0f, 1f);

        agent.speed = param.normalChaseSpeed;
        agent.acceleration = param.normalAcceleration;


        if (!AIDirector.Instance.hasFinishedGuide) blockAI = true;

        ObjectPool.instance.PreLoadGameObject(param.slowDownEffect, 10);
    }


    private void Update()
    {
        CacheAnimatorState();
    }

    void DealWithMonsterGuide()
    {
        if (AIDirector.Instance.hasFinishedGuide) return;

        if (AIDirector.Instance.canTriggerSanGuide)
        {
            AIDirector.Instance.canTriggerSanGuide = false;

            //make sure agent has path
            agent.enabled = true;
            agent.destination = param.chaseTarget.position;
            //make sure agent is static
            agent.speed = 0f;
            agent.acceleration = param.normalAcceleration;
            agent.isStopped = true;

            readyToChase = false;
            StartCoroutine(OnSpawnBehindPlayer((float)appearanceTimeline.GetComponent<PlayableDirector>().duration));


            transform.position = AIDirector.Instance.monsterSpawnPos;
            param.raidFlashEffect.SetActive(false);
            param.raidFlashEffect.SetActive(true);
            param.bodyMesh.SetActive(true);

            StartCoroutine(PlayAppearanceTimeline());

        }


        if (readyToChase)
        {
            SwitchToState(StateType.Chase);
            blockAI = false;

            StartCoroutine(PlayDizzyTimeline());
        }
    }

    IEnumerator PlayAppearanceTimeline()
    {
        appearanceTimeline.SetActive(true);
        PlayerInput.inputBlock = true;
        yield return new WaitForSeconds((float)appearanceTimeline.GetComponent<PlayableDirector>().duration);
        appearanceTimeline.SetActive(false);
        AIDirector.Instance.monsterAppearTimelineFinished = true;
        PlayerInput.inputBlock = false;
    }

    IEnumerator PlayDizzyTimeline()
    {
        yield return new WaitUntil(() => currentState.GetType() == typeof(MonsterDizzyState));
        firstDizzyTimeline.SetActive(true);
        PlayerInput.inputBlock = true;
        yield return new WaitForSeconds((float)firstDizzyTimeline.GetComponent<PlayableDirector>().duration);
        firstDizzyTimeline.SetActive(false);
        PlayerInput.inputBlock = false;
    }
    private void FixedUpdate()
    {
        if (AIDirector.isGameOver) return;

        if (AIDirector.Instance.canTriggerMonsterGuide && !AIDirector.Instance.monsterGuideFinished)
        {
            DealWithMonsterGuide();
        }


        if (blockAI) return;

        if (currentCoolDown > 0) currentCoolDown -= Time.fixedDeltaTime;
        else currentCoolDown = 0;

        //ground rotation
        UpdateBodyYAxis();

        Debug.Log(CanBeSeenByPlayer());

        bool raidWhenHearPlayer = false;
        if (currentState.GetType() == typeof(MonsterPatrolState))
        {

            raidWhenHearPlayer = /*UnityEngine.Random.Range(0, 2) != 0*/!CanBeSeenByPlayer() ? true : false;
        }

        //if (Keyboard.current.f10Key.isPressed)
        //{
        //    playerFirstFound = true;//so it chase player ingnoring eyesight
        //    Debug.Log(playerFirstFound);
        //    SwitchToState(StateType.Raid);
        //    StartCoroutine(OnSpawnBehindPlayer(0.2f));
        //}

        //if is in main story
        if ((AIDirector.Instance.isInFinalSceneTimeLine || AIDirector.Instance.isInMainStoryTimeLine || AIDirector.Instance.isInBodyChange) && currentState.GetType() != typeof(MonsterIdleState) && currentState.GetType() != typeof(MonsterDizzyState))
        {
            SwitchToState(StateType.Idle);
        }

        //attack first when meet
        if (!attackOver && playerInSphereTrigger && Vector3.Distance(param.chaseTarget.position, transform.position) <= param.catchDistance && currentState.GetType() == typeof(MonsterChaseState))
        {
            SwitchToState(StateType.Attack);
        }
        //if out of range during attack animation, chase player again
        if (attackOver && currentState.GetType() == typeof(MonsterAttackState))
        {
            SwitchToState(StateType.Idle);
        }


        //patrol if nothing to do
        if (!AIDirector.Instance.isInFinalSceneTimeLine && !AIDirector.Instance.isInMainStoryTimeLine && !AIDirector.Instance.isInBodyChange && (!playerInSphereTrigger && currentState.GetType() == typeof(MonsterIdleState)) ||
            (routeChanged && currentState.GetType() == typeof(MonsterPatrolState)))
        {
            SwitchToState(StateType.Patrol);
        }


        //raid when in idle/patrol state and main story is triggered or player is found during patrol 
        if ((!AIDirector.Instance.isInFinalSceneTimeLine && !AIDirector.Instance.isInMainStoryTimeLine
            && !AIDirector.Instance.isInBodyChange && AIDirector.Instance.tensiveTime
            && (currentState.GetType() == typeof(MonsterIdleState) || currentState.GetType() == typeof(MonsterPatrolState)))
            || (playerHeard && raidWhenHearPlayer && currentState.GetType() == typeof(MonsterPatrolState)))
        {
            playerFirstFound = true;//so it chase player ingnoring eyesight
            Debug.Log(playerFirstFound);
            SwitchToState(StateType.Raid);
            StartCoroutine(OnSpawnBehindPlayer(0.2f));

            AIDirector.Instance.tensiveTime = false;
        }

        //dash after raid to player
        if (readyToChase && currentState.GetType() == typeof(MonsterRaidState) ||
            (((playerHeard && !playerInSight && !raidWhenHearPlayer) || playerInSight) && currentState.GetType() == typeof(MonsterPatrolState)))
        {
            SwitchToState(StateType.Chase);
        }

        if (!playerInSphereTrigger && !playerFirstFound && hitTimes < param.dizzyHitTimes && currentState.GetType() == typeof(MonsterChaseState))
        {
            SwitchToState(StateType.Idle);
        }

        if (playerInSphereTrigger && hitTimes >= param.dizzyHitTimes && currentState.GetType() == typeof(MonsterChaseState))
        {
            SwitchToState(StateType.Dizzy);
        }

        if (currentCoolDown <= 0 && currentState.GetType() == typeof(MonsterDizzyState))
        {
            SwitchToState(StateType.Idle);
        }

        #region
        //if (PlayerInput.pi_Instance.TestInput1)
        //{
        //    SwitchToState(StateType.Raid);
        //    StartCoroutine(OnSpawnBehindPlayer());
        //}

        //if (readyToChase)
        //{
        //    SwitchToState(StateType.Chase);
        //}


        //if (routeChanged && (currentState.GetType() == typeof(MonsterIdleState) || currentState.GetType() == typeof(MonsterPatrolState)))
        //{
        //    SwitchToState(StateType.Patrol);
        //}
        #endregion

        currentState.OnStateStay();

        routeChanged = false;

        //记录上一帧玩家是否在视野中和球形Trigger中
        lastPlayerInSight = playerInSight;
        lastPlayerInSphereTrigger = playerInSphereTrigger;
    }

    private void RegisterState()
    {
        states.Add(StateType.Idle, new MonsterIdleState(this));
        states.Add(StateType.Chase, new MonsterChaseState(this));
        states.Add(StateType.Attack, new MonsterAttackState(this));
        states.Add(StateType.Patrol, new MonsterPatrolState(this));
        states.Add(StateType.Raid, new MonsterRaidState(this));
        states.Add(StateType.Dizzy, new MonsterDizzyState(this));
    }

    public void SwitchToState(StateType type)
    {
        if (currentState != null) currentState.OnStateExit();
        currentState = states[type];
        currentState.OnStateEnter();
    }

    private void UpdateBodyYAxis()
    {
        RaycastHit hit;
        int Rmask = LayerMask.GetMask("Terrain");

        Vector3 Point_dir = param.bodyMesh.transform.TransformDirection(Vector3.down);

        if (Physics.Raycast(param.bodyMesh.transform.position, Point_dir, out hit, 50.0f, Rmask))
        {

            Quaternion NextRot = Quaternion.LookRotation(Vector3.Cross(hit.normal, Vector3.Cross(transform.forward, hit.normal)), hit.normal);

            param.bodyMesh.transform.rotation = Quaternion.Lerp(param.bodyMesh.transform.rotation, NextRot, 0.1f);
        }
    }

    IEnumerator OnSpawnBehindPlayer(float delay)
    {

        yield return new WaitForSeconds(delay);
        readyToChase = true;
    }

    public void AdjustPatrolRoutes()
    {
        if (AIDirector.isGameOver) return;

        previousPatrolRoute = currentPatrolRoute;

        currentPatrolRoute = routes[AIDirector.Instance.CalculateRouteByPlayerDesiredDestination(routes)];

        if (currentPatrolRoute != previousPatrolRoute)
        {
            routeChanged = true;
            //Debug.Log("ChangeRoute");
            //Debug.Log(currentPatrolRoute.root.name);
        }
    }

    public void SlowDownOnAttacked(float slowDownRate)
    {
        if (currentSlowDownCoroutine != null)
            StopCoroutine(currentSlowDownCoroutine);

        currentSlowDownCoroutine = StartCoroutine(SlowDown(slowDownRate));

        GameObject newSlowDownEffect = ObjectPool.instance.GetGameObject(param.slowDownEffect, param.slowDownEffectPoints[UnityEngine.Random.Range(0, param.slowDownEffectPoints.Length)].transform.position, Quaternion.identity, ObjectPool.instance.poolRoot);
        ObjectPool.instance.SetGameObject(newSlowDownEffect, 2);
    }

    IEnumerator SlowDown(float slowDownRate)
    {
        agent.speed = Mathf.Clamp(agent.speed * UnityEngine.Random.Range(slowDownRate / 1.2f, slowDownRate), param.minSpeed, param.currentChaseSpeed);
        yield return new WaitForSeconds(1/*2.5f/AIDirector.Instance.currentDifficulty*/);
        agent.speed = param.currentChaseSpeed;
    }

    private void OnDisable()
    {
        PlayerController.MonsterSlowDownEvent -= SlowDownOnAttacked;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (currentState.GetType() != typeof(MonsterRaidState))
                playerFirstFound = false;

            if (currentState.GetType() != typeof(MonsterDizzyState))
                hitTimes = 0;
        }
    }

    void CacheAnimatorState()
    {
        //上一帧动画信息记录
        param.animatorCache.previousCurrentStateInfo = param.animatorCache.currentStateInfo;
        param.animatorCache.previousIsAnimatorTransitioning = param.animatorCache.isAnimatorTransitioning;
        param.animatorCache.previousNextStateInfo = param.animatorCache.nextStateInfo;

        //当前帧动画信息更新
        param.animatorCache.currentStateInfo = param.animator.GetCurrentAnimatorStateInfo(0);
        param.animatorCache.isAnimatorTransitioning = param.animator.IsInTransition(0);
        param.animatorCache.nextStateInfo = param.animator.GetNextAnimatorStateInfo(0);
    }

    private bool IsInScreen(Transform targetTransform)
    {
        Transform camTransform = Camera.main.transform;
        Vector2 viewPos = Camera.main.WorldToViewportPoint(targetTransform.position);
        Vector3 dir = (targetTransform.position - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir);//判断物体是否在相机前面

        if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            return true;
        else
            return false;
    }

    private bool CanBeSeenByPlayer()
    {
        if (IsInScreen(this.transform)) return true;

        Ray ray = new Ray(transform.position + new Vector3(0, 1f, 0),
            param.chaseTarget.transform.position - (transform.position + new Vector3(0, 1f, 0)));

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,
            Vector3.Distance(transform.position + new Vector3(0, 1f, 0), param.chaseTarget.transform.position)))
        {
            if (hit.transform.tag != "Monster" && hit.transform.tag != "Player")
            {
                return false;
            }
        }
        return true;
    }
    public void SwitchChaseSpeed(int speedUpIndex)
    {
        param.normalChaseSpeed = param.normalChaseSpeed + param.speedUpRate * speedUpIndex;
        param.fastChaseSpeed = param.fastChaseSpeed + param.speedUpRate * speedUpIndex;
    }
}
