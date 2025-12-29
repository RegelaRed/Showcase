using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Player Stats")]
public class PlayerVariables : ScriptableObject
{
    //Walk
    public float walkSpeed = 5;

    //Sprint
    public float sprintSpeed = 8f;

    //Jump
    public float jumpForce = 4;

    //Dash
    public float dashForce = 4;
    public int maxDashCharges = 2;
    public float dashRegenTime = 1f;
    public float dashDuration = 0.8f;

    //LayerMasks
    public LayerMask groundLayer;
}
