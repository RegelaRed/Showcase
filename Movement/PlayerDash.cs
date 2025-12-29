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
        regenTimer = ctx.PlayerVariables.dashRegenTime;
        
        dashTimer = ctx.PlayerVariables.dashDuration;
        isDashing = true;

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
    private void HandleDashRegen(float dt)
    {
        if (DashCharges >= ctx.PlayerVariables.maxDashCharges)
            return;

        regenTimer = Mathf.Max(0, regenTimer - dt);

        if (regenTimer <= 0)
        {
            DashCharges++;
            regenTimer = ctx.PlayerVariables.dashRegenTime;
        }
    }
    private void HandleDashCooldown(float dt)
    {
        if (!isDashing)
            return;

        dashTimer = Mathf.Max(0, dashTimer - dt);

        if (dashTimer <= 0)
            isDashing = false;
    }
    #endregion
}
