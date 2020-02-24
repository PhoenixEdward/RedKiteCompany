using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RedKite
{
    public class StateMachine : IStateMachine
    {
        // reference to associated sprite
        GameSprite owner;

        Objective objective;

        public IState currentState { get; set; }

        public IState previousState { get; set; }

        public static IState globalState { get; set; }

        public void SetCurrentState(IState _state){ currentState = _state; }
        public void SetGlobalState(IState _state) { globalState = _state; }
        public void SetPreviousState(IState _state) { previousState = _state; }

        public StateMachine(GameSprite _owner)
        {
            currentState = new IdleState();
            objective = new Objective(_owner, QuestMapper.Instance.CurrentQuest);
        }

        public StateMachine(GameSprite _owner, Quest _quest)
        {
            currentState = QuestMapper.Instance.LookupState();
            objective = new Objective(_owner, _quest);
        }

        public void Upate()
        {
            if (globalState != null)
                globalState.Execute(objective);

            if (currentState != null)
                currentState.Execute(objective);

        }

        public void ChangeState(IState newState)
        {
            previousState = currentState;

            previousState.Exit(objective);

            currentState = newState;

            currentState.Enter(previousState, objective);
        }

        public void RevertToPreviousState()
        {
            currentState = previousState;

            previousState.Exit(objective);

            currentState.Enter(previousState, objective);
        }

        public bool HandleMessage(Telegram message)
        {
            if(currentState != null)
                if (currentState.OnMessage(objective, message))
                    return true;

            
            if(globalState != null)
                if (globalState.OnMessage(objective, message))
                    return true;

            return false;
        }

    }
}
