using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedKite
{ 
    public class Unit : GameSprite
    {

        //Should destination be here?
        public TileMapper map;

        public int tileX;
        public int tileY;

        public List<Node> currentPath = null;

        int speed = 2;
        public int movement = 4;


        public int VerticalRow { get; set; }
        public int HorizontalRow { get; set; }

        public Vector2 Destination { get; set; } = Vector2.zero;
        public bool IsMoving { get; protected set; }
        public bool IsAnimated { get; set; }
        public bool IsReverseAnimated { get; set; }

        protected float timeSinceLastFrame = 0;
        protected readonly float charSecondsPerFrame = .125f;
        protected int Frame;
        protected Vector2 velocity = Vector2.zero;

        public override void Start()
        {
            //possibly shortcut in TileMapper code

            map = FindObjectOfType<TileMapper>();


            tileX = (int)transform.position.x;
            tileY = (int)transform.position.y;

            base.Start();
        }

        public virtual void Update()
        {

            if (currentPath != null)
            {
                IsMoving = true;

                Vector3 currentPos = transform.position;

                if (currentPos != new Vector3(currentPath[1].cell.x, currentPath[1].cell.y, currentPos.z))
                {

                    if (currentPos.x < currentPath[1].cell.x)
                    {
                        currentPos.x += Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.x - currentPath[1].cell.x));

                        velocity.x = 1;

                    }
                    else if (currentPos.x > currentPath[1].cell.x)
                    {
                        currentPos.x -= Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.x - currentPath[1].cell.x));
                        velocity.x = -1;

                    }
                    else
                        velocity.x = 0;

                    if (currentPos.y < currentPath[1].cell.y)
                    {
                        currentPos.y += Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.y - currentPath[1].cell.y));

                        velocity.y = 1;
                    }
                    else if (currentPos.y > currentPath[1].cell.y)
                    {
                        currentPos.y -= Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.y - currentPath[1].cell.y));

                        velocity.y = -1;
                    }

                    else
                        velocity.y = 0;

                    transform.position = currentPos;
                }
                else
                    MoveNextTile();
                if (currentPath.Count == 1)
                {
                    currentPath = null;
                }
            }

            if (timeSinceLastFrame > charSecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                if (IsMoving)
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

                    else if (velocity.y > 0)
                    {
                        if (verticalFrames > 3)
                            VerticalRow = 2;
                    }
                    else if (velocity.y < 0)
                    {
                        if (verticalFrames > 4)
                            VerticalRow = 4;
                    }
                }

                // move to stationary row if not moving. Could probably remove that row  and simply lock them to the first horizontal pane of the correct row.

                else
                {
                    if (VerticalRow == 1 & 0 < horizontalFrames)
                        HorizontalRow = 0;
                    else if (VerticalRow == 2 & 1 < horizontalFrames)
                        HorizontalRow = 1;
                    else if (VerticalRow == 3 & 1 < horizontalFrames)
                        HorizontalRow = 2;
                    else if (VerticalRow == 4)
                        HorizontalRow = 3;

                    VerticalRow = 0;
                }

                //could move this code under "is moving" and it would probably eliminate the need of the code above.

                if (HorizontalRow < horizontalFrames - 1)
                    HorizontalRow += 1;
                else
                    HorizontalRow = 0;

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
                velocity = Vector2.zero;
                IsMoving = false;
            }


            timeSinceLastFrame += Time.deltaTime;

            Sprite sprite = sprites[HorizontalRow, VerticalRow];

            sr.sprite = sprite;

        }

        void MoveNextTile()
        {

            if (currentPath == null)
                return;

            tileX = currentPath[1].cell.x;
            tileY = currentPath[1].cell.y;

            //remove the old current/first node from the path

            // Grab the new first node and move to that position

            if (currentPath.Count > 1)
            {
                currentPath.RemoveAt(0);

            }
            // We only have one tile left in the path that MUST be our destination. So now we set current path to null


        }

    }
}