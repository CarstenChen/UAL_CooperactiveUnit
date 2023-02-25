using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryInteractible : MonoBehaviour
{
    [Header("Story Interactibe Settings")]
    public int plotID;

    [Header("Data Setting")]
    public StorySpawner storySpawner;
    public int dataIndex;

    protected bool canInteract;

    protected void Start()
    {
        canInteract = storySpawner.GetCanInteract(dataIndex);

            this.GetComponentInChildren<Collider>().enabled = storySpawner.GetCanInteract(dataIndex);
    }


    private void OnDestroy()
    {
        storySpawner.SaveData(dataIndex, canInteract, this.GetComponentInChildren<Collider>().enabled);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!LinesManager.isPlayingLines && canInteract)
        {
            LinesManager.Instance.DisplayLine(plotID, 0);
                canInteract = false;
        }
    }
}
