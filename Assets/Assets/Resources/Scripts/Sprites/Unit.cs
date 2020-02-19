using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedKite
{
    [System.Serializable]
    public class Unit : GameSprite
    {
        //Should destination be here?
        protected static Grid grid;

        public List<Node> currentPath = null;

        readonly int speed = 2;

        public bool IsAnimated { get; set; }
        public bool IsReverseAnimated { get; set; }

        protected float timeSinceLastFrame = 0;
        protected readonly float charSecondsPerFrame = .125f;
        protected int Frame;
        protected Vector3 velocity = Vector3.zero;
        public GameObject mirror;
        
        public SpriteRenderer mirrorRender;

        public BoxCollider boxCollider;

        public Vector3 distanceFromCoord;

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
                    WalkingAnimation();
                }
            }
            // move to stationary row if not moving. This is currently being overwritten by the gamesprite update.


            if (currentPath == null)
            {
                velocity = Vector3.zero;
                IsMoving = false;
            }

            //correct if attempting to pass into an unavailable row.

            if (VerticalRow > verticalFrames | HorizontalRow > horizontalFrames)
            {
                VerticalRow = 0;
                HorizontalRow = 0;
            }

            timeSinceLastFrame += Time.deltaTime;

            StateMachine.Upate();

            base.Update();

            sr.sprite = sprites[HorizontalRow, VerticalRow];
            mirrorRender.sprite = sprites[HorizontalRow, VerticalRow];

            transform.localPosition += new Vector3(distanceFromCoord.x, 0, distanceFromCoord.y);

        }

        public void WalkingAnimation()
        {
            if (CameraMovement.facing == CameraMovement.Facing.NE)
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

            if (HorizontalRow < horizontalFrames - 1)
                HorizontalRow += 1;
            else
                HorizontalRow = 0;
        }
        public void Move()
        {
            Vector3 currentPos = Coordinate + distanceFromCoord;

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

                currentPath = null;
            }

        }

        public virtual void Embark(Vector3 destination, bool nextTo = false, bool isAttack = true)
        {
            Destination = destination;


            if(nextTo)
                currentPath = pathFinder.GeneratePathTo(Coordinate, destination, Movement, true);
            else
                currentPath = pathFinder.GeneratePathTo(Coordinate, destination, Movement);

            IsMoving = true;
        }

        public void ResetFrames()
        {
            HorizontalRow = 0;
            VerticalRow = 0;
        }
    }
}