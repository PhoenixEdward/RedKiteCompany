using System;
using System.Collections.Generic;
using System.Text;

namespace RedKite
{
    [Serializable]
    public abstract class Item
    {
        public string Name;
        public int Range;
        public int Burden;
        public int Uses;

        protected System.Random rnd = new System.Random();


        public virtual void Use(GameSprite giver, GameSprite receiver)
        {
            Uses--;

            //could put some logic here to delete from inventory if excedded num uses.
        }

        public virtual void Trade(GameSprite giver, GameSprite receiver)
        {
            giver.RemoveItem(this);
            receiver.AddItem(this);
        }

    }
}
