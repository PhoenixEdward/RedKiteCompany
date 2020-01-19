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
            spriteType = SpriteType.Character;

            Debug.Log(spriteName);

            base.Start();

            for (int i = 0; i < spawnOffset.Length; i++)
            {
                if(Utility.WithinBounds(level.tileMap.spawnPoint + spawnOffset[i], TileMapper.W,TileMapper.H))
                    if (TileMapper.tiles[(int)(level.tileMap.spawnPoint.x + spawnOffset[i].x), (int)(level.tileMap.spawnPoint.z + spawnOffset[i].z)].IsWalkable)
                    {
                        //check if spawn is occupied. Will need to change later to account for non hero units and other objects spawning
                        if (!activeSpawns.Contains(spawnOffset[i]))
                        {
                            transform.position = new Vector3(level.tileMap.spawnPoint.x + spawnOffset[i].x, 2 ,level.tileMap.spawnPoint.z + spawnOffset[i].z) + offset;
                            activeSpawns.Add(spawnOffset[i]);
                            break;
                        }
                    }

                if (i == spawnOffset.Length - 1)
                    failedSpawn = true;
            }

            if (failedSpawn)
            {
                gameObject.SetActive(false);
            }

            Coordinate.x = Mathf.Floor(transform.position.x);
            Coordinate.y = Mathf.Floor(transform.position.z);

            Debug.Log(Coordinate);

        }

        public override void Update()
        {
            base.Update();
        }

    }
}