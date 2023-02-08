using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EmitterManager : MonoBehaviour
{
    private static EmitterManager instance;
    public static EmitterManager Instance { get { return instance; } private set { } }

    public MeshEmitter[] emitters;



    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

        emitters = GetComponentsInChildren<MeshEmitter>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RandomArrangeEmitter()
    {
        for(int i = 0;i < emitters.Length; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-20 , 20), -10f, Random.Range(20, 40));

            emitters[i].transform.position = pos;
            emitters[i].ChangeMesh();
            emitters[i].EmitMesh();
        }

    }
}
