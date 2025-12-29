using UnityEngine;

public class PlayerGrounded
{
    #region Variables
    private readonly PlayerController ctx;
    public PlayerGrounded(PlayerController ctx)
    {
        this.ctx = ctx;
        CurrentHeight = ctx.StandHeight;
    }
    private bool isGrounded;
    public bool IsGrounded => isGrounded;
    private float CurrentHeight;
    #endregion
    public void Tick()
    {
        float groundCheckDist = CurrentHeight * 0.6f;
        isGrounded = Physics.Raycast(ctx.Rb.position, Vector3.down, groundCheckDist, ctx.PlayerVariables.groundLayer);

        Debug.DrawRay(ctx.Rb.position, Vector3.down * groundCheckDist, Color.yellow);
    }
}
