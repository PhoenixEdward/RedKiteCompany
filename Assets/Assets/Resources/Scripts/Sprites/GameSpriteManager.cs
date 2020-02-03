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
    }
}
