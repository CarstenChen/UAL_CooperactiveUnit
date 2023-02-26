using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTitleRotation : MonoBehaviour
{
    public GameObject title;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("GamePassed") == 1)
        {
            title.transform.localRotation = Quaternion.Euler(new Vector3(-90, 180, 0));
        }
        else
        {
            title.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
