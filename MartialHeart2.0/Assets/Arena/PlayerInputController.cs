using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : Player
{
    [Header("Input")]
    [SerializeField] Vector2 VectorMove;
    [SerializeField] float H;
    [SerializeField] float V;
    [Space]
    [SerializeField] bool blockHold;

    PlayerInputActions playerInputActions;

    public override void Awake()
    {
        base.Awake();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        configuration.Input.onActionTriggered += Input_onActionTriggered;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Walk();
    }

    public override void Update()
    {
        base.Update();
        BlockAction();
        ChangeState();
    }

    void Walk()
    {
        if (action == Action.MOVE)
            rb.velocity = new Vector2(H * walkSpeed, rb.velocity.y);
    }

    void BlockAction()
    {
        if (blockHold && (action == Action.MOVE || action == Action.BLOCK))
        {
            switch (state)
            {
                case State.VERTICAL:
                    anim.SetBool("BlockV", true);
                    anim.SetBool("BlockH", false);
                    break;

                case State.HORIZONTAL:
                    anim.SetBool("BlockH", true);
                    anim.SetBool("BlockV", false);
                    break;
            }
        }
    }

    void ChangeState()
    {
        if (action == Action.MOVE || action == Action.CLASHSTAY)
            if (V < 0)
                state = State.HORIZONTAL;
            else
                state = State.VERTICAL;
    }

    void Input_onActionTriggered(InputAction.CallbackContext context)
    {
        if (context.action.name == playerInputActions.Player.Movement.name)
        {
            Movement(context);
        }
        if (context.action.name == playerInputActions.Player.Attack.name)
        {
            Attack(context);
        }
        if (context.action.name == playerInputActions.Player.Block.name)
        {
            Block(context);
        }
        if (context.action.name == playerInputActions.Player.Pause.name)
        {
            Pause(context);
        }
    }

    public void Movement(InputAction.CallbackContext context)
    {
        VectorMove = context.ReadValue<Vector2>();
        H = VectorMove.x;
        V = VectorMove.y;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (action == Action.MOVE || action == Action.BLOCK)
            {
                atkTime = Time.time;

                switch (state)
                {
                    case State.VERTICAL:
                        anim.SetTrigger("AtkV");
                        break;

                    case State.HORIZONTAL:
                        anim.SetTrigger("AtkH");
                        break;
                }

                if ((isFacingRight && H > 0) || (!isFacingRight && H < 0))
                    StartCoroutine(Dash(H, dashSpeed, dashTime));
                else
                    rb.velocity = Vector2.zero;
            }

            if (action == Action.CLASHSTAY && !IsWalled)
            {
                atkTime = Time.time;

                switch (state)
                {
                    case State.VERTICAL:
                        anim.SetTrigger("AtkV");
                        break;

                    case State.HORIZONTAL:
                        anim.SetTrigger("AtkH");
                        break;
                }
                StartCoroutine(Dash(-face, clashBackDashSpeed, clashBackDashTime));
            }
        }
    }

    public void Block(InputAction.CallbackContext context)
    {
        if (context.performed)
            blockHold = true;

        if (context.canceled)
        {
            blockHold = false;
            anim.SetBool("BlockV", false);
            anim.SetBool("BlockH", false);
        }

        if (context.started && (action != Action.STOP && action != Action.HURT))
        {
            if (action == Action.CLASHSTAY && !opponent.IsWalled)
            {
                anim.SetTrigger("Push");
            }
        }

    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Pausa de jugador");
            if (action != Action.STOP)
                MatchManager.instance.TogglePause();
        }
    }

    public void RemoveInputs()
    {
        configuration.Input.onActionTriggered -= Input_onActionTriggered;

        playerInputActions.PauseMenu.Disable();
        playerInputActions.Player.Disable();

        Debug.Log("RemoveInputs() de" + name);
    }
}
