using UnityEngine;
namespace Controllers
{
    public class Dashing : CharacterState
    {
        Vector2 inputXY;

        float dashDistance = 5f;
        float dashDuration = 0.12f;
        float dashTimer;
        float prevDistance;
        float xVelocityEnd;
        float internalDashVelocity;
        float dashCoolDownTime = 0.5f;
        float lastdashTimeStamp;
        Vector3 dashDirection;
        public Dashing(CharacterController controller, CharacterMotor motor)
        {
            this.motor = motor;
            this.controller = controller;
        }
        public override void Update()
        {
  
            if (dashTimer <= dashDuration)
            {
                float currentDistance = (dashTimer / dashDuration) * dashDistance ;
                float distanceOffset = currentDistance - prevDistance;
                internalDashVelocity = distanceOffset / Time.deltaTime;
                motor.MoveExact(distanceOffset * dashDirection);
                prevDistance = currentDistance;
                xVelocityEnd = motor.Velocity.x/2;
            }
            else
            {
                
                motor.EnableGravity();
                controller.ChangeState(CharacterStateEnum.AirMovement);
                //CheckForTranstion(CharacterStateEnum.Moving);
                //CheckForTranstion(CharacterStateEnum.Idle);
            }
            dashTimer += Time.deltaTime;
        }
        public override void Enter()
        {
            inputXY.x = Input.GetAxisRaw("Horizontal");
            inputXY.y = Input.GetAxisRaw("Vertical");
            inputXY.Normalize();
            if (inputXY.magnitude <= 0)
                inputXY.x = 1;
            dashDirection = inputXY;
            motor.DisableGravity();
            motor.Velocity = Vector3.zero;
        }
        public override void Exit()
        {
            motor.Velocity = dashDirection * internalDashVelocity/2;
            lastdashTimeStamp = Time.time;
            dashTimer = 0f;
            prevDistance = 0f;
            dashDirection = Vector3.zero;
        }
        public override bool IsReadyForTransition()
        {
            if (Input.GetButtonDown("Dash")&& Time.time > lastdashTimeStamp+dashCoolDownTime)
                return true;
            return false;
        }

    }
}
