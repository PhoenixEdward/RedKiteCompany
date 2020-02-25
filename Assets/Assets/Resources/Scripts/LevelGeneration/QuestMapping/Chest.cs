using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedKite
{
    public class Chest : Prop
    {
        Item item;
        public bool IsLocked { get; private set; } = false;

        public override void Start()
        {
            base.Start();

            IsInteractable = true;
        }

        public override void Update()
        {
            base.Update();
        }

        public void StowItem(Item _item)
        {
            item = _item;
        }

        public void Open(GameSprite sprite)
        {
            sprite.AddItem(item);

            CombatMenu.DisplayFlashMessage(new FlashMessage(sprite.Name + " Obtained " + item.Name, Colors.AztecGold)); 

            gameObject.SetActive(false);
        }
    }
}
