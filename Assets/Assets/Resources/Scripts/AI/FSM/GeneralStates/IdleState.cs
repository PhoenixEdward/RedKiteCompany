using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedKite
{
    public class IdleState : IState
    {
        public void Enter(IState previousState, GameSprite owner)
        { }

        public void Execute(GameSprite owner)
        {
            if(owner is Enemy enemy)
                if(enemy.Ready)
                    enemy.Action(enemy, Skill.Wait);
        }

        public void Exit(GameSprite owner)
        {
            //there actually seems like potential for something here.
        }

        public bool OnMessage(GameSprite owner, Telegram message)
        {
            if(message.Msg == Message.InFOV & ((owner is Hero & message.Sender is Enemy) | (owner is Enemy & message.Sender is Hero)))
                if (Utility.ManhattanDistance(message.Sender.Coordinate, owner.Coordinate) < owner.PerceptionRange)
                {
                    owner.StateMachine.ChangeState(new CombatState());
                    //not sure if this is the proper way of doing this or if I need to send a message out to the telegraph?
                    owner.StateMachine.currentState.OnMessage(owner,
                        new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat,message.Sender.Stats.Dexterity.Modifier, 0), 
                        message.Sender, owner, Message.InFOV
                        ));
                    return true;
                }

            return false;
        }
    }
}
