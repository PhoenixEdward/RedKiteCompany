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
        public Material spriteMat;
        public Texture2D fogTexture;
        Reticle reticle;
        SpriteSelection spriteSelection;

        Color fogColor;

        public FOW fow;
        Glow glow;
        // Start is called before the first frame update
        void Awake()
        {
            TileMapper.Instance.Generate();

            pathFinder = new PathFinder();
            pathFinder.GenerateGraph();

            Vector3 offset = new Vector3(-0.35f, 0, -0.35f);

            grid = FindObjectOfType<Grid>();

            fow = FindObjectOfType<FOW>();

            reticle = FindObjectOfType<Reticle>();

            spriteSelection = FindObjectOfType<SpriteSelection>();

            modeler = new GameObject().AddComponent<Modeler>();
            modeler.name = "Modeler";

            heroes = new List<GameObject>();
            propInstances = new Dictionary<Vector3Int, GameObject>();

            GameSprite.fogTint = fogColor;

            //Weapon weapon = JsonUtility.FromJson<Weapon>("{\"Name\": \"Training Axe\",\"Uses\": 25,\"Anti\": false,\"DiceBonus\": 6,\"BaseBonus\": 6,\"Type\": {\"Major\": 1,\"Minor\": 0},\"Range\": 1}");

            Weapon weapon = new Weapon("Lute", 100, false, 6, 4, Skill.Form.Charming, Skill.Form.None, 1, 5);

            string lute = JsonUtility.ToJson(weapon);

            Debug.Log(lute);

            GameObject hero1 = new GameObject();
            hero1.name = "SwordGal";
            hero1.layer = 8;
            Hero unit1 = hero1.AddComponent<Hero>();
            unit1.spriteName = "SwordGal";
            unit1.Instantiate("Gongagoo", JobClass.Ranger, 5);
            unit1.Spawn();
            unit1.LearnSkill("Training Short Bow");
            heroes.Add(hero1);

            Debug.Log(unit1.Weapons[0].Name);
            Debug.Log(unit1.Weapons[0].Burden);
            Debug.Log(unit1.Weapons[0].Type);

            GameObject hero2 = new GameObject();
            hero2.name = "MageGuy";
            hero2.layer = 8;
            Hero unit2 = hero2.AddComponent<Hero>();
            unit2.spriteName = "Mage";
            unit2.Instantiate("Cestra", JobClass.Cleric, 5);
            unit2.Spawn();
            unit2.LearnSkill("Cure");
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

            glow = FindObjectOfType<Glow>();

            cam = FindObjectOfType<CameraMovement>();

            cam.enabled = true;

            BattleClock.Instance.Run();
        }

        private void Update()
        {
            GameSprite.UpdateAllSprites();
        }

        public void Regen()
        {
            spriteSelection.enabled = false;

            cam.enabled = false;

            TileMapper.Instance.Generate();

            pathFinder.GenerateGraph();

            modeler.Regen();

            Hero.ClearStatic();

            GameObject hero1 = new GameObject();
            hero1.name = heroes[0].GetComponent<GameSprite>().spriteName;
            hero1.layer = 8;
            Hero unit1 = hero1.AddComponent<Hero>();
            unit1.spriteType = GameSprite.SpriteType.Character;
            unit1.spriteLoad = heroes[0].GetComponent<GameSprite>().spriteLoad;
            unit1.Instantiate("Gongagoo", JobClass.Ranger, 5);
            unit1.Spawn();


            GameObject hero2 = new GameObject();
            hero2.name = "Unit 2";
            hero2.layer = 8;
            Hero unit2 = hero2.AddComponent<Hero>();
            unit2.spriteType = GameSprite.SpriteType.Character;
            unit2.spriteLoad = heroes[1].GetComponent<GameSprite>().spriteLoad;
            unit2.Instantiate("Cestra", JobClass.Bard, 5);
            unit2.Spawn();


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

            fow.GenerateFog();

            reticle.Generate();

            glow.Regen();

            spriteSelection.enabled = true;

            cam.enabled = true;

            BattleClock.Instance.Run();
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
            entity.IsVisible = true;

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