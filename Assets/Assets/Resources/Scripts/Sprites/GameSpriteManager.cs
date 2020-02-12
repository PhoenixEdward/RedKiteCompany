using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RedKite
{
    public class GameSpriteManager : MonoBehaviour
    {
        private static GameSpriteManager _instance;

        public static GameSpriteManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameSpriteManager>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject
                        {
                            name = typeof(GameSpriteManager).Name
                        };
                        _instance = obj.AddComponent<GameSpriteManager>();
                    }
                }
                return _instance;
            }
        }

        public List<GameSprite> Sprites { get; private set; } = new List<GameSprite>();
        public List<Hero> Heroes { get; private set; } = new List<Hero>();
        public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
        public List<Unit> Units { get; private set; } = new List<Unit>();

        public List<Prop> Props { get; private set; } = new List<Prop>();

        char[,] roomMap = TileMapper.Instance.map;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            GetSprites();

        }

        void Update()
        {
            int n = Sprites.Count - 2;
            int q = Sprites.Count - 1;

            while( n > 0)
            {
                GameSprite firstSprite = Sprites[q];
                if (n == Sprites.Count - 2)
                {
                    if (!firstSprite.gameObject.activeSelf)
                    {
                        Sprites.Remove(firstSprite);

                        if (firstSprite is Unit unit)
                            Units.Remove(unit);

                        if (firstSprite is Hero hero)
                            Heroes.Remove(hero);
                        else if (firstSprite is Enemy enemy)
                            Enemies.Remove(enemy);
                        else if (firstSprite is Prop prop)
                            Props.Remove(prop);

                        n--;
                        q--;

                        continue;
                    }
                }

                for (int i = 0; i < n; i++)
                {
                    GameSprite compareSprite = Sprites[i];

                    if (i == 0)
                    {
                        if(!compareSprite.gameObject.activeSelf)
                        {
                            Sprites.Remove(compareSprite);
                            if (compareSprite is Unit unit)
                                Units.Remove(unit);

                            if (compareSprite is Hero hero)
                                Heroes.Remove(hero);
                            else if (compareSprite is Enemy enemy)
                                Enemies.Remove(enemy);
                            else if (compareSprite is Prop prop)
                                Props.Remove(prop);

                            continue;
                        }
                    }

                    if (Utility.ManhattanDistance(compareSprite.Coordinate, firstSprite.Coordinate) < compareSprite.PerceptionRange)
                        Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, firstSprite.Stats.Dexterity.Modifier, 0),
                            firstSprite, compareSprite, Message.InFOV));
                    if (Utility.ManhattanDistance(firstSprite.Coordinate, compareSprite.Coordinate) < firstSprite.PerceptionRange)
                        Telegraph.Instance.DispatchMessage(new Telegram(new Telegram.BeatSignature(BattleClock.Instance.CurrentBeat, compareSprite.Stats.Dexterity.Modifier, 0),
                            compareSprite, firstSprite, Message.InFOV));
                }
                n--;
                q--;
            }
        }

        public void  GetSprites()
        {
            Sprites = FindObjectsOfType<GameSprite>().ToList();

            Heroes.Clear();
            Enemies.Clear();
            Props.Clear();
            Units.Clear();

            foreach (GameSprite sprite in Sprites)
            {
                if (sprite is Hero)
                    Heroes.Add((Hero)sprite);
                else if (sprite is Enemy)
                    Enemies.Add((Enemy)sprite);
                else if (sprite is Prop)
                    Props.Add((Prop)sprite);

                if (sprite is Unit)
                    Units.Add((Unit)sprite);
            }
        }

        public GameSprite GetSpriteFromID(int id)
        {
            foreach(GameSprite sprite in Sprites)
            {
                if (sprite.ID == id)
                    return sprite;
            }

            //this is gonna lead to some crazy bugs.
            return new GameSprite();
        }
    }
}
