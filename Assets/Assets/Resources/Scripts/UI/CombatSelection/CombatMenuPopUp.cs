using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{ 
    public class CombatMenuPopUp : MonoBehaviour
    {
        CombatMenu.Actionables actionables;

        public enum Screen
        {
            Weapons,
            Spells,
            None,
        }

        public enum Target
        {
            Hero,
            Enemy,
            Prop,
            None
        }
        public Screen screen;
        public Target target;
        BattleGrid battleGrid;
        Unit unit;
        Vector3Int selectedTile;
        CombatMenu menu;
        public GameObject skillScreen;

        List<Weapon> weapons = new List<Weapon>();
        List<Weapon> heals = new List<Weapon>();
        List<Buff> buffs = new List<Buff>();
        List<Buff> debuffs = new List<Buff>();

        // Start is called before the first frame update
        void Start()
        {
            menu = GetComponentInParent<CombatMenu>();
        }

        void Update()
        {

            if (target == Target.Enemy & actionables.Attackables != null)
                battleGrid.HighlightActionables(actionables.Attackables);
            else if (target == Target.Prop & actionables.Interactables != null)
                battleGrid.HighlightActionables(actionables.Interactables);
            else if (target == Target.Hero & actionables.Assistables != null)
                battleGrid.HighlightActionables(actionables.Assistables);
            else
                battleGrid.Clear();

        }

        public void Activate(Unit _unit, Vector3Int _selectedTile, CombatMenu.Actionables _actionables, BattleGrid _battleGrid)
        {
            actionables = _actionables;
            battleGrid = _battleGrid;
            unit = _unit;
            selectedTile = _selectedTile;

            foreach(Weapon weapon in unit.Weapons)
                weapons.Add(weapon);
            foreach (Weapon heal in unit.Heals)
                heals.Add(heal);
            foreach (Buff buff in unit.Buffs)
                buffs.Add(buff);
            foreach (Buff debuff in unit.Debuffs)
                debuffs.Add(debuff);

        }

        public void UnitWait()
        {
            unit.Action(unit, Skill.Wait);
            unit.Embark(new Vector3Int(selectedTile.x, 1, selectedTile.y));
            menu.Deactivate();
            BattleClock.Instance.Run();
        }

        public void TargetEnemy()
        {
            target = Target.Enemy;
        }
        public void TargetProp()
        {
            target = Target.Prop;
        }
        public void TargetHero()
        {
            target = Target.Hero;
        }
        public void TargetNone()
        {
            target = Target.None;
        }
        public void SkillScreenWeapons()
        {
            skillScreen.GetComponent<CombatMenuSkillScreen>().ActivateWeapons(weapons);

            screen = Screen.Weapons;
        }
        public void SkillScreenSpells()
        {
            skillScreen.GetComponent<CombatMenuSkillScreen>().ActivateSpells(buffs, heals);

            screen = Screen.Spells;
        }
    }
}