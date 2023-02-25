using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerChangeBody : MonoBehaviour
{

    public MeshCollection[] meshListGroups;
    public BodyMeshSpawner bodyMeshSpawner;

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "MainScene")
        {

            for (int i = 0; i < meshListGroups.Length; i++)
            {
                meshListGroups[i].isActive = bodyMeshSpawner.GetIsActive(i);
                meshListGroups[i].EnableMesh();
            }
        }

    }
    public void UpdatePlayerBodyMesh()
    {
        SoundManager.Instance.PlayBodyChangeSound();

        if (AIDirector.Instance.currentMainStoryIndex > meshListGroups.Length) return;


        meshListGroups[AIDirector.Instance.currentMainStoryIndex - 1].isActive = true;
        meshListGroups[AIDirector.Instance.currentMainStoryIndex - 1].EnableMesh();

        //foreach (var go in meshListGroups[AIDirector.Instance.currentMainStoryIndex-1].meshLists)
        //{
        //    go.SetActive(true);
        //}

    }

    private void OnDestroy()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            for (int i = 0; i < meshListGroups.Length; i++)
            {
                bodyMeshSpawner.SaveData(i, meshListGroups[i].isActive);
            }
        }
    }
}
