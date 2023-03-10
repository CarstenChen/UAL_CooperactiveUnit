using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class AIDirector : MonoBehaviour
{
    private static AIDirector instance;
    public static AIDirector Instance { get { return instance; } private set { } }

    [Header("Monster Settings")]
    public AIMonsterController monster;
    public AnimationCurve difficultyCurve;

    [Header("Player Settings")]
    public PlayerController player;
    public Transform playerPosCheck;
    public float startPlayerSan = 150f;
    public float totalPlayerSan = 300f;

    [Header("Story Settings")]
    public int mainStoryNum;
    public GameObject finalSceneGate;
    public GameObject finalSceneTimeline;
    public GameObject timelineCamera;

    [Header("Guide Settings")]
    public bool playerInGuide;
    public bool hasFinishedGuide;
    public CanvasGroup guideMask;
    public CanvasGroup moveGuideUI;
    public CanvasGroup sanGuideUI;
    public CanvasGroup screamGuideUI;
    public GameObject runUI;
    public Vector3 monsterSpawnPos;

    [Header("Data Settings")]
    public GameObject initializer;
    public GameDataSpawner gameDataSpawner;

    [System.NonSerialized] public bool onBeingCatched;
    [System.NonSerialized] public bool onCatchingState;
    [System.NonSerialized] public bool onScreamRange;
    [System.NonSerialized] public static bool isGameOver = false;
    [System.NonSerialized] public static float playerSan;
    /*[System.NonSerialized]*/ public int currentMainStoryIndex = 0;
    [System.NonSerialized] public bool tensiveTime;
    [System.NonSerialized] public bool isInMainStoryTimeLine;
    [System.NonSerialized] public bool isInFinalSceneTimeLine;
    [System.NonSerialized] public bool isInBodyChange;
    [System.NonSerialized] public Coroutine currentTensiveTimeCoroutine;
    [System.NonSerialized] public bool hasRespawn;
    [System.NonSerialized] public bool isInAGuide;
    [System.NonSerialized] public bool moveGuideFinished;
    [System.NonSerialized] public bool canTriggerSanGuide;
    [System.NonSerialized] public bool sanGuideFinished;
    [System.NonSerialized] public bool canTriggerMonsterGuide;
    [System.NonSerialized] public bool monsterGuideFinished;
    [System.NonSerialized] public bool monsterAppearTimelineFinished;
    [System.NonSerialized] public bool bulletTime;
    [System.NonSerialized] public int guideScreamCount;
    [System.NonSerialized] public float playerSuccessToScream;
    [System.NonSerialized] public float playerFailToScream;
    [System.NonSerialized] public bool playerScreamOnce;
    /*[System.NonSerialized]*/ public float timeAfterChase;
    protected bool previousOnScreamRange;
    protected bool canCountTimeAfterChase;
    public float currentDifficulty;
    protected Coroutine currentGuideCoroutine;
    protected bool getKeyToHideGuideUI;

    public delegate void MonsterEvent(int currentMainStoryIndex);
    public static event MonsterEvent MonsterSpeedUpEvent;

    protected bool backToStart;

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        playerSan = startPlayerSan;

        if (gameDataSpawner != null)
        {
            hasFinishedGuide = gameDataSpawner.GetHasFinishedGuide();

            if (!GameObject.Find("Spawner(Clone)") || !hasFinishedGuide)
            {
                gameDataSpawner.ResetData();
                Initrializer i = Instantiate(initializer).GetComponent<Initrializer>();
                i.sanAppleSpawner.RecordOriginalData();
                i.storySpawner.RecordOriginalData();
                i.mainFragmentSpawner.RecordOriginalData();
                i.sanAppleSpawner.ResetData();
                i.storySpawner.ResetData();
                i.mainFragmentSpawner.ResetData();
                i.bodyMeshSpawner.ResetData();
                i.pictureStateSpawner.ResetData();
            }

            hasFinishedGuide = gameDataSpawner.GetHasFinishedGuide();
            currentMainStoryIndex = gameDataSpawner.GetCurrentMainStoryIndex();
            playerSan = gameDataSpawner.GetPlayerSan();
            MonsterSpeedUpEvent(currentMainStoryIndex);
        }


        DealWithFinalSceneGateTimeline();



        if (instance == null)
            instance = this;
        isGameOver = false;
        hasRespawn = false;
        onBeingCatched = false;
        onCatchingState = false;
        onScreamRange = false;
        isInMainStoryTimeLine = false;
        isInFinalSceneTimeLine = false;
        isInBodyChange = false;
        backToStart = false;
    }



    private void Start()
    {


        //enter guide
        if (!hasFinishedGuide && !playerInGuide)
        {
            playerInGuide = true;
        }

    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(hasFinishedGuide);
        if (previousOnScreamRange && previousOnScreamRange != onScreamRange)
        {
            canCountTimeAfterChase = true;
            timeAfterChase = 0;
        }

        if (canCountTimeAfterChase)
        {
            timeAfterChase += Time.deltaTime;
        }

        if (Keyboard.current.pauseKey.isPressed)
        {
            Destroy(GameObject.Find("Spawner(Clone)"));
            hasFinishedGuide = false;
            if (!backToStart)
            {
                StartCoroutine(SceneLoader.instance.LoadScene("StartScene", Color.black));
                backToStart = true;
            }
        }
        if (playerSan > 0f)
        {
            if (isInFinalSceneTimeLine || isInMainStoryTimeLine)
            {
                return;
            }
            playerSan -= Time.deltaTime;
        }
        else
        {
            Destroy(GameObject.Find("Spawner(Clone)"));
            hasFinishedGuide = false;

            if (!backToStart)
            {
                StartCoroutine(SceneLoader.instance.LoadScene("StartScene", Color.black));
                backToStart = true;
            }

        }
        if (isGameOver)
        {
            Debug.Log("You lose");

            if (!hasRespawn)
            {
                StartCoroutine(SceneLoader.instance.LoadScene("MainScene", Color.black));
                hasRespawn = true;
            }
        }


        if (playerInGuide && !hasFinishedGuide)
        {
            Guide();
            return;
        }

        previousOnScreamRange = onScreamRange;
    }

    public void ReadMainStory()
    {
        if (currentMainStoryIndex >= mainStoryNum)
            return;

        currentMainStoryIndex++;

        Debug.Log("Player has read main story");

        if (currentTensiveTimeCoroutine != null)
            StopCoroutine(currentTensiveTimeCoroutine);

        currentTensiveTimeCoroutine = StartCoroutine(StartTensiveTime());

        MonsterSpeedUpEvent(currentMainStoryIndex);
    }

    

    //give player a hidden chance to overcome chase 
    public void RandomDecreaseHitTimes()
    {
        if (Random.Range(0, 3) != 0)
        {
            if (monster.hitTimes > 0)
            {
                monster.hitTimes--;
            }
        }
    }

    public void CalculateDifficulty()
    {
        currentDifficulty = difficultyCurve.Evaluate(Mathf.Clamp(
            playerFailToScream / (playerFailToScream + playerSuccessToScream), 0, 1f));
    }

    //create tensive moment after player finding something important
    IEnumerator StartTensiveTime()
    {
        //yield return new WaitForSeconds(Random.Range(10f, 15f));
        yield return new WaitUntil(() => timeAfterChase >= 60 && currentMainStoryIndex<3);
        tensiveTime = true;
        canCountTimeAfterChase = false;
        Debug.Log("tensiveTime");
    }

    //calculate which is the route that is the most possible player destination for monster petrol
    public int CalculateRouteByPlayerDesiredDestination(Waypoints[] routes)
    {
        int pickRoute = 0;

        for (int i = 0; i < routes.Length; i++)
        {
            if (routes != null && playerPosCheck != null)
            {
                if (Vector3.Distance(playerPosCheck.position, routes[pickRoute].root.position) 
                    >= Vector3.Distance(playerPosCheck.position, routes[i].root.position))
                {
                    pickRoute = i;
                }
            }
        }
        return pickRoute;
    }



    public void AddSan(float num)
    {
        playerSan = Mathf.Clamp(playerSan + num, 0f, totalPlayerSan);
    }

    public void Guide()
    {
        if(!isInAGuide && !moveGuideFinished)
        {
            GuideUIController.instance.ShowGuideUI(moveGuideUI);
            currentGuideCoroutine = StartCoroutine(WaitMoveGuide());
        }

        if (!moveGuideFinished) return;

        if (!isInAGuide && !sanGuideFinished && canTriggerSanGuide)
        {
            GuideUIController.instance.ShowGuideUI(guideMask);
            GuideUIController.instance.ShowGuideUI(sanGuideUI);
            currentGuideCoroutine = StartCoroutine(WaitSanGuide());
        }

        if (!sanGuideFinished) return;

        if(!isInAGuide && !monsterGuideFinished &&canTriggerMonsterGuide&& monsterAppearTimelineFinished)
        {
            isInAGuide = true;
        }

        if (isInAGuide && !monsterGuideFinished)
        {
            if (bulletTime)
            {
                screamGuideUI.gameObject.SetActive(true);
                runUI.SetActive(false);
                Time.timeScale = 0.3f;
            }
            else
            {
                screamGuideUI.gameObject.SetActive(true);
                runUI.SetActive(false);
                Time.timeScale = 1f;
                //screamGuideUI.gameObject.SetActive(false);
                //runUI.SetActive(true);
                //Time.timeScale = 1f;
            }
        }

        if (guideScreamCount >= 6)
        {
            monsterGuideFinished = true;
            screamGuideUI.gameObject.SetActive(false);
            runUI.SetActive(false);
        }

        if (!monsterGuideFinished) return;

        hasFinishedGuide = true;
}

IEnumerator WaitMoveGuide()
    {
        isInAGuide = true;
        PlayerInput.inputBlock = true;
        yield return new WaitForSeconds(1f);
        StartCoroutine(WaitKey());
        yield return new WaitUntil(() => getKeyToHideGuideUI == true);
        getKeyToHideGuideUI = false;
        moveGuideUI.gameObject.SetActive(false);
        isInAGuide = false;
        moveGuideFinished = true;
        PlayerInput.inputBlock = false;
    }
    IEnumerator WaitSanGuide()
    {
        isInAGuide = true;
        PlayerInput.inputBlock = true;
        yield return new WaitForSeconds(1f);
        StartCoroutine(WaitKey());
        yield return new WaitUntil(() => getKeyToHideGuideUI == true);
        getKeyToHideGuideUI = false;
        guideMask.gameObject.SetActive(false);
        sanGuideUI.gameObject.SetActive(false);
        isInAGuide = false;
        sanGuideFinished = true;
        PlayerInput.inputBlock = false;
    }

    IEnumerator WaitKey()
    {
        yield return null;

            if (Keyboard.current.anyKey.isPressed)
            {
                if (!Keyboard.current.escapeKey.isPressed)
                {
                    getKeyToHideGuideUI = true;
                }
            }
        else
        {
            StartCoroutine(WaitKey());
        }

    }

    private void OnDestroy()
    {
        gameDataSpawner.SaveData(currentMainStoryIndex, playerSan, hasFinishedGuide);
    }

    public IEnumerator MainStoryStateCount(GameObject timeline)
    {
        
        PlayerInput.inputBlock = true;
        isInMainStoryTimeLine = true;
        timelineCamera.SetActive(true);
        timeline.SetActive (true);
        yield return new WaitForSeconds((float)timeline.GetComponent<PlayableDirector>().duration);
        isInMainStoryTimeLine = false;
        timeline.SetActive(false);
        timelineCamera.SetActive(false);
        PlayerInput.inputBlock = false;

        DealWithFinalSceneGateTimeline();
    }

    void DealWithFinalSceneGateTimeline()
    {
        if (currentMainStoryIndex >= mainStoryNum)
        {
            StartCoroutine(WaitFinalSceneGateTimeline());
        }
    }

    IEnumerator WaitFinalSceneGateTimeline()
    {
        yield return new WaitUntil(() => LinesManager.isPlayingLines == false);
        PlayerInput.inputBlock = true;
        SoundManager.Instance.PlayMainGateSound();
        timelineCamera.SetActive(true);
        finalSceneTimeline.SetActive(true);
        isInFinalSceneTimeLine = true;
        yield return new WaitForSeconds((float)finalSceneTimeline.GetComponent<PlayableDirector>().duration);
        PlayerInput.inputBlock = false;
        finalSceneGate.SetActive(true);
        timelineCamera.SetActive(false);
        Instance.isInFinalSceneTimeLine = false;
    }
}
