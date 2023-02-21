using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MainFragmentSpawner", order = 3)]
public class MainFragmentSpawner : SpawnManagerScriptableObject
{
    public bool[] canInteracts;

    protected bool[] originalCanInteracts;

    public void Awake()
    {

    }

    public void RecordOriginalData()
    {
        originalCanInteracts = new bool[numberOfPrefabsToCreate];

        for (int i = 0; i < numberOfPrefabsToCreate; i++)
        {
            originalCanInteracts[i] = true;
        }
    }
    public void ResetData()
    {
        for (int i = 0; i < numberOfPrefabsToCreate; i++)
        {
            canInteracts[i] = originalCanInteracts[i];
        }
    }

    public void SaveData(int dataIndex, bool canInteract)
    {
        canInteracts[dataIndex] = canInteract;
    }

    public bool GetCanInteract(int dataIndex)
    {
        return canInteracts[dataIndex];
    }


}
