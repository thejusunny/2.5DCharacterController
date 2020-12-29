using UnityEngine;
namespace Controllers
{
    public class WallJumping : CharacterState
    {
        Vector2 inputXY;
        float jumpAirMovementSpeed = 7f;
        float groundExpiryTime = 0.1f;
        float stateTimer;
        int wallDirection;
        Vector2 wallJumpVelocity = new Vector2(-20, 17f);
        InputController inputController;
        public WallJumping(CharacterController controller, CharacterMotor motor)
        {
            this.motor = motor;
            this.controller = controller;
            inputController = controller.InputController;
        }
        public override void Update()
        {
            stateTimer += Time.deltaTime;
            motor.Velocity = new Vector2( wallJumpVelocity.x *wallDirection,wallJumpVelocity.y);
            if (stateTimer > groundExpiryTime)
            {
                CheckForTranstion(CharacterStateEnum.Moving);
                CheckForTranstion(CharacterStateEnum.Idle);
                CheckForTranstion(CharacterStateEnum.Dashing);
                CheckForTranstion(CharacterStateEnum.AirMovement);
            }
      
        }
        public float GetNormalizedInput(float input)
        {
            float valueToReturn = input > 0 ? 1 : input < 0 ? -1 : 0;

            return valueToReturn;
        }
        public override void Enter()
        {
            wallDirection = motor.WallDirection;
            inputController.JumpCommand.ClearFrameBuffer();
        }
        public override void Exit()
        {
            stateTimer = 0f;
        }
        public override bool IsReadyForTransition()
        {
        
            if (inputController.JumpCommand.isPressed && motor.OnWall)
                return true;
            return false;
        }
    }
}
