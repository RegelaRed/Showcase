using UnityEngine;

public class PlayerGrounded
{
    private readonly PlayerController ctx;
    public PlayerGrounded(PlayerController ctx)
    {
        this.ctx = ctx;
    }
    private bool isGrounded;
    public bool _isGrounded => isGrounded;
    private float _currentHeight;
    public void Awake()
    {
        _currentHeight = ctx.StandHeight;
    }
    public void Update()
    {
        float groundCheckDist = _currentHeight * 0.6f + 0.2f;
        isGrounded = Physics.Raycast(ctx.Orientation.position, Vector3.down, groundCheckDist, ctx.PlayerVariables.groundLayer);

        Debug.DrawRay(ctx.Orientation.position, Vector3.down * groundCheckDist, Color.yellow);
    }
}
