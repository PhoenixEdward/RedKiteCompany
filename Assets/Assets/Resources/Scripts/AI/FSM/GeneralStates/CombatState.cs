using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RedKite
{
    class CombatState : IState
    {
        List<GameSprite> Targets = new List<GameSprite>();
        List<bool> TargetPings = new List<bool>();
        GameSprite primaryTarget;
        bool hasTakenTurn = false;
        bool initiated = false;
        bool HeroMoving = false;
        bool isTrade = false;
        bool isInteraction = false;

        public void Enter(IState previousState, Objective objective)
        {
            if (previousState is IdleState)
            { } //There is a lot of potential here and means I can move combat menu and battle clock from directly manipulating units
            //to merely sending messages
            
        }

        public void Execute(Objective objective)
        {
            if (objective.Owner is Enemy attacker)
            {
                for (int i = 0; i < TargetPings.Count; i++)
                {
                    if (TargetPings[i] == false | !Targets[i].IsAlive)
                    {
                        Targets.RemoveAt(i);
                        TargetPings.RemoveAt(i);
                    }
                }

                if (Targets.Count == 0)
                {
                    objective.Owner.StateMachine.ChangeState(new IdleState());
                }

                if (attacker.Ready)
                {
                    if (primaryTarget == null & Targets.Count > 0)
                    {
                        primaryTarget = Targets.OrderBy(x => x.Health + x.Stats.Constitution.Modifier).ToList()[0];
                    }
                    //this will need to be adjusted for ranged. Will need to find a way of cacheing distance and DPB.
                    if (Utility.ManhattanDistance(primaryTarget.Coordinate, attacker.Coordinate) > attacker.MaxAttackRange & !hasTakenTurn)
                    {
                        hasTakenTurn = true;

                        attacker.Embark(primaryTarget.Coordinate, true, true);

                        Debug.Log(primaryTarget.Coordinate);

                    }
                    //need to add skil cycling here
                    //this may not be the best place for the FX but eff it
                    else if (!objective.Owner.IsMoving & Utility.ManhattanDistance(primaryTarget.Coordinate, attacker.Coordinate) <= attacker.MaxAttackRange & !BattleFX.IsActive)
                    {
                        if(attacker.Weapons.Count > 0)
                            if (attacker.Weapons[0].Anti == false & attacker.Weapons[0].Uses > 0)
                            { 
                                attacker.Action(primaryTarget, attacker.Weapons[0]);
                            }
                            else
                            { }
                        else if (attacker.Buffs.Count > 0)
                            if (attacker.Buffs[0].Anti == true & attacker.Buffs[0].Uses > 0)
                            { 
                                attacker.Action(primaryTarget, attacker.Buffs[0]);

                            }
                            else
                            { }
                        else
                            Debug.Log("No Skills");

                        hasTakenTurn = false;

                    }
                    else if(!objective.Owner.IsMoving & Utility.ManhattanDistance(primaryTarget.Coordinate, attacker.Coordinate) > attacker.MaxAttackRange)
                    {
                        //needs to be adjusted to include rest. Need to see if starting position is same as ending. Won't happen
                        //very often but necessary to give the AI for fairness.
                        attacker.Action(attacker, Skill.Wait);

                        hasTakenTurn = false;
                    }
                }

                for (int i = 0; i < TargetPings.Count; i++)
                    TargetPings[i] = false;
            }

            else if(HeroMoving)
            {
                if (!objective.Owner.IsMoving)
                {
                    HeroMoving = false;
                }
            }

            else if(initiated == true)
            {
                if (!objective.Owner.IsMoving)
                {
                    if (!isTrade & !isInteraction)
                    {
                        //this can be change to take an item.
                        objective.Owner.Action(primaryTarget, (Skill)objective.Owner.ActiveItem);
                    }
                    
                    else if(isTrade)
                    {
                        isTrade = false;
                        objective.Owner.Trade(primaryTarget, objective.Owner.ActiveItem);
                    }

                    else if(isInteraction)
                    {
                        isInteraction = false;
                        objective.Owner.Interact(primaryTarget);
                    }

                    initiated = false;
                }
            }
        }
        public void Exit(Objective objective)
        {
            Targets = new List<GameSprite>();
            primaryTarget = null;
        }

        public bool OnMessage(Objective objective, Telegram message)
        {
            if(message.Msg == Message.InFOV)
            {
                if(((objective.Owner is Hero & message.Sender is Enemy) | (objective.Owner is Enemy & message.Sender is Hero)))
                { 
                    if(!Targets.Contains(message.Sender))
                    { 
                        Targets.Add(message.Sender);
                        TargetPings.Add(true);
                        return true;
                    }
                    else
                    {
                        int n = Targets.FindIndex(x => x == message.Sender);
                        TargetPings[n] = true;
                    }
                }
            }

           if(message.Msg == Message.Die & Targets.Count == 1)
            {
                if(message.Sender == Targets[0])
                {
                    objective.Owner.StateMachine.ChangeState(new IdleState());
                }

                return true;
            }

            if (message.Msg == Message.Critical)
            {
                primaryTarget = message.Sender;
                return true;
            }

           if(message.Msg == Message.UseSkill)
            {
                initiated = true;
                primaryTarget = message.Sender;
                return true;
            }

           if(message.Msg == Message.HeroMove)
            {
                Hero hero = (Hero)message.Sender;

                hero.Embark(hero.Destination);

                HeroMoving = true;
            }

           if(message.Msg == Message.Trade)
            {
                initiated = true;
                primaryTarget = message.Sender;
                isTrade = true;
                return true;
            }

           if(message.Msg == Message.Interact)
            {
                initiated = true;
                isInteraction = true;
                primaryTarget = message.Sender;
                return true;
            }
            return false;
        }
        /*
        public void FindPathToATarget()
        {
            for(int i = 0; i < Targets.Length)
        }
        */
    }
}
