using UnityEngine;

public class PlayerMovementOld : MonoBehaviour
{
    #region Variables/references
    /// <summary>
    ///     fuckass reviewer
    ///     references
    ///     walk/sprint settings
    ///     jump settings
    ///     dash settings
    /// 
    ///     IsGrounded settings
    ///     IsWall settingd
    /// 
    /// </summary>
    [Header("References")]
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform cameraPoint;
    [SerializeField] private Transform playerBody;
    [SerializeField] private PlayerVariables playerVariables;
    [SerializeField] private LayerMask groundLayer;

    private enum MovementState
    {
        Air,
        Idle,
        Walk,
        Sprint,
        Crouch,
        Jump,
        Dash
    }
    private MovementState currentState = MovementState.Idle;

    [Header("Movement")]
    private Vector3 moveDirection;
    private float moveSpeed;
    private float moveMagnitude;
    private float curMaxSpeed;

    [Header("Dash")]
    private int dashCharges;
    private float dashCooldown;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 1.6f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float crouchCameraOffset = -0.6f;
    private float currentHeight;
    private float targetHeight;
    private float camDefaultY;

    [Header("Inputs")]
    private bool sprintInput;
    private bool jumpInput;
    private bool dashInput;

    [Header("State")]
    private bool isCrouch;
    private bool isGrounded;

    #endregion
    /// <summary>
    ///     code structure
    ///     update -> state runner
    ///     handleInpur, GroundCheck, ChangeState(OnEntrer, OnExit), Move functions
    ///     
    ///     undate handlers for -> Idle, Air, Walk, Sprint, Crouch, Jump, Dash
    /// </summary>
    /// 
    #region Updates
    private void Awake()
    {
        dashCharges = playerVariables.maxDashCharges;

        rigidBody.freezeRotation = true;

        camDefaultY = cameraPoint.localPosition.y;
        currentHeight = standHeight;
        targetHeight = standHeight;

    }
    private void Update()
    {
        IsGrounded();
        HandleInput();

        if (dashCharges < playerVariables.maxDashCharges)
        {
            dashCooldown -= Time.deltaTime;
            if (dashCooldown <= 0f)
            {
                dashCharges++;
                dashCooldown = playerVariables.dashRegenTime;
            }
        }
        if (HandleGlobalTransitions())
            return;

        switch (currentState)
        {
            case MovementState.Idle: IdleUpdate(); break;
            case MovementState.Air: AirUpdate(); break;
            case MovementState.Walk: WalkUpdate(); break;
            case MovementState.Sprint: SprintUpdate(); break;
            case MovementState.Crouch: CrouchUpdate(); break;
            case MovementState.Jump: JumpUpdate(); break;
            case MovementState.Dash: DashUpdate(); break;
        }
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        CapSpeed();
        LerpHeight();
    }

    #endregion

    #region Inputs

    private void HandleInput()
    {
        //if input is 0 then idle
        Vector3 forward = orientation.forward;
        Vector3 right = orientation.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
        moveMagnitude = moveDirection.magnitude;
        moveDirection = moveDirection.normalized;

        dashInput = Input.GetKeyDown(KeyCode.LeftShift);
        jumpInput = Input.GetKeyDown(KeyCode.Space) && currentState != MovementState.Jump;

        if (Input.GetKeyDown(KeyCode.LeftAlt))
            isCrouch = !isCrouch;

        if (Input.GetKeyDown(KeyCode.LeftControl))
            sprintInput = !sprintInput;

    }

    #endregion
    #region  State Updates

    public void IsGrounded()
    {
        float groundCheckDist = currentHeight * 0.6f + 0.2f;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDist, groundLayer);
        //_isGrounded = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out _, groundCheckDist, _groundLayer);

        Debug.DrawRay(transform.position, Vector3.down * groundCheckDist, Color.yellow);
    }

    private void ChangeState(MovementState state)
    {
        if (state == currentState)
            return;
        OnExit(currentState);
        currentState = state;
        OnEnter(state);
    }

    private void OnEnter(MovementState state)
    {
        Debug.Log("current state :" + state);
        switch (state)
        {
            case MovementState.Idle:
                Vector3 v = rigidBody.velocity;
                rigidBody.velocity = new Vector3(v.x * 0.2f, v.y, v.z * 0.2f);
                break;
            case MovementState.Walk:
                curMaxSpeed = playerVariables.walkSpeed;
                moveSpeed = playerVariables.walkSpeed;
                break;

            case MovementState.Sprint:
                curMaxSpeed = playerVariables.sprintSpeed;
                moveSpeed = playerVariables.sprintSpeed;
                break;

            case MovementState.Jump:
                rigidBody.AddForce(Vector3.up * playerVariables.jumpForce * 2, ForceMode.Impulse);
                break;

            case MovementState.Dash:
                if (dashCharges > 0)
                {
                    dashCharges--;
                    dashCooldown = playerVariables.dashRegenTime;

                    Vector3 dashDir = moveMagnitude > 0.1f ? moveDirection : orientation.forward;
                    rigidBody.AddForce(dashDir.normalized * playerVariables.dashForce, ForceMode.Impulse);
                }
                break;

            case MovementState.Crouch:
                curMaxSpeed = crouchSpeed;
                moveSpeed = crouchSpeed;
                targetHeight = crouchHeight;
                break;
        }
    }
    private void OnExit(MovementState state)
    {
        switch (state)
        {
            case MovementState.Air: break;
            case MovementState.Idle: break;
            case MovementState.Walk: break;
            case MovementState.Sprint: break;
            case MovementState.Jump: break;
            case MovementState.Dash: break;
            case MovementState.Crouch: targetHeight = standHeight; break;
        }
    }
    #endregion
    #region Helper Functions
    /// <summary>
    /// handles jump/dash/air
    /// </summary>
    /// <returns></returns>
    private bool HandleGlobalTransitions()
    {
        if (dashInput && dashCharges > 0)
        {
            //dash
            ChangeState(MovementState.Dash);
            return true;
        }
        if (!isGrounded)
        {
            //set to air
            ChangeState(MovementState.Air);
            return true;
        }
        if (jumpInput && isGrounded)
        {
            //jump
            ChangeState(MovementState.Jump);
            return true;
        }
        if (moveMagnitude < 0.1f)
        {
            ChangeState(MovementState.Idle);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Handles idle/sprint/walk transitions
    /// </summary>
    ///<returns></returns>
    private void ResolveState()
    {
        if (moveMagnitude < 0.1f)
            ChangeState(MovementState.Idle);

        else if (sprintInput)
            ChangeState(MovementState.Sprint);

        else
            ChangeState(MovementState.Walk);
    }

    #endregion

    #region Speed/Collision





    private void LerpHeight()
    {
        //changes player height for crouch state
        currentHeight = Mathf.Lerp(
                currentHeight,
                targetHeight,
                Time.fixedDeltaTime * 10f);

        playerBody.localScale = new Vector3(1f, currentHeight / standHeight, 1f);

        cameraPoint.localPosition = new Vector3(
                cameraPoint.localPosition.x,
                camDefaultY + (currentHeight < standHeight ? crouchCameraOffset : 0f),
                cameraPoint.localPosition.z);
    }
    private void CapSpeed()
    {
        //limits maximum velocity
        Vector3 vel = rigidBody.velocity;
        Vector3 horizontal = new Vector3(vel.x, 0f, vel.z);

        if (horizontal.magnitude > curMaxSpeed)
        {
            Vector3 capped = horizontal.normalized * curMaxSpeed;
            rigidBody.velocity = new Vector3(capped.x, vel.y, capped.z);
        }
    }

    private void ApplyMovement()
    {
        //enable/dissable movement, and is called from FixedUpdate
        if (currentState == MovementState.Walk || currentState == MovementState.Sprint ||
            currentState == MovementState.Air || currentState == MovementState.Crouch ||
            currentState == MovementState.Jump)
        {
            Move();
        }
    }
    private void Move()
    {
        //applys rb.addforce based on state
        if (moveMagnitude < 0.1f)
            return;

        if (isGrounded)
        {
            rigidBody.AddForce(moveDirection * moveSpeed * 0.8f, ForceMode.VelocityChange);
            return;
        }
        else
        {
            Vector3 airDir = moveDirection;

            if (Physics.Raycast(transform.position, moveDirection, out RaycastHit hit, 0.4f))
            {
                airDir = Vector3.ProjectOnPlane(moveDirection, hit.normal);
            }
            rigidBody.AddForce(airDir * moveSpeed * 0.5f, ForceMode.VelocityChange);
        }

    }

    #endregion

    #region State Updates

    private void AirUpdate()
    {
        if (isGrounded)
            ResolveState();
    }

    private void IdleUpdate()
    {
        if (moveMagnitude > 0.1f)
        {
            ChangeState(sprintInput ? MovementState.Sprint : MovementState.Walk);
            return;
        }
    }
    private void WalkUpdate()
    {
        if (sprintInput)
            ChangeState(MovementState.Sprint);
    }

    private void SprintUpdate()
    {
        if (isCrouch)
        {
            ChangeState(MovementState.Crouch);
            return;
        }

        if (!sprintInput)
        {
            ChangeState(MovementState.Walk);
            return;
        }

        if (moveMagnitude < 0.1f)
        {
            ChangeState(MovementState.Idle);
            return;
        }
    }
    private void JumpUpdate()
    {
        ChangeState(MovementState.Air);
    }


    private void DashUpdate()
    {
        dashCooldown -= Time.deltaTime;
        if (dashCooldown > 0f)
            return;

        if (isGrounded)
            ResolveState();
        else
            ChangeState(MovementState.Air);
    }
    private void CrouchUpdate()
    {
        if (!isCrouch)
            ResolveState();
    }
    #endregion
}
