using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IMovingPlatform:MonoBehaviour
{
    public abstract Vector3 Velocity { get; }
    public abstract void UpdateMovement();
}
public class MovingPlatform : IMovingPlatform
{
    public override Vector3 Velocity { get => velocity; }
    [SerializeField]private Vector3 velocity;
    private Vector3 prevPosition;
    [SerializeField] Vector3 Movedirection;
    [SerializeField] float moveSpeed;
    [SerializeField]bool start;
    Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        prevPosition = transform.position;
        position = transform.position;
    }
    public override void UpdateMovement()
    {
        prevPosition = transform.position;
        if (start)
            transform.Translate(Movedirection * moveSpeed * Time.deltaTime, Space.World);
        velocity = (transform.position - prevPosition) / Time.deltaTime;
    }
}
