using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSize : MonoBehaviour
{
    public float minSize;
    public float maxSize;
    private void OnEnable()
    {
        float random = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(random, 1, random);
    }
}
