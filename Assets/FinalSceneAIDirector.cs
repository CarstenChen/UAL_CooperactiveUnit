using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Cinemachine;

public class FinalSceneAIDirector : MonoBehaviour
{
    private static FinalSceneAIDirector instance;
    public static FinalSceneAIDirector Instance { get { return instance; } private set { } }

    public delegate void WinEvent();
    public static event WinEvent emittedObjectEvent;

    public Material playerBodyMtl;
    public Material monsterBodyMtl;

    public float winRate;
    public int currentPhase;

    [SerializeField] protected int winKeyNum;

    [SerializeField]protected float coolDown = 5f;
    [SerializeField] protected float currentCoolDown;
    [SerializeField] protected float[] phaseWinrates;
    [SerializeField] protected Material skyBoxMaterial;
    [SerializeField] protected float[] skyExposuresByPhases;
    [SerializeField] protected Color[] skyColorsByPhases;
    [SerializeField] protected CinemachineVirtualCamera finalCamera;
    [SerializeField] protected GameObject finalCameraMoveTo;
    [SerializeField] protected GameObject screamEffect;

    [Header("Player Settings")]
    public PlayerController player;
    public float playerWritingAnimTick;
    protected float tick;


    [System.NonSerialized] protected bool canPress = false;
    [System.NonSerialized] public bool autoWriting;
    protected float currentKeyNum;

    protected int previousPhase;
    protected bool phaseRaised;

    protected string currentKey;
    protected string previousKey;

    
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        ResetMaterial();
        finalCamera = GameObject.Find("CM vcam2").GetComponent<CinemachineVirtualCamera>();
        finalCamera.LookAt = finalCameraMoveTo.transform;
        finalCamera.Follow = finalCameraMoveTo.transform;
    }

    // Update is called once per frame
    void Update()
    {
        winRate = Mathf.Clamp( currentKeyNum / winKeyNum,0,1);

        for(int i = 0; i < phaseWinrates.Length; i++)
        {
            if (winRate >= phaseWinrates[i])
            {
                currentPhase = i;
            }
            else
            {
                break;
            }
        }

        if (previousPhase < currentPhase)
        {
            phaseRaised = true;
        }
        else
        {
            phaseRaised = false;
        }

        if (winRate >= 1)
        {
            emittedObjectEvent();
            finalCamera.Priority = 30;
            StartCoroutine(EndGame());
        }

        DealWithDissolveModel();

        if (autoWriting && winRate <1)
        {
            //type in
            DealWithAutoWriting();
            DealWithSystemForce();
        }
        DealWithSkyExposureChange();
        DealWithSkyColorChange();
        if (phaseRaised)
        {
            if(currentPhase!=phaseWinrates.Length)
            PlayScreamEffect();
        }

        DealWithSkyRotationChange();


        previousPhase = currentPhase;
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(SceneLoader.instance.LoadScene("Terrain", Color.white));

    }
    void DealWithAutoWriting()
    {
        tick -= Time.deltaTime;

        if (currentCoolDown > 0)
        {
            Debug.Log(canPress);
            canPress = false;
            currentCoolDown -= Time.deltaTime;
        }
        else
        {
            canPress = true;
        }

        if (Keyboard.current.anyKey.isPressed && canPress)
        {
            //InputSystem.onAnyButtonPress.CallOnce(ctrl => { currentKey = ctrl.name; });
            //if (currentKey == previousKey)
            //{
            //    return;
            //}

            currentKeyNum++;
            Debug.Log(canPress);
            EmitterManager.Instance.RandomArrangeEmitter();
            currentCoolDown = coolDown * (1-winRate);
            tick = playerWritingAnimTick;

            //InputSystem.onAnyButtonPress.CallOnce(ctrl => { previousKey = ctrl.name; });
            
        }

        if (tick > 0)
        {
            player.animator.SetBool("Writing", true);
        }
        else
        {
            player.animator.SetBool("Writing", false);
        }

    }

    void DealWithSystemForce()
    {
        if(Random.Range(0,25)==0)
        currentKeyNum=Mathf.Clamp(currentKeyNum- Random.Range(1, 2), 0,winKeyNum);
        
    }

    void DealWithDissolveModel()
    {
        //model dissolve
        Vector3 offset = new Vector3(0, 2 - 4 * winRate, 0);
        playerBodyMtl.SetVector("_DissolveOffset", offset);
        monsterBodyMtl.SetVector("_DissolveOffset", offset);
    }

    void DealWithSkyRotationChange()
    {
        skyBoxMaterial.SetFloat("_Rotation", 360 * winRate);
    }

    void DealWithSkyExposureChange()
    {
        skyBoxMaterial.SetFloat("_Exposure", skyExposuresByPhases[currentPhase]);
    }

    void DealWithSkyColorChange()
    {
        skyBoxMaterial.SetColor("_Tint", skyColorsByPhases[currentPhase]);
    }

     void ResetMaterial()
    {
        skyBoxMaterial.SetFloat("_Rotation", 0);
        skyBoxMaterial.SetFloat("_Exposure", skyExposuresByPhases[0]);
        skyBoxMaterial.SetColor("_Tint", skyColorsByPhases[0]);
    }
    void PlayScreamEffect()
    {
        screamEffect.SetActive(false);
        screamEffect.SetActive(true);

    }
}
