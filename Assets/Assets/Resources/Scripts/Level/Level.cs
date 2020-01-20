using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{
    public class Level : MonoBehaviour
    {
        static Level _instance;

        static public Level Instance
        {
            get
            {
                return _instance ?? (_instance = new Level());
            }
        }

        PathFinder pathFinder;
        Modeler modeler;
        List<GameObject> heroes;
        public static List<GameObject> propInstances;
        CameraMovement cam;
        // Start is called before the first frame update
        void Awake()
        {
            TileMapper.Instance.Generate();

            pathFinder = new PathFinder();
            pathFinder.GenerateGraph();

            modeler = new GameObject().AddComponent<Modeler>();

            heroes = new List<GameObject>();
            propInstances = new List<GameObject>();

            GameObject hero1 = new GameObject();
            hero1.name = "SwordGal";
            Hero unit1 = hero1.AddComponent<Hero>();
            unit1.spriteName = "SwordGal";
            heroes.Add(hero1);

            GameObject hero2 = new GameObject();
            hero2.name = "MageGuy";
            Hero unit2 = hero2.AddComponent<Hero>();
            unit2.spriteName = "MageGuy";
            heroes.Add(hero2);

            QuestMapper.Instance.Generate();

            foreach (KeyValuePair<Vector3, string> prop in QuestMapper.Instance.Props)
            {
                GameObject propInstance = new GameObject();
                propInstances.Add(propInstance);
                propInstance.name = prop.Value;
                Prop entity = propInstance.AddComponent<Prop>();
                entity.Coordinate = prop.Key;
                entity.isIso = true;
                entity.spriteName = prop.Value;
            }

            cam = FindObjectOfType<CameraMovement>();

            cam.enabled = true;
        }

        public void Regen()
        {
            cam.enabled = false;

            TileMapper.Instance.Generate();

            pathFinder.GenerateGraph();

            modeler.Regen();

            Hero.ClearStatic();

            GameObject hero1 = new GameObject();
            hero1.name = heroes[0].GetComponent<GameSprite>().spriteName;
            Hero unit1 = hero1.AddComponent<Hero>();
            unit1.spriteType = GameSprite.SpriteType.Character;
            unit1.spriteLoad = heroes[0].GetComponent<GameSprite>().spriteLoad;

            GameObject hero2 = new GameObject();
            hero2.name = "Unit 2";
            Hero unit2 = hero2.AddComponent<Hero>();
            unit2.spriteType = GameSprite.SpriteType.Character;
            unit2.spriteLoad = heroes[1].GetComponent<GameSprite>().spriteLoad;


            for (int i = 0; i < heroes.Count; i++)
            {
                Destroy(heroes[i]);
            }

            //add new heroes after old heroes have passed on data and been destroyed;
            heroes = new List<GameObject>();
            heroes.Add(hero1);
            heroes.Add(hero2);

            for (int i = 0; i < propInstances.Count; i++)
            {
                Destroy(propInstances[i]);
            }


            //need to cache data or find some way to save some of the modifications made by user.
            //not to keep them in the same place but keep the composition the same.
            propInstances = new List<GameObject>();

            QuestMapper.Instance.Generate();

            foreach (KeyValuePair<Vector3, string> prop in QuestMapper.Instance.Props)
            {
                GameObject propInstance = new GameObject();
                propInstances.Add(propInstance);
                propInstance.name = prop.Value;
                Prop entity = propInstance.AddComponent<Prop>();
                entity.Coordinate = prop.Key;
                entity.isIso = true;
                entity.spriteName = prop.Value;
            }

            GameSpriteManager.Instance.GetSprites();

            cam.enabled = true;
        }

        public void AddProp(Vector3 position, string name, Texture2D spriteSheet, bool _isIso)
        {
            if (QuestMapper.Instance.Props.ContainsKey(position))
                QuestMapper.Instance.Props.Remove(position);

            QuestMapper.Instance.Props.Add(position, name);

            GameObject propInstance = new GameObject();
            propInstances.Add(propInstance);
            Prop entity = propInstance.AddComponent<Prop>();
            entity.Coordinate = position;
            entity.isIso = _isIso;
            entity.spriteName = name;
            entity.spriteLoad = spriteSheet;

        }
        public void RemoveProp(Vector3 position)
        {
            if (QuestMapper.Instance.Props.ContainsKey(position))
                QuestMapper.Instance.Props.Remove(position);
        }
    }
}