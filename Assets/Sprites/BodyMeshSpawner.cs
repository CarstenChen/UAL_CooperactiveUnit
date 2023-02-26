using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BodyMeshSpawner", order = 5)]
public class BodyMeshSpawner : SpawnManagerScriptableObject
{
    public int bodyIndex;
    public bool[] meshgroup1Actives;
    public bool[] meshgroup2Actives;
    public void ResetData()
    {
        for (int i = 0; i < meshgroup1Actives.Length; i++)
        {
            meshgroup1Actives[i] = false;
        }

        for (int i = 0; i < meshgroup2Actives.Length; i++)
        {
            meshgroup2Actives[i] = false;
        }

        bodyIndex = 0;
    }

    public void SaveData(int dataIndex, bool isActive, int bodyIndex, int currentBodyIndex)
    {
        
        if (bodyIndex == 0)
        {
            meshgroup1Actives[dataIndex] = isActive;
        }
        else
        {
            meshgroup2Actives[dataIndex] = isActive;
        }


        this.bodyIndex = currentBodyIndex;
    }

    public bool GetIsActive(int bodyIndex,int dataIndex)
    {
        if(bodyIndex == 0)
        {
            return meshgroup1Actives[dataIndex];
        }
        else
        {
            return meshgroup2Actives[dataIndex];
        }

    }

    public int GetBodyIndex()
    {
        return bodyIndex;
    }
}
