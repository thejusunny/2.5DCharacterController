using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Controller
{
    public abstract class CharacterState
    {
        protected CharacterMotor motor;
        protected CharacterController controller;
        protected CharacterCollision collision;
        protected InputController inputController;
        public abstract void Update();
        public abstract void Enter();
        public abstract void Exit();
        public CharacterState(CharacterController controller, CharacterMotor motor)
        {
            this.controller = controller;
            this.motor = motor;
            this.collision = controller.CharacterCollision;
            this.inputController = controller.InputController;
        }
        public void CheckForTranstion(CharacterStateEnum stateToCheck)
        {
            if (controller.GetState(stateToCheck).IsReadyForTransition())
                controller.ChangeState(stateToCheck);
        }
        public abstract bool IsReadyForTransition();
    }
}
