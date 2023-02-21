using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initrializer : MonoBehaviour
{
    public SanAppleSpawner sanAppleSpawner;
    public StorySpawner storySpawner;
    public MainFragmentSpawner mainFragmentSpawner;
    public GameDataSpawner gameDataSpawner;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnDestroy()
    {
        
    }
}
