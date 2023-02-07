using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanInteractible : Interactibes
{
    public float sanRecover = 60f;
    public override void Interact()
    {
        base.Interact();

        AIDirector.Instance.AddSan(sanRecover);
        particle.Play();
        
    }
}
