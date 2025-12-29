using UnityEngine;

public class PlayerInput
{
    #region Variables
    private readonly PlayerController ctx;
    public PlayerInput(PlayerController ctx)
    {
        this.ctx = ctx;
    }
    public Vector3 MoveDirection { get; private set; }
    public float MoveMagnitude { get; private set; }
    public bool SprintPressed { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool DashPressed { get; private set; }
    public bool IsCrouch { get; private set; }

    #endregion
    public void Tick()
    {
        Vector3 forward = ctx.Orientation.forward;
        Vector3 right = ctx.Orientation.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        MoveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
        MoveMagnitude = MoveDirection.magnitude;
        MoveDirection = MoveDirection.normalized;

        DashPressed = Input.GetKeyDown(KeyCode.LeftShift);
        JumpPressed = Input.GetKeyDown(KeyCode.Space);

        if (Input.GetKeyDown(KeyCode.LeftAlt))
            IsCrouch = !IsCrouch;

        if (Input.GetKeyDown(KeyCode.LeftControl))
            SprintPressed = !SprintPressed;
    }

}
