using UnityEngine;


public class PlayerDash
{
    #region Variables
    private float dashTimer;
    private bool isDashing;

    public int DashCharges { get; private set; }
    private float regenTimer;

    public bool IsDashing => isDashing;
    public bool CanDash => !isDashing && DashCharges > 0;

    private readonly PlayerController ctx;
    public PlayerDash(PlayerController ctx)
    {
        this.ctx = ctx;
        DashCharges = ctx.PlayerVariables.maxDashCharges;
        regenTimer = ctx.PlayerVariables.dashRegenTime;
    }
    #endregion
    #region Public Functions
    public void StartDash()
    {
        if (!CanDash)
            return;

        DashCharges--;

        isDashing = true;
        dashTimer = ctx.PlayerVariables.dashDuration;

        Vector3 dir = ctx.Orientation.forward;
        ctx.Rb.velocity = Vector3.zero;
        ctx.Rb.AddForce(dir * ctx.PlayerVariables.dashForce, ForceMode.VelocityChange);
    }
    public void Tick()
    {
        float dt = Time.deltaTime;
        HandleDashCooldown(dt);
        HandleDashRegen(dt);
    }
    #endregion
    #region Private Functions
    private void HandleDashCooldown(float dt)
    {
        if (!isDashing)
            return;

        dashTimer -= dt;

        if (dashTimer <= 0)
        {
            EndDash();
        }
    }
    private void HandleDashRegen(float dt)
    {
        if (DashCharges >= ctx.PlayerVariables.maxDashCharges)
            return;

        regenTimer -= dt;
        if (regenTimer <= 0)
        {
            DashCharges++;
            regenTimer = ctx.PlayerVariables.dashRegenTime;
        }
    }
    public void EndDash()
    {
        isDashing = false;
        if (ctx.Ground.IsGrounded)
            ctx.State.ChangeState(MovementState.Sprint);
        else
            ctx.State.ChangeState(MovementState.Air);
    }
    #endregion
}
