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

    private void Start()
    {
        freeLookCamera = GameObject.Find("TPS FreeLook").GetComponent<CinemachineFreeLook>();
        virtualCamera = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
    }
    public override void Interact()
    {
        
        base.Interact();
        player.transform.position = standPoint.position;
        player.transform.rotation = standPoint.rotation;
        virtualCamera.LookAt = cameraCenter.transform;
        virtualCamera.Priority = 20;
        freeLookCamera.GetComponent<CinemachineInputProvider>().enabled = false;
        PlayerInput.inputBlock = true;
        FinalSceneAIDirector.Instance.autoWriting = true;
    }
}
