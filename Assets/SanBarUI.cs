using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SanBarUI : MonoBehaviour
{
    public Color normalColor;
    public Color alertColor;
    public Color dangerousColor;
    protected Image img;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        img.fillAmount = AIDirector.playerSan / AIDirector.Instance.totalPlayerSan;

        if (img.fillAmount >= 0.5f)
        {
            img.color = normalColor;
        }
        else if (img.fillAmount<0.5f && img.fillAmount >= 0.2f)
        {
            img.color = alertColor;
        }
        else
        {
            img.color = dangerousColor;
        }
    }
}
