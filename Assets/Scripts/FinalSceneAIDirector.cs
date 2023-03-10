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
    public float dissolveRange;
    public Material[] monsterBodyMtls;
    public float[] dissolveRanges;

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
    protected float stopForceTick;

    [Header("Data Settings")]
    public GameDataSpawner gameDataSpawner;


    [System.NonSerialized] protected bool canPress = false;
    [System.NonSerialized] public bool autoWriting;
    protected float currentKeyNum;

    protected int previousPhase;
    protected bool phaseRaised;

    protected string currentKey;
    protected string previousKey;

    protected bool hasEndGame;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (instance == null)
            instance = this;
    }
    void Start()
    {
        ResetMaterial();
        finalCamera = GameObject.Find("CM vcam2").GetComponent<CinemachineVirtualCamera>();
        finalCamera.LookAt = finalCameraMoveTo.transform;
        finalCamera.Follow = finalCameraMoveTo.transform;

        hasEndGame = false;
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

        if (winRate >= 0.92)
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

            if (stopForceTick > 0)
            {
                stopForceTick -= Time.deltaTime;
            }
            if (stopForceTick <= 0)
            {
                DealWithSystemForce();
            }
        }
        DealWithSkyExposureChange();
        DealWithSkyColorChange();
        if (phaseRaised)
        {
            stopForceTick = 1f;
            if (currentPhase!=phaseWinrates.Length)
            PlayScreamEffect();
            FinalSceneSoundManager.Instance.PlayPlayerSound();
        }

        DealWithSkyRotationChange();


        previousPhase = currentPhase;
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2f);

        if (!hasEndGame)
        {
            StartCoroutine(SceneLoader.instance.LoadScene("StartScene", Color.white));
            hasEndGame = true;
            PlayerPrefs.SetInt("GamePassed", 1);
            gameDataSpawner.SaveHasFinishedGuide(false);
        }

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
            currentKeyNum++;
            Debug.Log(canPress);
            EmitterManager.Instance.RandomArrangeEmitter();
            currentCoolDown = coolDown * (1-winRate);
            tick = playerWritingAnimTick;            
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

    // add input force to players so players have to crazily tap the key board.
    void DealWithSystemForce()
    {
        if (Random.Range(0, 25) == 0)
            currentKeyNum = Mathf.Clamp(currentKeyNum - Random.Range(1, 2), 0, winKeyNum);
    }

    void DealWithDissolveModel()
    {
        //model dissolve
        Vector3 offset = new Vector3(0, dissolveRange - 2f* dissolveRange * winRate, 0);
        playerBodyMtl.SetVector("_DissolveOffset", -offset);

        for (int i = 0; i < monsterBodyMtls.Length; i++)
        {
            Vector3 newOffset = new Vector3(0, dissolveRanges[i] - 2f* dissolveRanges[i] * winRate, 0);
            monsterBodyMtls[i].SetVector("_DissolveOffset", newOffset);
        }
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
