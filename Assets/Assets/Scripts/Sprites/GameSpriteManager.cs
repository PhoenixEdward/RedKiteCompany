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
                        GameObject obj = new GameObject
                        {
                            name = typeof(GameSpriteManager).Name
                        };
                        instance = obj.AddComponent<GameSpriteManager>();
                    }
                }
                return instance;
            }
        }

        public List<Unit> Units { get; private set; }

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
            Units = FindObjectsOfType<Unit>().ToList();

        }


    }
}
