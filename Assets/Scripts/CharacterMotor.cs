using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    private UnityEngine.CharacterController ccModule;
    private CharacterGravity gravityModule;
    private Quaternion rotation;
    private Vector3 currentMoveOffset;
    [SerializeField] private Vector3 currentWorldVelocity;
    [SerializeField] private float groundRaylength;
    [SerializeField] private float wallRaylength;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] bool isOnGround;
    [SerializeField] bool isOnWall;
    [SerializeField] float horizontalDrag = 3f;
    public bool OnGround => leftOnGround || rightOnGround;
    public bool OnWall => leftWall || rightWall;
    private Vector3 prevPosition;
    public Vector3 Velocity;
    [SerializeField] private bool clampZ;
    bool leftOnGround;
    bool rightOnGround;
    bool leftWall;
    bool rightWall;
    bool gravityOn = true;
    public int WallDirection { get => leftWall ? -1 : rightWall ? 1 : 0; }
    // Start is called before the first frame update
    void Start()
    {
        gravityModule = GetComponent<CharacterGravity>();
        ccModule = GetComponent<UnityEngine.CharacterController>();
    }
    // Update is called once per frame
    public void UpdateMotor()
    {
        DetectGroundCollisions();
        DetectWallCollisions();
        isOnGround = OnGround;
        isOnWall = OnWall;
        UpdateMovement();
    }
    private void DetectGroundCollisions()
    {
        leftOnGround = Physics.Raycast(transform.position - Vector3.right * 0.5f, Vector3.down, groundRaylength, groundLayer);
        rightOnGround = Physics.Raycast(transform.position + Vector3.right * 0.5f, Vector3.down, groundRaylength, groundLayer);
    }
    private void DetectWallCollisions()
    {
        leftWall = Physics.Raycast(transform.position - Vector3.right * 0.5f, Vector3.left ,wallRaylength, groundLayer);
        rightWall = Physics.Raycast(transform.position + Vector3.right * 0.5f, Vector3.right, wallRaylength, groundLayer);
    }
    private void Move(Vector3 movementOffset, float deltaTime)
    {
        ccModule.Move(movementOffset * deltaTime);
    }
    public void MoveTo(Vector3 postionToMove)
    {
        ccModule.Move(postionToMove - transform.position);
    }
    public void MoveExact(Vector3 movementOffset)
    {
        ccModule.Move(movementOffset);
    }
    public void SetVelocity(Vector3 velocity)
    {
        this.Velocity = velocity;

    }
    public void AddToVelocity(Vector3 velocityToAdd)
    {
        this.Velocity += velocityToAdd;
    }
    public Vector3 GetVelocity() { return Velocity; }

    void UpdateMovement()
    {
        ApplyGravity(ref Velocity);
        if (clampZ)
            Velocity.z = 0f;
        Move(Velocity, Time.deltaTime);
    }

    private void ApplyGravity(ref Vector3 velocity)
    {
        if (!isOnGround && gravityOn)
            Velocity += gravityModule.GetGravity() * Time.deltaTime;
    }

    public Vector3 GetGravity()
    {
        return gravityModule.GetGravity();
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundRaylength);
        Gizmos.DrawRay(transform.position -Vector3.right * 0.5f, Vector3.left * wallRaylength);
        Gizmos.DrawRay(transform.position +Vector3.right * 0.5f, Vector3.right * wallRaylength);
    }
    public void DisableGravity()
    {
        gravityOn = false;
        Velocity.y = 0f;
    }
    public void EnableGravity()
    {
        gravityOn = true;
    }
    public Vector3 GetPosition() { return transform.position; }
}
