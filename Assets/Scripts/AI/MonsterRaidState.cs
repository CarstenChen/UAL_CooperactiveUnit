using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Find a point behind and teleport there, so ready to attack player on its back
/// </summary>
public class MonsterRaidState : State
{
    private AIMonsterController monster;
    private NavMeshAgent agent;
    private Parameter param;
    public MonsterRaidState(AIMonsterController monster)
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
        //make sure agent is static
        agent.speed = 0f;
        agent.acceleration = param.fastDeccelaration;
        agent.isStopped = true;

        monster.readyToChase = false;

        monster.transform.position = GetPointBehindPlayer();
        Debug.Log("Raid");
    }
    public void OnStateStay()
    {
        //if teleport on an isle, teleport again
        if (!agent.hasPath)
        {
            monster.transform.position = GetPointBehindPlayer();
        }
    }

    public void OnStateExit()
    {
        agent.speed = param.normalChaseSpeed;
        agent.acceleration = param.normalAcceleration;
        agent.isStopped = false;

    }

    private Vector3 GetPointBehindPlayer()
    {
        float angleBehindPlayer = Random.Range(180 - param.raidAngleBehindPlayer, 180 + param.raidAngleBehindPlayer) + param.chaseTarget.GetComponent<PlayerController>().targetAngle;

        float offsetX = param.raidDistanceBehindPlayer * Mathf.Sin(angleBehindPlayer * Mathf.PI / 180);
        float offsetZ = param.raidDistanceBehindPlayer * Mathf.Cos(angleBehindPlayer * Mathf.PI / 180);

        Vector3 position = new Vector3(param.chaseTarget.transform.position.x + offsetX, 0, param.chaseTarget.transform.position.z + offsetZ);
        position = NavMesh.SamplePosition(position, out NavMeshHit hit, 25f, 1) ? hit.position : monster.transform.position;
        
        return position;
    }


}