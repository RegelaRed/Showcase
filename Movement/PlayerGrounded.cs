using UnityEngine;

public class PlayerGrounded
{
    private readonly PlayerController ctx;
    public PlayerGrounded(PlayerController ctx)
    {
        this.ctx = ctx;
        _currentHeight = ctx.StandHeight;
    }
    private bool isGrounded;
    public bool _isGrounded => isGrounded;
    private float _currentHeight;

    public void Tick()
    {
        float groundCheckDist = _currentHeight * 0.6f + 0.2f;
        isGrounded = Physics.Raycast(ctx.Rb.position, Vector3.down, groundCheckDist, ctx.PlayerVariables.groundLayer);

        Debug.DrawRay(ctx.Rb.position, Vector3.down * groundCheckDist, Color.yellow);
    }
}
