using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideUIController : MonoBehaviour
{
    public static GuideUIController instance { get; private set; }

    public CanvasGroup[] guideUI;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }


    public void ShowGuideUI(CanvasGroup uiCanvasGroup)
    {
        StartCoroutine(UIFadeIn(uiCanvasGroup));
        
    }
    IEnumerator UIFadeIn(CanvasGroup uiCanvasGroup)
    {
        yield return null;
        if (uiCanvasGroup.alpha < 1)
        {
            uiCanvasGroup.alpha = Mathf.Lerp(uiCanvasGroup.alpha, 1, 0.04f);
            if (1 - uiCanvasGroup.alpha < 0.05f)
            {
                uiCanvasGroup.alpha = 1;
            }
            StartCoroutine(UIFadeIn(uiCanvasGroup));
        }
        //else
        //{
        //    StopAllCoroutines();
        //    StartCoroutine(Wait(uiCanvasGroup));
        //}

    }

    IEnumerator Wait(CanvasGroup uiCanvasGroup)
    {
        yield return new WaitForSeconds(2f);
       StartCoroutine(UIFadeOut(uiCanvasGroup));
    }
    IEnumerator UIFadeOut(CanvasGroup uiCanvasGroup)
    {
        yield return null;
        if (uiCanvasGroup.alpha > 0)
        {
            uiCanvasGroup.alpha = Mathf.Lerp(uiCanvasGroup.alpha, 0, 0.04f);
            if ( uiCanvasGroup.alpha < 0.05f)
            {
                uiCanvasGroup.alpha = 0;
            }
            StartCoroutine(UIFadeOut(uiCanvasGroup));
        }
        else
        {
            StopAllCoroutines();
        }

    }
}
