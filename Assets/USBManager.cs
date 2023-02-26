using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USBManager : MonoBehaviour
{
    public GameObject[] usbs;
    public string[] animationStateNames;
    public MainFragmentSpawner mainFragmentSpawner;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < usbs.Length; i++)
        {
            if(!mainFragmentSpawner.GetCanInteract(i))
            usbs[i].GetComponent<Animator>().Play(animationStateNames[i]);
        }
    }
}
