using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RedKite
{
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


        public string Name { get; set; }
        public FormCombo Type { get; set; }

        public bool Anti { get; set; }

        public int Range { get; set; }


        public Skill() { }

        [JsonConstructor]
        public Skill(string _name, int _uses, bool _anti, int _diceBonus, int _baseBonus, Form _majorForm, Form _minorForm, int _range)
        {
            DiceBonus = _diceBonus;
            baseBonus = _baseBonus;
            Type = new FormCombo(_majorForm, _minorForm);
            Uses = _uses;
            Anti = _anti;
            Name = _name;
            Range = _range;

            //maybe make this a static variable?
            rnd = new Random();
        }

        public override void Use(Unit giver, Unit receiver)
        {
            giver.Ready = false;
            Uses--;
        }

        public class FormCombo
        {
            public Form Major { get; set; }
            public Form Minor { get; set; }

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
