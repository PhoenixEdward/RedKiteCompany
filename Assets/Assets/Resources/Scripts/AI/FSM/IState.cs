

namespace RedKite
{ 
    public interface  IState
    {
        void Enter(IState previous, Objective objective);
        void Execute(Objective objective);
        void Exit(Objective objective);

        bool OnMessage(Objective objective, Telegram message);
    }
}
