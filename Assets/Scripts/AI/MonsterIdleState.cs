using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterIdleState : State
{
    private AIMonsterController monster;
    private NavMeshAgent agent;
    private Parameter param;
    public MonsterIdleState(AIMonsterController monster)
    {
        this.monster = monster;
        this.agent = monster.agent;
        this.param = monster.param;
    }

    public void OnStateEnter()
    {
        param.bodyMesh.SetActive(false);
        agent.enabled = true;
        agent.speed = 0f;
        agent.isStopped = true;


        Debug.Log("Idle");
    }
    public void OnStateStay()
    {

    }

    public void OnStateExit()
    {
        agent.speed = param.normalChaseSpeed;
        agent.isStopped = false;
        param.bodyMesh.SetActive(true);
    }
}