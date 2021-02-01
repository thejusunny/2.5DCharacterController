using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Controller
{
    public class ExternalVelocityResolver//Moving platforms, wind and all the other environmental stuff
    {

        private CharacterCollision collision;
        public ExternalVelocityResolver(CharacterCollision collision)
        {
            this.collision = collision;
        }
        public Vector3 GetExternalVelocity()
        {
            Vector3 finalVelocity = Vector3.zero;
            if (collision.IsOnMovingPlatform)
            {
                finalVelocity += collision.PlatformInContact.Velocity;
            }
            return finalVelocity;
        }
    }
    public class CharacterMotor : MonoBehaviour
    {
        private UnityEngine.CharacterController ccModule;
        private CharacterGravity gravityModule;
        private CharacterCollision collision;
        private Quaternion rotation;
        [SerializeField] float horizontalDrag = 3f;
        private Vector3 prevPosition;
        public Vector3 Velocity;
        public Vector3 ExternalVelocity;
        public Vector3 GlobalVelocity => Velocity + frameMoveOffset + ExternalVelocity;
        private Vector3 frameMoveOffset;
        [SerializeField] private bool clampZ;
        [SerializeField] private float maxFallVelocity = 50;
        bool gravityOn = true;
        ExternalVelocityResolver externalVelocityResolver;
        // Start is called before the first frame update
        void Start()
        {
            gravityModule = GetComponent<CharacterGravity>();
            ccModule = GetComponent<UnityEngine.CharacterController>();
        }
        public void Init(CharacterCollision collision)
        {
            this.collision = collision;
            externalVelocityResolver = new ExternalVelocityResolver(collision);
        }
        // Update is called once per frame
        public void UpdateMotor()
        {
            UpdateMovement();
        }
        private void Move(Vector3 movementOffset, float deltaTime)
        {
            ccModule.Move(movementOffset * deltaTime);
        }
        public void MoveTo(Vector3 postionToMove)
        {
            Vector3 movementOffset = (postionToMove - transform.position);
            frameMoveOffset += movementOffset;
        }
        public void MoveExact(Vector3 movementOffset)
        {
            frameMoveOffset += movementOffset;
            //ccModule.Move(movementOffset);
        }
        void UpdateMovement()
        {
            Velocity += ApplyGravity();
            if (clampZ)
            {
                Velocity.z = 0f;
                frameMoveOffset.z = 0;
            }
            ExternalVelocity = externalVelocityResolver.GetExternalVelocity();
            Velocity.y = Mathf.Clamp(Velocity.y, -maxFallVelocity, float.PositiveInfinity);
            Move(Velocity, Time.deltaTime);
            Move(ExternalVelocity, Time.deltaTime);
            Move(frameMoveOffset, 1);
            frameMoveOffset = Vector3.zero;
        }

        private Vector3 ApplyGravity()
        {
            if (!collision.OnGround && gravityOn)
                return gravityModule.GetGravity() * Time.deltaTime;
            return Vector3.zero;
        }

        public Vector3 GetGravity()
        {
            return gravityModule.GetGravity();
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
}
