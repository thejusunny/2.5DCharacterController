using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public enum CharacterStateEnum
    {
        Idle, Moving, Jumping, Dashing, AirMovement, WallJumping
    };
    public class CharacterController : MonoBehaviour
    {

        private CharacterGravity gravityModule;
        private CharacterMotor motor;
        private InputController inputController;
        private Dictionary<CharacterStateEnum, CharacterState> states;
        [SerializeField] private CharacterStateEnum currentState;

        public CharacterStateEnum CurrentState { get => currentState;  }
        public InputController InputController { get => inputController; }

        private void Start()
        {
            motor = GetComponent<CharacterMotor>();
            gravityModule = GetComponent<CharacterGravity>();
            inputController = GetComponent<InputController>();
            BindStates();
            currentState = CharacterStateEnum.Moving;
        }
        private void BindStates()
        {
            states = new Dictionary<CharacterStateEnum, CharacterState>();
            states.Add(CharacterStateEnum.Moving, new Moving(this, motor));
            states.Add(CharacterStateEnum.Idle, new Idle(this, motor));
            states.Add(CharacterStateEnum.Jumping, new Jumping(this, motor));
            states.Add(CharacterStateEnum.Dashing, new Dashing(this, motor));
            states.Add(CharacterStateEnum.AirMovement, new Airmovement(this, motor));
            states.Add(CharacterStateEnum.WallJumping, new WallJumping(this, motor));
        }

        private void Update()
        {
            InputController.ReadInputs();
            ExecuteState();
            motor.UpdateMotor();
        }
        public CharacterState GetState(CharacterStateEnum stateToGet)
        {
            return states[stateToGet];
        }

        void ExecuteState()
        {
            states[CurrentState].Update();
        }
        public void ChangeState(CharacterStateEnum targetState)
        {
            states[CurrentState].Exit();
            currentState = targetState;
            states[CurrentState].Enter();
        }

    }

}
