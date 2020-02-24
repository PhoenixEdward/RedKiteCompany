using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedKite
{
    [Serializable]
    public class Quest
    {
        public string Name;

        int stateIndex = 0;

        public State state { get
            {
                return States[stateIndex];
            }
            private set {
                state = value;
            }
        }

        public List<State> States;

        public List<Key> Keys;

        public List<string> FlashMessages;

        public List<PropLoad> PropLoads;

        public List<Region> Regions;


        [Serializable]
        public class PropLoad
        {

        }
        [Serializable]
        public class Region
        {

        }

        [Serializable]
        public enum State
        {
            Gather,
            Complete
        }
    }
}
