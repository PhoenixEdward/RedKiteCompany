using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace RedKite
{
    class JsonMerchant
    {
        private static JsonMerchant _instance = null;
        public static JsonMerchant Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new JsonMerchant();

                return _instance;
            }
        }

        public Skill Load(string item)
        {
            dynamic output;

            TextAsset json = Resources.Load<TextAsset>("Scripts/Data/Inventory");

            Inventory inventory = JsonUtility.FromJson<Inventory>(json.text);

            output = inventory.Purchase(item);

            if (output is Weapon weapon)
                return weapon;
            else if (output is Buff buff)
                return buff;
            else
                return Skill.Wait;
        }

        [Serializable]
        class Inventory
        {
            public Weapon[] Brute;
            public Weapon[] Finesse;
            public Weapon[] Clever;
            public Weapon[] Wise;
            public Buff[] Charming;


            public Skill Purchase(string item)
            {
                Loot.LookupValue lookup = Loot.Keys[item];

                Skill output;

                if (lookup.majorForm == Skill.Form.Brute)
                    output = Brute[lookup.index];
                else if (lookup.majorForm == Skill.Form.Charming)
                    output = Charming[lookup.index];
                else if (lookup.majorForm == Skill.Form.Clever)
                    output = Clever[lookup.index];
                else if (lookup.majorForm == Skill.Form.Finesse)
                    output = Finesse[lookup.index];
                else if (lookup.majorForm == Skill.Form.Wise)
                    output = Wise[lookup.index];
                else
                    output = Brute[0];

                return output;
            }
        }

    }
}
