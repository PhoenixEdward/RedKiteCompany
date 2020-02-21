using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace RedKite
{
    [Serializable]
    public class Weapon : Skill
    {
        Weapon() { }
        public Weapon(string _name, int _uses, bool _anti, int _diceBonus, int _baseBonus, Form _majorForm, Form _minorForm, int _range, int _burden) 
            : base(_name, _uses, _anti, _diceBonus, _baseBonus, _majorForm, _minorForm, _range, _burden) { }


        public override void Use(GameSprite giver, GameSprite receiver)
        {
            //will need to replace later with some clearer way of identifying than anti weapon
            if(receiver is Unit unit)
            { 
                if(!Anti)
                { 
                    int chanceOfSuccess = giver.OffensiveRoll(Type.Major);
                    int chanceOfFailure = unit.DefensiveRoll();

                    if (chanceOfFailure > chanceOfSuccess)
                    {
                        UseFX(this, 0, receiver);
                        return;
                    }
                }
            }

            int statBonus;

            if (Type.Major == Form.Finesse)
                statBonus = giver.Stats.Dexterity.Modifier;
            else if (Type.Major == Form.Clever)
                statBonus = giver.Stats.Intelligence.Modifier;
            else if (Type.Major == Form.Wise)
                statBonus = giver.Stats.Wisdom.Modifier;
            else
                statBonus = giver.Stats.Strength.Modifier;

            int grossBonus = rnd.Next(0, DiceBonus) + baseBonus + statBonus;

            Debug.Log("Damage: " + grossBonus);
            Debug.Log("Stat Bonus: " + statBonus);
            Debug.Log("Enemy Health: " + receiver.Health);
            int change = receiver.ChangeHealth(grossBonus, Anti);

            UseFX(this, change, receiver);

            base.Use(giver, receiver);
        }
    }
}
