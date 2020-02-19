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
        bool hasTakenTurn;

        public void Enter(IState previousState, GameSprite owner)
        {
            if (previousState is IdleState)
            { } //There is a lot of potential here and means I can move combat menu and battle clock from directly manipulating units
            //to merely sending messages
            
        }

        public void Execute(GameSprite owner)
        {
            for (int i = 0; i < TargetPings.Count; i++)
            { 
                if(TargetPings[i] == false)
                {
                    Targets.RemoveAt(i);
                    TargetPings.RemoveAt(i);
                }
            }
            if (owner is Enemy attacker)
            {
                if(attacker.Ready)
                {
                    Debug.Log("Attack Flow");
                    if (primaryTarget == null & Targets.Count > 0)
                    {
                        primaryTarget = Targets.OrderBy(x => x.Health + x.Stats.Constitution.Modifier).ToList()[0];
                        Debug.Log("Finding Target");
                    }
                    //this will need to be adjusted for ranged. Will need to find a way of cacheing distance and DPB.
                    if (Utility.ManhattanDistance(primaryTarget.Coordinate, attacker.Coordinate) > 1)
                    {

                        attacker.Embark(primaryTarget.Coordinate, true, true);

                        Debug.Log(primaryTarget.Coordinate);

                    }
                    //need to add skil cycling here
                    else if(Utility.ManhattanDistance(primaryTarget.Coordinate, attacker.Coordinate) <= attacker.MaxAttackRange & !GameSprite.IsUsingSkill)
                    {
                        if(attacker.Weapons.Count > 0)
                            if (attacker.Weapons[0].Anti == false & attacker.Weapons[0].Uses > 0)
                                attacker.Action(primaryTarget, attacker.Weapons[0]);
                            else
                            { }
                        else if (attacker.Buffs.Count > 0)
                            if (attacker.Buffs[0].Anti == true & attacker.Buffs[0].Uses > 0)
                                attacker.Action(primaryTarget, attacker.Buffs[0]);
                            else
                            { }
                        else
                            Debug.Log("No Skills");
                    }
                    else if(!owner.IsMoving & Utility.ManhattanDistance(primaryTarget.Coordinate, attacker.Coordinate) > attacker.MaxAttackRange)
                    {
                        hasTakenTurn = false;
                        //needs to be adjusted to include rest. Need to see if starting position is same as ending. Won't happen
                        //very often but necessary to give the AI for fairness.
                        attacker.Action(attacker, Skill.Wait);
                    }
                }

                for (int i = 0; i < TargetPings.Count; i++)
                    TargetPings[i] = false;
            }
        }
        public void Exit(GameSprite owner)
        {
            Debug.Log("Exit");
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

            if (Targets.Count == 0)
            {
                owner.StateMachine.ChangeState(new IdleState());
                return true;
            }

           if(message.Msg == Message.Die & Targets.Count == 1 & message.Sender == Targets[0])
            {
                owner.StateMachine.ChangeState(new IdleState());
                return true;
            }

           if(message.Msg == Message.Critical)
            {
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
