using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedKite
{
    public class Telegram
    {
        public BeatSignature Signature { get; private set; }
        public GameSprite Sender { get; private set; }
        public GameSprite Receiver { get; private set; }
        public Message Msg { get; private set; }

        public Telegram(BeatSignature _beatSignature, GameSprite _sender, GameSprite _receiver, Message _msg)
        {
            Signature = _beatSignature;
            Sender = _sender;
            Receiver = _receiver;
            Msg = _msg;
        }

        public struct BeatSignature
        {
            public int Beat { get; private set; }
            public int Initiative { get; private set; }
            public int Delay { get; private set; }

            public BeatSignature(int _beat, int _intiative, int _delay)
            {
                Beat = _beat;
                Initiative = _intiative;
                Delay = _delay;
            }
        }
    }
}
