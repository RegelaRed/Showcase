using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables/references

    [Header("References")]
    [SerializeField] private Rigidbody _Rb;
    [SerializeField] private Transform _Orientation;
    [SerializeField] private Transform _CamPos;
    [SerializeField] private Transform _PlayerBody;
    [SerializeField] public PlayerVariables _Pv;

    [Header("Movement")]
    private Vector3 _moveDir = Vector3.zero;
    private float _moveSpeed = 0;
    float _moveMag = 0;

    [Header("Local")]
    private float _maxWalkSpeed = 6;
    private float _maxSprintSpeed = 10;
    private float _curMaxSpeed;

    private int _maxDashCharges = 2;
    private int _dashCharges = 2;
    private float _dashCooldownTime = 1.5f;
    private float _dashCooldown;

    [Header("Crouch")]
    [SerializeField] float _crouchHeight = 1.6f;
    [SerializeField] float _standHeight = 2f;
    [SerializeField] float _crouchSpeed = 3f;
    [SerializeField] float _crouchCamOffset = -0.6f;

    float _currentHeight;
    float _targetHeight;
    float _camDefaultY;


    [Header("StateChecks")]
    private float _dashTime = 0.2f;
    private float _dashTimer = 0;
    bool _dashPressed = false;
    bool _sprintToggle = false;
    bool _jumpPressed = false;
    bool _crouchToggle = false;

    [Header("Collision Check")]
    [SerializeField] private LayerMask _groundLayer;
    private bool _isGrounded;
    private RaycastHit _wallHit;

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

    #endregion
    /// <summary>
    ///     code structure
    ///     update -> state runner
    ///     handleInpur, GroundCheck, ChangeState(OnEntrer, OnExit), Move functions
    ///     
    ///     undate handlers for -> Idle, Air, Walk, Sprint, Crouch, Jump, Dash
    /// 
    /// </summary>
    /// 
    #region Updates
    private void Awake()
    {
        _Rb.freezeRotation = true;

        _camDefaultY = _CamPos.localPosition.y;
        _currentHeight = _standHeight;
        _targetHeight = _standHeight;

    }
    private void Update()
    {
        IsGrounded();
        HandleInput();

        if (_dashCharges < _maxDashCharges)
        {
            _dashCooldown -= Time.deltaTime;
            if (_dashCooldown <= 0f)
            {
                _dashCharges++;
                _dashCooldown = _dashCooldownTime;
            }
        }
        if (HandleGlobalTransitions())
            return;

        switch (_state)
        {
            case States.Idle: IdleUpdate(); break;
            case States.Air: AirUpdate(); break;
            case States.Walk: WalkUpdate(); break;
            case States.Sprint: SprintUpdate(); break;
            case States.Crouch: CrouchUpdate(); break;
            case States.Jump: JumpUpdate(); break;
            case States.Dash: DashUpdate(); break;
        }
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        CapSpeed();
        RemoveWallVelocity();
        LerpHeight();
    }

    #endregion

    #region Inputs

    private void HandleInput()
    {
        //if input is 0 then idle
        Vector3 forward = _Orientation.forward;
        Vector3 right = _Orientation.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        _moveDir = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
        _moveMag = _moveDir.magnitude;
        _moveDir = _moveDir.normalized;

        _dashPressed = Input.GetKeyDown(KeyCode.LeftShift);
        _jumpPressed = Input.GetKeyDown(KeyCode.Space) && _state != States.Jump;

        if (Input.GetKeyDown(KeyCode.LeftAlt))
            _crouchToggle = !_crouchToggle;

        if (Input.GetKeyDown(KeyCode.LeftControl))
            _sprintToggle = !_sprintToggle;

    }

    #endregion
    #region  State Updates

    public void IsGrounded()
    {
        float groundCheckDist = _currentHeight * 0.6f + 0.2f;
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDist, _groundLayer);
        //_isGrounded = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out _, groundCheckDist, _groundLayer);

        Debug.DrawRay(transform.position, Vector3.down * groundCheckDist, Color.yellow);
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
        Debug.Log("current state :" + state);
        switch (state)
        {
            case States.Idle:
                Vector3 v = _Rb.velocity;
                _Rb.velocity = new Vector3(v.x * 0.2f, v.y, v.z * 0.2f);
                break;
            case States.Walk:
                _curMaxSpeed = _maxWalkSpeed;
                _moveSpeed = _Pv._walkSpeed;
                break;

            case States.Sprint:
                _curMaxSpeed = _maxSprintSpeed;
                _moveSpeed = _Pv._sprintSpeed;
                break;

            case States.Jump:
                _Rb.AddForce(Vector3.up * _Pv._jumpForce * 2, ForceMode.Impulse);
                break;

            case States.Dash:
                if (_dashCharges > 0)
                {
                    _dashCharges--;
                    _dashCooldown = _dashCooldownTime;
                    _dashTimer = _dashTime;
                    Vector3 dashDir = _moveMag > 0.1f ? _moveDir : _Orientation.forward;
                    _Rb.AddForce(dashDir.normalized * _Pv._dashForce, ForceMode.Impulse);
                }
                break;

            case States.Crouch:
                _curMaxSpeed = _crouchSpeed;
                _moveSpeed = _crouchSpeed;
                _targetHeight = _crouchHeight;
                break;
        }
    }
    private void OnExit(States state)
    {
        switch (state)
        {
            case States.Air: break;
            case States.Idle: break;
            case States.Walk: break;
            case States.Sprint: break;
            case States.Jump: break;
            case States.Dash: break;
            case States.Crouch: _targetHeight = _standHeight; break;
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
        if (_dashPressed && _dashCharges > 0)
        {
            //dash
            ChangeState(States.Dash);
            return true;
        }
        if (!_isGrounded)
        {
            //set to air
            ChangeState(States.Air);
            return true;
        }
        if (_jumpPressed && _isGrounded)
        {
            //jump
            ChangeState(States.Jump);
            return true;
        }
        if (_moveMag < 0.1f)
        {
            ChangeState(States.Idle);
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
        if (_moveMag < 0.1f)
            ChangeState(States.Idle);

        else if (_sprintToggle)
            ChangeState(States.Sprint);

        else
            ChangeState(States.Walk);
    }

    #endregion

    #region Speed/Collision


    private void RemoveWallVelocity()
    {
        if (_isGrounded)
            return;

        Vector3 checkDir = _Rb.velocity;
        checkDir.y = 0f;

        if (checkDir.sqrMagnitude < 0.01f)
            return;

        checkDir.Normalize();

        if (!Physics.Raycast(transform.position, checkDir, out _wallHit, 0.4f))
            return;

        Vector3 vel = _Rb.velocity;

        float intoWall = Vector3.Dot(vel, _wallHit.normal);

        if (intoWall < 0f)
        {
            _Rb.velocity -= _wallHit.normal * intoWall;
        }
    }


    private void LerpHeight()
    {
        //changes player height for crouch state
        _currentHeight = Mathf.Lerp(
                _currentHeight,
                _targetHeight,
                Time.fixedDeltaTime * 10f);

        _PlayerBody.localScale = new Vector3(1f, _currentHeight / _standHeight, 1f);

        _CamPos.localPosition = new Vector3(
                _CamPos.localPosition.x,
                _camDefaultY + (_currentHeight < _standHeight ? _crouchCamOffset : 0f),
                _CamPos.localPosition.z);
    }
    private void CapSpeed()
    {
        //limits maximum velocity
        Vector3 vel = _Rb.velocity;
        Vector3 horizontal = new Vector3(vel.x, 0f, vel.z);

        if (horizontal.magnitude > _curMaxSpeed)
        {
            Vector3 capped = horizontal.normalized * _curMaxSpeed;
            _Rb.velocity = new Vector3(capped.x, vel.y, capped.z);
        }
    }

    private void ApplyMovement()
    {
        //enable/dissable movement, and is called from FixedUpdate
        if (_state == States.Walk || _state == States.Sprint ||
            _state == States.Air || _state == States.Crouch ||
            _state == States.Jump)
        {
            Move();
        }
    }
    private void Move()
    {
        //applys rb.addforce based on state
        if (_moveMag < 0.1f)
            return;

        if (_isGrounded)
        {
            _Rb.AddForce(_moveDir * _moveSpeed * 0.8f, ForceMode.VelocityChange);
            return;
        }
        else
        {
            Vector3 airDir = _moveDir;

            if (Physics.Raycast(transform.position, _moveDir, out _wallHit, 0.4f))
            {
                airDir = Vector3.ProjectOnPlane(_moveDir, _wallHit.normal);
            }

            _Rb.AddForce(airDir * _moveSpeed * 0.5f, ForceMode.VelocityChange);
        }

    }

    #endregion

    #region State Updates

    private void AirUpdate()
    {
        if (_isGrounded)
            ResolveState();
    }

    public void IdleUpdate()
    {
        if (_moveMag > 0.1f)
        {
            ChangeState(_sprintToggle ? States.Sprint : States.Walk);
            return;
        }
    }
    public void WalkUpdate()
    {
        if (_sprintToggle)
            ChangeState(States.Sprint);
    }

    public void SprintUpdate()
    {
        if (_crouchToggle)
        {
            ChangeState(States.Crouch);
            return;
        }

        if (!_sprintToggle)
        {
            ChangeState(States.Walk);
            return;
        }

        if (_moveMag < 0.1f)
        {
            ChangeState(States.Idle);
            return;
        }
    }
    private void JumpUpdate()
    {
        ChangeState(States.Air);
    }


    private void DashUpdate()
    {
        _dashTimer -= Time.deltaTime;
        if (_dashTimer > 0f)
            return;

        if (_isGrounded)
            ResolveState();
        else
            ChangeState(States.Air);
    }
    private void CrouchUpdate()
    {
        if (!_crouchToggle)
            ResolveState();
    }
    #endregion
}
