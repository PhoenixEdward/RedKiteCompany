using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RedKite
{
    [Serializable]
    public class Skill : Item
    {
        public enum Form
        {
            None,
            Brute,
            Stoic,
            Finesse,
            Clever,
            Wise,
            Charming
        }


        public string Name;
        public FormCombo Type;

        public bool Anti;

        public int Range;

        public int Burden;

        public static Skill Wait = new Skill("Wait", 1000, false, 0, 0, Form.None, Form.None, 0, 0);
        public static Skill Alert = new Skill("Alert", 1000, false, 0, 0, Form.None, Form.None, 0, 0);

        public Skill() { }

        public Skill(string _name, int _uses, bool _anti, int _diceBonus, int _baseBonus, Form _majorForm, Form _minorForm, int _range, int _burden)
        {
            DiceBonus = _diceBonus;
            baseBonus = _baseBonus;
            Type = new FormCombo(_majorForm, _minorForm);
            Uses = _uses;
            Anti = _anti;
            Name = _name;
            Range = _range;
            Burden = _burden;
            //maybe make this a static variable?
            rnd = new Random();
        }

        public override void Use(GameSprite giver, GameSprite receiver)
        {
            GameSprite.IsUsingSkill = true;
            Uses--;
        }

        [Serializable]
        public class FormCombo
        {
            public Form Major;
            public Form Minor;

            public FormCombo()
            {
                Major = Form.None;
                Minor = Form.None;
            }

            public FormCombo(Form _major, Form _minor)
            {
                Major = _major;
                Minor = _minor;
            }
        }

    }
}
