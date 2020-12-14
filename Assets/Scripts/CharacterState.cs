using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Controllers
{
    public abstract class CharacterState
    {
        protected CharacterController ccModule;
        public abstract void Update();
        public abstract void Enter();
        public abstract void Exit();
        public void CheckForTranstion(CharacterStateEnum stateToCheck)
        {
            if (ccModule.GetState(stateToCheck).IsReadyForTransition())
                ccModule.ChangeState(stateToCheck);
        }
        public abstract bool IsReadyForTransition();
    }
    public class Moving : CharacterState
    {
        Vector2 inputXY;
        float moveSpeed = 10f;
        public Moving(CharacterController cc)
        {
            this.ccModule = cc;
        }
        public override void Update()
        {
            inputXY.x = Input.GetAxis("Horizontal");
            inputXY.y = Input.GetAxis("Vertical");
            ccModule.SetMoveOffset(new Vector3(inputXY.x, 0f, inputXY.y) * moveSpeed);
            //CheckForTranstion(CharacterStateEnum.Idle);
            CheckForTranstion(CharacterStateEnum.Jumping);
            CheckForTranstion(CharacterStateEnum.Dashing);

        }
        public override void Enter()
        {

        }

        public override void Exit()
        {
            
        }

        public override bool IsReadyForTransition()
        {
            if (Mathf.Abs( ccModule.Velocity.x) >= 0&& ccModule.OnGround)
                return true;
            return false;
        }

    

    }
    public class Idle : CharacterState
    {
        Vector2 inputXY;
        float moveSpeed = 10f;
        public Idle(CharacterController cc)
        {
            this.ccModule = cc;
        }
        public override void Update()
        {
           
            ccModule.SetMoveOffset(Vector3.zero);
            CheckForTranstion(CharacterStateEnum.Moving);
            CheckForTranstion(CharacterStateEnum.Jumping);
            CheckForTranstion(CharacterStateEnum.Dashing);

        }
        public override void Enter()
        {

        }

        public override void Exit()
        {
            
        }

        public override bool IsReadyForTransition()
        {
            if (ccModule.Velocity.magnitude <= 0&&ccModule.OnGround)
                return true;
            return false;
        }

    

    }
    public class Jumping : CharacterState
    {
        Vector2 inputXY;
        float jumpAirMovementSpeed = 10f;
        float jumpHeight=5f;
        public Jumping(CharacterController cc)
        {
            this.ccModule = cc;
        }
        public override void Update()
        {

            inputXY.x = Input.GetAxis("Horizontal");
            inputXY.y = Input.GetAxis("Vertical");
            ccModule.SetMoveOffset(new Vector3(inputXY.x, 0f, inputXY.y) * jumpAirMovementSpeed);
            CheckForTranstion(CharacterStateEnum.Moving);

        }
        public override void Enter()
        {
            float TargetVelocity = Mathf.Sqrt(-2f * jumpHeight * ccModule.GetGravity().y);
            ccModule.SetVerticalVelocity(TargetVelocity);
        }
        public override void Exit()
        {

        }
        public override bool IsReadyForTransition()
        {
            if (Input.GetKey(KeyCode.Space)&&ccModule.OnGround)
                return true;
            return false;
        }

    }
    public class Dashing : CharacterState
    {
        Vector2 inputXY;

        float dashDistance = 10f;
        float dashDuration = 0.1f;
        float dashTimer;
        float prevDistance;
        public Dashing(CharacterController cc)
        {
            this.ccModule = cc;
        }
        public override void Update()
        {
   
            Vector3 dashDirection = inputXY.normalized;
            if (dashTimer <= dashDuration)
            {
                float currentDistance = (dashTimer / dashDuration) * dashDistance ;
                float distanceOffset = currentDistance - prevDistance;
                ccModule.SetMovePosition(distanceOffset * dashDirection);
                prevDistance = currentDistance;
            }
            else
            {
                ccModule.EnableGravity();
                CheckForTranstion(CharacterStateEnum.Moving);
                CheckForTranstion(CharacterStateEnum.Idle);
            }
            dashTimer += Time.deltaTime;
        }
        public override void Enter()
        {
            inputXY.x = Input.GetAxis("Horizontal");
            inputXY.y = Input.GetAxis("Vertical");
            inputXY.Normalize();
            ccModule.DisableGravity();
        }
        public override void Exit()
        {
            dashTimer = 0f;
            prevDistance = 0f;
  
        }
        public override bool IsReadyForTransition()
        {
            if (Input.GetKey(KeyCode.LeftShift))
                return true;
            return false;
        }

    }
}
