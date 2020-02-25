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

        public List<string> LootLoad;
        public List<Skill> Loot;

        public List<string> FlashMessages;

        public List<PropLoad> PropLoads;

        public List<Region> Regions;

        public int BeatLimit;

        public bool Complete = false;

        [Serializable]
        public class PropLoad
        {
            public string Name;
            public int TilesPerUnit;
        }
        [Serializable]
        public class Region
        {
            public string TopWallTexture;
            public string SideWallTexture;
            public string TopFloorTexture;
            public string SideFloorTexture;
        }

        [Serializable]
        public enum State
        {
            Gather,
            Complete
        }

        public void Instantiate()
        {
            foreach(string item in LootLoad)
            {
                Loot.Add(JsonMerchant.Instance.Load(item));
            }
        }
    }
}
