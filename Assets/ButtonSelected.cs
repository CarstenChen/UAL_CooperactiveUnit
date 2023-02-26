using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ButtonSelected : MonoBehaviour
{
    //public GameObject constraint;
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.Select();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!constraint.activeSelf)
        //    button.Select();
    }
}
