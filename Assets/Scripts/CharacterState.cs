using System.Collections;
using System.Collections.Generic;
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
}
