using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public bool xRandom;
    public bool yRandom;
    public bool zRandom;
    public float range;
    // Start is called before the first frame update
    void OnEnable()
    {
        float x = xRandom ? Random.Range(-range, range) : 0;
        float y = yRandom ? Random.Range(-range, range) : 0;
        float z = zRandom ? Random.Range(-range, range) : 0;
        transform.rotation = Quaternion.Euler(new Vector3(x,y,z));
    }
}
