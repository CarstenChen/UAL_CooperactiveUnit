using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    private static AIDirector instance;
    public static AIDirector Instance { get { return instance; } private set { } }

    [SerializeField] protected AIMonsterController monster;
    protected int currentMainStoryIndex;
    protected Coroutine currentTensiveTimeCoroutine;

    public bool tensiveTime;



    // Start is called before the first frame update
    void Awake()
    {
        if(instance==null)
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInput.pi_Instance.TestInput1)
        {
            currentMainStoryIndex++;
            Debug.Log("Player has read main story");

            if(currentTensiveTimeCoroutine!=null)
            StopCoroutine(currentTensiveTimeCoroutine);
            currentTensiveTimeCoroutine = StartCoroutine(StartTensiveTime());            
        }
    }
    IEnumerator StartTensiveTime()
    {
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        Debug.Log("Monster Raid");
        tensiveTime = true;
    }
}
