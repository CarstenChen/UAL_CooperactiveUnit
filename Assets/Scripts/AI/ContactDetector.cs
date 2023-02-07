using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDetector : MonoBehaviour
{
    private static ContactDetector instance;
    public static ContactDetector Instance { get { return instance; } private set { } }

    public static bool contactWithPlayer; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "PlayerBody")
        {
            contactWithPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PlayerBody")
        {
            contactWithPlayer = false;
        }
    }
}
