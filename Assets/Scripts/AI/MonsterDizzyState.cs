using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
public class MonsterDizzyState : State
{
    private AIMonsterController monster;
    private NavMeshAgent agent;
    private Parameter param;

    protected float tick;
    protected bool firstDizzy = true;
    public MonsterDizzyState(AIMonsterController monster)
    {
        this.monster = monster;
        this.agent = monster.agent;
        this.param = monster.param;
    }

    public void OnStateEnter()
    {
        //stop agent
        agent.speed = 0f;
        agent.isStopped = true;
        agent.enabled = false;

        Debug.Log("Dizzy");

        monster.currentCoolDown = param.coolDown[Mathf.Clamp(monster.dizzyTimes++, 0, param.coolDown.Length - 1)];

        SoundManager.Instance.StopMonsterSound();

        param.dizzyObj.SetActive(true);
    }
    public void OnStateStay()
    {

    }



    public void OnStateExit()
    {
        agent.enabled = true;
        agent.speed = param.normalChaseSpeed;
        agent.isStopped = false;

        monster.hitTimes = 0;

        param.dizzyObj.SetActive(false);
    }
}