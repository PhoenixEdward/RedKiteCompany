﻿using System;
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

        public void Enter(IState previousState, GameSprite owner)
        {
            if (previousState is IdleState)
            { } //There is a lot of potential here and means I can move combat menu and battle clock from directly manipulating units
            //to merely sending messages
            
        }

        public void Execute(GameSprite owner)
        {
            if (owner is Enemy attacker)
            {
                for (int i = 0; i < TargetPings.Count; i++)
                {
                    if (TargetPings[i] == false)
                    {
                        Targets.RemoveAt(i);
                        TargetPings.RemoveAt(i);
                    }
                }

                if (Targets.Count == 0)
                {
                    owner.StateMachine.ChangeState(new IdleState());
                }

                if (attacker.Ready)
                {
                    Debug.Log("Attack Flow");
                    if (primaryTarget == null & Targets.Count > 0)
                    {
                        primaryTarget = Targets.OrderBy(x => x.Health + x.Stats.Constitution.Modifier).ToList()[0];
                        Debug.Log("Finding Target");
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
                    else if (!owner.IsMoving & Utility.ManhattanDistance(primaryTarget.Coordinate, attacker.Coordinate) <= attacker.MaxAttackRange & !BattleFX.IsActive)
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
                    else if(!owner.IsMoving & Utility.ManhattanDistance(primaryTarget.Coordinate, attacker.Coordinate) > attacker.MaxAttackRange)
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
                if (!owner.IsMoving)
                {
                    owner.Action(owner, Skill.Wait);
                    HeroMoving = false;
                }
            }

            else if(initiated == true)
            {
                if (!owner.IsMoving)
                {
                    Debug.Log("Runomundo");
                    owner.Action(primaryTarget, owner.ActiveSkill);
                    initiated = false;
                }
            }
        }
        public void Exit(GameSprite owner)
        {
            Targets = null;
            primaryTarget = null;
        }

        public bool OnMessage(GameSprite owner, Telegram message)
        {
            if(message.Msg == Message.InFOV)
            {
                if(((owner is Hero & message.Sender is Enemy) | (owner is Enemy & message.Sender is Hero)))
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
                    owner.StateMachine.ChangeState(new IdleState());
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

           if(message.Msg == Message.Wait)
            {
                Hero hero = (Hero)message.Sender;

                hero.Embark(hero.Destination);

                HeroMoving = true;
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
