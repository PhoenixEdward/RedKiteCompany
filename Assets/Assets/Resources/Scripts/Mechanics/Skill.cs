using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

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


        public FormCombo Type;

        public bool Anti;

        public int DiceBonus;
        public int BaseBonus;

        public static Skill Wait = new Skill("Wait", 1000, false, 0, 0, Form.None, Form.None, 0, 0);
        public static Skill Alert = new Skill("Alert", 1000, false, 0, 0, Form.None, Form.None, 0, -1000);
        public static Skill Interact = new Skill("Interact", 1000, false, 0, 0, Form.None, Form.None, 0, 0);
        public static Skill TradeItem = new Skill("TradeItem", 1000, false, 0, 0, Form.None, Form.None, 0, 0);
        public Skill() { }

        public Skill(string _name, int _uses, bool _anti, int _diceBonus, int _baseBonus, Form _majorForm, Form _minorForm, int _range, int _burden)
        {
            DiceBonus = _diceBonus;
            BaseBonus = _baseBonus;
            Type = new FormCombo(_majorForm, _minorForm);
            Uses = _uses;
            Anti = _anti;
            Name = _name;
            Range = _range;
            Burden = _burden;
            //maybe make this a static variable?
            rnd = new System.Random();
        }

        public override void Use(GameSprite giver, GameSprite receiver)
        {
            base.Use(giver, receiver); ;
        }

        protected void UseFX(Skill skill, int text, GameSprite primaryTarget)
        {
            GameObject go = new GameObject();
            BattleFX fx = go.AddComponent<BattleFX>();

            if (skill.Type.Major == Skill.Form.Clever | skill.Type.Major == Skill.Form.Wise)
                fx.Activate(BattleFX.Type.Magic, text, primaryTarget.Coordinate);
            else if (skill.Type.Major == Skill.Form.Finesse)
                fx.Activate(BattleFX.Type.Ranged, text, primaryTarget.Coordinate);
            else
                fx.Activate(BattleFX.Type.Melee, text, primaryTarget.Coordinate);
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
