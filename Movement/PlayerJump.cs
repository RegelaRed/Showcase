using UnityEngine;

public class PlayerJump
{
    private float jumpStateTime = 0.5f;
    private float jumpTimer;
    public bool IsJumping => jumpTimer > 0;
    private PlayerController ctx;
    public PlayerJump(PlayerController ctx)
    {
        this.ctx = ctx;
    }

    public void Tick()
    {
        HandleJumpDuration();
    }

    public void StartJump()
    {
        jumpTimer = jumpStateTime;
        Vector3 vel = ctx.Rb.velocity;
        vel.y = 0;
        ctx.Rb.velocity = vel;
        ctx.Rb.AddForce(Vector3.up * ctx.PlayerVariables.jumpForce * 2, ForceMode.Impulse);
    }

    private void HandleJumpDuration()
    {
        if (jumpTimer <= 0)
            return;
        jumpTimer = Mathf.Max(0, jumpTimer - Time.deltaTime);
    }
}
