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
        //make sure agent has path
        agent.enabled = true;
        agent.isStopped = false;

        //stop braking
        agent.autoBraking = false;

        patrolRoute = monster.currentPatrolRoute;

        //teleport to first wayPoint
        if (patrolRoute.wayPoints.Length != 0)
        {
            if (patrolRoute.invisiblePoints.Count != 0)
            {
                destPointIndex = patrolRoute.invisiblePoints[Random.Range(0, patrolRoute.invisiblePoints.Count)];
            }
            else
            {
                destPointIndex = 0;
            }

            //temporarily stop agent on change position
            agent.enabled = false;
            monster.transform.position = patrolRoute.wayPoints[destPointIndex].position;
            agent.enabled = true;
            agent.SetDestination(patrolRoute.wayPoints[destPointIndex].position);
        }

        Debug.Log("Patrol");

        SoundManager.Instance.PlayMonsterSound();
    }
    public void OnStateStay()
    {
        //Debug.Log(agent.hasPath);
        
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
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

        if (agent.enabled == true)
            agent.destination = patrolRoute.wayPoints[destPointIndex].position;
        
        //Debug.Log(string.Format("Going to {0}", destPointIndex));
        //repeat a cycle
        destPointIndex = (destPointIndex + 1) % patrolRoute.wayPoints.Length;
    }
}