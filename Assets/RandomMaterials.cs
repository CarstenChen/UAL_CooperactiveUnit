using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterials : MonoBehaviour
{
    public Material[] materials;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];
    }
}
