using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBoxBoundary : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GetComponent<MeshRenderer>().enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
