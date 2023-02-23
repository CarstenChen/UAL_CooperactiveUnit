using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BodyMeshSpawner", order = 5)]
public class BodyMeshSpawner : SpawnManagerScriptableObject
{
    public bool[] isActives;

    public void ResetData()
    {
        for (int i = 0; i < numberOfPrefabsToCreate; i++)
        {
            isActives[i] = false;
        }
    }

    public void SaveData(int dataIndex, bool isActive)
    {
        isActives[dataIndex] = isActive;
    }

    public bool GetIsActive(int dataIndex)
    {
        return isActives[dataIndex];
    }
}
