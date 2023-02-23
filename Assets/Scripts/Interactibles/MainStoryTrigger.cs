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

    [Header("Data Setting")]
    public MainFragmentSpawner mainFragmentSpawner;
    public int dataIndex;

    private void Awake()
    {
        showCamera.Priority = 8;
    }

    private void Start()
    {
        GetComponent<Collider>().enabled = mainFragmentSpawner.GetCanInteract(dataIndex);
        effect.SetActive(mainFragmentSpawner.GetCanInteract(dataIndex));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SoundManager.Instance.PlayMainStoryTriggerSound();
            if (!LinesManager.isPlayingLines)
            {
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
    }

    IEnumerator PlayerBodyChangeEffect(float delay)
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


    private void OnDestroy()
    {
        mainFragmentSpawner.SaveData(dataIndex, effect.activeSelf);
    }
}
