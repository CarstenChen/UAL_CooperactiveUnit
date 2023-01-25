using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterChaseState : State
{
    private AIMonsterController monster;
    private NavMeshAgent agent;
    private Parameter param;
    public MonsterChaseState(AIMonsterController monster)
    {
        this.monster = monster;
        this.agent = monster.agent;
        this.param = monster.param;
    }

    public void OnStateEnter()
    {
        agent.isStopped = false;
        Debug.Log("Chase");
    }
    public void OnStateStay()
    {
        if (agent.enabled == true)
            agent.SetDestination(param.chaseTarget.position);
    }

    public void OnStateExit()
    {
        monster.readyToChase = false;
    }
}