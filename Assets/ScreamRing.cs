using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreamRing : MonoBehaviour
{
    [Range(0,1)]
    public float scale;
    public Sprite normalSprite;
    public Sprite highlightSprite;
    public PlayerScreenEffects playerScreenEffects;
    public AIMonsterController monster;

    public bool isGetingClose;

    protected Image ringImage;
    protected RectTransform imageTransform;

    protected Vector3 originSize;

    float tick;
    // Start is called before the first frame update
    void Start()
    {
        ringImage = GetComponent<Image>();
        imageTransform = GetComponent<RectTransform>();
        originSize = imageTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScreenEffects.vignette.intensity.value - scale > 0)
        {
            ResetForwardRing();
        }


        UpdateImage();

        imageTransform.localScale = originSize * (1.2f - scale);
    }

    public void ResetForwardRing()
    {
        scale = Mathf.Clamp(playerScreenEffects.vignette.intensity.value + Random.Range(0.1f, 0.15f),0,0.9f);
    }

    public IEnumerator AfterScream()
    {
        DisableRing();
        yield return new WaitForSeconds(3f);
        EnableRing();

    }
    public void DisableRing()
    {
        ringImage.enabled = false;
    }
    public void EnableRing()
    {
        ringImage.enabled = true;
    }

    protected void UpdateImage()
    {
        if (Mathf.Abs(playerScreenEffects.vignette.intensity.value - scale) < 0.05f)
        {
            ringImage.sprite = highlightSprite;
        }
        else
        {
            ringImage.sprite = normalSprite;
        }
    }
}
