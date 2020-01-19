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

            modeler = new GameObject().AddComponent<Modeler>();

            heroes = new List<Hero>();



            GameObject hero1 = new GameObject();
            hero1.name = "SwordGal";
            Hero unit1 = hero1.AddComponent<Hero>();
            unit1.spriteType = GameSprite.SpriteType.Character;
            unit1.spriteName = "SwordGal";
            heroes.Add(unit1);

            GameObject hero2 = new GameObject();
            hero2.name = "MageGuy";
            Hero unit2 = hero2.AddComponent<Hero>();
            unit2.spriteType = GameSprite.SpriteType.Character;
            unit2.spriteName = "MageGuy";
            heroes.Add(unit2);
        }
    }
}