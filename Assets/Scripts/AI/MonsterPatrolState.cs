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
    }

    public void OnStateEnter()
    {
        patrolRoute = monster.currentPatrolRoute;
    }
    public void OnStateStay()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPoint();
    }

    public void OnStateExit()
    {

    }

    private void GoToNextPoint()
    {
        if (patrolRoute.wayPoints.Length == 0)
            return;
        //将代理设置为前往当前选定的目标。
        agent.destination = patrolRoute.wayPoints[destPointIndex].position;

        //选择数组中的下一个点作为目标，循环到开始
        destPointIndex = (destPointIndex + 1) % patrolRoute.wayPoints.Length;
    }
    
}