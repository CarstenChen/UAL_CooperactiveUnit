using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public Transform root;
    public Transform[] wayPoints;

    private void Start()
    {
        root = transform;

        wayPoints = new Transform[transform.childCount];

        for(int i = 0; i < transform.childCount; i++)
        {
            wayPoints[i] = transform.GetChild(i).transform;
        }
        
    }
}
