using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerChangeBody : MonoBehaviour
{

    public MeshCollection[] meshListGroups;
    public BodyMeshSpawner bodyMeshSpawner;

    private void Start()
    {
        for (int i = 0; i < meshListGroups.Length; i++)
        {
            meshListGroups[i].SetActive( bodyMeshSpawner.GetIsActive(i));
        }
    }
    public void UpdatePlayerBodyMesh()
    {
        if (AIDirector.Instance.currentMainStoryIndex > meshListGroups.Length) return;

        foreach (var go in meshListGroups[AIDirector.Instance.currentMainStoryIndex-1].meshLists)
        {
            go.SetActive(true);
        }

        meshListGroups[AIDirector.Instance.currentMainStoryIndex - 1].isActive = true;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < meshListGroups.Length; i++)
        {
            bodyMeshSpawner.SaveData(i,meshListGroups[i].isActive);
        }
    }
}
