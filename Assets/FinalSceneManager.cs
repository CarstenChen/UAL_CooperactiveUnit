using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FinalSceneManager : MonoBehaviour
{
    private static FinalSceneManager instance;
    public static FinalSceneManager Instance { get { return instance; } private set { } }

    public Material playerBodyMtl;
    public Material monsterBodyMtl;

    [SerializeField] protected int winKeyNum;

    [SerializeField]protected float coolDown = 5f;
    [SerializeField] protected float currentCoolDown;
    [SerializeField] protected bool canPress = false;

    public bool autoWriting;
    protected float currentKeyNum;
    public float winRate;

    public delegate void WinEvent();
    public static event WinEvent emittedObjectEvent;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        winRate = Mathf.Clamp( currentKeyNum / winKeyNum,0,1);

        if (winRate >= 1)
        {
            emittedObjectEvent();
        }

        //model dissolve
        Vector3 offset = new Vector3(0, 2 - 4 * winRate, 0);
        playerBodyMtl.SetVector("_DissolveOffset", offset);
        monsterBodyMtl.SetVector("_DissolveOffset", offset);

        if (autoWriting && winRate <1)
        {
            //type in
            DealWithAutoWriting();
            DealWithSystemForce();
        }

        Debug.Log(PlayerInput.inputBlock);
    }

    void DealWithAutoWriting()
    {
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
        }
    }

    void DealWithSystemForce()
    {
        if(Random.Range(0,25)==0)
        currentKeyNum=Mathf.Clamp(currentKeyNum- Random.Range(1, 3), 0,winKeyNum);
        
    }
}
