using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    private static AIDirector instance;
    public static AIDirector Instance { get { return instance; } private set { } }

    [Header("Monster Settings")]
    public AIMonsterController monster;
    [Header("Player Settings")]
    public PlayerController player;
    public Transform playerPosCheck;
    public float totalPlayerSan = 300f;
    [Header("Story Settings")]
    public int mainStoryNum;
    public GameObject finalSceneGate;
    public GameObject finalSceneTimeline;

    [System.NonSerialized] public bool onBeingCatched;
    [System.NonSerialized] public bool onCatchingState;
    [System.NonSerialized]public static bool isGameOver=false;
    [System.NonSerialized] public static float playerSan;


    public int currentMainStoryIndex=0;
    public bool tensiveTime;
    public bool isInMainStoryTimeLine;
    public bool isInFinalSceneTimeLine;

    protected Coroutine currentTensiveTimeCoroutine;
    protected bool hasRespawn;

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (instance==null)
        instance = this;
        isGameOver = false;
        hasRespawn = false;
        onBeingCatched = false;
        onCatchingState = false;
        isInMainStoryTimeLine = false;
        isInFinalSceneTimeLine=false;
    }
    private void Start()
    {
        playerSan = totalPlayerSan;

        GuideUIController.instance.ShowGuideUI(GuideUIController.instance.guideUI[0]);
    }
    // Update is called once per frame
    void Update()
    {
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
            if(routes!=null && playerPosCheck != null)
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
}
