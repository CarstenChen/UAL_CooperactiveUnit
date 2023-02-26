using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class ConsoleInteractible : Interactibes
{
    [Header("Console Settings")]
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineFreeLook freeLookCamera;
    public Transform standPoint;
    public GameObject cameraCenter;
    public CanvasGroup autoWritingGuideUI;
    protected bool getKeyToHideGuideUI;

    protected override void Start()
    {
        base.Start();
        freeLookCamera = GameObject.Find("TPS FreeLook").GetComponent<CinemachineFreeLook>();
        virtualCamera = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
    }
    public override void Interact()
    {

        base.Interact();
        GuideUIController.instance.ShowGuideUI(autoWritingGuideUI);
        StartCoroutine(WaitAutoWritingGuide());
        freeLookCamera.GetComponent<CinemachineInputProvider>().enabled = false;
        PlayerInput.inputBlock = true;
        StartCoroutine(StartAutoWriting());
    }

    IEnumerator StartAutoWriting()
    {
        yield return new WaitForSeconds(0.05f);
        FinalSceneAIDirector.Instance.autoWriting = true;
        player.transform.position = standPoint.position;
        player.transform.rotation = standPoint.rotation;
        virtualCamera.LookAt = cameraCenter.transform;
        virtualCamera.Priority = 20;
    }

    IEnumerator WaitAutoWritingGuide()
    {
        PlayerInput.inputBlock = true;
        yield return new WaitForSeconds(1f);
        StartCoroutine(WaitKey());
        yield return new WaitUntil(() => getKeyToHideGuideUI == true);
        getKeyToHideGuideUI = false;
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
