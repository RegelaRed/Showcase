using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform Orientation;
    [SerializeField] private Transform cam_pos;

    [Header("Movement")]
    [SerializeField] public PlayerVariables PV;

    [Header("Local")]
    private Vector3 MoveDir = Vector3.zero;
    private float moveSpeed = 0;

    [Header("StateChecks")]
    private float dashTime = 0.2f;
    private float dashTimer = 0;
    bool _dashPressed = false;
    bool _sprintToggle = false;
    bool _jumpPressed = false;
    float moveMag = 0;

    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundLayer;
    private bool _isGrounded;
    private bool _wallCheck;
    private RaycastHit _wallHit;

    [Header("")]
    private float _maxWalkSpeed = 6;
    private float _maxSprintSpeed = 10;
    private float _curMaxSpeed;
    private int _maxDashCharges = 2;
    private float _dashCooldownTime = 1.5f;
    private int dashCharges;
    private float _dashCooldown;

    private enum States
    {
        Air,
        Idle,
        Walk,
        Sprint,
        Crouch,
        Jump,
        Dash
    }
    private States _state = States.Idle;

    /// <summary>
    ///     code structure
    ///     update -> state runner
    ///     handleInpur, GroundCheck, ChangeState( OnEntrer, OnExit), Move functions
    ///     
    ///     undate handlers for -> Idle, Air, Walk, Sprint, Crouch, Jump, Dash
    /// 
    /// </summary>

    private void Awake()
    {
        rb.freezeRotation = true;
    }
    private void Update()
    {
        IsGrounded();
        HandleInput();
        if (dashCharges < _maxDashCharges)
        {
            _dashCooldown -= Time.deltaTime;
            if (_dashCooldown <= 0f)
            {
                dashCharges++;
                _dashCooldown = _dashCooldownTime;
            }
        }
        if (_dashPressed && _state != States.Dash)
        {
            ChangeState(States.Dash);
            return;
        }

        switch (_state)
        {
            case States.Idle: IdleUpdate(); break;
            case States.Air: AirUpdate(); break;
            case States.Walk: WalkUpdate(); break;
            case States.Sprint: SprintUpdate(); break;
            case States.Crouch: break;
            case States.Jump: JumpUpdate(); break;
            case States.Dash: DashUpdate(); break;
        }
    }
    private void FixedUpdate()
    {
        CapSpeed();
    }

    //helper Functions
    private void HandleInput()
    {
        //if input is 0 then idle
        Vector3 forward = Orientation.forward;
        Vector3 right = Orientation.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        MoveDir = (forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal")).normalized;
        moveMag = MoveDir.magnitude;

        _dashPressed = Input.GetKeyDown(KeyCode.LeftShift);
        _jumpPressed = Input.GetKeyDown(KeyCode.Space) && _state != States.Jump;


        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _sprintToggle = !_sprintToggle;
        }
    }

    private void ChangeState(States state)
    {
        if (state == _state)
            return;
        OnExit(_state);
        _state = state;
        OnEnter(state);
    }

    private void OnEnter(States state)
    {
        Debug.Log("current state" + state);
        switch (state)
        {
            case States.Idle:
                Vector3 v = rb.velocity;
                rb.velocity = new Vector3(v.x * 0.2f, v.y, v.z * 0.2f);
                //rb.drag = 8f;
                break;
            case States.Walk:
                _curMaxSpeed = _maxWalkSpeed;
                moveSpeed = PV._walkSpeed;
                break;

            case States.Sprint:
                _curMaxSpeed = _maxSprintSpeed;
                moveSpeed = PV._sprintSpeed;
                break;

            case States.Jump:
                rb.AddForce(Vector3.up * PV._jumpForce * 2, ForceMode.Impulse);
                break;

            case States.Dash:
                if (dashCharges > 0)
                {
                    dashCharges--;
                    _dashCooldown = _dashCooldownTime;
                    dashTimer = dashTime;
                    Vector3 dashDir = moveMag > 0.1f ? MoveDir : Orientation.forward;
                    rb.AddForce(dashDir.normalized * PV._dashForce, ForceMode.Impulse);
                }
                break;
        }
    }
    private void OnExit(States state)
    {
        switch (state)
        {
            case States.Air: break;
            case States.Idle:
                // rb.drag = 0f;
                break;
            case States.Walk: break;
            case States.Sprint: break;
            case States.Jump: break;
            case States.Dash: break;
            case States.Crouch: break;
        }
    }

    private void CapSpeed()
    {
        Vector3 vel = rb.velocity;
        Vector3 horizontal = new Vector3(vel.x, 0f, vel.z);

        if (horizontal.magnitude > _curMaxSpeed)
        {
            Vector3 capped = horizontal.normalized * _curMaxSpeed;
            rb.velocity = new Vector3(capped.x, vel.y, capped.z);
        }
    }

    public void IsGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.5f, _groundLayer);
    }

    public void IsWall()
    {
        _wallCheck = Physics.Raycast(transform.position, MoveDir, out _wallHit, 0.6f, _groundLayer);
    }

    private void Move()
    {
        if (moveMag < 0.1f)
            return;

        if (_isGrounded)
        {
            rb.AddForce(MoveDir * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
            return;
        }
        else
        {
            Vector3 airDir = MoveDir;

            if (Physics.Raycast(transform.position, MoveDir, out _wallHit, 0.6f, _groundLayer))
                airDir = Vector3.ProjectOnPlane(MoveDir, _wallHit.normal);

            rb.AddForce(airDir * moveSpeed * 0.6f * Time.deltaTime, ForceMode.VelocityChange);
        }
    }

    private void ResolveState()
    {
        if (moveMag < 0.1f)
            ChangeState(States.Idle);

        else if (_sprintToggle)
            ChangeState(States.Sprint);

        else
            ChangeState(States.Walk);
    }

    private bool HandleGroundTransitions()
    {
        if (!_isGrounded)
        {
            ChangeState(States.Air);
            return true;
        }
        if (_jumpPressed)
        {
            ChangeState(States.Jump);
            return true;
        }
        return false;
    }

    //Update Handlers
    private void AirUpdate()
    {
        if (_isGrounded)
            ResolveState();
        Move();
    }

    public void IdleUpdate()
    {
        if (!_isGrounded)
        {
            ChangeState(States.Air);
            return;
        }
        if (moveMag > 0.1f)
        {
            ChangeState(_sprintToggle ? States.Sprint : States.Walk);
            return;
        }
        if (_jumpPressed)
        {
            ChangeState(States.Jump);
        }
    }
    public void WalkUpdate()
    {
        if (HandleGroundTransitions())
            return;

        Move();

        if (_sprintToggle)
            ChangeState(States.Sprint);
        else if (moveMag < 0.1f)
            ChangeState(States.Idle);
    }

    public void SprintUpdate()
    {
        if (HandleGroundTransitions())
            return;

        Move();

        if (!_sprintToggle)
            ChangeState(States.Walk);
        else if (moveMag < 0.1f)
            ChangeState(States.Idle);
    }
    private void JumpUpdate()
    {
        ChangeState(States.Air);
    }


    private void DashUpdate()
    {
        dashTimer -= Time.deltaTime;
        if (dashTimer > 0f)
            return;

        if (_isGrounded)
            ResolveState();
        else
            ChangeState(States.Air);
    }
}
