using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedKite
{ 
    public class Hero : GameSprite
    {
        public TileMapper map;

        public int tileX;
        public int tileY;

        public bool isMoving = false;

        public List<Node> currentPath = null;

        int speed = 2;
        public int movement = 2;

        public override void Start()
        {
            //possibly shortcut in TileMapper code

            map = FindObjectOfType<TileMapper>();

            transform.position = new Vector3(map.spawnPoint.x, map.spawnPoint.y + 1, -1);

            tileX = (int)transform.position.x;
            tileY = (int)transform.position.y;

            base.Start();
        }

        private void Update()
        {
            if (currentPath != null)
            {
                isMoving = true;

               int currNode = 0;

                while (currNode < currentPath.Count - 1)
                {

                    Vector3 start = new Vector3(currentPath[currNode].x, currentPath[currNode].y) +
                        new Vector3(0, 0, -1f);

                    Vector3 end = new Vector3(currentPath[currNode + 1].x, currentPath[currNode + 1].y) +
                        new Vector3(0, 0, -1f);

                    Debug.DrawLine(start, end, Color.red);

                    currNode++;
                }
                Vector3 currentPos = transform.position;

                if (currentPos != new Vector3(currentPath[1].x, currentPath[1].y, currentPos.z))
                {

                    if (currentPos.x < currentPath[1].x)
                        currentPos.x += Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.x - currentPath[1].x));
                    if (currentPos.x > currentPath[1].x)
                        currentPos.x -= Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.x - currentPath[1].x));
                    if (currentPos.y < currentPath[1].y)
                        currentPos.y += Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.y - currentPath[1].y));
                    if (currentPos.y > currentPath[1].y)
                        currentPos.y -= Mathf.Min(speed * Time.deltaTime, Mathf.Abs(currentPos.y - currentPath[1].y));

                    transform.position = currentPos;
                }
                else
                    MoveNextTile();
                if(currentPath.Count == 1)
                {
                    currentPath = null;
                }
            }
            else
                isMoving = false;
        }

        void MoveNextTile()
        {

            if (currentPath == null)
                return;

            tileX = currentPath[1].x;
            tileY = currentPath[1].y;

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