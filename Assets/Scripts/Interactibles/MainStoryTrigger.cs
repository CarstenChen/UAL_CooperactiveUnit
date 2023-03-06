using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Cinemachine;
using UnityEngine.InputSystem;

public class MainStoryTrigger : Interactibes
{
    public CinemachineVirtualCamera showCamera;
    public CinemachineFreeLook freeLookCamera;
    public PlayerController player;
    public GameObject lightProb;

    [Header("Data Settings")]
    public MainFragmentSpawner mainFragmentSpawner;
    public int dataIndex;

    [Header("Timeline Settings")]
    public GameObject timeline;
    protected MeshRenderer renderer;

    [Header("UI Settings")]
    public CanvasGroup autoWritingGuideUI;
    protected bool getKeyToHideGuideUI;
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
        lightProb.SetActive(mainFragmentSpawner.GetCanInteract(dataIndex));
    }

    public override void Interact()
    {
        base.Interact();

        if (!LinesManager.isPlayingLines)
        {
            AIDirector.Instance.ReadMainStory();
            GuideUIController.instance.ShowGuideUI(autoWritingGuideUI);
            StartCoroutine(WaitAutoWritingGuide());
            //LinesManager.Instance.DisplayLine(AIDirector.Instance.currentMainStoryIndex + 1, 0);
            //AIDirector.Instance.ReadMainStory();
            //PortalAnimStateChange.Instance.animCount++;
            //PortalAnimStateChange.Instance.isPlay = true;

            //StartCoroutine(PlayMainStoryTriggerSound());
            //StartCoroutine(AIDirector.Instance.MainStoryStateCount(timeline));
            StartCoroutine(PlayerBodyChangeEffect(9f));
           
        }
    }

    public override void HideObject()
    {
        lightProb.SetActive(false);
        StartCoroutine(HideSelf());
    }
    IEnumerator PlayMainStoryTriggerSound()
    {
        yield return new WaitForSeconds(0.6f);
        SoundManager.Instance.PlayMainStoryTriggerSound();
    }

    IEnumerator HideSelf()
    {
        yield return new WaitUntil(() => PlayerChangeBody.playerCompleteAutomaticWriting == true);
        yield return new WaitForSeconds(0.5f);
        base.HideObject();
    }

    IEnumerator PlayerBodyChangeEffect(float delay)
    {
        AIDirector.Instance.isInBodyChange = true;
        PlayerController.ChangeToFaceCamera();
        PlayerChangeBody.playerCompleteAutomaticWriting = false;
        //yield return new WaitForSeconds(delay);
        player.GetComponent<PlayerChangeBody>().UpdatePlayerBodyMesh();
        yield return new WaitUntil(()=> PlayerChangeBody.playerCompleteAutomaticWriting ==true);
        AIDirector.Instance.isInBodyChange = false;
        PlayerController.ChangeToFPSCamera();
        DealWithMainStoryMovie();

    }

    void DealWithMainStoryMovie()
    {
        StartCoroutine(PlayMainStoryTriggerSound());
        LinesManager.Instance.DisplayLine(AIDirector.Instance.currentMainStoryIndex + 1, 0);
        PortalAnimStateChange.Instance.animCount++;
        PortalAnimStateChange.Instance.isPlay = true;
        StartCoroutine(AIDirector.Instance.MainStoryStateCount(timeline));
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

    IEnumerator WaitAutoWritingGuide()
    {
        PlayerInput.inputBlock = true;
        yield return new WaitForSeconds(1f);
        StartCoroutine(WaitKey());
        yield return new WaitUntil(() => getKeyToHideGuideUI == true);
        getKeyToHideGuideUI = false;
        //GuideUIController.instance.HideGuideUI(autoWritingGuideUI);
        autoWritingGuideUI.gameObject.SetActive(false);
    }

    IEnumerator WaitKey()
    {
        yield return null;

        if (Keyboard.current.anyKey.isPressed)
        {
            if (!Keyboard.current.escapeKey.isPressed)
            {
                getKeyToHideGuideUI = true;
            }
        }
        else
        {
            StartCoroutine(WaitKey());
        }

    }
}
