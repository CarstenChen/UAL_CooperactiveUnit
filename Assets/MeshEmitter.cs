using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MeshEmitter : MonoBehaviour
{

    public GameObject[] meshList;
    public GameObject objectPool;

    public float lowSpeed;
    public float highSpeed;

    public float shortLifeTime;
    public float longLifeTime;

    protected GameObject currentMesh;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMesh()
    {
        currentMesh = meshList[Random.Range(0, meshList.Length)];
    }

    public void EmitMesh()
    {
        Quaternion dir = Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));    
        GameObject obj = ObjectPool.instance.GetGameObject(currentMesh, transform.position, dir, objectPool.transform);

        EmittedObject eo = obj.GetComponent<EmittedObject>();
        if (eo != null)
        {
            eo.speed = Random.Range(lowSpeed, highSpeed);
            eo.time = Random.Range(shortLifeTime, longLifeTime
                );
        }

    }
}
