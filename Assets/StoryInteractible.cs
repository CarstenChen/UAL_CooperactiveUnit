using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryInteractible : Interactibes
{
    public override void Interact()
    {
        base.Interact();

        LinesManager.Instance.DisplayLine(AIDirector.Instance.currentMainStoryIndex, 0);

        AIDirector.Instance.ReadMainStory();
    }
}
