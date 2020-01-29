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
                spawnPoints = TileMapper.Instance.GetSpawnPoints();
                firstSpawn = false;
            }

            foreach(Vector3 spawnPoint in spawnPoints)
            {
                if (!activeSpawns.Contains(spawnPoint))
                {
                    Coordinate = grid.WorldToCell(spawnPoint);
                    activeSpawns.Add(spawnPoint);
                    break;
                }
            }

            mirrorRender.material.SetColor("_Color", Color.blue);

            IsVisible = true;
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