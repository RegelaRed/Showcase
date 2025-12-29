using UnityEngine;

public class PlayerMovement
{
    #region Variables
    private PlayerController ctx;
    public PlayerMovement(PlayerController ctx)
    {
        this.ctx = ctx;
    }
    private float currentMoveSpeed;

    #endregion
    #region Public Functions
    public void SetWalk()
    {
        currentMoveSpeed = ctx.PlayerVariables.walkSpeed;
    }
    public void SetSprint()
    {
        currentMoveSpeed = ctx.PlayerVariables.sprintSpeed;
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
            //movement when player is on ground
            ctx.Rb.AddForce(movDir * currentMoveSpeed * 0.8f, ForceMode.VelocityChange);
            return;
        }
        //movement when player is not on ground
        ctx.Rb.AddForce(movDir * currentMoveSpeed * 0.05f, ForceMode.VelocityChange);
    }

    public void StopMove()
    {
        if (!ctx.Ground.IsGrounded)
            return;
        ctx.Rb.velocity = new Vector3(0, ctx.Rb.velocity.y, 0);
    }
    #endregion

    private void CapSpeed()
    {
        //limits maximum velocity
        Vector3 vel = ctx.Rb.velocity;
        Vector3 horizontal = new Vector3(vel.x, 0f, vel.z);

        if (horizontal.magnitude > currentMoveSpeed)
        {
            Vector3 capped = horizontal.normalized * currentMoveSpeed;
            ctx.Rb.velocity = new Vector3(capped.x, vel.y, capped.z);
        }
    }
}
