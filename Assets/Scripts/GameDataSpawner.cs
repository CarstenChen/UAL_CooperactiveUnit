using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameDataSpawner", order = 4)]
public class GameDataSpawner : SpawnManagerScriptableObject
{
    public int currentMainStoryIndex;
    public  float playerSan;
    public bool hasFinishedGuide;

    protected int defaultMainStoryIndex = 0;
    protected float defaltPlayerSan = 260;
    protected bool defaultHasFinishedGuide = false;

    public void ResetData()
    {
        currentMainStoryIndex = defaultMainStoryIndex;
        playerSan = defaltPlayerSan;
        hasFinishedGuide = defaultHasFinishedGuide;
    }

    public void SaveData(int currentMainStoryIndex, float playerSan, bool hasFinishedGuide)
    {
        this.currentMainStoryIndex = currentMainStoryIndex;
        this.playerSan = playerSan;
        this.hasFinishedGuide = hasFinishedGuide;
    }

    public int GetCurrentMainStoryIndex()
    {
        return currentMainStoryIndex;
    }

    public float GetPlayerSan()
    {
        return playerSan;
    }

    public bool GetHasFinishedGuide()
    {
        return hasFinishedGuide;
    }

}
