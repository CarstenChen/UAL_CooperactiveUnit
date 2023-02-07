using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanInteractible : Interactibes
{
    [Header("San Interactibe Settings")]
    public ParticleSystem sanRecoverParticle = default;

    public float sanRecover = 60f;
    public override void Interact()
    {
        base.Interact();

        AIDirector.Instance.AddSan(sanRecover);
        sanRecoverParticle.Play();
        
    }
}
