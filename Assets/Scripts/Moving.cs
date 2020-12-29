using UnityEngine;
namespace Controllers
{
    public class Moving : CharacterState
    {
        Vector2 inputXY;
        float moveSpeed = 10f;
        public Moving(CharacterController controller,CharacterMotor motor)
        {
            this.motor = motor;
            this.controller = controller;
        }
        public override void Update()
        {
            inputXY.x = Input.GetAxisRaw("Horizontal");
            inputXY.y = Input.GetAxisRaw("Vertical");
            motor.Velocity.x = inputXY.x * moveSpeed;
            CheckForTranstion(CharacterStateEnum.Idle);
            CheckForTranstion(CharacterStateEnum.Jumping);
            CheckForTranstion(CharacterStateEnum.Dashing);
            CheckForTranstion(CharacterStateEnum.AirMovement);

        }
        public override void Enter()
        {

        }

        public override void Exit()
        {
   
        }

        public override bool IsReadyForTransition()
        {
            if (Mathf.Abs( Input.GetAxis("Horizontal"))>0 && motor.OnGround)
                return true;
            return false;
        }

    

    }
}
