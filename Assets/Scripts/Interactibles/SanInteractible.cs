using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanInteractible : Interactibes
{
    [Header("San Interactibe Settings")]
    public GameObject sanRecoverParticle = default;
    public GameObject hideGameObject;
    public float startDelay;
    public float duration;
    public float sanRecover = 60f;

    protected bool hidden = false;
    public override void Interact()
    {
        base.Interact();

        AIDirector.Instance.AddSan(sanRecover);
        sanRecoverParticle.SetActive(false);
        sanRecoverParticle.SetActive(true);

        if (hideGameObject != null)
        {
            StartCoroutine(StartHideGameObject());
            //hideGameObject.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && canInteract)
        {
            AIDirector.Instance.canTriggerSanGuide = true;

        }
    }

    IEnumerator StartHideGameObject()
    {
        yield return new WaitForSeconds(startDelay);
        StartCoroutine(HideGameObject());
        StartCoroutine(CountTime());
    }
    IEnumerator HideGameObject()
    {
        yield return null;
        if (!hidden)
        {
            hideGameObject.transform.Translate(new Vector3(0, -0.04f, 0));
            StartCoroutine(HideGameObject());
        }
        else
        {
            StopAllCoroutines();
        }

    }

    IEnumerator CountTime()
    {
        yield return new WaitForSeconds(duration);
        hidden = true;
    }
}
