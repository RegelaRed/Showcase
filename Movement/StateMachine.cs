using UnityEngine;

public class StateMachine
{
    private readonly PlayerController ctx;
    public StateMachine(PlayerController ctx)
    {
        this.ctx = ctx;
    }
    public enum MovementState
    {
        Air,
        Idle,
        Walk,
        Sprint,
        Crouch,
        Jump,
        Dash
    }
    public MovementState currentState { get; private set; } = MovementState.Idle;

    public void Update()
    {
        switch (currentState)
        {
            case MovementState.Idle: IdleUpdate(); break;
            case MovementState.Air: AirUpdate(); break;
            case MovementState.Walk: WalkUpdate(); break;
            case MovementState.Sprint: SprintUpdate(); break;
            case MovementState.Crouch: CrouchUpdate(); break;
            case MovementState.Jump: JumpUpdate(); break;
            case MovementState.Dash: DashUpdate(); break;
        }
    }

    public void ChangeState(MovementState state)
    {
        if (state == currentState)
            return;
        OnExit(currentState);
        currentState = state;
        OnEnter(state);
    }
    private void OnEnter(MovementState state)
    {
        Debug.Log("Current State " + currentState);

        switch (state)
        {
            case MovementState.Idle:

                break;
            case MovementState.Walk:
                ctx.Move.SetWalk();
                break;
            case MovementState.Sprint:
                ctx.Move.SetSprint();
                break;
            case MovementState.Crouch:
                break;
            case MovementState.Jump:
                break;
            case MovementState.Dash:
                Vector3 dashDir = ctx.Input.moveMagnitude > 0.1f ? ctx.Input.moveDirection : ctx.Orientation.forward;
                ctx.Dash.StartDash(dashDir);
                break;

        }
    }
    private void OnExit(MovementState state)
    {
        // if (HandleGlobalTransitions())
        //     return;
        switch (state)
        {
            case MovementState.Air: break;
            case MovementState.Idle: break;
            case MovementState.Walk: break;
            case MovementState.Sprint: break;
            case MovementState.Jump: break;
            case MovementState.Dash: break;
        }
    }

    #region Helper Functions
    /// <summary>
    /// handles jump/dash/air
    /// </summary>
    /// <returns></returns>
    private bool HandleGlobalTransitions()
    {
        if (ctx.Input.dashInput && ctx.Dash.dashCharges > 0)
        {
            //dash
            ChangeState(MovementState.Dash);
            return true;
        }
        if (!ctx.Ground._isGrounded)
        {
            //set to air
            ChangeState(MovementState.Air);
            return true;
        }
        if (ctx.Input.jumpInput && ctx.Ground._isGrounded)
        {
            //jump
            ChangeState(MovementState.Jump);
            ctx.Jump.Execute();
            return true;
        }
        if (ctx.Input.moveMagnitude < 0.1f)
        {
            ChangeState(MovementState.Idle);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Handles idle/sprint/walk transitions
    /// </summary>
    ///<returns></returns>
    public void ResolveState()
    {
        if (ctx.Input.moveMagnitude < 0.1f)
            ChangeState(MovementState.Idle);

        else if (ctx.Input.sprintInput)
            ChangeState(MovementState.Sprint);

        else
            ChangeState(MovementState.Walk);
    }

    #endregion


    #region State Updates

    private void AirUpdate()
    {
        if (ctx.Ground._isGrounded)
            ResolveState();
    }

    private void IdleUpdate()
    {
        if (HandleGlobalTransitions())
            return;

        if (ctx.Input.moveMagnitude > 0.1f)
        {
            ChangeState(ctx.Input.sprintInput ? MovementState.Sprint : MovementState.Walk);
            return;
        }
    }
    private void WalkUpdate()
    {
        if (HandleGlobalTransitions())
            return;

        if (ctx.Input.sprintInput)
            ChangeState(MovementState.Sprint);
    }

    private void SprintUpdate()
    {
        if (HandleGlobalTransitions())
            return;

        if (ctx.Input.isCrouch)
        {
            ChangeState(MovementState.Crouch);
            return;
        }

        if (!ctx.Input.sprintInput)
        {
            ChangeState(MovementState.Walk);
            return;
        }

        if (ctx.Input.moveMagnitude < 0.1f)
        {
            ChangeState(MovementState.Idle);
            return;
        }
    }
    private void JumpUpdate()
    {
        ResolveState();
    }


    private void DashUpdate()
    {
        ResolveState();
    }
    private void CrouchUpdate()
    {
        if (!ctx.Input.isCrouch)
            ResolveState();
    }
    #endregion
}
