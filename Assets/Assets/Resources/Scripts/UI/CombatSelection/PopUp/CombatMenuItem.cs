using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{
    public class CombatMenuItem : MonoBehaviour
    {
        CombatMenuPopUp menu;

        private Item _action;

        //maybe an enum for type of interaction?
        public Item Action
        {
            get
            {
                if (_action is Skill skill)
                    return skill;
                else
                    return (Key)_action;
            }

            set
            {
                if (value is Weapon weapon)
                    _action = weapon;
                else if (value is Buff buff)
                    _action = buff;
                else
                    _action = value;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            menu = GetComponentInParent<CombatMenuPopUp>();
        }

        public void SendToMenu()
        {
            if (Action is Weapon weapon)
                menu.UseSkill(weapon);
            else if (Action is Buff buff)
                menu.UseSkill(buff);
            else if(Action is Skill skill)
            {
                menu.UseSkill(skill);
            }
            else if(Action is Item item)
            {
                menu.UseSkill(item);
            }
        }
    }
}