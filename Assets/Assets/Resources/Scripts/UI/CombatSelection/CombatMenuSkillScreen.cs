using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RedKite
{
    public class CombatMenuSkillScreen : MonoBehaviour
    {
        Skill activeSkill;
        public GameObject skillButton;
        List<GameObject> skills = new List<GameObject>();
        Unit unit;
        Unit target;
        // Start is called before the first frame update
        public void ActivateWeapons(List<Weapon> weapons)
        {
            if(skills != null)
            {
                foreach (GameObject skill in skills)
                    Destroy(skill);
            }

            Vector2 startingPosition = new Vector2(0, 135f);

            for(int i = 0; i < weapons.Count; i++)
            {
                GameObject newButton = Instantiate(skillButton);

                RectTransform rect = GetComponent<RectTransform>();

                newButton.GetComponent<RectTransform>().SetParent(rect);

                newButton.GetComponent<RectTransform>().localPosition = startingPosition + new Vector2(0, i * 50);

                newButton.GetComponent<CombatMenuItem>().Action = weapons[i];

                newButton.GetComponentInChildren<Text>().text = weapons[i].Name;

                skills.Add(newButton);
            }
        }

        public void ActivateSpells(List<Buff> buffs, List<Weapon> heals)
        {
            if (skills != null)
            {
                foreach (GameObject skill in skills)
                    Destroy(skill);
            }

            Vector2 startingPosition = new Vector2(0, 135f);

            for (int i = 0; i < buffs.Count; i++)
            {
                GameObject newButton = Instantiate(skillButton);

                RectTransform rect = GetComponent<RectTransform>();

                newButton.GetComponent<RectTransform>().SetParent(rect);

                newButton.GetComponent<RectTransform>().localPosition = startingPosition + new Vector2(0, i * 50);

                newButton.GetComponent<CombatMenuItem>().Action = buffs[i];

                newButton.GetComponentInChildren<Text>().text = buffs[i].Name;


                skills.Add(newButton);
            }

            for (int i = 0; i < heals.Count; i++)
            {
                GameObject newButton = Instantiate(skillButton);

                RectTransform rect = GetComponent<RectTransform>();

                newButton.transform.SetParent(rect);

                newButton.GetComponent<RectTransform>().localPosition = startingPosition + new Vector2(0, i * 50);

                newButton.GetComponent<CombatMenuItem>().Action = heals[i];

                newButton.GetComponentInChildren<Text>().text = heals[i].Name;

                skills.Add(newButton);
            }
        }

        public void UseSkill(Weapon weapon)
        {
                weapon.Use(unit, target);
        }

        public void UseSkill(Buff buff)
        {
            buff.Use(unit, target);
        }


        // Update is called once per frame
        void Update()
        {
        
        }
    }
}