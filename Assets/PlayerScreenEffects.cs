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
    public Material vignetteMtl;
    public float vignetteIntesity = 0.5f;

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

    protected bool ringLocked;
    private void OnEnable()
    {
        ringLocked = false;
        vignetteImg.enabled = false;
        ringImage.enabled = false;
        vignetteMtl.SetFloat("_FullScreenIntensity", 0);
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        vignetteMtl.SetFloat("_FullScreenIntensity", 0f);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            if (this.enabled == false) return;

            EnableEffect();

            vignetteImg.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            ringImage.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            ringImage.sprite = normalSprite;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Monster")
        {
            if (this.enabled == false) return;
            EnableEffect();

            float distance = Vector3.Distance(other.transform.position, transform.position);
            vignetteScaleValue = Mathf.Clamp(distance / monsterDetectRange, 0, 1);
            //vignette.intensity.value = Mathf.Clamp(value, 0, 1);
            vignetteImg.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1) * vignetteScaleValue;
            vignetteMtl.SetFloat("_ScreenEdgeSize", vignetteScaleValue);

            if (vignetteScaleValue - ringScaleValue < 0)
            {
                ResetForwardRing();
            }
            ringImage.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1) * ringScaleValue;

            UpdateRingImage();
        }
    }

    protected void EnableEffect()
    {
        vignetteImg.enabled = true;
        if (!ringLocked)
            ringImage.enabled = true;
        vignetteMtl.SetFloat("_FullScreenIntensity", vignetteIntesity);
    }

    private void OnDisable()
    {
        if (currentRingCoroutine != null)
            StopCoroutine(currentRingCoroutine);

        if (vignetteImg != null)
        {
            vignetteImg.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            vignetteImg.enabled = false;

        }

        if (ringImage != null)
        {
            ringImage.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            ringImage.enabled = false;
        }


        if (vignetteMtl != null)
            vignetteMtl.SetFloat("_FullScreenIntensity", 0f);
    }

    public void ResetForwardRing()
    {

        ringScaleValue = Mathf.Clamp(vignetteScaleValue - UnityEngine.Random.Range(0.1f, 0.15f), 0, 1f);
    }

    public void DealWithRingDisplay()
    {
        if (currentRingCoroutine != null)
            StopCoroutine(currentRingCoroutine);
        currentRingCoroutine = StartCoroutine(AfterScream());

    }

    IEnumerator AfterScream()
    {
        ringLocked = true;
        ringImage.enabled = false;
        yield return new WaitForSeconds(3f);
        ringImage.enabled = true;
        ringLocked = false;
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
