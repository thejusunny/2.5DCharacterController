using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{

    [System.Serializable]
    public class JumpData
    {
        [SerializeField]private float jumpAirMovementSpeed = 10f;
        [SerializeField] private float jumpHeight = 5f;
        [SerializeField] private float maxHorizontalDistance = 20f;

        public float JumpAirMovementSpeed { get => jumpAirMovementSpeed;  }
        public float JumpHeight { get => jumpHeight; }
        public float MaxHorizontalDistance { get => maxHorizontalDistance; }
    }
    [System.Serializable]
    public class MoveData
    {
        [SerializeField] private float moveSpeed = 8f;
        public float MoveSpeed { get => moveSpeed;  }
    }
    [System.Serializable]
    public class DashData
    {
        [SerializeField] private float dashDistance = 5f;
        [SerializeField] private float dashDuration = 0.08f;

        public float DashDistance { get => dashDistance;  }
        public float DashDuration { get => dashDuration; }
    }
    [System.Serializable]
    public class AirmovementData
    {
        [SerializeField] private float airMovementSpeed = 10f;
        [SerializeField] private float maxHorizontalDistance = 20f;
        [SerializeField] private float movementRegainSpeed = 8f;

        public float AirMovementSpeed { get => airMovementSpeed;  }
        public float MaxHorizontalDistance { get => maxHorizontalDistance;}
        public float MovementRegainSpeed { get => movementRegainSpeed;}
    }
    [System.Serializable]
    public class HookData
    {
        [SerializeField] private float hookSpeed = 10f;
        [SerializeField] private float hookMovementRegainSpeed = 5;
        [SerializeField] private float hookAirmovementSpeed;
        [SerializeField] AnimationCurve hookTravelCurve;

        public float HookSpeed { get => hookSpeed;  }
        public float HookMovementRegainSpeed { get => hookMovementRegainSpeed;  }
        public float HookAirmovementSpeed { get => hookAirmovementSpeed;  }
        public AnimationCurve HookTravelCurve { get => hookTravelCurve;  }
    }
    [System.Serializable]
    public class WallJumpData
    {
        [SerializeField]Vector2 wallJumpVelocity = new Vector2(-20, 17f);

        public Vector2 WallJumpVelocity { get => wallJumpVelocity; }
    }
    public enum CharacterStateEnum
    {
        Idle, Moving, Jumping, Dashing, AirMovement, WallJumping, Hooking
    };
    public class CharacterController : MonoBehaviour
    {
        private CharacterGravity gravityModule;
        private CharacterMotor motor;
        private InputController inputController;
        private Dictionary<CharacterStateEnum, CharacterState> states;
        [SerializeField] private CharacterStateEnum currentState;
        [SerializeField] private CharacterCollision characterCollision;
        private CharacterStateEnum prevState;
        public CharacterStateEnum CurrentState { get => currentState;  }
        public InputController InputController { get => inputController; }
        public CharacterCollision CharacterCollision { get => characterCollision; }

        #region StateDataObjects
        [SerializeField] private MoveData moveData;
        [SerializeField] private JumpData jumpData;
        [SerializeField] private DashData dashData;
        [SerializeField] private AirmovementData airmovementData;
        [SerializeField] private WallJumpData wallJumpData;
        [SerializeField] private HookData hookData;
        #endregion

        private void Start()
        {
            motor = GetComponent<CharacterMotor>();
            gravityModule = GetComponent<CharacterGravity>();
            inputController = GetComponent<InputController>();
            characterCollision.Init(this.transform);
            motor.Init(characterCollision);
            BindStates();
            currentState = CharacterStateEnum.Moving;
        }
        private void BindStates()
        {
            states = new Dictionary<CharacterStateEnum, CharacterState>();
            states.Add(CharacterStateEnum.Moving, new Moving(this, motor,moveData));
            states.Add(CharacterStateEnum.Idle, new Idle(this, motor));
            states.Add(CharacterStateEnum.Jumping, new Jumping(this, motor,jumpData));
            states.Add(CharacterStateEnum.Dashing, new Dashing(this, motor,dashData));
            states.Add(CharacterStateEnum.AirMovement, new Airmovement(this, motor,airmovementData));
            states.Add(CharacterStateEnum.WallJumping, new WallJumping(this, motor,wallJumpData));
            states.Add(CharacterStateEnum.Hooking, new HookState(this, motor,hookData));
        }

        private void Update()
        {
            InputController.ReadInputs();
            CharacterStateEnum executingState = currentState;
            ExecuteState();
            if (executingState != currentState)
            {
                return;
            }
            characterCollision.ProcessCollisions();
            motor.UpdateMotor();
        }
        public CharacterState GetState(CharacterStateEnum stateToGet)
        {
            return states[stateToGet];
        }
        public CharacterStateEnum GetPrevState() { return prevState; }

        void ExecuteState()
        {
            states[CurrentState].Update();
        }
        public void ChangeState(CharacterStateEnum targetState)
        {
            states[CurrentState].Exit();
            prevState = currentState;
            currentState = targetState;
            states[CurrentState].Enter();
        }
    }
}
