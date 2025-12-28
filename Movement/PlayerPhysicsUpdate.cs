using UnityEngine;

public class PlayerPhysicsUpdate
{
    private PlayerController ctx;
    public PlayerPhysicsUpdate(PlayerController ctx)
    {
        this.ctx = ctx;
    }
    private float currentMoveSpeed;
    public void SetWalk()
    {
        currentMoveSpeed = ctx.PlayerVariables.walkSpeed;
    }
    public void SetSprint()
    {
        currentMoveSpeed = ctx.PlayerVariables.sprintSpeed;
    }
    public void Update()
    {
        //applys rb.addforce based on state
        if (ctx.Input.moveMagnitude < 0.1f)
            return;

        Vector3 movDir = ctx.Input.moveDirection;

        if (ctx.Ground._isGrounded)
        {
            ctx.Rb.AddForce(movDir * currentMoveSpeed * 0.8f, ForceMode.VelocityChange);
            return;
        }
        else
        {
            Vector3 airDir = ctx.Input.moveDirection;

            if (Physics.Raycast(ctx.transform.position, movDir, out RaycastHit hit, 0.4f))
            {
                airDir = Vector3.ProjectOnPlane(movDir, hit.normal);
            }
            ctx.Rb.AddForce(airDir * currentMoveSpeed * 0.5f, ForceMode.VelocityChange);
        }
    }
}
