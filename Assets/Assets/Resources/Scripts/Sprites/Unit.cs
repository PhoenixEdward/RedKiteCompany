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

        PathFinder pathFinder = new PathFinder();

        protected List<Node> currentPath = null;

        int speed = 2;
        public int movement = 4;

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

        public BoxCollider collider;

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

            collider = gameObject.AddComponent<BoxCollider>();
            collider.center += new Vector3(0, 0.5f, 0);
        }

        public override void Update()
        {
            if (currentPath != null)
            {
                IsMoving = true;

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
                        distanceFromCoord.x  -= Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.x - currentPath[0].cell.x));
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

        public void Move(Vector3 destination)
        {
            currentPath = pathFinder.GeneratePathTo(Coordinate, destination);
        }

        public void ResetFrames()
        {
            HorizontalRow = 0;
            VerticalRow = 0;
        }
    }
}