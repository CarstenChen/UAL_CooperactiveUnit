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

        ////闪现到第一个点
        //if (patrolRoute.wayPoints.Length != 0)
        //{
        //    monster.transform.position = patrolRoute.wayPoints[0].position;
        //    agent.SetDestination(patrolRoute.wayPoints[0].position);
        //}


        agent.autoBraking = false;
        Debug.Log("Patrol");
    }
    public void OnStateStay()
    {
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

        agent.destination = patrolRoute.wayPoints[destPointIndex].position;
        Debug.Log(string.Format("正在前往{0}", destPointIndex));
        //选择数组中的下一个点作为目标，循环到开始
        destPointIndex = (destPointIndex + 1) % patrolRoute.wayPoints.Length;
    }

    public float GetPathRemainingDistance(NavMeshAgent agent)
    {
        if (agent.pathPending ||
            agent.pathStatus == NavMeshPathStatus.PathInvalid ||
            agent.path.corners.Length == 0)
            return -1f;

        float distance = 0.0f;
        for (int i = 0; i < agent.path.corners.Length - 1; ++i)
        {
            distance += Vector3.Distance(agent.path.corners[i], agent.path.corners[i + 1]);
        }

        return distance;
    }

}