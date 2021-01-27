using UnityEngine;
namespace Controller
{
    public class Airmovement : CharacterState
    {
        Vector2 inputXY;
        AirmovementData airmovementData;
        float newAirmovementSpeed;
        float gravityMultiplier=3.5f;
        public Airmovement(CharacterController controller, CharacterMotor motor, AirmovementData airmovementData) : base(controller, motor)
        {
            this.airmovementData = airmovementData;
        }
        public override void Update()
        {
            inputXY.x = Input.GetAxis("Horizontal");
            inputXY.y = Input.GetAxis("Vertical");
            if(controller.GetPrevState() == CharacterStateEnum.Jumping)
                motor.Velocity.y += Physics2D.gravity.y * (gravityMultiplier - 1) * Time.deltaTime;
            if (controller.GetPrevState() == CharacterStateEnum.Hooking)
            {
                if(Mathf.Abs(inputXY.x)>0)
                motor.Velocity.x = Mathf.Lerp(motor.Velocity.x, inputXY.x * newAirmovementSpeed, airmovementData.MovementRegainSpeed * Time.deltaTime);
            }
            else
                motor.Velocity.x = Mathf.Lerp(motor.Velocity.x, inputXY.x * newAirmovementSpeed, airmovementData.MovementRegainSpeed * Time.deltaTime);
            CheckForTranstion(CharacterStateEnum.Moving);
            CheckForTranstion(CharacterStateEnum.Idle);
            CheckForTranstion(CharacterStateEnum.Dashing);
            CheckForTranstion(CharacterStateEnum.WallJumping);
            CheckForTranstion(CharacterStateEnum.Hooking);
        }
        public override void Enter()
        {
            motor.EnableGravity();
            newAirmovementSpeed = Mathf.Clamp((airmovementData.AirMovementSpeed + Mathf.Abs( motor.Velocity.x)), 0, airmovementData.MaxHorizontalDistance);
        }
        public override void Exit() { }
        public override bool IsReadyForTransition()
        {
            if (controller.CurrentState!= CharacterStateEnum.Jumping && controller.CurrentState!= CharacterStateEnum.Dashing && !collision.OnGround)
                return true;
            return false;
        }
    }
}
