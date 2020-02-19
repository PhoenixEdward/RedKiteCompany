using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{
    public class GameSprite : MonoBehaviour
    {
        public int ID { get; private set; } = 0;
        static List<int> activeIDs = new List<int>();


        public StateMachine StateMachine { get; private set; }

        public string Name { get; private set; }

        public List<Weapon> Weapons
        {
            get;
            private set;
        } = new List<Weapon>();

        public List<Weapon> Heals
        {
            get;
            private set;
        } = new List<Weapon>();

        public List<Buff> Buffs
        {
            get;
            private set;
        } = new List<Buff>();

        public List<Buff> Debuffs
        {
            get;
            private set;
        } = new List<Buff>();

        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public bool Ready;
        public int Level { get; private set; }
        public Stats Stats { get; protected set; }
        public JobClass jobClass { get; private set; }
        public int Movement { get; private set; }
        public int Fatigue { get; private set; }
        public int PerceptionRange { get; private set; }
        public Vector3 Destination { get; set; } = Vector3.zero;

        public Skill ActiveSkill { get; private set; } = Skill.Alert;
        public static PathFinder pathFinder { get; private set; }

        protected System.Random rnd = new System.Random();

        public enum SpriteType {
            Tile,
            Character,
            Prop
        }

        public SpriteType spriteType;
        public string spriteName;

        public int verticalFrames;
        public int horizontalFrames;

        public int VerticalRow { get; set; }
        public int HorizontalRow { get; set; }

        public Vector3Int Coordinate = new Vector3Int(0, 0, -1);

        static Grid grid;

        //thse will need to be switched to protected after level editor is done
        public Texture2D spriteLoad;
        public Sprite[,] sprites;

        public static Color fogTint;

        protected SpriteRenderer sr;
        protected Vector2Int FrameDimensions;

        static protected CameraMovement cam;

        static CameraMovement.Facing currentCamFacing;

        static Material spriteMask;

        protected Vector3 offset = new Vector3(0, 0, 0);

        //This will be set by fog of war. Unsure about stipulations of set.
        public bool IsVisible { get; set; } = false;

        public bool isIso;

        public bool IsMoving;

        public int MaxAttackRange { get; private set; }
        public int MaxAssistRange { get; private set; }

        public bool IsAlive { get; private set; } = true;

        public virtual void Start()
        {

            //load static variables if null
            if(grid == null)
            {
                grid = FindObjectOfType<Grid>();
            }

            if (spriteMask == null)
            {
                spriteMask = Resources.Load<Material>("RenderTargets/Materials/SpriteMat");
                spriteMask.SetTexture("_MainTex2", GameObject.FindGameObjectWithTag("FogCam").GetComponent<Camera>().activeTexture);
                spriteMask.SetTexture("_MainTex3", FindObjectOfType<WallRender>().wallRender);
            }
            if (spriteLoad == null)
            { 

                if (spriteType == SpriteType.Character)
                    spriteLoad = Resources.Load<Texture2D>("Characters/" + spriteName);
                else if (spriteType == SpriteType.Tile)
                    spriteLoad = Resources.Load<Texture2D>("Tiles/" + spriteName);
                else if (spriteType == SpriteType.Prop)
                    spriteLoad = Resources.Load<Texture2D>("Props/" + spriteName);

            }

            FrameDimensions = FrameDimensions == Vector2.zero ? new Vector2Int(150, 150) : FrameDimensions;

            verticalFrames = spriteLoad.height / FrameDimensions.y;
            horizontalFrames = spriteLoad.width / FrameDimensions.x;

            sprites = new Sprite[horizontalFrames, verticalFrames];

            for (int y = 0; y < verticalFrames; y++)
            {
                for (int x = 0; x < horizontalFrames; x++)
                {
                    Sprite sprite = Sprite.Create(spriteLoad, new Rect(new Vector2(x * FrameDimensions.x, y * FrameDimensions.y), FrameDimensions), new Vector2(0.5f, 0f));
                    sprites[x, y] = sprite;
                }
            }

            sr = gameObject.AddComponent<SpriteRenderer>();

            sr.material = spriteMask;

            sr.sortingLayerName = "Units";

        }

        //needs to be updated to cache some of these variables.
        public static void UpdateAllSprites()
        {
            spriteMask.SetTexture("_MainTex2", GameObject.FindGameObjectWithTag("FogCam").GetComponent<Camera>().activeTexture);
            spriteMask.SetTexture("_MainTex3", FindObjectOfType<WallRender>().wallRender);
            spriteMask.SetColor("_FogColor", GameObject.FindObjectOfType<FOW>().fogColor);
        }

        public virtual void Update()
        {

            if (!IsVisible)
                sr.enabled = false;
            else
                sr.enabled = true;

            if (CameraMovement.facing == CameraMovement.Facing.NE)
            {
                transform.rotation = Quaternion.Euler(0, 45f, 0);

                transform.localPosition = grid.CellToWorld(Coordinate) + new Vector3(0, 0.5f, 0);

                if (horizontalFrames > 3 & !IsMoving)
                {
                    VerticalRow = verticalFrames - 1;
                    HorizontalRow = 3;
                }
            }
            else if (CameraMovement.facing == CameraMovement.Facing.SE)
            {

                transform.rotation = Quaternion.Euler(0, 135f, 0);

                transform.localPosition = grid.CellToWorld(Coordinate) + new Vector3(0f, 0, 1f) + new Vector3(0, 0.5f, 0);


                if (horizontalFrames > 2 & !IsMoving)
                {
                    VerticalRow = verticalFrames - 1;
                    HorizontalRow = 2;
                }
            }
            else if (CameraMovement.facing == CameraMovement.Facing.SW)
            {
                transform.rotation = Quaternion.Euler(0, 225f, 0);


                transform.localPosition = grid.CellToWorld(Coordinate) + new Vector3(1f, 0, 1f) + new Vector3(0, 0.5f, 0);


                if (horizontalFrames > 1 & !IsMoving)
                {
                    VerticalRow = verticalFrames - 1;
                    HorizontalRow = 1;
                }
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 315f, 0);

                transform.localPosition = grid.CellToWorld(Coordinate) + new Vector3(1, 0, 0) + new Vector3(0, 0.5f, 0);

                if (!IsMoving)
                {
                    VerticalRow = verticalFrames - 1;
                    HorizontalRow = 0;
                }
            }

            if (Health <= 0)
            {
                IsAlive = false;
                gameObject.SetActive(false);
            }
        }

        public void ReStart(string textureName, Texture2D texture)
        {
            spriteName = textureName.Substring(0, textureName.Length - 4);

            spriteLoad = texture;

            FrameDimensions = FrameDimensions == Vector2.zero ? new Vector2Int(150, 150) : FrameDimensions;

            verticalFrames = spriteLoad.height / FrameDimensions.y;
            horizontalFrames = spriteLoad.width / FrameDimensions.x;

            sprites = new Sprite[horizontalFrames, verticalFrames];

            for (int y = 0; y < verticalFrames; y++)
            {
                for (int x = 0; x < horizontalFrames; x++)
                {
                    Sprite sprite = Sprite.Create(spriteLoad, new Rect(new Vector2(x * FrameDimensions.x, y * FrameDimensions.y), FrameDimensions), new Vector2(0.5f, 0f));
                    sprites[x, y] = sprite;
                }
            }

        }

        public void GetMaxSkillDistances()
        {

            //find the maximum distance for interactions
            foreach (Skill skill in Weapons)
            {
                if (skill.Range > MaxAttackRange)
                    MaxAttackRange = skill.Range;
            }

            foreach (Skill skill in Heals)
            {
                if (skill.Range > MaxAssistRange)
                    MaxAssistRange = skill.Range;
            }

            foreach (Skill skill in Buffs)
            {
                if (skill.Range > MaxAssistRange)
                    MaxAssistRange = skill.Range;
            }

            foreach (Skill skill in Debuffs)
            {
                if (skill.Range > MaxAttackRange)
                    MaxAttackRange = skill.Range;
            }
        }
        public void Instantiate(string _name, JobClass _jobClass, int _level)
        {
            Name = _name;
            jobClass = _jobClass;

            Stats = new Stats(jobClass);

            Movement = 4 + (Stats.Dexterity.Modifier / 4);

            MaxHealth = 20 + Stats.Constitution.Modifier;

            Level = 1;

            for (int level = 0; level < _level; level++)
                LevelUp();

            while (ID == 0)
            {
                int tempID = rnd.Next(0, 9999);

                if (!activeIDs.Contains(tempID))
                {
                    ID = tempID;
                    activeIDs.Add(tempID);
                }
            }
        }

        public void Spawn()
        {
            if (rnd == null)
                rnd = new System.Random();

            Health = MaxHealth;

            Ready = false;

            if (pathFinder == null)
            {
                pathFinder = new PathFinder();
                pathFinder.GenerateGraph();
            }

            GetMaxSkillDistances();

            StateMachine = new StateMachine(this); 
        }

        public void StartTurn()
        {
            Ready = true;
            Movement = 4 + (Stats.Dexterity.Modifier / 4);

            PerceptionRange = Mathf.Max(10, 5 + (Stats.Intelligence.Modifier / 5));

            if (Stats.Strength.Altered)
                Stats.Strength.DecrementBuffDuration();
            if (Stats.Constitution.Altered)
                Stats.Constitution.DecrementBuffDuration();
            if (Stats.Dexterity.Altered)
                Stats.Dexterity.DecrementBuffDuration();
            if (Stats.Intelligence.Altered)
                Stats.Intelligence.DecrementBuffDuration();
            if (Stats.Wisdom.Altered)
                Stats.Wisdom.DecrementBuffDuration();
            if (Stats.Charisma.Altered)
                Stats.Charisma.DecrementBuffDuration();
        }

        public void EndTurn(int burden)
        {
            Fatigue += Mathf.Max(Mathf.Max(35 - (Stats.Dexterity.Modifier / 2), 10) + burden, 0);
            Ready = false;
        }

        public void DecreaseFatigue()
        {
            if (Fatigue > 0)
                Fatigue--;

            if (Fatigue <= 0)
                StartTurn();
        }

        public void Action(GameSprite _unit, Skill _skill)
        {
            if (_skill is Weapon weapon)
                if (Weapons.Contains(weapon) | Heals.Contains(weapon))
                    weapon.Use(this, _unit);
                else
                { }
            else if (_skill is Buff buff)
                if (Buffs.Contains(buff))
                    buff.Use(this, _unit);

            EndTurn(_skill.Burden);
        }

        public int ChangeHealth(int change, bool anti)
        {

            if (change < 0)
            {
                Debug.Log("Mistake");
                return 0;
            }

            if (anti)
            {
                Health = Mathf.Min(change + Health, MaxHealth);
                return change;
            }
            else
            {
                int damage = jobClass != JobClass.Ranger | jobClass != JobClass.Fighter ?
                change - Stats.Constitution.Modifier : change - Stats.Dexterity.Modifier;

                Health = Mathf.Max(0, Health - damage);

                return Mathf.Min(0,-damage);
            }

            return change;

        }

        public void LearnSkill(string item)
        {
            dynamic skill;

            if (Loot.Keys[item].majorForm != Skill.Form.Charming)
                skill = JsonMerchant.Instance.Load<Weapon>(item);
            else
                skill = JsonMerchant.Instance.Load<Buff>(item);

            if ((jobClass == JobClass.Ranger | jobClass == JobClass.Fighter) & skill.Type.Major == Skill.Form.Finesse)
                Weapons.Add(skill);
            else if ((jobClass == JobClass.Mage | jobClass == JobClass.Bard) & skill.Type.Major == Skill.Form.Clever)
                Weapons.Add(skill);
            else if ((jobClass == JobClass.Cleric | jobClass == JobClass.Mage) & skill.Type.Major == Skill.Form.Wise)
                Heals.Add(skill);
            else if ((jobClass == JobClass.Bard | jobClass == JobClass.Ranger) & skill.Type.Major == Skill.Form.Charming & skill.Anti == false)
                Buffs.Add(skill);
            else if ((jobClass == JobClass.Bard | jobClass == JobClass.Ranger) & skill.Type.Major == Skill.Form.Charming & skill.Anti == true)
                Debuffs.Add(skill);
            else if (skill.Type.Major == Skill.Form.Brute)
                Weapons.Add(skill);

            GetMaxSkillDistances();
        }

        public int AbilityCheck(Skill.Form _type)
        {
            int roll;

            if (_type == Skill.Form.Brute)
                roll = Stats.Strength.Roll(10);
            else if (_type == Skill.Form.Charming)
                roll = Stats.Charisma.Roll(10);
            else if (_type == Skill.Form.Clever)
                roll = Stats.Intelligence.Roll(10);
            else if (_type == Skill.Form.Finesse)
                roll = Stats.Dexterity.Roll(10);
            else if (_type == Skill.Form.Stoic)
                roll = Stats.Constitution.Roll(10);
            else
                roll = Stats.Wisdom.Roll(10);

            return roll;

        }

        public void Buff(Skill.Form _type, int _amount, int _duration, bool anti)
        {
            int amount;

            if (anti)
                amount = -_amount;
            else
                amount = _amount;

            if (_type == Skill.Form.Brute)
                Stats.Strength.Buff(amount, _duration);
            else if (_type == Skill.Form.Charming)
                Stats.Charisma.Buff(amount, _duration);
            else if (_type == Skill.Form.Clever)
                Stats.Intelligence.Buff(amount, _duration);
            else if (_type == Skill.Form.Finesse)
                Stats.Dexterity.Buff(amount, _duration);
            else if (_type == Skill.Form.Stoic)
                Stats.Constitution.Buff(amount, _duration);
            else
                Stats.Wisdom.Buff(amount, _duration);

        }

        public void SetActiveSkill(Skill skill)
        {
            ActiveSkill = skill;
        }

        public int DefensiveRoll()
        {
            int roll;

            roll = Stats.Dexterity.Roll(6);

            return roll;
        }

        public int OffensiveRoll(Skill.Form _type)
        {
            int roll;

            if (_type == Skill.Form.Finesse)
                roll = Stats.Dexterity.Roll(10);
            else if (_type == Skill.Form.Clever)
                roll = Stats.Intelligence.Roll(10);
            else if (_type == Skill.Form.Wise)
                roll = Stats.Wisdom.Roll(10);
            else
                roll = Stats.Charisma.Roll(10);

            return roll;
        }

        public void LevelUp()
        {
            Stats.LevelUp(jobClass);

            Level++;
        }

        public bool HandleMessage(Telegram message) { return StateMachine.HandleMessage(message); }
    }
}