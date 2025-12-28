using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Transform _orientation;
    [SerializeField] private PlayerVariables _playerVariables;
    [SerializeField] private float _standHeight = 2f;

    // Public access (read-only)
    public Rigidbody Rb => _rigidBody;
    public Transform Orientation => _orientation;
    public PlayerVariables PlayerVariables => _playerVariables;
    public float StandHeight => _standHeight;

    // Modules
    public PlayerInput Input { get; private set; }
    public PlayerPhysicsUpdate Movement { get; private set; }
    public PlayerGrounded Ground { get; private set; }
    public PlayerDash Dash { get; private set; }
    public PlayerJump Jump { get; private set; }
    public StateMachine State { get; private set; }

    private void Awake()
    {
        _rigidBody.freezeRotation = true;

        Input = new PlayerInput(this);
        Movement = new PlayerPhysicsUpdate(this);
        Ground = new PlayerGrounded(this);
        Dash = new PlayerDash(this);
        Jump = new PlayerJump(this);
        State = new StateMachine(this);
    }
    private void Update()
    {
        Input.Tick();
        Ground.Tick();
        State.Tick();
        Dash.Tick();
    }
    private void FixedUpdate()
    {
        Movement.Tick();
    }
}
