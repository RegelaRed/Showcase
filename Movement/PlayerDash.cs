using UnityEngine;


public class PlayerDash
{
    private readonly PlayerController ctx;

    //dash settings

    private float dashCooldownTimer;
    public bool IsOnCooldown => dashCooldownTimer > 0f;
    public int dashCharges { get; private set; }
    public bool HasCharges => dashCharges > 0;
    private float dashCooldown;
    public PlayerDash(PlayerController ctx)
    {
        this.ctx = ctx;
        dashCharges = ctx.PlayerVariables.maxDashCharges;
    }

    public void Update()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;
    }

    public bool CanDash()
    {
        return dashCharges > 0 && dashCooldownTimer <= 0f;
    }

    public void StartDash(Vector3 direction)
    {
        if (!CanDash())
            return;

        dashCharges--;
        dashCooldownTimer = ctx.PlayerVariables.dashCooldownTime;

        ctx.Rb.AddForce(
            direction.normalized * ctx.PlayerVariables.dashForce,
            ForceMode.Impulse
        );
    }

}
