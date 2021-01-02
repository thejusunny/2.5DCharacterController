using UnityEngine;
namespace Controllers
{
    public class Jumping : CharacterState
    {
        Vector2 inputXY;
        float jumpAirMovementSpeed = 5f;
        float jumpHeight=2.5f;
        float horizontalStartVelocityX;
        float currentJumpDistance;
        Vector3 jumpStartPosition;
        bool peekReached = false;
        float stateTimer;
        float groundExpiryTime = 0.1f;
        float maxDistanceOnXLeft;
        float maxDistanceOnXRight;
        float maxHorizontalDistance = 14f;
        float speedOffset = 0f;
        InputController inputController;
        public Jumping(CharacterController controller, CharacterMotor motor)
        {
            this.motor = motor;
            this.controller = controller;
            inputController = controller.InputController;
        }
        public override void Update()
        {
            stateTimer += Time.deltaTime;
            inputXY.x = GetNormalizedInput(Input.GetAxisRaw("Horizontal"));
            if (Vector3.Distance(motor.GetPosition(), jumpStartPosition) >= jumpHeight)
            {
                controller.ChangeState(CharacterStateEnum.AirMovement);
                return;
            }
            motor.Velocity.x = inputXY.x * speedOffset;
            if (stateTimer > groundExpiryTime)
            {
                CheckForTranstion(CharacterStateEnum.Moving);
                CheckForTranstion(CharacterStateEnum.Idle);
                CheckForTranstion(CharacterStateEnum.Dashing);
                CheckForTranstion(CharacterStateEnum.WallJumping);
            }
            CheckForTranstion(CharacterStateEnum.WallJumping);
            Debug.DrawLine(new Vector3(maxDistanceOnXLeft, 0f, 0f), new Vector3(maxDistanceOnXLeft,2f,0f),Color.red);
            Debug.DrawLine(new Vector3(maxDistanceOnXRight, 0f, 0f), new Vector3(maxDistanceOnXRight, 2f, 0f), Color.red);
        }
        public float GetNormalizedInput(float input)
        {
            float valueToReturn = input > 0 ? 1 : input < 0 ? -1 : 0;

            return valueToReturn;
        }
        public override void Enter()
        {
            inputController.GetCommad(CommandType.Jump).ClearFrameBuffer();
            horizontalStartVelocityX = Mathf.Abs( motor.Velocity.x);
            float TargetVelocity = Mathf.Sqrt(-2f * jumpHeight * motor.GetGravity().y);
            motor.Velocity = Vector3.up* TargetVelocity;
            float timeToLand = Mathf.Sqrt(Mathf.Abs( (2 * jumpHeight) / motor.GetGravity().y))*2;
            speedOffset = Mathf.Clamp((jumpAirMovementSpeed + horizontalStartVelocityX), 0, maxHorizontalDistance);
            maxDistanceOnXLeft = (motor.GetPosition().x - speedOffset*timeToLand);
            maxDistanceOnXRight = (motor.GetPosition().x + speedOffset * timeToLand);
            jumpStartPosition = motor.GetPosition();
            currentJumpDistance = 0f;
        }
        public override void Exit()
        {
            horizontalStartVelocityX = 0f;
            peekReached = false;
            stateTimer = 0f;
        }
        public override bool IsReadyForTransition()
        {
            if (inputController.GetCommad(CommandType.Jump).IsPressed() &&motor.OnGround)
                return true;
            return false;
        }
    }
}
