using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGameObject : MonoBehaviour
{
    public GameObject[] goLists;
    // Start is called before the first frame update
    void Start()
    {
        goLists[Random.Range(0, goLists.Length)].SetActive(true);
    }
}
