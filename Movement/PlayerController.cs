using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Transform orientation;
    [SerializeField] private PlayerVariables playerVariables;
    [SerializeField] private float standHeight = 2f;

    // Public access (read-only)
    public Rigidbody Rb => rigidBody;
    public Transform Orientation => orientation;
    public PlayerVariables PlayerVariables => playerVariables;
    public float StandHeight => standHeight;

    // Modules
    public PlayerInput Input { get; private set; }
    public PlayerPhysicsUpdate Move { get; private set; }
    public PlayerGrounded Ground { get; private set; }
    public PlayerDash Dash { get; private set; }
    public PlayerJump Jump { get; private set; }
    public StateMachine State { get; private set; }

    private void Awake()
    {
        rigidBody.freezeRotation = true;

        Input = new PlayerInput(this);
        Move = new PlayerPhysicsUpdate(this);
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
        Move.Tick();
    }
}
