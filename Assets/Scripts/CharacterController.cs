using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public enum CharacterStateEnum
    {
        Idle, Moving, Jumping, Dashing, AirMovement
    };
    public class CharacterController : MonoBehaviour
    {

        private CharacterGravity gravityModule;
        private CharacterMotor motor;
        private Dictionary<CharacterStateEnum, CharacterState> states;
        [SerializeField] private CharacterStateEnum currentState;

        public CharacterStateEnum CurrentState { get => currentState;  }

        private void Start()
        {
            motor = GetComponent<CharacterMotor>();
            gravityModule = GetComponent<CharacterGravity>();
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
        }

        private void Update()
        {
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
