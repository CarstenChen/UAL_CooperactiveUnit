using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshCollection : MonoBehaviour
{
    public GameObject[] meshLists;

    private void Awake()
    {
        foreach (var go in meshLists)
        {
            go.SetActive(false);
        }
    }
}
