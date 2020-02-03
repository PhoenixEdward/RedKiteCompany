using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{
    public class BattleClock : MonoBehaviour
    {
        List<Unit> units;
        private static BattleClock _instance;
        public static int Beats { get; private set; }

        public static BattleClock Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BattleClock();
                return _instance;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            units = GameSpriteManager.Instance.Units;
            //need to rethink how units spawn. They need to be cached in someway then re-entered in to the pool when in aggro distance. Need some sort of UI message "AN ENEMY STIRS"
        }

        // Update is called once per frame
        public void Run()
        {
            units = units == null ? GameSpriteManager.Instance.Units : units;

            bool unitReady = false;

            while(unitReady == false)
            { 
                foreach(Unit unit in units)
                {
                    Beats++;
                    if (unit.Fatigue >= 0)
                        unit.DecreaseFatigue();
                    if(unit.Fatigue == 0)
                        unitReady = true;
                }
            }
        }
    }
}