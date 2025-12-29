using UnityEngine;

public class StateMachine
{
    #region Variables
    private readonly PlayerController ctx;
    public StateMachine(PlayerController ctx)
    {
        this.ctx = ctx;
    }
    public MovementState CurrentState { get; private set; } = MovementState.Idle;

    #endregion
    #region Public Methods
    public void Tick()
    {
        if (HandleGlobalTransitions())
            return;

        switch (CurrentState)
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
        if (state == CurrentState)
            return;
        //OnExit(CurrentState);

        var prevState = CurrentState;
        CurrentState = state;
        Debug.Log("Entering New State from " + prevState + " to " + state);

        OnEnter(state);
    }
    #endregion
    #region State Lifecycle
    private void OnEnter(MovementState state)
    {
        switch (state)
        {
            case MovementState.Idle:
                ctx.Movement.StopMove();
                break;

            case MovementState.Walk:
                ctx.Movement.SetWalk();
                break;

            case MovementState.Sprint:
                ctx.Movement.SetSprint();
                break;

            case MovementState.Crouch:
                break;

            case MovementState.Jump:
                ctx.Jump.StartJump();
                break;

            case MovementState.Dash:
                ctx.Dash.StartDash();
                break;
        }

    }
    private void OnExit(MovementState state) { }

    #endregion
    #region Helper Functions

    /// <summary>
    /// handles jump/dash/air transitions
    /// </summary>
    /// <returns></returns>
    private bool HandleGlobalTransitions()
    {
        if (CurrentState == MovementState.Dash || CurrentState == MovementState.Jump)
            return false;

        if (ctx.Input.DashPressed && ctx.Dash.DashCharges > 0)
        {
            //dash
            ChangeState(MovementState.Dash);
            return true;
        }
        if (!ctx.Ground.IsGrounded)
        {
            //set to air
            ChangeState(MovementState.Air);
            return true;
        }
        if (ctx.Input.JumpPressed && ctx.Ground.IsGrounded)
        {
            ChangeState(MovementState.Jump);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Handles idle/sprint/walk transitions
    /// </summary>
    ///<returns></returns>
    private void ResolveState()
    {
        if (ctx.Input.MoveMagnitude < 0.1f)
            ChangeState(MovementState.Idle);

        else if (ctx.Input.SprintPressed)
            ChangeState(MovementState.Sprint);

        else
            ChangeState(MovementState.Walk);
    }

    #endregion
    #region State Updates
    private void AirUpdate()
    {
        if (ctx.Ground.IsGrounded)
            ResolveState();
    }

    private void IdleUpdate()
    {
        if (ctx.Input.MoveMagnitude > 0.1f)
        {
            ChangeState(ctx.Input.SprintPressed ? MovementState.Sprint : MovementState.Walk);
            return;
        }
    }
    private void WalkUpdate()
    {
        if (ctx.Input.SprintPressed)
            ChangeState(MovementState.Sprint);

        if (ctx.Input.MoveMagnitude < 0.1f)
            ChangeState(MovementState.Idle);


    }

    private void SprintUpdate()
    {
        if (ctx.Input.IsCrouch)
        {
            ChangeState(MovementState.Crouch);
            return;
        }

        if (!ctx.Input.SprintPressed)
        {
            ChangeState(MovementState.Walk);
            return;
        }

        if (ctx.Input.MoveMagnitude < 0.1f)
        {
            ChangeState(MovementState.Idle);
            return;
        }
    }

    private void JumpUpdate()
    {
        if (!ctx.Jump.IsJumping)
            ResolveState();
    }
    private void DashUpdate()
    {
        if (!ctx.Dash.IsDashing)
            ResolveState();
    }

    private void CrouchUpdate()
    {
        if (!ctx.Input.IsCrouch)
            ResolveState();
    }
    #endregion
}
