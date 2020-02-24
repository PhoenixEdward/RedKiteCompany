using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedKite
{
    [Serializable]
    public class Key : Item
    {
        public override void Use(GameSprite giver, GameSprite reciever)
        {
            //here it can send messages to the quest.
            base.Use(giver, reciever);
        }
    }
}
