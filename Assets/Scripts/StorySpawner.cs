using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StorySpawner", order = 2)]
public class StorySpawner : SpawnManagerScriptableObject
{
    public bool[] canInteracts;
    public bool[] isItemActives;

    protected bool[] originalCanInteracts;
    protected bool[] originalIsItemActives;

    public void Awake()
    {

    }
    public void RecordOriginalData()
    {
        originalCanInteracts = new bool[numberOfPrefabsToCreate];
        originalIsItemActives = new bool[numberOfPrefabsToCreate];

        for (int i = 0; i < numberOfPrefabsToCreate; i++)
        {
            originalCanInteracts[i] = true;
            originalIsItemActives[i] = true;
        }
    }
    public void ResetData()
    {
        for (int i = 0; i < numberOfPrefabsToCreate; i++)
        {
            canInteracts[i] = originalCanInteracts[i];
            isItemActives[i] = originalIsItemActives[i];
        }
    }

    public void SaveData(int dataIndex, bool canInteract, bool isItemActive)
    {
        canInteracts[dataIndex] = canInteract;
        isItemActives[dataIndex] = isItemActive;
    }

    public bool GetCanInteract(int dataIndex)
    {
        return canInteracts[dataIndex];
    }

    public bool GetIsItemActive(int dataIndex)
    {
        return isItemActives[dataIndex];
    }


}