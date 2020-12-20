using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public enum CharacterStateEnum
    {
        Idle,Moving,Jumping,Dashing
    };
    public class CharacterController : MonoBehaviour
    {
        UnityEngine.CharacterController ccModule;
        private CharacterGravity gravityModule;
        private Vector3 verticalVelocity;
        private Quaternion rotation;
        private Vector3 currentMoveOffset;
        private Dictionary<CharacterStateEnum, CharacterState> states;
        [SerializeField]private CharacterStateEnum currentState;
        [SerializeField]private Vector3 currentWorldVelocity;
        [SerializeField] private float raylength;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField]bool isOnGround;
        public bool OnGround => leftOnGround||rightOnGround;
        private Vector3 prevPosition;
        public Vector3 Velocity => currentWorldVelocity;
        [SerializeField]private bool clampZ;
        bool leftOnGround;
        bool rightOnGround;
        bool gravityOn = true;
        private void Start()
        {
            ccModule = GetComponent<UnityEngine.CharacterController>();
            gravityModule = GetComponent<CharacterGravity>();
            BindStates();
            currentState = CharacterStateEnum.Moving;
        }
        private void BindStates()
        {
            states = new Dictionary<CharacterStateEnum, CharacterState>();
            states.Add(CharacterStateEnum.Moving, new Moving(this));
            states.Add(CharacterStateEnum.Idle, new Idle(this));
            states.Add(CharacterStateEnum.Jumping, new Jumping(this));
            states.Add(CharacterStateEnum.Dashing, new Dashing(this));
        }
        public void SetMoveOffset(Vector3 moveOffset)
        {
            currentMoveOffset += moveOffset * Time.deltaTime;
            currentWorldVelocity = currentMoveOffset/Time.deltaTime;
        }
        public void SetMovePosition(Vector3 movePosition)
        {
            currentMoveOffset += movePosition;
            currentWorldVelocity = currentMoveOffset / Time.deltaTime;
        }
        private void Update()
        {
            DetectGroundCollisions();
            isOnGround = OnGround;
            ExecuteState();
            UpdateMovement();
        }
        public CharacterState GetState(CharacterStateEnum stateToGet)
        {
            return states[stateToGet];
        }
        private void DetectGroundCollisions()
        {
            
            leftOnGround = Physics.Raycast(transform.position - Vector3.right* 0.5f, Vector3.down, raylength, groundLayer);
            rightOnGround = Physics.Raycast(transform.position +Vector3.right* 0.5f, Vector3.down, raylength, groundLayer);
        }
        void ExecuteState()
        {
            states[currentState].Update();
        }
        public void ChangeState(CharacterStateEnum targetState)
        {
            states[currentState].Exit();
            currentState = targetState;
            states[currentState].Enter();
        }
        void UpdateMovement()
        {
            Vector3 finalMovementOffset = currentMoveOffset;
            Vector3 verticalMovement = Vector3.zero;
            verticalMovement = ComputeVerticalVelocity();
            finalMovementOffset += verticalMovement;
            if (clampZ)
                finalMovementOffset.z = 0f;
            ccModule.Move(finalMovementOffset);
            currentMoveOffset = Vector3.zero;
        }
        Vector3 ComputeVerticalVelocity()
        {
            if (!isOnGround&&gravityOn)
                verticalVelocity += ApplyGravity();
            currentWorldVelocity.y += verticalVelocity.y;
            return verticalVelocity*Time.deltaTime;
        }
        public Vector3 GetVerticalVelocity() { return verticalVelocity; }
        public void SetVerticalVelocity(float velocityY)
        {
            verticalVelocity.y = velocityY;
        }
        private Vector3 ApplyGravity()
        {
            return gravityModule.GetGravity() * Time.deltaTime;
        }

        public Vector3 GetGravity()
        {
            return gravityModule.GetGravity();
        }
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down * raylength);
        }
        public void DisableGravity()
        {
            gravityOn = false;
            verticalVelocity = Vector3.zero;
        }
        public void EnableGravity()
        {
            gravityOn = true;
        }
        public Vector3 GetPosition() { return transform.position; }

    }

}
