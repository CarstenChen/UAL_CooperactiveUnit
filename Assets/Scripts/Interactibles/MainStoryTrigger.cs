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

    public GameObject hideGameObject;
    public float startDelay;
    public float duration;

    protected bool hidden = false;

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
        canInteract = mainFragmentSpawner.GetCanInteract(dataIndex);

        if (AIDirector.Instance.hasFinishedGuide && AIDirector.Instance.currentMainStoryIndex>=1)
        {
            if (hideGameObject != null)
            {
                hideGameObject.SetActive(false);
            }
        }
    }

    public override void Interact()
    {
        base.Interact();

        if (!LinesManager.isPlayingLines)
        {

            //LinesManager.Instance.DisplayLine(AIDirector.Instance.currentMainStoryIndex + 1, 0);
            //AIDirector.Instance.ReadMainStory();
            //PortalAnimStateChange.Instance.animCount++;
            //PortalAnimStateChange.Instance.isPlay = true;

            //StartCoroutine(PlayMainStoryTriggerSound());
            //StartCoroutine(AIDirector.Instance.MainStoryStateCount(timeline));
            StartCoroutine(PlayerBodyChangeEffect(9f));

            if (hideGameObject != null && hideGameObject.activeSelf)
            {
                //StartCoroutine(StartHideGameObject());
                hideGameObject.SetActive(false);
            }
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
        if (AIDirector.Instance.currentMainStoryIndex != 0)
        {
            //open guide UI
            autoWritingGuideUI.gameObject.SetActive(true);
            GuideUIController.instance.ShowGuideUI(autoWritingGuideUI);
            StartCoroutine(WaitAutoWritingGuide());
            //move camera
            PlayerController.ChangeToFaceCamera();
        }

        AIDirector.Instance.isInBodyChange = true;
        PlayerChangeBody.playerCompleteAutomaticWriting = false;

        //wait automatic writing and dissolve
        player.GetComponent<PlayerChangeBody>().UpdatePlayerBodyMesh();
        yield return new WaitUntil(()=> PlayerChangeBody.playerCompleteAutomaticWriting ==true);

        AIDirector.Instance.isInBodyChange = false;
        PlayerController.ChangeToFPSCamera();

        //play story timeline
        DealWithMainStoryMovie();
    }

    void DealWithMainStoryMovie()
    {
        StartCoroutine(PlayMainStoryTriggerSound());
        LinesManager.Instance.DisplayLine(AIDirector.Instance.currentMainStoryIndex, 0);
        AIDirector.Instance.ReadMainStory();
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
        autoWritingGuideUI.alpha = 0;
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

    IEnumerator StartHideGameObject()
    {
        yield return new WaitForSeconds(startDelay);
        StartCoroutine(HideGameObject());
        StartCoroutine(CountTime());
    }
    IEnumerator HideGameObject()
    {
        yield return null;
        if (!hidden)
        {
            hideGameObject.transform.Translate(new Vector3(0, -0.04f, 0));
            StartCoroutine(HideGameObject());
        }
        else
        {
            StopAllCoroutines();
        }

    }

    IEnumerator CountTime()
    {
        yield return new WaitForSeconds(duration);
        hidden = true;
        hideGameObject.SetActive(false);

    }
}
