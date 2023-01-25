using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterPatrolState : State
{
    private AIMonsterController monster;
    private Parameter param;
    private Waypoints patrolRoute;
    private NavMeshAgent agent; 
    private int destPointIndex = 0;

    public MonsterPatrolState(AIMonsterController monster)
    {
        this.monster = monster;
        this.param = monster.param;
        this.agent = monster.agent;
    }

    public void OnStateEnter()
    {
        destPointIndex = 0;
        patrolRoute = monster.currentPatrolRoute;

        //闪现到第一个点
        if (patrolRoute.wayPoints.Length != 0)
        {
            //强制换位置需要暂时禁用agent
            agent.enabled = false;
            monster.transform.position = patrolRoute.wayPoints[destPointIndex].position;
            agent.enabled = true;
            agent.SetDestination(patrolRoute.wayPoints[destPointIndex].position);
        }


        agent.autoBraking = false;
        Debug.Log("Patrol");
    }
    public void OnStateStay()
    {
        //Debug.Log(agent.hasPath);
        
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPoint();
    }

    public void OnStateExit()
    {
        agent.autoBraking = true;
    }

    private void GoToNextPoint()
    {
        if (patrolRoute.wayPoints.Length == 0)
            return;

        //将代理设置为前往当前选定的目标。
        if (agent.enabled == true)
            agent.destination = patrolRoute.wayPoints[destPointIndex].position;
        
        Debug.Log(string.Format("正在前往{0}", destPointIndex));
        //选择数组中的下一个点作为目标，循环到开始
        destPointIndex = (destPointIndex + 1) % patrolRoute.wayPoints.Length;
    }
}