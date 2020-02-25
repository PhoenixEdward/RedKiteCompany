using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace RedKite
{
    [Serializable]
    public class Buff: Skill
    {
        public int Duration;

        public Buff() { }

        public Buff(string _name, int _uses, bool _anti, int _diceBonus, int _baseBonus, Form _majorForm, Form _minorForm, int _range, int _burden, int _duration) 
            : base(_name, _uses, _anti, _diceBonus, _baseBonus, _majorForm, _minorForm, _range, _burden)
        {
            Duration = _duration;
        }

        public override void Use(GameSprite giver, GameSprite receiver)
        {

            base.Use(giver, receiver);

            int chanceSuccessMajor = giver.AbilityCheck(Type.Major);
            int chanceFailureMinor = receiver.AbilityCheck(Type.Minor);

            int statBonus;

            /*if(Type.Minor == Form.Brute)
                statBonus = giver.stats.dexterity.Modifier;
            else if (Type.Minor == Form.Finesse)
                statBonus = giver.stats.dexterity.Modifier;
            else if (Type.Minor == Form.Clever)
                statBonus = giver.stats.intelligence.Modifier;
            else if (Type.Minor == Form.Wise)
                statBonus = giver.stats.wisdom.Modifier;
            else*/

            statBonus = giver.Stats.Charisma.Roll(DiceBonus);

            int grossBonus = BaseBonus + statBonus;

            if (chanceFailureMinor < chanceSuccessMajor)
            {
                receiver.Buff(Type.Minor, grossBonus, Duration, Anti);

                Debug.Log(receiver.Name + " Strength Altered " + receiver.Stats.Strength.Altered);

                Debug.Log("Disarmed for: " + grossBonus + " To " + Type.Minor.ToString());
            }

            UseFX(this, grossBonus, receiver);
        }
    }
}
