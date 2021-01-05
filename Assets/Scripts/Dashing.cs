using UnityEngine;
namespace Controllers
{
    public class Dashing : CharacterState
    {
        Vector2 inputXY;
        float dashTimer;
        float prevDistance;
        float xVelocityEnd;
        float internalDashVelocity;
        float dashCoolDownTime = 0.5f;
        float lastdashTimeStamp;
        Vector3 dashDirection;
        DashData dashData;
        InputController inputController;
        public Dashing(CharacterController controller, CharacterMotor motor,DashData dashData)
        {
            this.motor = motor;
            this.controller = controller;
            this.inputController = controller.InputController;
            this.dashData = dashData;
        }
        public override void Update()
        {
  
            if (dashTimer <= dashData.DashDuration)
            {
                float currentDistance = (dashTimer / dashData.DashDuration) * dashData.DashDistance ;
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
            if (inputController.GetCommad(CommandType.Dash).IsPressed()&& Time.time > lastdashTimeStamp+dashCoolDownTime)
                return true;
            return false;
        }

    }
}
