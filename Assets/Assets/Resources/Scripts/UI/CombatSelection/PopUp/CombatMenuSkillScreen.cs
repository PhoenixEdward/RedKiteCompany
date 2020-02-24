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
        public RectTransform skillListBox;
        // Start is called before the first frame update
        public void ActivateWeapons(List<Weapon> weapons)
        {
            if(skills != null)
            {
                foreach (GameObject skill in skills)
                    Destroy(skill);
            }

            Vector3 startingPosition = new Vector3(0, -35f, 0);

            int spacingIndex = 0;

            for(int i = 0; i < weapons.Count; i++)
            {
                GameObject newButton = Instantiate(skillButton);

                newButton.GetComponent<RectTransform>().SetParent(skillListBox);

                newButton.GetComponent<RectTransform>().localPosition = startingPosition - new Vector3(0, spacingIndex * 60, 0);

                newButton.GetComponent<CombatMenuItem>().Action = weapons[i];

                newButton.GetComponentInChildren<Text>().text = weapons[i].Name;

                skills.Add(newButton);

                spacingIndex++;
            }
        }

        public void ActivateDebuffs(List<Buff> debuffs)
        {
            if (skills != null)
            {
                foreach (GameObject skill in skills)
                    Destroy(skill);
            }

            Vector3 startingPosition = new Vector3(0, -35f, 0);

            int spacingIndex = 0;

            for (int i = 0; i < debuffs.Count; i++)
            {
                GameObject newButton = Instantiate(skillButton);

                newButton.GetComponent<RectTransform>().SetParent(skillListBox);

                newButton.GetComponent<RectTransform>().localPosition = startingPosition - new Vector3(0, spacingIndex * 60, 0);

                newButton.GetComponent<CombatMenuItem>().Action = debuffs[i];

                newButton.GetComponentInChildren<Text>().text = debuffs[i].Name;

                skills.Add(newButton);

                spacingIndex++;
            }
        }

        public void ActivateHealing(List<Weapon> heals)
        {
            Debug.Log("Execute");

            if (skills != null)
            {
                foreach (GameObject skill in skills)
                    Destroy(skill);
            }

            Vector3 startingPosition = new Vector3(0, -35f, 0);

            int spacingIndex = 0;


            for (int i = 0; i < heals.Count; i++)
            {
                Debug.Log("HEal Execute");

                GameObject newButton = Instantiate(skillButton);

                newButton.transform.SetParent(skillListBox);

                newButton.GetComponent<RectTransform>().localPosition = startingPosition - new Vector3(0, spacingIndex * 60, 0);

                newButton.GetComponent<CombatMenuItem>().Action = heals[i];

                newButton.GetComponentInChildren<Text>().text = heals[i].Name;

                skills.Add(newButton);

                spacingIndex++;
            }
        }

        public void ActivateBuffs(List<Buff> buffs)
        {
            if (skills != null)
            {
                foreach (GameObject skill in skills)
                    Destroy(skill);
            }

            Vector3 startingPosition = new Vector3(0, -35f, 0);

            int spacingIndex = 0;

            for (int i = 0; i < buffs.Count; i++)
            {
                GameObject newButton = Instantiate(skillButton);

                newButton.GetComponent<RectTransform>().SetParent(skillListBox);

                newButton.GetComponent<RectTransform>().localPosition = startingPosition - new Vector3(0, spacingIndex * 60, 0);

                newButton.GetComponent<CombatMenuItem>().Action = buffs[i];

                newButton.GetComponentInChildren<Text>().text = buffs[i].Name;

                skills.Add(newButton);

                spacingIndex++;
            }
        }

        // This is where tooltips will go
        void Update()
        {
        
        }
    }
}