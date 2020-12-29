using UnityEngine;
namespace Controllers
{
    public class Idle : CharacterState
    {
        Vector2 inputXY;
        float moveSpeed = 10f;
        public Idle(CharacterController controller,CharacterMotor motor)
        {
            this.motor = motor;
            this.controller = controller;
        }
        public override void Update()
        {

            CheckForTranstion(CharacterStateEnum.Moving);
            CheckForTranstion(CharacterStateEnum.Jumping);
            CheckForTranstion(CharacterStateEnum.Dashing);
            CheckForTranstion(CharacterStateEnum.AirMovement);

        }
        public override void Enter()
        {
            motor.Velocity = Vector3.zero;
        }

        public override void Exit()
        {
            
        }

        public override bool IsReadyForTransition()
        {
            if (Mathf.Abs( motor.Velocity.x) <= 0.1f  && motor.OnGround)
                return true;
            return false;
        }

    

    }
}
