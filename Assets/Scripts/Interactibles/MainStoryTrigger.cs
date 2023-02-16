using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainStoryTrigger : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
           
            if (!LinesManager.isPlayingLines)
            {
                LinesManager.Instance.DisplayLine(AIDirector.Instance.currentMainStoryIndex, 0);
                AIDirector.Instance.ReadMainStory();

                StartCoroutine(MainStoryStateCount());
            }

            StartCoroutine(LockMainStoryTrigger(10f));
        }
    }

    IEnumerator LockMainStoryTrigger(float delay)
    {
        yield return new WaitForSeconds(delay);

        this.gameObject.SetActive(false);
    }

    IEnumerator MainStoryStateCount()
    {
        AIDirector.Instance.isInMainStoryTimeLine = true;
        yield return new WaitForSeconds(5f);
        AIDirector.Instance.isInMainStoryTimeLine = false;
    }
}
