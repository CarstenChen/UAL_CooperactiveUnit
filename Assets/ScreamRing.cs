using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreamRing : MonoBehaviour
{
    [Range(0,1)]
    public float scale =1f;
    public Sprite normalSprite;
    public Sprite highlightSprite;
    public PlayerScreenEffects playerScreenEffects;
    public AIMonsterController monster;

    public bool isGetingClose;

    protected Image ringImage;
    protected RectTransform imageTransform;

    protected Vector3 originSize;
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
        if (playerScreenEffects.vignetteScaleValue - scale < 0)
        {
            ResetForwardRing();
        }
        imageTransform.localScale = originSize * scale;

        UpdateImage();

        
    }

    public void ResetForwardRing()
    {
        //scale = Mathf.Clamp(playerScreenEffects.currentScaleValue - Random.Range(0.1f, 0.15f),0,1f);
        Debug.Log(string.Format("{0},{1}",scale, playerScreenEffects.vignetteScaleValue));
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
        if (Mathf.Abs(playerScreenEffects.vignetteScaleValue - scale) < 0.05f)
        {
            ringImage.sprite = highlightSprite;
        }
        else
        {
            ringImage.sprite = normalSprite;
        }
    }
}
