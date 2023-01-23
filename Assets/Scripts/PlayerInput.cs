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
    protected bool pl_Attack;


    public Vector2 MoveInput
    {
        get
        {
            if (inputBlock) { return Vector2.zero; }
            return pl_MoveInput;
        }
    }
    public bool JumpInput { get { return pl_Jump && !inputBlock; } }
    public bool AttackInput { get { return pl_Attack && !inputBlock; } }

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
        pl_Jump = value.isPressed;
    }

    void OnFire(InputValue value)
    {
        //冲掉前一个输入，保持attack是true
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(SetAttackParameter());
    }

    IEnumerator SetAttackParameter()
    {
        pl_Attack = true;
        yield return new WaitForSeconds(attackInputInterval);
        pl_Attack = false;
    }
}
