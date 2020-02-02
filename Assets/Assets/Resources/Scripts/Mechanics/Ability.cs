using System;
using System.Collections.Generic;
using System.Text;

namespace RedKite
{
    public class Ability
    {

        public int BaseStat { get; set; }

        private int _modifier;
        public int Modifier
        {
            get
            {
                Console.WriteLine("Current Buff: " + buff);
                return _modifier + buff;

            }
            private set
            {
                _modifier = value;
            }
        }

        int buff = 0;
        int buffDuration = 0;
        public bool Altered { get; private set; }

        System.Random rnd = new System.Random();

        public Ability()
        {
        }

        public void Increase(int roll)
        {
            BaseStat += roll;
            Modifier = (int)Math.Floor((BaseStat - 8f) / 2);
        }

        public int Roll( int dieSides)
        {
            int roll = rnd.Next(1, dieSides + 1) + Modifier;

            return roll;
        }

        public void Buff(int _buff, int _buffDuration)
        {
            Console.WriteLine("Buffed for: " + _buffDuration);
            buff = _buff;
            buffDuration = _buffDuration;
            Altered = true;
        }

        public void DecrementBuffDuration()
        {
            //needs to be adjusted to allow for stacking of buffs on abilitys. Maybe make buffs a list?
            buffDuration--;

            if (buffDuration <= 0)
            {
                Console.WriteLine("Buff Depleted");
                buff = 0;
                Altered = false;
            }
        }
    }
}
