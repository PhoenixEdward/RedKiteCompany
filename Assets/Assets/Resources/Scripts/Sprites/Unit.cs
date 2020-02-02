using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedKite
{
    [System.Serializable]
    public class Unit : GameSprite
    {
        public string Name { get; private set; }

        public List<Skill> Skills
        {
            get;
            private set;
        } = new List<Skill>();


        public int Health { get; private set; }
        public int MaxHealth { get; private set; }

        public bool Ready;

        public int Level { get; private set; }

        public Stats Stats { get; protected set; }
        public JobClass jobClass { get; private set; }
        public int Movement { get; private set; }
        public int Initiative { get; private set; }

        protected System.Random rnd;

        //Should destination be here?
        protected static Grid grid;

        readonly PathFinder pathFinder = new PathFinder();

        protected List<Node> currentPath = null;

        readonly int speed = 2;

        public Vector3 Destination { get; set; } = Vector3.zero;
        public bool IsAnimated { get; set; }
        public bool IsReverseAnimated { get; set; }

        protected float timeSinceLastFrame = 0;
        protected readonly float charSecondsPerFrame = .125f;
        protected int Frame;
        protected Vector3 velocity = Vector3.zero;
        public Vector3Int nextCell;
        public GameObject mirror;
        
        public SpriteRenderer mirrorRender;

        public BoxCollider boxCollider;

        public Vector3 distanceFromCoord;

        public void Instantiate(string _name, JobClass _jobClass, int _level)
        {
            Name = _name;
            jobClass = _jobClass;

            Stats = new Stats(jobClass);

            Movement = 4 + (Stats.Dexterity.Modifier / 2);

            MaxHealth = 20 + Stats.Constitution.Modifier;

            Level = 1;

            for (int level = 0; level < _level; level++)
                LevelUp();
        }

        public void Spawn()
        {
            if (rnd == null)
                rnd = new System.Random();

            Initiative = rnd.Next(0, 10) + Stats.Dexterity.Modifier;

            Health = MaxHealth;

            Ready = true;
        }

        public void StartTurn()
        {
            Movement = 4 + (Stats.Dexterity.Modifier / 2);

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

        public void NextTurn()
        {
            Ready = true;
        }

        public void Wait()
        {
            Ready = false;
        }

        public void Action(Unit _unit, Skill _skill)
        {
            if (_skill.Type.Major != Skill.Form.None)
                if (Skills.Contains(_skill))
                    _skill.Use(this, _unit);

            Ready = false;
        }

        public void ChangeHealth(int change, bool anti)
        {

            if (change < 0)
            {
                return;
            }

            if (anti)
                Health = Mathf.Min(change + Health, MaxHealth);
            else
            {
                int damage = jobClass != JobClass.Ranger | jobClass != JobClass.Fighter ?
                change - Stats.Constitution.Modifier : change - Stats.Dexterity.Modifier;

                Health = Mathf.Max(0, Health - damage);
            }

        }

        public void LearnSkill(string item)
        {
            dynamic skill;

            if (Loot.Keys[item].majorForm != Skill.Form.Charming)
                skill = JsonMerchant.Instance.Load<Weapon>(item);
            else
                skill = JsonMerchant.Instance.Load<Buff>(item);

            if ((jobClass == JobClass.Ranger | jobClass == JobClass.Fighter) & skill.Type.Major == Skill.Form.Finesse)
                Skills.Add(skill);
            else if ((jobClass == JobClass.Mage | jobClass == JobClass.Bard) & skill.Type.Major == Skill.Form.Clever)
                Skills.Add(skill);
            else if ((jobClass == JobClass.Cleric | jobClass == JobClass.Mage) & skill.Type.Major == Skill.Form.Wise)
                Skills.Add(skill);
            else if ((jobClass == JobClass.Bard | jobClass == JobClass.Ranger) & skill.Type.Major == Skill.Form.Charming)
                Skills.Add(skill);
            else if (skill.Type.Major == Skill.Form.Brute)
                Skills.Add(skill);


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

        public override void Start()
        {
            base.Start();

            //possibly shortcut in TileMapper code
            grid = FindObjectOfType<Grid>();
            //transform.parent = grid.transform;
            //level.tileMap = FindObjectOfType<TileMapper>();

            currentPath = null;

            mirror = new GameObject();
            mirrorRender = mirror.AddComponent<SpriteRenderer>();
            mirrorRender.material.shader = Shader.Find("Unlit/GlowMask");

            mirror.transform.SetParent(transform);
            mirror.layer = 13;

            boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center += new Vector3(0, 0.5f, 0);
        }

        public override void Update()
        {
            if (currentPath != null)
            {
                Move();
            }
            if (timeSinceLastFrame > charSecondsPerFrame)
            {

                timeSinceLastFrame = 0;
                if (IsMoving)
                {
                    if(CameraMovement.facing == CameraMovement.Facing.NE)
                    { 
                        if (velocity.x > 0)
                        {
                            if (verticalFrames > 1)
                                VerticalRow = 1;
                        }
                        else if (velocity.x < 0)
                        {
                            if (verticalFrames > 2)
                                VerticalRow = 3;
                        }

                        else if (velocity.z > 0)
                        {
                            if (verticalFrames > 3)
                                VerticalRow = 2;
                        }
                        else if (velocity.z < 0)
                        {
                            VerticalRow = 0;
                        }
                    }
                    else if (CameraMovement.facing == CameraMovement.Facing.SE)
                    {
                        if (velocity.z < 0)
                        {
                            if (verticalFrames > 1)
                                VerticalRow = 1;
                        }
                        else if (velocity.z > 0)
                        {
                            if (verticalFrames > 2)
                                VerticalRow = 3;
                        }

                        else if (velocity.x > 0)
                        {
                            if (verticalFrames > 3)
                                VerticalRow = 2;
                        }
                        else if (velocity.x < 0)
                        {
                            VerticalRow = 0;
                        }
                    }
                    else if (CameraMovement.facing == CameraMovement.Facing.SW)
                    {
                        if (velocity.z < 0)
                        {
                            if (verticalFrames > 1)
                                VerticalRow = 2;
                        }
                        else if (velocity.z > 0)
                        {
                            if (verticalFrames > 2)
                                VerticalRow = 0;
                        }

                        else if (velocity.x < 0)
                        {
                            if (verticalFrames > 3)
                                VerticalRow = 1;
                        }
                        else if (velocity.x > 0)
                        {
                            VerticalRow = 3;
                        }
                    }
                    else
                    {
                        if (velocity.x < 0)
                        {
                            if (verticalFrames > 1)
                                VerticalRow = 2;
                        }
                        else if (velocity.x > 0)
                        {
                            if (verticalFrames > 2)
                                VerticalRow = 0;
                        }

                        else if (velocity.z > 0)
                        {
                            if (verticalFrames > 3)
                                VerticalRow = 1;
                        }
                        else if (velocity.z < 0)
                        {
                            VerticalRow = 3;
                        }
                    }
                }

                // move to stationary row if not moving. Could probably remove that row  and simply lock them to the first horizontal pane of the correct row.

                else
                {
                    if (VerticalRow == 1 & 2 <= horizontalFrames)
                        HorizontalRow = 2;
                    else if (VerticalRow == 2 & 1 <= horizontalFrames)
                        HorizontalRow = 1;
                    else if (VerticalRow == 3 & 0 <= horizontalFrames)
                        HorizontalRow = 0;
                    else if (VerticalRow == 0 & 4 <= horizontalFrames)
                        HorizontalRow = 3;

                }

                //could move this code under "is moving" and it would probably eliminate the need of the code above.
                if(IsMoving)
                { 
                    if (HorizontalRow < horizontalFrames - 1)
                        HorizontalRow += 1;
                    else
                        HorizontalRow = 0;
                }

                if (IsAnimated && Frame < 3)
                {
                    HorizontalRow += 1;
                    Frame++;
                }
                else
                {
                    Frame = 0;
                    IsAnimated = false;
                }
                if (IsReverseAnimated && Frame < 3)
                    HorizontalRow -= 1;
                else
                {
                    Frame = 0;
                    IsReverseAnimated = false;
                }
            }

            if (currentPath == null)
            {
                velocity = Vector3.zero;
                IsMoving = false;
            }

            if (VerticalRow > verticalFrames | HorizontalRow > horizontalFrames)
            {
                VerticalRow = 0;
                HorizontalRow = 0;
            }

            timeSinceLastFrame += Time.deltaTime;

            base.Update();

            sr.sprite = sprites[HorizontalRow, VerticalRow];
            mirrorRender.sprite = sprites[HorizontalRow, VerticalRow];

            transform.localPosition += new Vector3(distanceFromCoord.x, 0, distanceFromCoord.y);

        }

        public void Move()
        {
            Vector3 currentPos = Coordinate + distanceFromCoord;

            nextCell = currentPath[0].cell;

            if (currentPos != currentPath[0].cell)
            {

                if (currentPos.x < currentPath[0].cell.x)
                {
                    distanceFromCoord.x += Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.x - currentPath[0].cell.x));

                    velocity.x = 1;

                }
                else if (currentPos.x > currentPath[0].cell.x)
                {
                    distanceFromCoord.x -= Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.x - currentPath[0].cell.x));
                    velocity.x = -1;

                }
                else
                    velocity.x = 0;

                if (currentPos.y < currentPath[0].cell.y)
                {
                    distanceFromCoord.y += Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.y - currentPath[0].cell.y));

                    velocity.z = 1;

                }
                else if (currentPos.y > currentPath[0].cell.y)
                {
                    distanceFromCoord.y -= Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.y - currentPath[0].cell.y));

                    velocity.z = -1;
                }

                else
                    velocity.z = 0;

            }
            else
            {
                MoveNextTile();
            }
        }

        void MoveNextTile()
        {

            if (currentPath == null)
                return;

            //remove the old current/first node from the path

            Coordinate = currentPath[0].cell;

            distanceFromCoord = Vector3.zero;

            // Grab the new first node and move to that position

            if (currentPath.Count > 1)
            {
                currentPath.RemoveAt(0);

            }


            if (currentPath.Count == 1)
            {
                Debug.Log("Arrived");
                currentPath = null;
            }

        }

        public virtual void Embark(Vector3 destination)
        {
            currentPath = pathFinder.GeneratePathTo(Coordinate, destination);
            IsMoving = true;
        }

        public void ResetFrames()
        {
            HorizontalRow = 0;
            VerticalRow = 0;
        }
    }
}