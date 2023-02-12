using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ConsoleInteractible : Interactibes
{
    [Header("Console Settings")]
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineFreeLook freeLookCamera;
    public Transform standPoint;
    public GameObject cameraCenter;

    protected override void Start()
    {
        base.Start();
        freeLookCamera = GameObject.Find("TPS FreeLook").GetComponent<CinemachineFreeLook>();
        virtualCamera = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
    }
    public override void Interact()
    {
        
        base.Interact();
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
}
