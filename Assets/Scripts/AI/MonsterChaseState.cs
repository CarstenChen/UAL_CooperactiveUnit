using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterChaseState : State
{
    private AIMonsterController monster;
    private NavMeshAgent agent;
    private Parameter param;

    protected float tick;

    public MonsterChaseState(AIMonsterController monster)
    {
        this.monster = monster;
        this.agent = monster.agent;
        this.param = monster.param;
    }
    public void OnStateEnter()
    {

        //make sure agent has path
        agent.enabled = true;
        agent.destination = param.chaseTarget.position;

        //random chase speed
        agent.speed = Random.Range(0,2)!=0? param.normalChaseSpeed : param.fastChaseSpeed;
        param.currentChaseSpeed = agent.speed;

        //make sure agent is not static
        agent.isStopped = false;
        agent.autoBraking = false;

        PlayerScreenEffects.Instance.enabled = true;

        Debug.Log("Chase");

        SoundManager.Instance.PlayMonsterSound();
    }
    public void OnStateStay()
    {

        agent.SetDestination(param.chaseTarget.position);

    }
    public void OnStateExit()
    {
        monster.readyToChase = false;
        monster.playerInSphereTrigger = false;
        monster.playerHeard = false;
        agent.speed = param.normalChaseSpeed;
        agent.autoBraking = true;
        PlayerScreenEffects.Instance.enabled = false;
    }

}


