using System;
using System.Collections.Generic;
using System.Text;

namespace RedKite
{
    public abstract class Item
    {
        public int Uses { get; set; }

        public int DiceBonus { get; set; }
        public int baseBonus { get; set; }
        protected System.Random rnd;


        public virtual void Use(Unit giver, Unit reciever)
        {
        }
    }
}
