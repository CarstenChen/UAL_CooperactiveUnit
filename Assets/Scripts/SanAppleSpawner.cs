using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SanAppleSpawner", order = 1)]
public class SanAppleSpawner : SpawnManagerScriptableObject
{
    public float[] recoverCounts;
    public bool[] canInteracts;
    public bool[] isItemActives;

    protected float[] originalCounts;
    protected bool[] originalCanInteracts;
    protected bool[] originalIsItemActives;

    public void Awake()
    {

    }
    public void RecordOriginalData()
    {
        originalCounts = new float[numberOfPrefabsToCreate];
        originalCanInteracts = new bool[numberOfPrefabsToCreate];
        originalIsItemActives = new bool[numberOfPrefabsToCreate];

        for (int i = 0; i < numberOfPrefabsToCreate; i++)
        {
            originalCounts[i] = 0;
            originalCanInteracts[i] = true;
            originalIsItemActives[i] = true;
        }
    }
        public void ResetData()
    {
        for (int i = 0; i < numberOfPrefabsToCreate; i++)
        {
            recoverCounts[i] = originalCounts[i];
            canInteracts[i] = originalCanInteracts[i];
            isItemActives[i] = originalIsItemActives[i];
        }
    }

    public void SaveData(int dataIndex, float recoverCount, bool canInteract, bool isItemActive)
    {
        recoverCounts[dataIndex] = recoverCount;
        canInteracts[dataIndex] = canInteract;
        isItemActives[dataIndex] = isItemActive;
    }

    public float GetRecoverCount(int dataIndex)
    {
        return recoverCounts[dataIndex];
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
