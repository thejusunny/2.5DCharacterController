using UnityEngine;
namespace Controllers
{
    public class Dashing : CharacterState
    {
        Vector2 inputXY;
        float dashTimer;
        float prevDistance;
        float dashCoolDownTime = 0.5f;
        float lastdashTimeStamp;
        Vector3 dashDirection;
        DashData dashData;
        InputController inputController;
        float dashSpeed;
        public Dashing(CharacterController controller, CharacterMotor motor,DashData dashData)
        {
            this.motor = motor;
            this.controller = controller;
            this.inputController = controller.InputController;
            this.dashData = dashData;
            this.dashSpeed = dashData.DashDistance / dashData.DashDuration;
        }
        public override void Update()
        {
  
            if (dashTimer <= dashData.DashDuration)
            {
                float currentDistance = (dashTimer / dashData.DashDuration) * dashData.DashDistance ;
                float distanceOffset = currentDistance - prevDistance;
                motor.MoveExact(distanceOffset * dashDirection);
                prevDistance = currentDistance;
            }
            else
            {
                
                motor.EnableGravity();
                controller.ChangeState(CharacterStateEnum.AirMovement);
                return;
            }
            dashTimer += Time.deltaTime;
        }
        public override void Enter()
        {
            inputXY.x = Input.GetAxisRaw("Horizontal");
            inputXY.y = Input.GetAxisRaw("Vertical");
            inputXY.Normalize();
            if (1 - Mathf.Abs(inputXY.x) < 0.07)
            {
                inputXY.x = Mathf.Sign(inputXY.x);
                inputXY.y = 0f;
            }
            else if (Mathf.Abs(inputXY.x) > 0.15)
            {
                inputXY.x = Mathf.Sign(inputXY.x) * 0.7f;
                inputXY.y = Mathf.Sign(inputXY.y) * 0.7f;
            }
            else
            {
                inputXY.x = 0;
                inputXY.y = Mathf.Sign(inputXY.y);
            }
            inputXY.Normalize();// keyboard
            if (inputXY.magnitude <= 0)
                inputXY.x = 1;
            dashDirection = inputXY;
            motor.DisableGravity();
            motor.Velocity = Vector3.zero;
        }
        public override void Exit()
        {
            motor.Velocity = dashDirection * dashSpeed/2.5f;
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
