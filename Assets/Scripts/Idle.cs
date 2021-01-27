using UnityEngine;
namespace Controller
{
    public class Idle : CharacterState
    {
        Vector2 inputXY;
        float moveSpeed = 10f;
        public Idle(CharacterController controller,CharacterMotor motor) : base(controller, motor){}
        public override void Update()
        {

            CheckForTranstion(CharacterStateEnum.Moving);
            CheckForTranstion(CharacterStateEnum.Jumping);
            CheckForTranstion(CharacterStateEnum.Dashing);
            CheckForTranstion(CharacterStateEnum.AirMovement);
            CheckForTranstion(CharacterStateEnum.Hooking);

        }
        public override void Enter()
        {
            motor.Velocity = Vector3.zero;
        }

        public override void Exit(){   }
        public override bool IsReadyForTransition()
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) <= 0 && collision.OnGround)
                return true;
            return false;
        }

    

    }
}
