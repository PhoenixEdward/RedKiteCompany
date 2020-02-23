using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RedKite
{
    [Serializable]
    public class StateMachine
    {
        // reference to associated sprite
        GameSprite owner;

     

        public IState currentState { get; private set; }

        public IState previousState { get; private set; }

        public static IState globalState { get; set; }

        void SetCurrentState(IState _state){ currentState = _state; }
        void SetGlobalState(IState _state) { globalState = _state; }
        void SetPreviousState(IState _state) { previousState = _state; }

        public StateMachine(GameSprite _owner)
        {
            currentState = new IdleState();
            owner = _owner;
        }

        public void Upate()
        {
            if (globalState != null)
                globalState.Execute(owner);

            if (currentState != null)
                currentState.Execute(owner);

        }

        public void ChangeState(IState newState)
        {
            previousState = currentState;

            previousState.Exit(owner);

            currentState = newState;

            currentState.Enter(previousState, owner);
        }

        public void RevertToPreviousState()
        {
            currentState = previousState;

            previousState.Exit(owner);

            currentState.Enter(previousState, owner);
        }

        public bool HandleMessage(Telegram message)
        {
            if(currentState != null)
                if (currentState.OnMessage(owner, message))
                    return true;

            
            if(globalState != null)
                if (globalState.OnMessage(owner, message))
                    return true;

            return false;
        }

    }
}
