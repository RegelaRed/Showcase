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
        ctx.Rb.AddForce(Vector3.up * ctx.PlayerVariables.jumpForce * 2, ForceMode.Impulse);
        ctx.Rb.AddForce(ctx.Orientation.forward.normalized * 0.2f, ForceMode.Impulse);
    }
}
