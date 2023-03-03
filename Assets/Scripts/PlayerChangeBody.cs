using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerChangeBody : MonoBehaviour
{

    public MeshCollection[] meshListGroups1;
    public MeshCollection[] meshListGroups2;
    public BodyMeshSpawner bodyMeshSpawner;
    public GameObject[] bodies;
    public int currentBodyIndex;

    public static bool playerCompleteAutomaticWriting;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            currentBodyIndex = bodyMeshSpawner.GetBodyIndex();

            if (currentBodyIndex == 0)
            {
                bodies[0].SetActive(true);
                bodies[1].SetActive(false);
            }
            else
            {
                Destroy(bodies[0]);
                bodies[1].SetActive(true);
                StartCoroutine(ResetAnimator());
            }

            for (int i = 0; i < meshListGroups1.Length; i++)
            {
                meshListGroups1[i].isActive = bodyMeshSpawner.GetIsActive(0, i);
                if (currentBodyIndex == 0)
                    meshListGroups1[i].EnableMesh();
            }

            for (int i = 0; i < meshListGroups2.Length; i++)
            {
                meshListGroups2[i].isActive = bodyMeshSpawner.GetIsActive(1, i);
                if (currentBodyIndex == 1)
                    meshListGroups2[i].EnableMesh();
            }
        }

    }
    public void UpdatePlayerBodyMesh()
    {
        SoundManager.Instance.PlayBodyChangeSound();
        if (AIDirector.Instance.currentMainStoryIndex > 3) return;

        if (AIDirector.Instance.currentMainStoryIndex == 3)
        {
            Destroy(bodies[0]);
            currentBodyIndex = 1;
            bodies[1].SetActive(true);
            StartCoroutine(ResetAnimator());

            for (int i = 0; i < meshListGroups2.Length; i++)
            {
                meshListGroups2[i].isActive = true;
                meshListGroups2[i].EnableMesh();
            }            
        }
        else
        {
            currentBodyIndex = 0;
            meshListGroups1[AIDirector.Instance.currentMainStoryIndex - 1].isActive = true;
            meshListGroups1[AIDirector.Instance.currentMainStoryIndex - 1].EnableMesh();
        }


        //foreach (var go in meshListGroups[AIDirector.Instance.currentMainStoryIndex-1].meshLists)
        //{
        //    go.SetActive(true);
        //}

    }

    IEnumerator ResetAnimator()
    {
        
        yield return new WaitForSeconds(0.2f);
        GetComponent<Animator>().Rebind();
        GetComponent<Animator>().Update(0);
    }

    private void OnDestroy()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            if (currentBodyIndex == 0)
            {
                for (int i = 0; i < meshListGroups1.Length; i++)
                {
                    bodyMeshSpawner.SaveData(i, meshListGroups1[i].isActive, 0,currentBodyIndex);
                }
                for (int i = 0; i < meshListGroups2.Length; i++)
                {
                    bodyMeshSpawner.SaveData(i, false, 1,currentBodyIndex);
                }
            }
            else
            {
                for (int i = 0; i < meshListGroups1.Length; i++)
                {
                    bodyMeshSpawner.SaveData(i, true, 0, currentBodyIndex);
                }
                for (int i = 0; i < meshListGroups2.Length; i++)
                {
                    bodyMeshSpawner.SaveData(i, meshListGroups2[i].isActive,1, currentBodyIndex);
                }

            }

        }
    }
}
