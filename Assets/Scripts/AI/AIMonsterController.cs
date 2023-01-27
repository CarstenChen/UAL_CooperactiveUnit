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
}

public class AIMonsterController : MonoBehaviour
{
    [Header("Monster Settings")]
    public Parameter param;
    public LayerMask adjustNormalLayer;
    public Transform body;
    public Waypoints[] routes;

    [NonSerialized] public NavMeshAgent agent;
    [NonSerialized] public bool readyToChase;
    [NonSerialized] public bool routeChanged;
    [NonSerialized] public bool playerInSight;
    [NonSerialized] public bool playerInSphereTrigger;
    [NonSerialized] public Waypoints currentPatrolRoute;

    protected Waypoints previousPatrolRoute;
    protected Dictionary<StateType, State> states = new Dictionary<StateType, State>();
    protected bool lastPlayerInSight;
    protected bool lastPlayerInSphereTrigger;



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
    }

    private void Start()
    {
        RegisterState();
        SwitchToState(StateType.Idle);

        InvokeRepeating("AdjustPatrolRoutes", 0f, 3f);
    }

    private void Update()
    {
        //贴地
        UpdateBodyYAxis();

        if (PlayerInput.pi_Instance.TestInput1)
        {
            SwitchToState(StateType.Raid);
            StartCoroutine(SpawnBehindPlayer());
        }

        if (readyToChase)
        {
            SwitchToState(StateType.Chase);
        }


        if (routeChanged && (currentState.GetType() == typeof(MonsterIdleState) || currentState.GetType() == typeof(MonsterPatrolState)))
        {
            SwitchToState(StateType.Patrol);
        }

        currentState.OnStateStay();

        routeChanged = false;

        //记录上一帧玩家是否在视野中和球形Trigger中
        lastPlayerInSight = playerInSight;
        lastPlayerInSphereTrigger = playerInSight;
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

    IEnumerator SpawnBehindPlayer()
    {
        agent.enabled = false;
        yield return new WaitForSeconds(2f);
        agent.enabled = true;
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

        if (currentPatrolRoute!=previousPatrolRoute)
        {
            routeChanged = true;
            Debug.Log("ChangeRoute");
        }
    }
}
