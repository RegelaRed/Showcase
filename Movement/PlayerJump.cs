using UnityEngine;

public class PlayerJump
{
    private PlayerController ctx;
    public PlayerJump(PlayerController ctx)
    {
        this.ctx = ctx;
    }
    public void StartJump()
    {
        Vector3 vel = ctx.Rb.velocity;
        vel.y = 0;
        ctx.Rb.velocity = vel;
        ctx.Rb.AddForce(Vector3.up * ctx.PlayerVariables.jumpForce * 2, ForceMode.Impulse);
    }
}
