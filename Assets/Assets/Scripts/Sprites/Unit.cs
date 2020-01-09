using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedKite
{ 
    [System.Serializable]
    public class Unit : GameSprite
    {

        Vector3 offset = new Vector3(0.35f, 0, 0.35f);

        //Should destination be here?
        public TileMapper map;
        static Grid grid;

        PathFinder pathFinder = new PathFinder();

        public Vector3Int Coordinate;

        protected List<Node> currentPath = null;

        int speed = 2;
        public int movement = 4;


        public int VerticalRow { get; set; }
        public int HorizontalRow { get; set; }

        public Vector3 Destination { get; set; } = Vector3.zero;
        public bool IsMoving;
        public bool IsAnimated { get; set; }
        public bool IsReverseAnimated { get; set; }

        protected float timeSinceLastFrame = 0;
        protected readonly float charSecondsPerFrame = .125f;
        protected int Frame;
        protected Vector3 velocity = Vector3.zero;

        public override void Start()
        {
            //possibly shortcut in TileMapper code
            grid = FindObjectOfType<Grid>();

            map = FindObjectOfType<TileMapper>();

            currentPath = null;

            base.Start();
        }

        public virtual void Update()
        {
  

            if (currentPath != null)
            {
                IsMoving = true;

                Vector3 cellPos = grid.CellToWorld(currentPath[0].cell);

                cellPos.y = transform.position.y;

                Debug.Log(cellPos);

                Vector3 currentPos = transform.position - offset;

                if (currentPos != cellPos)
                {

                    if (currentPos.x < cellPos.x)
                    {
                        currentPos.x += Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.x - cellPos.x));

                        velocity.x = 1;
                        Debug.Log("Is moving up");

                    }
                    else if (currentPos.x > cellPos.x)
                    {
                        currentPos.x -= Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.x - cellPos.x));
                        velocity.x = -1;
                        Debug.Log("Is moving down");

                    }
                    else
                        velocity.x = 0;
                    
                    if (currentPos.z < cellPos.z)
                    {
                        currentPos.z += Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.z - cellPos.z));

                        velocity.z = 1;
                        Debug.Log("Is moving right");

                    }
                    else if (currentPos.z > cellPos.z)
                    {
                        currentPos.z -= Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.z - cellPos.z));

                        Debug.Log("Is moving left");

                        velocity.z = -1;
                    }

                    else
                        velocity.z = 0;

                    transform.position = currentPos + offset;
                }
                else
                { 
                    MoveNextTile();
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
                            VerticalRow = 3;
                    }
                    else if (velocity.x < 0)
                    {
                        if (verticalFrames > 2)
                            VerticalRow = 1;
                    }

                    else if (velocity.z > 0)
                    {
                        if (verticalFrames > 3)
                            VerticalRow = 2;
                    }
                    else if (velocity.z < 0)
                    {
                        if (verticalFrames > 4)
                            VerticalRow = 4;
                    }
                }

                // move to stationary row if not moving. Could probably remove that row  and simply lock them to the first horizontal pane of the correct row.

                else
                {
                    if (VerticalRow == 1 & 1 <= horizontalFrames)
                        HorizontalRow = 0;
                    else if (VerticalRow == 2 & 2 <= horizontalFrames)
                        HorizontalRow = 1;
                    else if (VerticalRow == 3 & 3 <= horizontalFrames)
                        HorizontalRow = 2;
                    else if (VerticalRow == 4 & 4 <= horizontalFrames)
                        HorizontalRow = 3;

                    VerticalRow = 0;
                }

                //could move this code under "is moving" and it would probably eliminate the need of the code above.
                if(velocity != Vector3.zero)
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


            timeSinceLastFrame += Time.deltaTime;

            Sprite sprite = sprites[HorizontalRow, VerticalRow];

            sr.sprite = sprite;

        }

        void MoveNextTile()
        {

            if (currentPath == null)
                return;

            //remove the old current/first node from the path

            Debug.Log(currentPath.Count);

            Coordinate.x = currentPath[0].cell.x;
            Coordinate.y = currentPath[0].cell.y;

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

        public void Move(int x, int y)
        {
            currentPath = pathFinder.GeneratePathTo(this, x, y);
        }

    }
}