using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Controllers
{
    public abstract class CharacterState
    {
        protected CharacterMotor motor;
        protected CharacterController controller;
        public abstract void Update();
        public abstract void Enter();
        public abstract void Exit();
        public void CheckForTranstion(CharacterStateEnum stateToCheck)
        {
            if (controller.GetState(stateToCheck).IsReadyForTransition())
                controller.ChangeState(stateToCheck);
        }
        public abstract bool IsReadyForTransition();
    }
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
            inputXY.x = Input.GetAxis("Horizontal");
            inputXY.y = Input.GetAxis("Vertical");
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
            if (Mathf.Abs( motor.Velocity.x) <= 0f  && motor.OnGround)
                return true;
            return false;
        }

    

    }
    public class Jumping : CharacterState
    {
        Vector2 inputXY;
        float jumpAirMovementSpeed = 4f;
        float jumpHeight=3f;
        float horizontalStartVelocityX;
        float currentJumpDistance;
        Vector3 jumpStartPosition;
        bool peekReached = false;
        float stateTimer;
        float groundExpiryTime = 0.1f;
        float maxDistanceOnXLeft;
        float maxDistanceOnXRight;
        float maxHorizontalDistance = 8f;
        float speedOffset = 0f;
        public Jumping(CharacterController controller, CharacterMotor motor)
        {
            this.motor = motor;
            this.controller = controller;
        }
        public override void Update()
        {
            stateTimer += Time.deltaTime;
            inputXY.x = GetNormalizedInput(Input.GetAxis("Horizontal"));
            if (Vector3.Distance(motor.GetPosition(), jumpStartPosition) >= jumpHeight)
                controller.ChangeState(CharacterStateEnum.AirMovement);
            motor.Velocity.x = inputXY.x * speedOffset;
            if (stateTimer > groundExpiryTime)
            {
                CheckForTranstion(CharacterStateEnum.Moving);
                CheckForTranstion(CharacterStateEnum.Idle);
                CheckForTranstion(CharacterStateEnum.Dashing);
            }
            Debug.DrawLine(new Vector3(maxDistanceOnXLeft, 0f, 0f), new Vector3(maxDistanceOnXLeft,2f,0f),Color.red);
            Debug.DrawLine(new Vector3(maxDistanceOnXRight, 0f, 0f), new Vector3(maxDistanceOnXRight, 2f, 0f), Color.red);
        }
        public float GetNormalizedInput(float input)
        {
            float valueToReturn = input > 0 ? 1 : input < 0 ? -1 : 0;

            return valueToReturn;
        }
        public override void Enter()
        {
            horizontalStartVelocityX = Mathf.Abs( motor.Velocity.x);
            float TargetVelocity = Mathf.Sqrt(-2f * jumpHeight * motor.GetGravity().y);
            motor.Velocity = Vector3.up* TargetVelocity;
            float timeToLand = Mathf.Sqrt(Mathf.Abs( (2 * jumpHeight) / motor.GetGravity().y))*2;
            speedOffset = Mathf.Clamp((jumpAirMovementSpeed + horizontalStartVelocityX), 0, maxHorizontalDistance);
            maxDistanceOnXLeft = (motor.GetPosition().x - speedOffset*timeToLand);
            maxDistanceOnXRight = (motor.GetPosition().x + speedOffset * timeToLand);
            jumpStartPosition = motor.GetPosition();
            currentJumpDistance = 0f;
        }
        public override void Exit()
        {
            horizontalStartVelocityX = 0f;
            peekReached = false;
            stateTimer = 0f;
        }
        public override bool IsReadyForTransition()
        {
            if (Input.GetButtonDown("Jump")&&motor.OnGround)
                return true;
            return false;
        }

    }
    public class Dashing : CharacterState
    {
        Vector2 inputXY;

        float dashDistance = 10f;
        float dashDuration = 0.17f;
        float dashTimer;
        float prevDistance;
        float xVelocityEnd;
        float internalDashVelocity;
        float dashCoolDownTime = 0.5f;
        float lastdashTimeStamp;
        Vector3 dashDirection;
        public Dashing(CharacterController controller, CharacterMotor motor)
        {
            this.motor = motor;
            this.controller = controller;
        }
        public override void Update()
        {
  
            if (dashTimer <= dashDuration)
            {
                float currentDistance = (dashTimer / dashDuration) * dashDistance ;
                float distanceOffset = currentDistance - prevDistance;
                internalDashVelocity = distanceOffset / Time.deltaTime;
                motor.MoveExact(distanceOffset * dashDirection);
                prevDistance = currentDistance;
                xVelocityEnd = motor.Velocity.x/2;
            }
            else
            {
                
                motor.EnableGravity();
                controller.ChangeState(CharacterStateEnum.AirMovement);
                //CheckForTranstion(CharacterStateEnum.Moving);
                //CheckForTranstion(CharacterStateEnum.Idle);
            }
            dashTimer += Time.deltaTime;
        }
        public override void Enter()
        {
            inputXY.x = Input.GetAxis("Horizontal");
            inputXY.y = Input.GetAxis("Vertical");
            inputXY.Normalize();
            dashDirection = inputXY;
            motor.DisableGravity();
            motor.Velocity = Vector3.zero;
        }
        public override void Exit()
        {
            motor.Velocity = dashDirection * internalDashVelocity/4;
            lastdashTimeStamp = Time.time;
            dashTimer = 0f;
            prevDistance = 0f;
        }
        public override bool IsReadyForTransition()
        {
            if (Input.GetButtonDown("Dash")&& Time.time > lastdashTimeStamp+dashCoolDownTime)
                return true;
            return false;
        }

    }
    public class Airmovement : CharacterState
    {
        Vector2 inputXY;
        float airMovementSpeed = 5f;
        float maxHorizontalDistance =8f;
        public Airmovement(CharacterController controller, CharacterMotor motor)
        {
            this.motor = motor;
            this.controller = controller;
        }
        public override void Update()
        {
            inputXY.x = Input.GetAxis("Horizontal");
            inputXY.y = Input.GetAxis("Vertical");
            motor.Velocity.x = inputXY.x * airMovementSpeed;
            CheckForTranstion(CharacterStateEnum.Moving);
            CheckForTranstion(CharacterStateEnum.Idle);
            CheckForTranstion(CharacterStateEnum.Dashing);

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
