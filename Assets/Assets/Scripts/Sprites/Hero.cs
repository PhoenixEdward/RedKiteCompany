using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedKite
{ 
    public class Hero : Unit
    {
        bool failedSpawn;
        static List<Vector2> activeSpawns = new List<Vector2>();

        public static Vector2[] spawnOffset = {
            Vector2.zero,
            new Vector2(0,2),
            new Vector2(0,-2),
            new Vector2(2,0),
            new Vector2(2,2),
            new Vector2(2,-2),
            new Vector2(-2, 0),
            new Vector2(-2, 2),
            new Vector2(2,-2)

        };


        public override void Start()
        {
            base.Start();

            for (int i = 0; i < spawnOffset.Length; i++)
            {
                if (map.tiles[(int)(map.spawnPoint.x + spawnOffset[i].x), (int)(map.spawnPoint.y + spawnOffset[i].y)].IsWalkable)
                {
                    //check if spawn is occupied. Will need to change later to account for non hero units and other objects spawning
                    if (!activeSpawns.Contains(spawnOffset[i]))
                    {
                        transform.position = new Vector3(map.spawnPoint.x + spawnOffset[i].x, map.spawnPoint.y + spawnOffset[i].y, -1);
                        activeSpawns.Add(spawnOffset[i]);
                        break;
                    }
                }

                if (i == spawnOffset.Length - 1)
                    failedSpawn = true;
            }

            if (failedSpawn)
            {
                this.gameObject.SetActive(false);
            }

        }

        public override void Update()
        {
            base.Update();
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