using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterAttackState : State
{
    private AIMonsterController monster;
    private NavMeshAgent agent;
    private Parameter param;
    public MonsterAttackState(AIMonsterController monster)
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

        //inherit chase speed;
        agent.speed = param.currentChaseSpeed;
        agent.acceleration = param.fastDeccelaration;

        //make sure agent is not static
        agent.isStopped = false;

        Debug.Log("Attack");

        param.animator.SetBool("Attack", true);

        AIDirector.Instance.onBeingCatched = true;
    }
    public void OnStateStay()
    {
        if(param.animatorCache.currentStateInfo.IsName("Attack")&& param.animatorCache.currentStateInfo.normalizedTime >= 1.0f)
        {
            
            monster.attackOver = true;
        }
    }

    public void OnStateExit()
    {
        agent.acceleration = param.normalAcceleration;
        AIDirector.Instance.onBeingCatched = false;
        AIDirector.isGameOver = true;

    }
}