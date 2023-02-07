using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryInteractible : Interactibes
{
    public int plotID;
    public override void Interact()
    {
        base.Interact();

        LinesManager.Instance.DisplayLine(plotID, 0);

        
    }
}
