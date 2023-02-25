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

    [Header("Timeline Setting")]
    public GameObject timeline;
    protected MeshRenderer renderer;
    protected override void Awake()
    {
        base.Awake();
        showCamera.Priority = 8;
    }

    protected override void Start()
    {
        GetComponent<Collider>().enabled = mainFragmentSpawner.GetCanInteract(dataIndex);

        renderer = destroyAfterCollected.GetComponent<MeshRenderer>();
        renderer.enabled = mainFragmentSpawner.GetCanInteract(dataIndex);
        particle.SetActive(mainFragmentSpawner.GetCanInteract(dataIndex));
    }

    public override void Interact()
    {
        base.Interact();

        if (!LinesManager.isPlayingLines)
        {
            

            //showCamera.Priority = 20;
            //freeLookCamera.GetComponent<CinemachineInputProvider>().enabled = false;



            LinesManager.Instance.DisplayLine(AIDirector.Instance.currentMainStoryIndex + 1, 0);
            AIDirector.Instance.ReadMainStory();

            StartCoroutine(PlayMainStoryTriggerSound());
            StartCoroutine(AIDirector.Instance.MainStoryStateCount(timeline));
            StartCoroutine(PlayerBodyChangeEffect(9f));

            //GetComponent<Collider>().enabled = false;
        }
    }

    IEnumerator PlayMainStoryTriggerSound()
    {
        yield return new WaitForSeconds(0.6f);
    SoundManager.Instance.PlayMainStoryTriggerSound();
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
        //showCamera.Priority = 8;
        //StartCoroutine(ResetCamera());
    }

    IEnumerator ResetCamera()
    {
        yield return new WaitForSeconds(2.5f);
        freeLookCamera.GetComponent<CinemachineInputProvider>().enabled = true;
        PlayerInput.inputBlock = false;
    }


    private void OnDestroy()
    {
        mainFragmentSpawner.SaveData(dataIndex, renderer.enabled);
    }
}
