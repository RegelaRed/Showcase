using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Player Stats")]
public class PlayerVariables : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed = 5;
    public float sprintSpeed = 12;
    public float jumpForce = 4;
    public float dashForce = 4;
    public int maxDashCharges = 2;
    public float dashCooldownTime = 1.5f;
    public float maxWalkSpeed = 6f;
    public float maxSprintSpeed = 10f;
    public LayerMask groundLayer;
}
