using UnityEngine;

public class PlayerInput
{
    private readonly PlayerController ctx;
    public PlayerInput(PlayerController ctx)
    {
        this.ctx = ctx;
    }
    public Vector3 moveDirection { get; private set; }
    public float moveMagnitude { get; private set; }

    public bool sprintInput { get; private set; }
    public bool jumpInput { get; private set; }
    public bool dashInput { get; private set; }
    public bool isCrouch { get; private set; }



    public void Tick()
    {
        //if input is 0 then idle
        Vector3 forward = ctx.Orientation.forward;
        Vector3 right = ctx.Orientation.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
        moveMagnitude = moveDirection.magnitude;
        moveDirection = moveDirection.normalized;

        dashInput = Input.GetKeyDown(KeyCode.LeftShift);
        jumpInput = Input.GetKeyDown(KeyCode.Space);

        if (Input.GetKeyDown(KeyCode.LeftAlt))
            isCrouch = !isCrouch;

        if (Input.GetKeyDown(KeyCode.LeftControl))
            sprintInput = !sprintInput;
    }

}
