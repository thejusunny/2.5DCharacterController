using UnityEngine;
namespace Controller
{
    public class Dashing : CharacterState
    {
        Vector2 inputXY;
        float dashTimer;
        float prevDistance;
        float dashCoolDownTime = 0.5f;
        float lastdashTimeStamp;
        Vector3 dashDirection;
        DashData dashData;
        float dashSpeed;
        Vector3 originalDashDirection;
        bool ceilingExit;
        public Dashing(CharacterController controller, CharacterMotor motor,DashData dashData):base(controller,motor)
        { 
            this.dashData = dashData;
            this.dashSpeed = dashData.DashDistance / dashData.DashDuration;
        }
        public override void Update()
        {
            if (dashTimer <= dashData.DashDuration)
            {
                if (collision.OnCeiling)
                {

                    RaycastHit hitInfo = collision.RightCelingHit ? collision.RightCeilingRayHit : collision.LeftCelingHit ? collision.LeftCeilingRayHit : new RaycastHit();
                    Vector2 dashDirectionAlongNormal = Vector2.zero;
                    if (originalDashDirection.x < 0)//Left dash
                    {
                        if (hitInfo.transform != null)
                        {
                            dashDirectionAlongNormal = Vector3.ProjectOnPlane(Vector3.right * -1, hitInfo.normal);
                      
                        }
                        Debug.DrawRay(motor.GetPosition(), dashDirectionAlongNormal * 5f);
                        //Debug.Break();
                        Debug.Log("LeftDashCorrection");
                    }
                    else if (originalDashDirection.x > 0)//Right dash
                    {
                        if (hitInfo.transform != null)
                        {
                            dashDirectionAlongNormal = Vector3.ProjectOnPlane(Vector3.right * 1f, hitInfo.normal);
                        }
                        Debug.DrawRay(motor.GetPosition(), dashDirectionAlongNormal * 5f);
                        Debug.DrawRay(motor.GetPosition(), dashDirectionAlongNormal * 5f);
                        Debug.Log("RightDashCorrection");
                    }
                    dashDirection = dashDirectionAlongNormal;
                }
                else
                    dashDirection = originalDashDirection;
                float currentDistance = (dashTimer / dashData.DashDuration) * dashData.DashDistance ;
                float distanceOffset = currentDistance - prevDistance;
                motor.MoveExact(distanceOffset * dashDirection);
                prevDistance = currentDistance;
            }
            else
            {
                motor.EnableGravity();
                controller.ChangeState(CharacterStateEnum.AirMovement);
                return;
            }
            dashTimer += Time.deltaTime;
        }
        public override void Enter()
        {
            inputXY = inputController.GetCommad(CommandType.DashDirection).GetAxis();
            inputXY = CorrectedInput(inputXY);
            inputXY.Normalize();// keyboard
            if (inputXY.magnitude <= 0)
                inputXY.x = 1;
            dashDirection = inputXY;
            originalDashDirection = dashDirection;
            motor.DisableGravity();
            motor.Velocity = Vector3.zero;
            inputController.GetCommad(CommandType.DashDirection).ClearFrameBuffer();
        }
        public Vector2 CorrectedInput(Vector2 input)
        {
            Vector2 newInput;
            newInput.x = input.x < -0.3 ? -1 : input.x > 0.3f ? 1 : 0;
            newInput.y = input.y < -0.3 ? -1 : input.y > 0.3f ? 1 : 0;
            return newInput;
        }
        public override void Exit()
        {
            if(!ceilingExit)
                motor.Velocity = dashDirection * dashSpeed/2.5f;
            lastdashTimeStamp = Time.time;
            dashTimer = 0f;
            prevDistance = 0f;
            dashDirection = Vector3.zero;
            ceilingExit = false;
        }
        public override bool IsReadyForTransition()
        {
            if (inputController.GetCommad(CommandType.Dash).IsPressed()&& Time.time > lastdashTimeStamp+dashCoolDownTime)
                return true;
            return false;
        }

    }
}
