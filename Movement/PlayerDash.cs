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
    public bool IsOnCooldown => dashCooldownTimer > 0f;
    public int dashCharges { get; private set; } = 2;
    public bool HasCharges => dashCharges > 0;
    private float dashCooldown;


    public void Tick()
    {
        if (ctx.State.currentState == StateMachine.MovementState.Dash)
        {
            ctx.Dash.StartDash();
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
        if (dashCharges <= 0) return;

        dashCharges--;
        dashCooldown = ctx.PlayerVariables.dashCooldownTime;

        Vector3 dir = ctx.Input.moveMagnitude > 0.1f
            ? ctx.Input.moveDirection
            : ctx.Orientation.forward;

        ctx.Rb.AddForce(dir * ctx.PlayerVariables.dashForce, ForceMode.Impulse);
    }


}
