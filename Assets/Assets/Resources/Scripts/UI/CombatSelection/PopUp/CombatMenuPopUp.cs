using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RedKite
{ 
    public class CombatMenuPopUp : MonoBehaviour
    {
        CombatMenu.Actionables actionables;
        GameSprite[] actionableSprites;
        int[] actionableDistances;

        public enum Screen
        {
            Weapons,
            Spells,
            None,
            Interactions,
            Inventory
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
        public GameObject actionSelection;
        public GameObject offenseSkillTypeSelection;
        public GameObject supportSkillTypeSelection;

        Button[] actionButtons;
        Button[] offenseSkillTypeButtons;
        Button[] supportSkillTypeButtons;

        int lockOnIndex;

        Reticle reticle;

        List<Weapon> weapons = new List<Weapon>();
        List<Weapon> heals = new List<Weapon>();
        List<Buff> buffs = new List<Buff>();
        List<Buff> debuffs = new List<Buff>();
        List<Item> inventory = new List<Item>();

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
                actionableDistances = actionables.AttackbleDistances.ToArray();
                CycleUnits(actionableSprites);
                CameraMovement.LockOn(actionables.Attackables.ToArray(), lockOnIndex);
            }
            else if (target == Target.Prop & actionables.Interactables.Count > 0)
            {
                battleGrid.HighlightActionables(actionables.Interactables.ToArray(), lockOnIndex);
                actionableSprites = actionables.Interactables.ToArray();
                actionableDistances = actionables.InteractableDistances.ToArray();
                CycleUnits(actionableSprites);
                CameraMovement.LockOn(actionables.Interactables.ToArray(), lockOnIndex);
            }
            else if (target == Target.Hero & actionables.Assistables.Count > 0)
            {
                battleGrid.HighlightActionables(actionables.Assistables.ToArray(), lockOnIndex);
                actionableSprites = actionables.Assistables.ToArray();
                actionableDistances = actionables.AssistableDistances.ToArray();
                CycleUnits(actionableSprites);
                CameraMovement.LockOn(actionables.Assistables.ToArray(), lockOnIndex);
            }
            else
            {
                CameraMovement.LockOff();
                lockOnIndex = 0;
            }

        }

        public void UseSkill(Item item)
        {
            if(item is Skill skill)
            { 
                if(skill != Skill.Interact)
                { 
                    if (screen != Screen.Inventory)
                    {
                        unit.Destination = new Vector3Int(selectedTile.x, selectedTile.y, 1);
                        Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, unit.Stats.Dexterity.Modifier, 0), unit, unit, Message.HeroMove));
                        unit.SetActiveSkill(skill);
                        Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, unit.Stats.Dexterity.Modifier, 0), actionableSprites[lockOnIndex], unit, Message.UseSkill));
                        menu.Deactivate();
                    }
                    else
                    {
                        //need to adjust this later so that trading can be a minor action.
                        unit.Destination = new Vector3Int(selectedTile.x, selectedTile.y, 1);
                        Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, unit.Stats.Dexterity.Modifier, 0), unit, unit, Message.HeroMove));
                        unit.SetActiveSkill(skill);
                        Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, unit.Stats.Dexterity.Modifier, 0), actionableSprites[lockOnIndex], unit, Message.Trade));
                        menu.Deactivate();
                    }
                }
                else
                {
                    Debug.Log("Interact");
                    unit.Destination = new Vector3Int(selectedTile.x, selectedTile.y, 1);
                    Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, unit.Stats.Dexterity.Modifier, 0), unit, unit, Message.HeroMove));
                    unit.SetActiveSkill(skill);
                    Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, unit.Stats.Dexterity.Modifier, 0), actionableSprites[lockOnIndex], unit, Message.Interact));
                    menu.Deactivate();
                }
            }
        }

        public void Close()
        {
            menu.Deactivate();
        }

        public void CycleUnits(GameSprite[] interactables)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (lockOnIndex < interactables.Length - 1)
                    lockOnIndex++;
                else
                    lockOnIndex = 0;
                skillScreen.GetComponent<CombatMenuSkillScreen>().UpdateButtons(actionableDistances[lockOnIndex]);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (lockOnIndex > 0)
                    lockOnIndex--;
                else
                    lockOnIndex = interactables.Length - 1;

                skillScreen.GetComponent<CombatMenuSkillScreen>().UpdateButtons(actionableDistances[lockOnIndex]);
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
            foreach (Item item in unit.Inventory)
                inventory.Add(item);

            actionButtons = actionSelection.GetComponentsInChildren<Button>();
            offenseSkillTypeButtons = offenseSkillTypeSelection.GetComponentsInChildren<Button>();
            supportSkillTypeButtons = supportSkillTypeSelection.GetComponentsInChildren<Button>();

            //need to change this to actually removing and realigning the buttons but bleh it sounds difficult and annoything. Actuallly not that bad but this is better for beta testing.

            foreach(Button button in actionButtons)
            {
                if (button.gameObject.name == "Attack")
                    if ((weapons.Count == 0 & debuffs.Count == 0) | actionables.Attackables.Count == 0)
                        button.interactable = false;
                    else
                        button.interactable = true;
                else if (button.gameObject.name == "Assist")
                    if (heals.Count == 0 & buffs.Count == 0 | (actionables.Assistables.Count == 1 & actionables.Assistables[0] == unit))
                        button.interactable = false;
                    else
                        button.interactable = true;
                else if (button.gameObject.name == "Interact")
                    if (actionables.Interactables.Count == 0)
                        button.interactable = false;
                    else
                        button.interactable = true;
            }

            foreach (Button button in offenseSkillTypeButtons)
            {
                if (button.gameObject.name == "Weapons")
                    if (weapons.Count == 0)
                        button.interactable = false;
                    else
                        button.interactable = true;
                else if (button.gameObject.name == "Debuffs")
                    if (debuffs.Count == 0)
                        button.interactable = false;
                    else
                        button.interactable = true;
            }

            foreach (Button button in supportSkillTypeButtons)
            {
                if (button.gameObject.name == "Heals")
                    if (heals.Count == 0)
                        button.interactable = false;
                    else
                        button.interactable = true;
                else if (button.gameObject.name == "Buffs")
                    if (buffs.Count == 0)
                        button.interactable = false;
                    else
                        button.interactable = true;
                else if (button.gameObject.name == "Trade")
                    if (actionables.Assistables.Count == 1 & actionables.Assistables[0] == unit)
                        button.interactable = false;
                    else
                        button.interactable = true;
            }
        }

        //needs to be changed

        public void UnitWait()
        {
            unit.Destination = new Vector3Int(selectedTile.x, selectedTile.y, 1);
            Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, unit.Stats.Dexterity.Modifier, 0), unit, unit, Message.HeroMove));
            unit.SetActiveSkill(Skill.Wait);
            Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, unit.Stats.Dexterity.Modifier, 0), unit, unit, Message.UseSkill));
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

        public void SkillScreenInteractions()
        {
            skillScreen.GetComponent<CombatMenuSkillScreen>().ActivateInteractions(actionables.Interactables);

            screen = Screen.Interactions;
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

        public void SkillScreenInventory(bool isTrade)
        {
            skillScreen.GetComponent<CombatMenuSkillScreen>().ActivateInventory(inventory, isTrade);

            screen = Screen.Inventory;
        }
    }
}