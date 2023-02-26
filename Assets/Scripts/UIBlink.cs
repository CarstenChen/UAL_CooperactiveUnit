using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBlink : MonoBehaviour
{
    public TextMeshProUGUI startButtonText;
    public float minAlpha;
    public float maxAlpha;

    public bool fadeOut = true;
    // Start is called before the first frame update

    void Start()
    {
        startButtonText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeOut)
        {
            StartCoroutine(ColorFadeOut());
        }
        else
        {
            StartCoroutine(ColorFadeIn());
        }
    }

    IEnumerator ColorFadeIn()
    {
        startButtonText.color = Color.Lerp(startButtonText.color, new Color(255, 255, 255, maxAlpha), 0.02f);

        yield return new WaitForSeconds(1f);

        fadeOut = true;
    }

    IEnumerator ColorFadeOut()
    {
        startButtonText.color =  Color.Lerp(startButtonText.color, new Color(255, 255, 255, minAlpha), 0.02f);

        yield return new WaitForSeconds(1f);

        fadeOut = false;
    }
}
