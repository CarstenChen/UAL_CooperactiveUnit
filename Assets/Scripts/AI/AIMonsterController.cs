using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

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
    public float minSpeed;
    public float[] coolDown;
    public int dizzyHitTimes=3;
    public float catchDistance = 2f;

    [Header("Animation Settings")]
    public Animator animator;
    public AnimatorInfo animatorCache;

    [Header("Effect Settings")]
    public GameObject raidFlashEffect;

    [NonSerialized] public float currentChaseSpeed;
}

public class AIMonsterController : MonoBehaviour
{
    [Header("Monster Settings")]
    public Parameter param;
    public LayerMask adjustNormalLayer;
    public Transform body;
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

    public enum StateType
    {
        Idle, Chase, Attack, Patrol, Raid,Dizzy
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
    }

    private void Start()
    {
        RegisterState();

        //以Idle状态进入
        SwitchToState(StateType.Idle);

        InvokeRepeating("AdjustPatrolRoutes", 0f, 3f);

        agent.speed = param.normalChaseSpeed;
        agent.acceleration = param.normalAcceleration;
    }
    private void Update()
    {
        CacheAnimatorState();
    }
    private void FixedUpdate()
    {
        if (currentCoolDown > 0) currentCoolDown -= Time.fixedDeltaTime;
        else currentCoolDown = 0;

        //ground rotation
        UpdateBodyYAxis();

        bool raidWhenSeePlayer = false;
        if(currentState.GetType() == typeof(MonsterPatrolState))
        {
            raidWhenSeePlayer = UnityEngine.Random.Range(0, 2) != 0 ? true : false;
        }

        //attack first when meet
        if(!attackOver && playerInSphereTrigger && Vector3.Distance(param.chaseTarget.position,transform.position)<= param.catchDistance && currentState.GetType() == typeof(MonsterChaseState))
        {
            SwitchToState(StateType.Attack);
        }
        //if out of range during attack animation, chase player again
        if(attackOver && currentState.GetType() == typeof(MonsterAttackState))
        {
            SwitchToState(StateType.Idle);
        }


        //patrol if nothing to do
        if ((!playerInSphereTrigger && currentState.GetType() == typeof(MonsterIdleState)) ||
            (routeChanged && currentState.GetType() == typeof(MonsterPatrolState)))
        {
            SwitchToState(StateType.Patrol);
        }


        //raid when in idle/patrol state and main story is triggered or player is found during patrol 
        if ((AIDirector.Instance.tensiveTime && (currentState.GetType() == typeof(MonsterIdleState) || currentState.GetType() == typeof(MonsterPatrolState))) ||
           (playerInSight && raidWhenSeePlayer && currentState.GetType() == typeof(MonsterPatrolState)))
        {
            playerFirstFound = true;//so it chase player ingnoring eyesight
            Debug.Log(playerFirstFound);
            SwitchToState(StateType.Raid);
            StartCoroutine(OnSpawnBehindPlayer());

            AIDirector.Instance.tensiveTime = false;
        }

        //dash after raid to player
        if (readyToChase && currentState.GetType() == typeof(MonsterRaidState) || 
            ((playerHeard||playerInSight) && !raidWhenSeePlayer && currentState.GetType() == typeof(MonsterPatrolState)))
        {
            SwitchToState(StateType.Chase);
        }

        if(!playerInSphereTrigger && !playerFirstFound && hitTimes< param.dizzyHitTimes && currentState.GetType() == typeof(MonsterChaseState))
        {
            SwitchToState(StateType.Idle);
        }

        if(playerInSphereTrigger && hitTimes >= param.dizzyHitTimes && currentState.GetType() == typeof(MonsterChaseState))
        {
            SwitchToState(StateType.Dizzy);
        }

        if(currentCoolDown<=0 && currentState.GetType() == typeof(MonsterDizzyState))
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

        Vector3 Point_dir = body.TransformDirection(Vector3.down);

        if (Physics.Raycast(body.position, Point_dir, out hit, 50.0f, Rmask))
        {

            Quaternion NextRot = Quaternion.LookRotation(Vector3.Cross(hit.normal, Vector3.Cross(transform.forward, hit.normal)), hit.normal);

            body.rotation = Quaternion.Lerp(body.rotation, NextRot, 0.1f);
        }
    }

    IEnumerator OnSpawnBehindPlayer()
    {

        yield return new WaitForSeconds(2f);
        readyToChase = true;
    }

    public void AdjustPatrolRoutes()
    {
        previousPatrolRoute = currentPatrolRoute;

        currentPatrolRoute = routes[AIDirector.Instance.CalculateRouteByPlayerDesiredDestination(routes)];

        if (currentPatrolRoute != previousPatrolRoute)
        {
            routeChanged = true;
           // Debug.Log("ChangeRoute");
        }
    }

    public void SlowDownOnAttacked(float slowDownRate)
    {
        if (currentSlowDownCoroutine != null)
            StopCoroutine(currentSlowDownCoroutine);

        currentSlowDownCoroutine = StartCoroutine(SlowDown(slowDownRate));

        Debug.Log(string.Format("Hit {0} Times", hitTimes));
    }

    IEnumerator SlowDown(float slowDownRate)
    {
        agent.speed = Mathf.Clamp(agent.speed * UnityEngine.Random.Range(slowDownRate/2, slowDownRate), param.minSpeed, param.currentChaseSpeed);
        yield return new WaitForSeconds(3f);
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
            if(currentState.GetType() != typeof(MonsterRaidState))
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
}
