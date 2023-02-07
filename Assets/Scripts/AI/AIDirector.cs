using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    private static AIDirector instance;
    public static AIDirector Instance { get { return instance; } private set { } }

    public AIMonsterController monster;
    public PlayerController player;
    public float totalPlayerSan = 300f;
    public int mainStoryNum;

    public static bool isGameOver=false;
    public static float playerSan;


    public int currentMainStoryIndex=0;
    protected Coroutine currentTensiveTimeCoroutine;
    public bool tensiveTime;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance==null)
        instance = this;
        isGameOver = false;
    }

    private void Start()
    {
        playerSan = totalPlayerSan;
    }
    // Update is called once per frame
    void Update()
    {
        if (playerSan > 0f)
        {
            playerSan -= Time.deltaTime;
        }
        else
        {
            isGameOver = true;
        }

        if (isGameOver)
        {
           Debug.Log("You lose");
            Time.timeScale = 0;
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
        yield return new WaitForSeconds(Random.Range(50f, 100f));
        Debug.Log("Monster Raid");
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

        for (int i = 0; i < routes.Length - 1; i++)
        {
            if (Vector3.Distance(player.transform.position, routes[i].root.position) > Vector3.Distance(player.transform.position, routes[i + 1].root.position))
            {
                pickRoute = i + 1;
            }
        }

        return pickRoute;
    }

    public void AddSan(float num)
    {
        playerSan = Mathf.Clamp(playerSan + num, 0f, totalPlayerSan);
    }
}
