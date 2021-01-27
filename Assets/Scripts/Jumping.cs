using UnityEngine;
namespace Controller
{
    public class Jumping : CharacterState
    {
        Vector2 inputXY;
        float horizontalStartVelocityX;
        float currentJumpDistance;
        Vector3 jumpStartPosition;
        bool peekReached = false;
        float stateTimer;
        float groundExpiryTime = 0.1f;
        float maxDistanceOnXLeft;
        float maxDistanceOnXRight;
        float speedOffset = 0f;
        JumpData jumpData;
        float jumpButtonHoldTimer = 0f;
        float lowJumpMultiplier=10f;
        public Jumping(CharacterController controller, CharacterMotor motor, JumpData jumpData) : base(controller, motor)
        {
            this.jumpData = jumpData;
        }
        public override void Update()
        {

            //JumpCorrection
            
            stateTimer += Time.deltaTime;
            if (Input.GetButton("Jump"))
            {
                jumpButtonHoldTimer += Time.deltaTime;
            }
            else
            {
                motor.Velocity.y += Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
                //controller.ChangeState(CharacterStateEnum.AirMovement);
                //return;
            }
            inputXY.x = GetNormalizedInput(Input.GetAxisRaw("Horizontal"));
            if (Vector3.Distance(motor.GetPosition(), jumpStartPosition) >= jumpData.JumpHeight)
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
                CheckForTranstion(CharacterStateEnum.Hooking);
                if (collision.OnCeiling && !collision.BothRaysCelingHit)
                {
                    JumpCorrection();

                }

                if (collision.BothRaysCelingHit)
                {
                    motor.Velocity.y = 0f;
                    controller.ChangeState(CharacterStateEnum.AirMovement);
                }
            }
            CheckForTranstion(CharacterStateEnum.WallJumping);
            Debug.DrawLine(new Vector3(maxDistanceOnXLeft, 0f, 0f), new Vector3(maxDistanceOnXLeft,2f,0f),Color.red);
            Debug.DrawLine(new Vector3(maxDistanceOnXRight, 0f, 0f), new Vector3(maxDistanceOnXRight, 2f, 0f), Color.red);
        }

        private void JumpCorrection()
        {
            float stepsToMove = 0;
            if (!collision.LeftCelingHit)
            {
                //Debug.Break();
                //for (float i = 0; i < 2; i+=0.1f)
                //{
                //    bool rayHit = Physics.Raycast(motor.GetPosition() + (Vector3.left * i) + motor.RightCeilingRayOffset, Vector3.up,1.2f,1<<8);
                //    if (!rayHit)
                //    {
                //        stepsToMove = i;
                //        break;
                //    }
                //    Debug.DrawRay(motor.GetPosition() + (Vector3.left * i) + motor.RightCeilingRayOffset, Vector3.up * 1.2f);
                //}
                //Debug.Log("StepsToMove" + stepsToMove);
                motor.Velocity.x -= 3f;
                motor.Velocity.y += 0.3f;
                //motor.MoveExact(new Vector3(-stepsToMove, 0f, 0f));
            }
            else if (!collision.RightCelingHit)
            {
                //for (float i = 0; i < 2; i++)
                //{
                //    bool rayHit = Physics.Raycast(motor.GetPosition() + (Vector3.right * i) + motor.LeftCeilingRayOffset, Vector3.up, 1.2f, 1 << 8);
                //    if (!rayHit)
                //    {
                //        stepsToMove = i;
                //        break;
                //    }
                //    Debug.DrawRay(motor.GetPosition() + (Vector3.right * i) + motor.LeftCeilingRayOffset, Vector3.up * 1.2f);
                //}
                motor.Velocity.x += 3f;
                motor.Velocity.y += 0.3f;
                //motor.MoveExact(new Vector3(stepsToMove, 0f, 0f));
            }
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
            float TargetVelocity = Mathf.Sqrt(-2f * jumpData.JumpHeight * motor.GetGravity().y);
            motor.Velocity = Vector3.up* TargetVelocity;
            float timeToLand = Mathf.Sqrt(Mathf.Abs( (2 * jumpData.JumpHeight) / motor.GetGravity().y))*2;
            speedOffset = Mathf.Clamp((jumpData.JumpAirMovementSpeed + horizontalStartVelocityX), 0, jumpData.MaxHorizontalDistance);
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
            jumpButtonHoldTimer = 0f;
        }
        public override bool IsReadyForTransition()
        {
            if (inputController.GetCommad(CommandType.Jump).IsPressed() && collision.OnGround)
                return true;
            return false;
        }
    }
}
