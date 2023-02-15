using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGateTrigger : MonoBehaviour
{
    public string finalSceneName = "FinalScene";

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        StartCoroutine(SceneLoader.instance.LoadScene(finalSceneName, Color.black)) ;
        
    }
}
