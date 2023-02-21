using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class AIDirector : MonoBehaviour
{
    private static AIDirector instance;
    public static AIDirector Instance { get { return instance; } private set { } }

    [Header("Monster Settings")]
    public AIMonsterController monster;
    [Header("Player Settings")]
    public PlayerController player;
    public Transform playerPosCheck;
    public float startPlayerSan = 150f;
    public float totalPlayerSan = 300f;
    [Header("Story Settings")]
    public int mainStoryNum;
    public GameObject finalSceneGate;
    public GameObject finalSceneTimeline;

    [System.NonSerialized] public bool onBeingCatched;
    [System.NonSerialized] public bool onCatchingState;
    [System.NonSerialized] public static bool isGameOver = false;
    [System.NonSerialized] public static float playerSan;


    public int currentMainStoryIndex = 0;
    public bool tensiveTime;
    public bool isInMainStoryTimeLine;
    public bool isInFinalSceneTimeLine;

    protected Coroutine currentTensiveTimeCoroutine;
    protected bool hasRespawn;

    [Header("Guide Settings")]
    public bool playerInGuide;
    public bool hasFinishedGuide;
    public CanvasGroup guideMask;
    public CanvasGroup moveGuideUI;
    public CanvasGroup sanGuideUI;
    public CanvasGroup screamGuideUI;
    public GameObject runUI;

    public bool isInAGuide;

    protected bool moveGuideFinished;

    public bool canTriggerSanGuide;
    protected bool sanGuideFinished;

    public bool canTriggerMonsterGuide;
    public Vector3 monsterSpawnPos;
    public bool monsterGuideFinished;
    public bool bulletTime;
    public int guideScreamCount;

    protected Coroutine currentGuideCoroutine;
    protected bool getKeyToHideGuideUI;

    [Header("Data Settings")]
    public GameObject initializer;
    public GameDataSpawner gameDataSpawner;

    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 1;
        playerSan = startPlayerSan;

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
        }
        hasFinishedGuide = gameDataSpawner.GetHasFinishedGuide();
        currentMainStoryIndex = gameDataSpawner.GetCurrentMainStoryIndex();
        playerSan = gameDataSpawner.GetPlayerSan();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (instance == null)
            instance = this;
        isGameOver = false;
        hasRespawn = false;
        onBeingCatched = false;
        onCatchingState = false;
        isInMainStoryTimeLine = false;
        isInFinalSceneTimeLine = false;



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
        //if (Keyboard.current.escapeKey.isPressed)
        //{
        //    isGameOver = true;
        //}
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
            isGameOver = true;
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

    }

    //create tensive moment after player finding something important
    IEnumerator StartTensiveTime()
    {
        yield return new WaitForSeconds(Random.Range(10f, 15f));
        tensiveTime = true;
    }

    //give player a hidden chance to overcome chase 
    public void RandomDecreaseHitTimes()
    {
        if (UnityEngine.Random.Range(0, 3) != 0)
        {
            if (monster.hitTimes > 0)
            {
                monster.hitTimes--;
            }
        }
    }

    //calculate which is the route that is the most possible player destination for monster petrol
    public int CalculateRouteByPlayerDesiredDestination(Waypoints[] routes)
    {
        int pickRoute = 0;

        for (int i = 0; i < routes.Length; i++)
        {
            if (routes != null && playerPosCheck != null)
            {
                if (Vector3.Distance(playerPosCheck.position, routes[pickRoute].root.position) >= Vector3.Distance(playerPosCheck.position, routes[i].root.position))
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

        if(!isInAGuide && !monsterGuideFinished &&canTriggerMonsterGuide)
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
                screamGuideUI.gameObject.SetActive(false);
                runUI.SetActive(true);
                Time.timeScale = 1f;

            }
        }

        if (guideScreamCount == 2)
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
}
