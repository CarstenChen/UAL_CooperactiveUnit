using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshCollection : MonoBehaviour
{
    public GameObject[] meshLists;
    public bool isActive;

    public void SetActive(bool isActive)
    {
        foreach (var go in meshLists)
        {
                    if (isActive)
        {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }

        }

    }
}
