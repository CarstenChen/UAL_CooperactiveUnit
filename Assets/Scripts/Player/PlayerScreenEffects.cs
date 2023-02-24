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
    //public Image vignetteImg;

    public float effectScaleValue;

    //public Material vignetteMtl;
    //public float vignetteIntesity = 0.5f;

    [Header("Ring Settings")]
    [Range(0, 1)]
    public float ringScaleValue = 1f;

    //public Sprite normalSprite;
    //public Sprite highlightSprite;
    public AIMonsterController monster;
    public bool isGetingClose;
    //public Image ringImage;


    [Header("Model Settings")]
    public GameObject shieldModel;
    public float originalShieldSize;
    public float minShieldSize;
    public Color shieldColor1;
    public Color shieldColor2;
    public Material shieldMaterial;
    public GameObject attackModel;
    public float originalAttackSize;


    protected RectTransform imageTransform;

    protected Vector3 originSize;
    protected Coroutine currentRingCoroutine;

    public bool ringLocked;
    public bool playerCannotScream;

    protected bool firstTimeToScream;

    public Volume volume;
    protected Vignette vignette;

    private void OnEnable()
    {
        ringLocked = false;

        shieldModel.SetActive(false);
        attackModel.SetActive(false);
        //attackModel.transform.localScale = new Vector3(1, 1, 1) * originalAttackSize;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGet(out vignette);
        this.enabled = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            if (this.enabled == false) return;

            EnableEffect();

            shieldModel.transform.localScale = new Vector3(1, 1, 1) * originalShieldSize;
            attackModel.transform.localScale = new Vector3(1, 1, 1) * originalAttackSize;

            shieldMaterial.SetColor("_FresnelColor", shieldColor1);
            shieldMaterial.SetColor("_BackColor", shieldColor1);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Monster")
        {
            if (this.enabled == false) return;

            float distance = Vector3.Distance(other.transform.position, transform.position);
            effectScaleValue = Mathf.Clamp(distance / monsterDetectRange, 0, 1);
            //effectScaleValue = distance / monsterDetectRange;

            if (effectScaleValue >= 1f)
            {
                DisableEffect();
                ringLocked = false;
                return;
            }

            if (!firstTimeToScream)
            {
                GuideUIController.instance.ShowGuideUI(GuideUIController.instance.guideUI[1]);
                firstTimeToScream = true;
            }

            AIDirector.Instance.onCatchingState = true;

            EnableEffect();

            attackModel.transform.localScale = new Vector3(1, 1, 1) * originalAttackSize*effectScaleValue;
            vignette.intensity.value = 1 - effectScaleValue;

            if (effectScaleValue - ringScaleValue < -0.05f || (playerCannotScream && effectScaleValue - ringScaleValue>0.1f))
            {
                ResetForwardRing();
            }

            if (ringScaleValue> minShieldSize / originalShieldSize)
            {
                playerCannotScream = false;
                shieldModel.transform.localScale = new Vector3(1, 1, 1) * originalShieldSize * ringScaleValue;
                UpdateRingImage();
            }
            else
            {
                playerCannotScream = true;
                shieldModel.SetActive(false);
            }
        }
    }

    protected void EnableEffect()
    {
        if (!shieldModel.activeSelf)
        {
            ResetForwardRing();

            if (!ringLocked && !playerCannotScream)
            {

                shieldModel.SetActive(true);
            }
        }

        attackModel.SetActive(true);
        vignette.active = true;

    }

    protected void DisableEffect()
    {
        if (currentRingCoroutine != null)
            StopCoroutine(currentRingCoroutine);

        if (attackModel != null)
        {
            attackModel.transform.localScale = new Vector3(1, 1, 1) * originalAttackSize;
            attackModel.SetActive(false);
            vignette.active = false;
        }

        if (shieldModel != null)
        {
            shieldModel.transform.localScale = new Vector3(1, 1, 1) * originalShieldSize;

            shieldModel.SetActive(false);
        }
    }

    private void OnDisable()
    {
        
        DisableEffect();

        AIDirector.Instance.onCatchingState = false;
    }


    public void ResetForwardRing()
    {

        ringScaleValue = Mathf.Clamp(effectScaleValue - UnityEngine.Random.Range(0.1f, 0.1f), 0, 1f);
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
        //ringImage.enabled = false;
        shieldModel.SetActive(false);
        yield return new WaitForSeconds(3f);
        //ringImage.enabled = true;
        shieldModel.SetActive(true);
        ringLocked = false;
        ResetForwardRing();
    }

    protected void UpdateRingImage()
    {
        if (!AIDirector.Instance.monsterGuideFinished)
        {
            if (Mathf.Abs(effectScaleValue - ringScaleValue) < 0.08f)
            {
                if (!AIDirector.Instance.monsterGuideFinished && shieldModel.activeSelf)
                {
                    AIDirector.Instance.bulletTime = true;
                }

                //ringImage.sprite = highlightSprite;
                shieldMaterial.SetColor("_FresnelColor", shieldColor2);
                shieldMaterial.SetColor("_BackColor", shieldColor2);
            }
            else
            {
                //ringImage.sprite = normalSprite;
                shieldMaterial.SetColor("_FresnelColor", shieldColor1);
                shieldMaterial.SetColor("_BackColor", shieldColor1);
            }
        }
        else
        {
            if (Mathf.Abs(effectScaleValue - ringScaleValue) < 0.05f)
            {
                //ringImage.sprite = highlightSprite;
                shieldMaterial.SetColor("_FresnelColor", shieldColor2);
                shieldMaterial.SetColor("_BackColor", shieldColor2);
            }
            else
            {
                //ringImage.sprite = normalSprite;
                shieldMaterial.SetColor("_FresnelColor", shieldColor1);
                shieldMaterial.SetColor("_BackColor", shieldColor1);
            }
        }

    }
}
