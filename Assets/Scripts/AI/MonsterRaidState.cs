using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
        monster.readyToChase = false;
        monster.transform.position = GetPointBehindPlayer();
        Debug.Log("Raid");
    }
    public void OnStateStay()
    {
        Debug.Log(agent.enabled);
        //Debug.Log(agent.hasPath);
        //if (!agent.hasPath&&!agent.pathPending)
        //{
        //    monster.transform.position = GetPointBehindPlayer();
        //}
    }

    public void OnStateExit()
    {
        
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