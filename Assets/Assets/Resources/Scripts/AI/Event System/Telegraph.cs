using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{ 
    public class Telegraph : MonoBehaviour
    {
        private static Telegraph _instance;
        public static Telegraph Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Telegraph();
                return _instance;
            }
        }

        List<Telegram> messages;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void ReceiveMessage(Telegram message)
        {

            messages.Add(message);

            //receiver.StateMachine.HandleMessage(message);
        }

        public void DispatchMessage(Telegram message)
        {
            message.Receiver.StateMachine.HandleMessage(message);
        }
    }
}