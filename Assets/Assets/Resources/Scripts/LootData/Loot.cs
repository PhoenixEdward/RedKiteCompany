using System;
using System.Collections.Generic;
using System.Text;

namespace RedKite
{
    public static class Loot
    {
        public static Dictionary<string, LookupValue> Keys = new Dictionary<string, LookupValue>
        {
            { "Training Axe" , new LookupValue(Skill.Form.Brute, 0)},
            { "Training Gauntlets" , new LookupValue(Skill.Form.Brute,1)},
            { "Training Sword" , new LookupValue(Skill.Form.Finesse,2)},
            { "Training Dagger" , new LookupValue(Skill.Form.Finesse,3)},
            { "Training Long Bow" , new LookupValue(Skill.Form.Finesse,4)},
            { "Training Short Bow" , new LookupValue(Skill.Form.Finesse,5)},
            { "Firebolt" , new LookupValue(Skill.Form.Clever,6)},
            { "Cure" , new LookupValue(Skill.Form.Wise,7)},
            { "Disarm" , new LookupValue(Skill.Form.Charming,8)}
        };

        public struct LookupValue
        {
            public Skill.Form majorForm;
            public int index;

            public LookupValue(Skill.Form _majorForm, int _index)
            {
                majorForm = _majorForm;
                index = _index;
            }
        }
    }
}
