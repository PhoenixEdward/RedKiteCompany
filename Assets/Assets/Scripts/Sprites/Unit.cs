using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RedKite
{
    public class Unit : GameSprite
    {
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
        public bool IsComplete { get; private set; }
        public string State { get; set; }

        protected float timeSinceLastFrame = 0;
        protected readonly float charSecondsPerFrame = .125f;
        protected int Frame;
        protected Vector2 velocity = Vector2.zero;

        protected List<GameSprite> collidables;

        protected float timeSinceLastMove = 0;
        protected float secondsPerMove = 0.0167f;


        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();

            map = FindObjectOfType<TileMapper>();

            tileX = (int)transform.position.x;
            tileY = (int)transform.position.y;


        }


        // Update is called once per frame
        public virtual void Update()
        {
            //UpdateCollision();

            if (currentPath != null)
            {
                IsMoving = true;

                int currNode = 0;

                while (currNode < currentPath.Count - 1)
                {

                    Vector3 start = new Vector3(currentPath[currNode].cell.x, currentPath[currNode].cell.y) +
                        new Vector3(0, 0, -1f);

                    Vector3 end = new Vector3(currentPath[currNode + 1].cell.x, currentPath[currNode + 1].cell.y) +
                        new Vector3(0, 0, -1f);

                    Debug.DrawLine(start, end, Color.red);

                    currNode++;
                }
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
            else
                IsMoving = false;

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
                        if(verticalFrames > 4)
                            VerticalRow = 4;
                    }
                   

                }

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
                IsComplete = true;
            }
            else
            {
                IsComplete = false;
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
