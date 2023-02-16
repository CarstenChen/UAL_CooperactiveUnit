using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class RockAntAIController : MonoBehaviour
{
    public Waypoints patrolRoute;

    private NavMeshAgent agent;
    private int destPointIndex = 0;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        //make sure agent has path
        agent.enabled = true;
        agent.isStopped = false;

        //stop braking
        agent.autoBraking = false;

        //teleport to first wayPoint
        if (patrolRoute.wayPoints.Length != 0)
        {
            destPointIndex = 0;

            //temporarily stop agent on change position
            agent.enabled = false;
            transform.position = patrolRoute.wayPoints[0].position;
            agent.enabled = true;
            agent.SetDestination(patrolRoute.wayPoints[destPointIndex].position);
        }
    }
    void Update()
    {
        //Debug.Log(agent.hasPath);
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
            GoToNextPoint();
    }

    private void GoToNextPoint()
    {
        if (patrolRoute.wayPoints.Length == 0)
            return;
        if (agent.enabled == true)
        {
            if (destPointIndex == 0)
            {
                agent.enabled = false;
                transform.position = patrolRoute.wayPoints[0].position;
                agent.enabled = true;
                agent.SetDestination(patrolRoute.wayPoints[destPointIndex].position);
            }
            else

                agent.destination = patrolRoute.wayPoints[destPointIndex].position;
        }


        //Debug.Log(string.Format("ÕýÔÚÇ°Íù{0}", destPointIndex));
        //repeat a cycle
        destPointIndex = (destPointIndex + 1) % patrolRoute.wayPoints.Length;
    }
}


