using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    //定义单件
    public static PlayerInput pi_Instance;
    public static PlayerInput Instance
    {
        get
        {
            return pi_Instance;
        }

        private set { }
    }

    public bool inputBlock;
    protected Vector2 pl_MoveInput;
    protected bool pl_Jump;
    protected bool pl_Scream;
    protected bool pl_Test1;


    public Vector2 MoveInput
    {
        get
        {
            if (inputBlock) { return Vector2.zero; }
            return pl_MoveInput;
        }
    }
    public bool JumpInput { get { return pl_Jump && !inputBlock; } }
    public bool ScreamInput { get { return pl_Scream && !inputBlock; } }

    public bool TestInput1 { get { return pl_Test1; } }

    protected const float attackInputInterval = 0.03f;
    protected Coroutine currentCoroutine;

    void Awake()
    {
        //初始化单件
        if (pi_Instance == null)
            pi_Instance = this;
        else if (pi_Instance != this)
        {
            throw new UnityException("There can not be more than one PlayerInput Scripts");
        }
    }

    void OnMove(InputValue value)
    {
        pl_MoveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        //pl_Jump = value.isPressed;
    }

    void OnScream (InputValue value)
    {
        //冲掉前一个输入，保持attack是true
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(SetAttackParameter());
    }

    void OnTestKey1 (InputValue value)
    {
        StartCoroutine(ResetTestButton());
    }

    IEnumerator SetAttackParameter()
    {
        pl_Scream = true;
        yield return new WaitForSeconds(attackInputInterval);
        pl_Scream = false;
    }

    IEnumerator ResetTestButton()
    {
        pl_Test1 = true;
        yield return new WaitForEndOfFrame();
        pl_Test1 = false;
    }
}
