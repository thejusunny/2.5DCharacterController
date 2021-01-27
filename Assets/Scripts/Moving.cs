using UnityEngine;
namespace Controller
{
    public class Moving : CharacterState
    {
        Vector2 inputXY;
        MoveData moveData;
        float timeToReachMaxSpeed = 0.1f;
        float stateTimer;
        public Moving(CharacterController controller,CharacterMotor motor, MoveData moveData) : base(controller, motor)
        {
            this.moveData = moveData;
        }
        public override void Update()
        {
            stateTimer += Time.deltaTime;
            inputXY.x = NormalizeXInput(Input.GetAxisRaw("Horizontal"));

            if (Mathf.Abs(inputXY.x) <= 0)
                motor.Velocity.x = 0;
            else
            {
                motor.Velocity.x = Mathf.Lerp(motor.Velocity.x, inputXY.x * moveData.MoveSpeed, moveData.MoveSpeed* (stateTimer/timeToReachMaxSpeed)  * Time.deltaTime);
            }
            motor.Velocity.y = 0;
            CheckForTranstion(CharacterStateEnum.Idle);
            CheckForTranstion(CharacterStateEnum.Jumping);
            CheckForTranstion(CharacterStateEnum.Dashing);
            CheckForTranstion(CharacterStateEnum.AirMovement);
            CheckForTranstion(CharacterStateEnum.Hooking);

        }
        public float NormalizeXInput(float inputX)
        {
            return Mathf.Abs(inputX) <= 0.01f ? 0 : Mathf.Sign(inputX);
        }
        public override void Enter(){}
        public override void Exit()
        {
            stateTimer = 0f;
        }

        public override bool IsReadyForTransition()
        {
            if (Mathf.Abs( Input.GetAxis("Horizontal"))>0 && collision.OnGround)
                return true;
            return false;
        }

    

    }
}
