using UnityEngine;
namespace Controllers
{
    public class Airmovement : CharacterState
    {
        Vector2 inputXY;
        AirmovementData airmovementData;
        float newAirmovementSpeed;
        public Airmovement(CharacterController controller, CharacterMotor motor, AirmovementData airmovementData)
        {
            this.motor = motor;
            this.controller = controller;
            this.airmovementData = airmovementData;
        }
        public override void Update()
        {
            inputXY.x = Input.GetAxis("Horizontal");
            inputXY.y = Input.GetAxis("Vertical");
            motor.Velocity.x = Mathf.Lerp(motor.Velocity.x, inputXY.x * newAirmovementSpeed, airmovementData.MovementRegainSpeed * Time.deltaTime);

                
            CheckForTranstion(CharacterStateEnum.Moving);
            CheckForTranstion(CharacterStateEnum.Idle);
            CheckForTranstion(CharacterStateEnum.Dashing);
            CheckForTranstion(CharacterStateEnum.WallJumping);
            if (motor.OnGround)
                motor.Velocity.x = 0f;

        }
        public override void Enter()
        {
            newAirmovementSpeed = Mathf.Clamp((airmovementData.AirMovementSpeed + Mathf.Abs( motor.Velocity.x)), 0, airmovementData.MaxHorizontalDistance);
        }
        public override void Exit()
        {
            //motor.Velocity = Vector3.zero;
        }
        public override bool IsReadyForTransition()
        {
            if (controller.CurrentState!= CharacterStateEnum.Jumping && controller.CurrentState!= CharacterStateEnum.Dashing && !motor.OnGround)
                return true;
            return false;
        }

    }
}
