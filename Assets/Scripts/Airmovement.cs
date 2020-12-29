using UnityEngine;
namespace Controllers
{
    public class Airmovement : CharacterState
    {
        Vector2 inputXY;
        float airMovementSpeed = 8f;
        float maxHorizontalDistance =14f;
        float movementRegainSpeed = 8f;
        public Airmovement(CharacterController controller, CharacterMotor motor)
        {
            this.motor = motor;
            this.controller = controller;
        }
        public override void Update()
        {
            inputXY.x = Input.GetAxis("Horizontal");
            inputXY.y = Input.GetAxis("Vertical");
            motor.Velocity.x = Mathf.Lerp(motor.Velocity.x, inputXY.x * airMovementSpeed,movementRegainSpeed*Time.deltaTime);
            CheckForTranstion(CharacterStateEnum.Moving);
            CheckForTranstion(CharacterStateEnum.Idle);
            CheckForTranstion(CharacterStateEnum.Dashing);
            CheckForTranstion(CharacterStateEnum.WallJumping);
        }
        public override void Enter()
        {
            airMovementSpeed = Mathf.Clamp((airMovementSpeed +Mathf.Abs( motor.Velocity.x)), 0, maxHorizontalDistance);
        }
        public override void Exit()
        {
          
        }
        public override bool IsReadyForTransition()
        {
            if (controller.CurrentState!= CharacterStateEnum.Jumping && controller.CurrentState!= CharacterStateEnum.Dashing && !motor.OnGround)
                return true;
            return false;
        }

    }
}
