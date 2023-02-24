using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Cinemachine;

public class MainStoryTrigger : Interactibes
{
    public CinemachineVirtualCamera showCamera;
    public CinemachineFreeLook freeLookCamera;
    public PlayerController player;

    [Header("Data Setting")]
    public MainFragmentSpawner mainFragmentSpawner;
    public int dataIndex;

    protected override void Awake()
    {
        base.Awake();
        showCamera.Priority = 8;
    }

    protected override void Start()
    {
        GetComponent<Collider>().enabled = mainFragmentSpawner.GetCanInteract(dataIndex);
        destroyAfterCollected.SetActive(mainFragmentSpawner.GetCanInteract(dataIndex));
    }

    public override void Interact()
    {
        base.Interact();

        if (!LinesManager.isPlayingLines)
        {
            SoundManager.Instance.PlayMainStoryTriggerSound();

            showCamera.Priority = 20;
            freeLookCamera.GetComponent<CinemachineInputProvider>().enabled = false;
            PlayerInput.inputBlock = true;

            LinesManager.Instance.DisplayLine(AIDirector.Instance.currentMainStoryIndex + 1, 0);
            AIDirector.Instance.ReadMainStory();

            StartCoroutine(AIDirector.Instance.MainStoryStateCount());
            StartCoroutine(PlayerBodyChangeEffect(2.5f));

            GetComponent<Collider>().enabled = false;
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {    
    //        if (!LinesManager.isPlayingLines)
    //        {
    //            showCamera.Priority = 20;
    //            freeLookCamera.GetComponent<CinemachineInputProvider>().enabled = false;
    //            PlayerInput.inputBlock = true;

    //            LinesManager.Instance.DisplayLine(AIDirector.Instance.currentMainStoryIndex + 1, 0);
    //            AIDirector.Instance.ReadMainStory();

    //            StartCoroutine(AIDirector.Instance.MainStoryStateCount());
    //            StartCoroutine(PlayerBodyChangeEffect(2.5f));

    //            GetComponent<Collider>().enabled = false;
    //        }


    //    }
    //}

    IEnumerator PlayerBodyChangeEffect(float delay)
    {
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


    private void OnDestroy()
    {
        mainFragmentSpawner.SaveData(dataIndex, destroyAfterCollected.activeSelf);
    }
}
