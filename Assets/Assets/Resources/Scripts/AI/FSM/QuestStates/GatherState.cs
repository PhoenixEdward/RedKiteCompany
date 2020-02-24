using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedKite
{
    public class GatherState : IState
    {
        bool[] collectedItems;

        public void Enter(IState previous, Objective objective)
        {

        }

        public void Exit(Objective objective)
        {

        }

        public void Execute(Objective objective)
        {

        }

        public bool OnMessage(Objective objective, Telegram telegram)
        {
            return false;
        }
    }
}
