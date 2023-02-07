using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EmitterManager : MonoBehaviour
{
    public MeshEmitter[] emitters;
    // Start is called before the first frame update
    void Start()
    {
        PlayerInput.inputBlock = true;
        emitters = GetComponentsInChildren<MeshEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            RandomArrangeEmitter();
        }
    }

    public void RandomArrangeEmitter()
    {
        for(int i = 0;i < emitters.Length; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-8, 8), -10f, Random.Range(3, 12));

            emitters[i].transform.position = pos;
            emitters[i].ChangeMesh();
            emitters[i].EmitMesh();
        }

    }
}
