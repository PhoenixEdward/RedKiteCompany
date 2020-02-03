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


        public override void Use(Unit giver, Unit receiver)
        {
            int chanceOfSuccess = giver.OffensiveRoll(Type.Major);
            int chanceOfFailure = receiver.DefensiveRoll();

            if (chanceOfFailure > chanceOfSuccess)
            {
                Debug.Log("Miss");
                return;
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

            receiver.ChangeHealth(grossBonus, Anti);

            base.Use(giver, receiver);
        }
    }
}
