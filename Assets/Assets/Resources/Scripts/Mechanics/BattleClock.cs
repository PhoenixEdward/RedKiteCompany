using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RedKite
{
    public class BattleClock : MonoBehaviour
    {
        List<Hero> heroes;
        List<Enemy> enemies;
        List<Unit> units;
        private static BattleClock _instance;
        public int CurrentBeat { get; private set; }
        public bool IsEnemyTurn;

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
            //units = GameSpriteManager.Instance.Units;
            heroes = GameSpriteManager.Instance.Heroes;
            enemies = GameSpriteManager.Instance.Enemies;
            units = GameSpriteManager.Instance.Units;
            //need to rethink how units spawn. They need to be cached in someway then re-entered in to the pool when in aggro distance. Need some sort of UI message "AN ENEMY STIRS"
        }

        void Update()
        {
            heroes = GameSpriteManager.Instance.Heroes;
            enemies = GameSpriteManager.Instance.Enemies;
            units = GameSpriteManager.Instance.Units;

            if (units.All(x => !x.Ready | !x.IsAlive) & units.All(x=> x.IsMoving == false) & !BattleFX.IsActive)
                Run();

            IsEnemyTurn = enemies.Any(x => x.Ready);
        }

        public void Run()
        {
            GameSpriteManager.Instance.UpdateSpriteLists();

            bool unitReady = false;

            while(unitReady == false)
            { 
                foreach(Unit unit in units.Where(x=> x.IsAlive))
                {
                    CurrentBeat++;
                    if (unit.Fatigue >= 0)
                        unit.DecreaseFatigue();
                    if(unit.Fatigue == 0)
                        unitReady = true;
                }
            }
        }
    }
}