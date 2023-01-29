using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[Serializable]
public class Parameter
{
    public Transform chaseTarget;
    public Animator animator;
    public float raidDistanceBehindPlayer;
    public float raidAngleBehindPlayer = 60f;
    public float fastDeccelaration = 100f;
    public float normalAcceleration = 10f;
    public float normalChaseSpeed = 10f;
    public float fastChaseSpeed = 15f;
    public float minSpeed;

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
    [NonSerialized] public bool playerFirstFound;
    [NonSerialized] public Waypoints currentPatrolRoute;

    protected Waypoints previousPatrolRoute;
    protected Dictionary<StateType, State> states = new Dictionary<StateType, State>();
    protected bool lastPlayerInSight;
    protected bool lastPlayerInSphereTrigger;

    protected Coroutine currentSlowDownCoroutine;

    public enum StateType
    {
        Idle, Chase, Attack, Patrol, Raid
    }

    protected State currentState;

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        param.animator = GetComponent<Animator>();

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

    private void FixedUpdate()
    {
        //ground rotation
        UpdateBodyYAxis();

        bool raidWhenSeePlayer = false;
        if(currentState.GetType() == typeof(MonsterPatrolState))
        {
            raidWhenSeePlayer = UnityEngine.Random.Range(0, 2) != 0 ? true : false;
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
            (playerInSight && !raidWhenSeePlayer && currentState.GetType() == typeof(MonsterPatrolState)))
        {
            SwitchToState(StateType.Chase);
        }

        if(!playerInSphereTrigger && !playerFirstFound && currentState.GetType() == typeof(MonsterChaseState))
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
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, -Vector3.up);

        Debug.DrawRay(transform.position + Vector3.up * 0.5f, -Vector3.up, Color.red);

        if (Physics.Raycast(ray, out hit, 3f, adjustNormalLayer, QueryTriggerInteraction.Ignore))
        {
            if (hit.normal == Vector3.up)
            {
                if (body.rotation != transform.rotation)
                {
                    body.rotation = Quaternion.Lerp(body.rotation, transform.rotation, 0.1f);
                }
                else
                {
                    return;
                }
            }

            Quaternion groundRotation = Quaternion.LookRotation(Vector3.Cross(hit.normal, Vector3.Cross(body.forward, hit.normal)), hit.normal);
            body.rotation = Quaternion.Lerp(body.rotation, groundRotation, 0.1f);
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

        int pickRoute = 0;

        for (int i = 0; i < routes.Length - 1; i++)
        {
            if (Vector3.Distance(param.chaseTarget.position, routes[i].root.position) > Vector3.Distance(param.chaseTarget.position, routes[i + 1].root.position))
            {
                pickRoute = i + 1;
            }
        }

        currentPatrolRoute = routes[pickRoute];

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
    }

    IEnumerator SlowDown(float slowDownRate)
    {
        agent.speed = Mathf.Clamp(agent.speed * UnityEngine.Random.Range(0, slowDownRate), param.minSpeed, param.currentChaseSpeed);
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
        }

    }
}
