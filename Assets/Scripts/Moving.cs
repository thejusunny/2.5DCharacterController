using UnityEngine;
namespace Controllers
{
    public class Moving : CharacterState
    {
        Vector2 inputXY;
        MoveData moveData;
        public Moving(CharacterController controller,CharacterMotor motor, MoveData moveData)
        {
            this.motor = motor;
            this.controller = controller;
            this.moveData = moveData;
        }
        public override void Update()
        {
            inputXY.x = NormalizeXInput(Input.GetAxisRaw("Horizontal"));
            if (Mathf.Abs(inputXY.x) <= 0)
                motor.Velocity.x = 0;
            else 
            motor.Velocity.x = Mathf.Lerp(motor.Velocity.x, inputXY.x * moveData.MoveSpeed, moveData.MoveSpeed * Time.deltaTime);
            CheckForTranstion(CharacterStateEnum.Idle);
            CheckForTranstion(CharacterStateEnum.Jumping);
            CheckForTranstion(CharacterStateEnum.Dashing);
            CheckForTranstion(CharacterStateEnum.AirMovement);

        }
        public float NormalizeXInput(float inputX)
        {
            return Mathf.Abs(inputX) <= 0.01f ? 0 : inputX;
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
