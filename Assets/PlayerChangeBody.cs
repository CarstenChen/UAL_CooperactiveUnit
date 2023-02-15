using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerChangeBody : MonoBehaviour
{

    public MeshCollection[] meshListGroups;

    public void UpdatePlayerBodyMesh()
    {
        if (AIDirector.Instance.currentMainStoryIndex > meshListGroups.Length) return;

        foreach (var go in meshListGroups[AIDirector.Instance.currentMainStoryIndex-1].meshLists)
        {
            go.SetActive(true);
        }      
    }
}
