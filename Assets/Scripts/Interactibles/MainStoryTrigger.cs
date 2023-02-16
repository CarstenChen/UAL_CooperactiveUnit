using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Cinemachine;

public class MainStoryTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera showCamera;
    public CinemachineFreeLook freeLookCamera;
    public GameObject effect;
    public PlayerController player;

    private void Awake()
    {
        showCamera.Priority = 8;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            showCamera.Priority = 20;
            freeLookCamera.GetComponent<CinemachineInputProvider>().enabled = false;
            PlayerInput.inputBlock = true;

            if (!LinesManager.isPlayingLines)
            {
                LinesManager.Instance.DisplayLine(AIDirector.Instance.currentMainStoryIndex, 0);
                AIDirector.Instance.ReadMainStory();

                StartCoroutine(MainStoryStateCount());
            }

            StartCoroutine(LockMainStoryTrigger(2.5f));
        }
    }

    IEnumerator LockMainStoryTrigger(float delay)
    {
        effect.SetActive(false);
        yield return new WaitForSeconds(delay);
        player.GetComponent<PlayerChangeBody>().UpdatePlayerBodyMesh();
        yield return new WaitForSeconds(delay);
        showCamera.Priority = 8;
        StartCoroutine(ResetCamera());
    }

    IEnumerator ResetCamera()
    {
        yield return new WaitForSeconds(2.5f);
        freeLookCamera.GetComponent<CinemachineInputProvider>().enabled = true;
        PlayerInput.inputBlock = false;
    }
    IEnumerator MainStoryStateCount()
    {
        AIDirector.Instance.isInMainStoryTimeLine = true;
        yield return new WaitForSeconds(5f);
        AIDirector.Instance.isInMainStoryTimeLine = false;


        if (AIDirector.Instance.currentMainStoryIndex >= AIDirector.Instance.mainStoryNum)
        {
            AIDirector.Instance.finalSceneTimeline.SetActive(true);
            StartCoroutine(WaitTimeline());
        }
    }

    IEnumerator WaitTimeline()
    {
        yield return new WaitForSeconds((float)AIDirector.Instance.finalSceneTimeline.GetComponent<PlayableDirector>().duration);
        AIDirector.Instance.finalSceneGate.SetActive(true);
    }
}
