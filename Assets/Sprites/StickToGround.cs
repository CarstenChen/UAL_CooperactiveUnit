using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToGround : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RaycastHit hit;

        int Rmask = LayerMask.GetMask("Terrain");

        Vector3 Point_dir = transform.TransformDirection(Vector3.down);


        if (Physics.Raycast(transform.position, Point_dir, out hit, 50.0f, Rmask))
        {
            transform.position = hit.transform.position;
        }
    }
}
