using UnityEngine;

public class PlayerJump
{
    private PlayerController ctx;
    public PlayerJump(PlayerController ctx)
    {
        this.ctx = ctx;
    }
    public void Execute()
    {
        ctx.Rb.AddForce(Vector3.up * ctx.PlayerVariables.jumpForce * 2, ForceMode.Impulse);
    }
}
