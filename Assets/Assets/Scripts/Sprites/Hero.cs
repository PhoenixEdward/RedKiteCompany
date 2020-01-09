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

        public static Vector3[] spawnOffset = {
            Vector3.zero,
            new Vector3(0,0,2),
            new Vector3(0,0,-2),
            new Vector3(2,0,0),
            new Vector3(2,0,2),
            new Vector3(2,0,-2),
            new Vector3(-2,0, 0),
            new Vector3(-2,0, 2),
            new Vector3(2,0,-2)

        };


        public override void Start()
        {
            base.Start();

            for (int i = 0; i < spawnOffset.Length; i++)
            {
                if(Utility.WithinBounds(map.spawnPoint + spawnOffset[i], TileMapper.W,TileMapper.H))
                    if (TileMapper.tiles[(int)(map.spawnPoint.x + spawnOffset[i].x), (int)(map.spawnPoint.z + spawnOffset[i].z)].IsWalkable)
                    {
                        //check if spawn is occupied. Will need to change later to account for non hero units and other objects spawning
                        if (!activeSpawns.Contains(spawnOffset[i]))
                        {
                            transform.position = new Vector3(map.spawnPoint.x + spawnOffset[i].x, 2 ,map.spawnPoint.z + spawnOffset[i].z);
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

            Coordinate.x = (int)transform.position.x;
            Coordinate.y = (int)transform.position.z;

        }

        public override void Update()
        {
            base.Update();
        }

    }
}