using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{ 
    public class CombatMenuPopUp : MonoBehaviour
    {
        CombatMenu.Actionables actionables;
        GameSprite[] actionableSprites;

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
        public Screen screen = Screen.None;
        public Target target = Target.None;
        BattleGrid battleGrid;
        Unit unit;
        Vector3Int selectedTile;
        CombatMenu menu;
        public GameObject skillScreen;
        int lockOnIndex;

        Reticle reticle;

        List<Weapon> weapons = new List<Weapon>();
        List<Weapon> heals = new List<Weapon>();
        List<Buff> buffs = new List<Buff>();
        List<Buff> debuffs = new List<Buff>();

        // Start is called before the first frame update
        void Start()
        {
            menu = GetComponentInParent<CombatMenu>();

            TargetNone();
        }

        void Update()
        {

            if (target == Target.Enemy & actionables.Attackables.Count > 0)
            {
                battleGrid.HighlightActionables(actionables.Attackables.ToArray(), lockOnIndex);
                actionableSprites = actionables.Attackables.ToArray();
                CycleUnits(actionableSprites);
                CameraMovement.LockOn(actionables.Attackables.ToArray(), lockOnIndex);
            }
            else if (target == Target.Prop & actionables.Interactables.Count > 0)
            {
                battleGrid.HighlightActionables(actionables.Interactables.ToArray(), lockOnIndex);
                actionableSprites = actionables.Interactables.ToArray();
                CycleUnits(actionableSprites);
                CameraMovement.LockOn(actionables.Interactables.ToArray(), lockOnIndex);
            }
            else if (target == Target.Hero & actionables.Assistables.Count > 0)
            {
                battleGrid.HighlightActionables(actionables.Assistables.ToArray(), lockOnIndex);
                actionableSprites = actionables.Assistables.ToArray();
                CycleUnits(actionableSprites);
                CameraMovement.LockOn(actionables.Assistables.ToArray(), lockOnIndex);
            }
            else
            {
                CameraMovement.LockOff();
                lockOnIndex = 0;
            }

        }

        public void UseSkill(Weapon weapon)
        {
            Debug.Log("The Fuck????");

            Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, unit.Stats.Dexterity.Modifier, 0), unit, unit, Message.Wait));
            unit.SetActiveSkill(weapon);
            Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, unit.Stats.Dexterity.Modifier, 0), actionableSprites[lockOnIndex], unit, Message.UseSkill));
            menu.Deactivate();
        }

        public void UseSkill(Buff buff)
        {
            Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, unit.Stats.Dexterity.Modifier, 0), unit, unit, Message.Wait));
            unit.SetActiveSkill(buff);
            Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, unit.Stats.Dexterity.Modifier, 0), actionableSprites[lockOnIndex], unit, Message.UseSkill));
            menu.Deactivate();
        }

        public void Close()
        {
            menu.Deactivate();
        }

        public void CycleUnits(GameSprite[] interactables)
        {
            Debug.Log(interactables.Length);

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (lockOnIndex < interactables.Length - 1)
                    lockOnIndex++;
                else
                    lockOnIndex = 0;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (lockOnIndex > 0)
                    lockOnIndex--;
                else
                    lockOnIndex = interactables.Length - 1;
            }
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
            unit.Destination = new Vector3Int(selectedTile.x, selectedTile.y, 1);

            Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, unit.Stats.Dexterity.Modifier, 0), unit, unit, Message.Wait));

            unit.Action(unit, Skill.Wait);
            menu.Deactivate();
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
        public void SkillScreenHeals()
        {
            skillScreen.GetComponent<CombatMenuSkillScreen>().ActivateHealing(heals);

            screen = Screen.Spells;
        }

        public void SkillScreenDebuffs()
        {
            skillScreen.GetComponent<CombatMenuSkillScreen>().ActivateDebuffs(debuffs);

            screen = Screen.Weapons;
        }

        public void SkillScreenBuffs()
        {
            skillScreen.GetComponent<CombatMenuSkillScreen>().ActivateBuffs(buffs);

            screen = Screen.Spells;
        }
    }
}