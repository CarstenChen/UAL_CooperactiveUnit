using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryInteractible : Interactibes
{
    public int plotID;
    public GameObject hideGameObject;
    public float startDelay;
    public float duration;

    protected bool hidden=false;

    public override void Interact()
    {
        base.Interact();

        LinesManager.Instance.DisplayLine(plotID, 0);

        if (hideGameObject != null)
        {
            StartCoroutine(StartHideGameObject());
            //hideGameObject.SetActive(false);
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
