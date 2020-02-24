using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedKite
{
    public class IdleState : IState
    {
        public void Enter(IState previousState, Objective objective)
        { }

        public void Execute(Objective objective)
        {
            if (objective.Owner is Enemy enemy)
                if (enemy.Ready)
                    enemy.Action(enemy, Skill.Wait);
                else { }
            else if (objective.Owner is Hero hero)
                hero.StateMachine.ChangeState(new CombatState());
        }

        public void Exit(Objective objective)
        {
            //there actually seems like potential for something here.
        }

        public bool OnMessage(Objective objective, Telegram message)
        {
            if(message.Msg == Message.InFOV & ((objective.Owner is Hero & message.Sender is Enemy) | (objective.Owner is Enemy & message.Sender is Hero)))
                if (Utility.ManhattanDistance(message.Sender.Coordinate, objective.Owner.Coordinate) < objective.Owner.Perception)
                {
                    objective.Owner.StateMachine.ChangeState(new CombatState());
                    //not sure if this is the proper way of doing this or if I need to send a message out to the telegraph?
                    objective.Owner.StateMachine.currentState.OnMessage(objective,
                        new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat,message.Sender.Stats.Dexterity.Modifier, 0), 
                        message.Sender, objective.Owner, Message.InFOV
                        ));
                    return true;
                }

            return false;
        }
    }
}
