using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedKite
{
    public interface IStateMachine
    {
        IState currentState { get; set; }

        IState previousState { get; set; }

        void SetCurrentState(IState _state);
        void SetPreviousState(IState _state);

        void Upate();
        void ChangeState(IState newState);

        void RevertToPreviousState();

        bool HandleMessage(Telegram message);
    }
}
