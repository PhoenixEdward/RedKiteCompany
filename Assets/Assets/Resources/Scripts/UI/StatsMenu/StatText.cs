using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RedKite
{
    public class StatText : MonoBehaviour
    {
        public string name;
        Text text;
        public Unit unit { get; set; }
        // Start is called before the first frame update
        void Start()
        {
            text = GetComponent<Text>();
        }

        void Update()
        {
            if(unit!= null)
            {
                if (name == "UnitName")
                    text.text = unit.Name.ToString();
                else if (name == "Fatigue")
                    text.text = unit.Fatigue.ToString();
                else if (name == "JobClass")
                    text.text = unit.jobClass.ToString();
                else if (name == "Strength")
                    text.text = unit.Stats.Strength.BaseStat.ToString();
                else if (name == "Constitution")
                    text.text = unit.Stats.Constitution.BaseStat.ToString();
                else if (name == "Dexterity")
                    text.text = unit.Stats.Dexterity.BaseStat.ToString();
                else if (name == "Intelligence")
                    text.text = unit.Stats.Intelligence.BaseStat.ToString();
                else if (name == "Wisdom")
                    text.text = unit.Stats.Wisdom.BaseStat.ToString();
                else
                    text.text = unit.Stats.Charisma.BaseStat.ToString();
            }
        }
    }
}