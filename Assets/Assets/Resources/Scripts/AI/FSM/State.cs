using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedKite
{ 
    public interface  IState
    {
        void Enter(IState previous, GameSprite sprite);
        void Execute(GameSprite sprite);
        void Exit(GameSprite sprite);

        bool OnMessage(GameSprite sprite, Telegram message);
    }
}
