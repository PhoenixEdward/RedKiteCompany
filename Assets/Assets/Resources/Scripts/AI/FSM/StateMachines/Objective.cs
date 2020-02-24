using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedKite
{
    public class Objective
    {
        public GameSprite Owner;
        Quest quest;
        Type CurrentObjective;

        public Objective(GameSprite _actor, Quest _quest)
        {
            Owner = _actor;
            quest = _quest;
        }

        public void SetCurrentObjective(Type type)
        {
            CurrentObjective = type;
        }
    }

    public enum Type
    {
        Battle,
        Protect
    }
}
