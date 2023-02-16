using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustNormalToGround : MonoBehaviour
{
    private void OnEnable()
    {
        RaycastHit hit;
        int Rmask = LayerMask.GetMask("Terrain");

        Vector3 Point_dir = transform.TransformDirection(Vector3.down);

        if (Physics.Raycast(transform.position, Point_dir, out hit, 50.0f, Rmask))
        {

            Quaternion NextRot = Quaternion.LookRotation(Vector3.Cross(hit.normal, Vector3.Cross(transform.forward, hit.normal)), hit.normal);

            transform.rotation = NextRot;
        }
    }
}
