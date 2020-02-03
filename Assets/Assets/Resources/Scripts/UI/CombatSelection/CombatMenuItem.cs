using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{
    public class CombatMenuItem : MonoBehaviour
    {
        CombatMenuSkillScreen menu;

        private dynamic _action;
        public dynamic Action
        {
            get
            {
                return _action;
            }

            set
            {
                if (value is Weapon weapon)
                    _action = weapon;
                else if (value is Buff buff)
                    _action = buff;

            }
        }
        // Start is called before the first frame update
        void Start()
        {
            menu = GetComponentInParent<CombatMenuSkillScreen>();
        }

        public void SendToMenu()
        {
            if (Action is Weapon weapon)
                menu.UseSkill(weapon);
            else if (Action is Buff buff)
                menu.UseSkill(buff);
        }
    }
}