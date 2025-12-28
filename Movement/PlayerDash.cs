using UnityEngine;


public class PlayerDash
{
    private readonly PlayerController ctx;
    public PlayerDash(PlayerController ctx)
    {
        this.ctx = ctx;
        dashCharges = ctx.PlayerVariables.maxDashCharges;
    }
    //dash settings

    private float dashCooldownTimer;
    private float dashCooldown;
    public bool IsOnCooldown => dashCooldownTimer > 0f;
    public int dashCharges { get; private set; } = 2;
    public bool HasCharges => dashCharges > 0;


    public void Tick()
    {
        if (ctx.State.CurrentState == StateMachine.MovementState.Dash && CanDash())
        {
            ctx.Dash.StartDash();
            dashCharges--;
            if (dashCooldownTimer <= 0f) dashCooldownTimer = ctx.PlayerVariables.dashCooldownTime;
        }

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;
    }

    public bool CanDash()
    {
        return dashCharges > 0 && dashCooldownTimer <= 0f;
    }
    public void StartDash()
    {
        Vector3 dir = ctx.Input.MoveMagnitude > 0.1f ? ctx.Input.MoveDirection : ctx.Orientation.forward;

        ctx.Rb.AddForce(dir * ctx.PlayerVariables.dashForce, ForceMode.Impulse);
    }
}
