﻿using System.Collections;
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
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Level>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject
                        {
                            name = typeof(Level).Name
                        };
                        _instance = obj.AddComponent<Level>();
                    }
                }
                return _instance;
            }
        }

        //perhaps it instantiates the grid?
        public Grid grid;
        PathFinder pathFinder;
        Modeler modeler;
        List<GameObject> heroes;
        Dictionary<Vector3Int,GameObject> propInstances = new Dictionary<Vector3Int, GameObject>();
        CameraMovement cam;
        // Start is called before the first frame update
        void Awake()
        {
            TileMapper.Instance.Generate();

            pathFinder = new PathFinder();
            pathFinder.GenerateGraph();

            Vector3 offset = new Vector3(-0.35f, 0, -0.35f);

            grid = FindObjectOfType<Grid>();

            modeler = new GameObject().AddComponent<Modeler>();

            heroes = new List<GameObject>();
            propInstances = new Dictionary<Vector3Int, GameObject>();

            GameObject hero1 = new GameObject();
            hero1.name = "SwordGal";
            Hero unit1 = hero1.AddComponent<Hero>();
            unit1.spriteName = "Mage";
            heroes.Add(hero1);

            GameObject hero2 = new GameObject();
            hero2.name = "MageGuy";
            Hero unit2 = hero2.AddComponent<Hero>();
            unit2.spriteName = "Mage";
            heroes.Add(hero2);

            QuestMapper.Instance.Generate();

            foreach (KeyValuePair<Vector3Int, string> prop in QuestMapper.Instance.Props)
            {
                GameObject propInstance = new GameObject();
                propInstances.Add(prop.Key, propInstance);
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

            foreach(Vector3Int key in propInstances.Keys)
            {
                Destroy(propInstances[key]);
            }


            //need to cache data or find some way to save some of the modifications made by user.
            //not to keep them in the same place but keep the composition the same.
            propInstances = new Dictionary<Vector3Int, GameObject>();

            QuestMapper.Instance.Generate();

            foreach (KeyValuePair<Vector3Int, string> prop in QuestMapper.Instance.Props)
            {
                GameObject propInstance = new GameObject();
                propInstances.Add(prop.Key, propInstance);
                propInstance.name = prop.Value;
                Prop entity = propInstance.AddComponent<Prop>();
                entity.Coordinate = prop.Key;
                entity.isIso = true;
                entity.spriteName = prop.Value;
            }

            GameSpriteManager.Instance.GetSprites();

            cam.enabled = true;
        }

        public void AddProp(Vector3Int position, string name, Texture2D spriteSheet, bool _isIso)
        {
            if (QuestMapper.Instance.Props.ContainsKey(position))
                QuestMapper.Instance.Props.Remove(position);

            if (propInstances.ContainsKey(position))
            { 
                Destroy(propInstances[position]);
                propInstances.Remove(position);
            }

            QuestMapper.Instance.Props.Add(position, name);

            GameObject propInstance = new GameObject();
            propInstances.Add(position, propInstance);
            Prop entity = propInstance.AddComponent<Prop>();
            entity.Coordinate = position;
            entity.isIso = _isIso;
            entity.spriteName = name;
            entity.spriteLoad = spriteSheet;

        }
        public void RemoveProp(Vector3Int position)
        {
            if (QuestMapper.Instance.Props.ContainsKey(position))
                QuestMapper.Instance.Props.Remove(position);

            if (propInstances.ContainsKey(position))
            {
                Destroy(propInstances[position]);
                propInstances.Remove(position);
            }
        }
    }
}