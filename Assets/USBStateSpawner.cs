using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/USBStateSpawner", order = 7)]
public class USBStateSpawner : SpawnManagerScriptableObject
{
    public bool[] isActives;

    public void ResetData()
    {
        for (int i = 0; i < numberOfPrefabsToCreate; i++)
        {
            if (i == 0)
            {
                isActives[i] = true;
            }
            else
            {
                isActives[i] = false;
            }

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
