using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainStoryTrigger : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            LinesManager.Instance.DisplayLine(AIDirector.Instance.currentMainStoryIndex, 0);

            AIDirector.Instance.ReadMainStory();

            StartCoroutine(LockMainStoryTrigger(3f));
        }
    }

    IEnumerator LockMainStoryTrigger(float delay)
    {
        yield return new WaitForSeconds(delay);

        this.gameObject.SetActive(false);
    }
}
