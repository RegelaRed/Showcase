using UnityEngine;

public class PlayerPhysicsUpdate
{
    private PlayerController ctx;
    public PlayerPhysicsUpdate(PlayerController ctx)
    {
        this.ctx = ctx;
    }
    private float currentMoveSpeed;
    private float currentMaxSpeed;
    public void SetWalk()
    {
        currentMoveSpeed = ctx.PlayerVariables.walkSpeed;
        currentMaxSpeed = ctx.PlayerVariables.maxWalkSpeed;
    }
    public void SetSprint()
    {
        currentMoveSpeed = ctx.PlayerVariables.sprintSpeed;
        currentMaxSpeed = ctx.PlayerVariables.maxSprintSpeed;
    }
    public void StopMove()
    {
        ctx.Rb.velocity = new Vector3(0, ctx.Rb.velocity.y, 0);
    }
    public void Tick()
    {
        CapSpeed();

        //applys rb.addforce based on state
        if (ctx.Input.MoveMagnitude < 0.1f)
            return;

        Vector3 movDir = ctx.Input.MoveDirection;

        if (ctx.Ground.IsGrounded)
        {
            ctx.Rb.AddForce(movDir * currentMoveSpeed * 0.8f, ForceMode.VelocityChange);
        }
        else
        {
            ctx.Rb.AddForce(movDir * currentMoveSpeed * 0.05f, ForceMode.VelocityChange);
        }
    }
    private void CapSpeed()
    {
        //limits maximum velocity
        Vector3 vel = ctx.Rb.velocity;
        Vector3 horizontal = new Vector3(vel.x, 0f, vel.z);

        if (horizontal.magnitude > currentMaxSpeed)
        {
            Vector3 capped = horizontal.normalized * currentMaxSpeed;
            ctx.Rb.velocity = new Vector3(capped.x, vel.y, capped.z);
        }
    }
}
