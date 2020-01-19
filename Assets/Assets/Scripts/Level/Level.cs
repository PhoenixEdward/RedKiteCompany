using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{
    public class Level : MonoBehaviour
    {
        PathFinder pathFinder;
        public TileMapper tileMap;
        Modeler modeler;
        List<Hero> heroes;
        // Start is called before the first frame update
        void Awake()
        {
            tileMap = new TileMapper();
            tileMap.Generate();

            pathFinder = new PathFinder();
            pathFinder.GenerateGraph();

            modeler = new Modeler();
            modeler.Generate();

            heroes = new List<Hero>();



            GameObject hero1 = new GameObject();
            hero1.name = "SwordGal4D";
            Hero unit1 = hero1.AddComponent<Hero>();
            unit1.spriteType = GameSprite.SpriteType.Character;
            unit1.spriteName = "SwordGal4D";
            heroes.Add(unit1);
        }
    }
}