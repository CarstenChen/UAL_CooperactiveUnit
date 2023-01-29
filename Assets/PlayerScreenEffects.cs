using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using UnityEngine.UI;

public class PlayerScreenEffects : MonoBehaviour
{
    protected static PlayerScreenEffects instance;
    public static PlayerScreenEffects Instance { get { return instance; } private set { } }



    //public Volume volume;
    public float monsterDetectRange;
    //[NonSerialized] public Vignette vignette;

    [Header("Vignette Settings")]
    public Image vignetteImg;
    public float vignetteScaleValue;

    [Header("Ring Settings")]
    [Range(0, 1)]
    public float ringScaleValue = 1f;
    public Sprite normalSprite;
    public Sprite highlightSprite;
    public AIMonsterController monster;

    public bool isGetingClose;

    public Image ringImage;
    protected RectTransform imageTransform;

    protected Vector3 originSize;


    Coroutine currentRingCoroutine;
    private void OnEnable()
    {
        vignetteImg.enabled = true;
        ringImage.enabled = true;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        this.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SphereCollider>().radius = monsterDetectRange;
        //volume.profile.TryGet<Vignette>(out vignette);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag =="Monster")
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            vignetteScaleValue = Mathf.Clamp(distance / monsterDetectRange, 0, 1);
            //vignette.intensity.value = Mathf.Clamp(value, 0, 1);
            vignetteImg.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1) * vignetteScaleValue;

            if (vignetteScaleValue - ringScaleValue < 0)
            {
                ResetForwardRing();
            }
            ringImage.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1) * ringScaleValue;

            UpdateRingImage();
        }
    }

    private void OnDisable()
    {
        if (currentRingCoroutine != null)
            StopCoroutine(currentRingCoroutine);
        if(vignetteImg!=null)
        vignetteImg.enabled = false;
        if (ringImage != null)
        ringImage.enabled = false;
    }

    public void ResetForwardRing()
    {
        
        ringScaleValue = Mathf.Clamp(vignetteScaleValue - UnityEngine.Random.Range(0.1f, 0.15f),0,1f);
    }

    public void DealWithRingDisplay()
    {
        if(currentRingCoroutine!=null)
        StopCoroutine(currentRingCoroutine);
        currentRingCoroutine = StartCoroutine(AfterScream());

    }

    IEnumerator AfterScream()
    {
        ringImage.enabled = false;
        yield return new WaitForSeconds(3f);
        ringImage.enabled = true;
        Debug.Log("A");
        ResetForwardRing();
    }

    protected void UpdateRingImage()
    {
        if (Mathf.Abs(vignetteScaleValue - ringScaleValue) < 0.05f)
        {
            ringImage.sprite = highlightSprite;
        }
        else
        {
            ringImage.sprite = normalSprite;
        }
    }
}
