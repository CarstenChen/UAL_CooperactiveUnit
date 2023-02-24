using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryInteractible : Interactibes
{
    [Header("Story Interactibe Settings")]
    public int plotID;

    [Header("Data Setting")]
    public StorySpawner storySpawner;
    public int dataIndex;

    protected override void Start()
    {
        base.Start();

        canInteract = storySpawner.GetCanInteract(dataIndex);

        MeshRenderer renderer = destroyAfterCollected.GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.enabled = storySpawner.GetCanInteract(dataIndex);

        if(particle!=default)
        particle.SetActive(storySpawner.GetCanInteract(dataIndex));
    }

    public override void Interact()
    {
        base.Interact();

        if (!LinesManager.isPlayingLines)
        {
            LinesManager.Instance.DisplayLine(plotID, 0);
        }
    }

    private void OnDestroy()
    {
        storySpawner.SaveData(dataIndex, canInteract, destroyAfterCollected.activeSelf);
    }
}
