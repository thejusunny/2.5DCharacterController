﻿using UnityEngine;
namespace Controller
{
    public class WallJumping : CharacterState
    {
        Vector2 inputXY;
        float groundExpiryTime = 0.1f;
        float stateTimer;
        int wallDirection;
        WallJumpData wallJumpData;
        public WallJumping(CharacterController controller, CharacterMotor motor,WallJumpData wallJumpData) : base(controller, motor)
        {
            this.wallJumpData = wallJumpData;
        }
        public override void Update()
        {
            stateTimer += Time.deltaTime;
            motor.Velocity = new Vector2(wallJumpData.WallJumpVelocity.x *wallDirection, wallJumpData.WallJumpVelocity.y);
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
            wallDirection = collision.WallDirection;
            inputController.GetCommad(CommandType.Jump).ClearFrameBuffer();
        }
        public override void Exit()
        {
            stateTimer = 0f;
        }
        public override bool IsReadyForTransition()
        {
        
            if (inputController.GetCommad(CommandType.Jump).IsPressed() && collision.OnWall)
                return true;
            return false;
        }
    }
}
