using System;
using System.Collections.Generic;
using System.Text;

namespace RedKite
{
    [Serializable]
    public abstract class Item
    {
        public int Uses;

        public int DiceBonus;
        public int baseBonus;
        protected System.Random rnd = new System.Random();


        public virtual void Use(GameSprite giver, GameSprite reciever)
        {
        }

    }
}
