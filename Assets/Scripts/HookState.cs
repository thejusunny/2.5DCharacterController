using UnityEngine;
namespace Controller
{
    public class HookState : CharacterState
    {
        Vector2 inputXY;
        float stateTimer;
        int wallDirection;
        float hookSpeed=55;
        WallJumpData wallJumpData;
        float playerRadius = 3f;
        LayerMask hookLayer;
        Hook hookTarget;
        Vector2 hookDirection;
        float distanceToHook;
        float hoverDuration = 0.2f;
        float hookEndTime;
        bool reachedHook;
        Vector2 startPos;
        HookData data;
        public HookState(CharacterController controller, CharacterMotor motor,HookData data) : base(controller, motor)
        {
            this.data = data;
            hookLayer = 1<<9;
        }
        public override void Update()
        {
            stateTimer += Time.deltaTime;
            inputXY.x = Input.GetAxis("Horizontal");
            inputXY.y = Input.GetAxis("Vertical");
            float distanceTravelled = 0;
            if (!reachedHook&& (distanceTravelled = Vector3.Distance(startPos,controller.transform.position)) <distanceToHook)
            {
                motor.Velocity = hookDirection * data.HookTravelCurve.Evaluate(distanceTravelled/distanceToHook)* data.HookSpeed ;  
            }
            else
            {
                
                if (!reachedHook)
                {
                    reachedHook = true;
                    motor.EnableGravity();
                    motor.Velocity = hookDirection*data.HookSpeed/3f;
                    hookEndTime = Time.time;
                }
                if(Mathf.Abs(inputXY.x)>0)
                    motor.Velocity.x = Mathf.Lerp(motor.Velocity.x, inputXY.x * data.HookAirmovementSpeed, data.HookMovementRegainSpeed * Time.deltaTime);
            }
            if (reachedHook && Time.time > hookEndTime + hoverDuration)
            {
                controller.ChangeState(CharacterStateEnum.AirMovement);
                return;
            }
            

        }
        public float GetNormalizedInput(float input)
        {
            float valueToReturn = input > 0 ? 1 : input < 0 ? -1 : 0;

            return valueToReturn;
        }
        public override void Enter()
        {
            hookDirection = (hookTarget.transform.position - controller.transform.position).normalized;
            distanceToHook = (hookTarget.transform.position - controller.transform.position).magnitude;
            motor.DisableGravity();
            startPos = controller.transform.position;
        }
        public override void Exit()
        {
            stateTimer = 0f;
            hookTarget = null;
            hookEndTime = 0f;
            reachedHook = false;
        }
        public override bool IsReadyForTransition()
        {
            if (inputController.GetCommad(CommandType.Hook).IsPressed())
            {
                return CheckForHooksInRange();
            }
            return false;
        }
        private bool CheckForHooksInRange()
        {
            Collider[] colls = Physics.OverlapSphere(controller.transform.position, playerRadius,hookLayer);
            if(colls.Length>0)
            {
                Hook nearestHook = GetNearestHook(colls);
                if (nearestHook)
                {
                    hookTarget = nearestHook;
                    return true;
                }

            }
            return false;

        }
        private Hook GetNearestHook(Collider []colls)
        {
            Transform closestHook = colls[0].transform;
            float closestDistance = Vector2.Distance(controller.transform.position, closestHook.position);
            for (int i = 0; i < colls.Length; i++)
            {
                Transform currentHook = colls[i].transform;
                float currentDistance = Vector2.Distance(controller.transform.position, currentHook.transform.position);
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closestHook = colls[i].transform;
                }
            }
            return closestHook.GetComponent<Hook>();

        }
    }
}
