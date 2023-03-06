using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshCollection : MonoBehaviour
{
    public GameObject[] meshLists;
    public bool isActive;

    public void EnableMesh()
    {
        if (meshLists.Length == 0)
        {
            PlayerChangeBody.playerCompleteAutomaticWriting = true;
            return;
        }

        for(int i = 0; i < meshLists.Length; i++)
        {
            if (isActive)
            {
                meshLists[i].SetActive(true);
            }
            else
            {
                meshLists[i].SetActive(false);
            }
        }

    }
}
