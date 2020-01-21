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
        static bool firstSpawn = true;
        static Vector3[] spawnPoints;
        static int unitCount = 2;

        public override void Start()
        {
            spriteType = SpriteType.Character;

            base.Start();

            if(firstSpawn)
            {
                spawnPoints = TileMapper.Instance.GetSpawnPoints(unitCount);
                firstSpawn = false;
            }

            foreach(Vector3 spawnPoint in spawnPoints)
            {
                if (!activeSpawns.Contains(spawnPoint))
                {
                    transform.position = spawnPoint + offset + Vector3.up;
                    activeSpawns.Add(spawnPoint);
                    break;
                }
            }

            Coordinate.x = (int)Mathf.Floor(transform.position.x);
            Coordinate.y = (int)Mathf.Floor(transform.position.z);

        }

        public static void ClearStatic()
        {
            activeSpawns.Clear();
            firstSpawn = true;
        }

        public override void Update()
        {
            base.Update();
        }

    }
}