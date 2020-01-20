using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{
    public class Level : MonoBehaviour
    {
        PathFinder pathFinder;
        Modeler modeler;
        List<GameObject> heroes;
        CameraMovement cam;
        // Start is called before the first frame update
        void Awake()
        {
            TileMapper.Generate();

            pathFinder = new PathFinder();
            pathFinder.GenerateGraph();

            modeler = new GameObject().AddComponent<Modeler>();

            heroes = new List<GameObject>();

            GameObject hero1 = new GameObject();
            hero1.name = "SwordGal";
            Hero unit1 = hero1.AddComponent<Hero>();
            unit1.spriteType = GameSprite.SpriteType.Character;
            unit1.spriteName = "SwordGal";
            heroes.Add(hero1);

            GameObject hero2 = new GameObject();
            hero2.name = "MageGuy";
            Hero unit2 = hero2.AddComponent<Hero>();
            unit2.spriteType = GameSprite.SpriteType.Character;
            unit2.spriteName = "MageGuy";
            heroes.Add(hero2);

            cam = FindObjectOfType<CameraMovement>();

            cam.enabled = true;
        }

        public void Regen()
        {
            cam.enabled = false;

            TileMapper.Generate();

            modeler.Regen();

            Hero.ClearStatic();

            for (int i = 0; i < heroes.Count; i++)
            {
                Destroy(heroes[i]);
            }

            heroes = new List<GameObject>();

            GameObject hero1 = new GameObject();
            hero1.name = "SwordGal";
            Hero unit1 = hero1.AddComponent<Hero>();
            unit1.spriteType = GameSprite.SpriteType.Character;
            unit1.spriteName = "SwordGal";
            heroes.Add(hero1);

            GameObject hero2 = new GameObject();
            hero2.name = "MageGuy";
            Hero unit2 = hero2.AddComponent<Hero>();
            unit2.spriteType = GameSprite.SpriteType.Character;
            unit2.spriteName = "MageGuy";
            heroes.Add(hero2);

            GameSpriteManager.Instance.GetSprites();

            cam.enabled = true;
        }

    }
}