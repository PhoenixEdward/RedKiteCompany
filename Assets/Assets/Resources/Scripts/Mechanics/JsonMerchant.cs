using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

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

        public T Load<T>(string item)
        {
            T output;

            using (StreamReader r = new StreamReader(@"C:\Users\phoen\source\repos\SandBox\LootData\Inventory.json"))
            {
                string json = r.ReadToEnd();

                List<T> inventory = JsonConvert.DeserializeObject<List<T>>(json);

                output = inventory[Loot.Keys[item].index];

                return output;
            }
        }

        class Inventory
        {
            List<Skill> skills = new List<Skill>();
            /*
            List<Weapon> Brute = new List<Weapon>();
            List<Weapon> Finesse = new List<Weapon>();
            List<Weapon> Clever = new List<Weapon>();
            List<Weapon> Wise = new List<Weapon>();
            List<Buff> Charming = new List<Buff>();
            */


            public Skill Purchase(string item)
            {
                Loot.LookupValue lookup = Loot.Keys[item];

                Skill output;

                Console.WriteLine(skills.Count);

                /*
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
                */

                output = skills[0];

                return output;
            }
        }

    }
}
