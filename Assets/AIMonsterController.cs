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
    public float raidAngleBehindPlayer=60f;
}

public class AIMonsterController : MonoBehaviour
{
    [Header("Monster Settings")]
    public Parameter param;
    public LayerMask adjustNormalLayer;
    public Transform body;

    [NonSerialized]
    public NavMeshAgent agent;
    public bool readyToChase;

    protected Dictionary<StateType, State> states = new Dictionary<StateType, State>();


    public enum StateType
    {
        Idle, Chase, Attack, Patrol, Raid
    }

    protected State currentState;

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); 
        param.animator = GetComponent<Animator>();
    }

    private void Start()
    {
        RegisterState();
        SwitchToState(StateType.Idle);
    }

    private void Update()
    {
        agent.SetDestination(param.chaseTarget.position);

        UpdateBodyYAxis();

        if (PlayerInput.pi_Instance.TestInput1)
        {
            SwitchToState(StateType.Raid);
        }

        if (readyToChase)
        {
            SwitchToState(StateType.Chase);
        }
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
}
