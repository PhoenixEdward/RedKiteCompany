using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RedKite
{
    public class GameSpriteManager : MonoBehaviour
    {
        private static GameSpriteManager instance;

        public static GameSpriteManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameSpriteManager>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(GameSpriteManager).Name;
                        instance = obj.AddComponent<GameSpriteManager>();
                    }
                }
                return instance;
            }
        }

        public List<GameSprite> Sprites { get; private set; }
        public GameSprite Player { get; private set; }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            Sprites = FindObjectsOfType<GameSprite>().ToList();

        }


    }
}
