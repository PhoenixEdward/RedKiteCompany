using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

namespace RedKite
{ 
    public class Enemy : Unit
    {
        static bool isFirstRun = true;
        static int spawnRoom;
        static List<Vector3Int> spawnTiles;
        static HashSet<int> filledRooms = new HashSet<int>();
        static Dictionary<int,int> roomEnemyCount = new Dictionary<int,int>();
        static Vector3Int spawnCenter;
        static System.Random rand = new System.Random();
        static int maxEnemies;
        public static int totalEnemies;
        static List<Vector2Int> enemySpawnPoints;

        private Vector3Int spawnPoint;
        private bool failedSpawn = false;


        public override void Start()
        {
            base.Start();

            if(isFirstRun)
            {
                isFirstRun = false;
                
                for(int i = 0; i < TileMapper.Instance.RoomTiles.Length; i++)
                {
                    roomEnemyCount[i] = 0;
                }

                filledRooms.Add(0);

            }

            //check whether all rooms are filled
            //zero indicates that the spawn room has not yet been initialized or that it has reached capacity on previous run.

            if (spawnRoom == 0)
            {
                List<int> potentialRooms = roomEnemyCount.Keys.Where(i => !filledRooms.Contains(i)).ToList();

                if (potentialRooms.Count != 0)
                {
                    potentialRooms.Shuffle<int>();

                    spawnRoom = potentialRooms[0];

                    Debug.Log("spawn room: " + spawnRoom);

                    spawnTiles = new List<Vector3Int>();
                    spawnTiles = TileMapper.Instance.RoomTiles[spawnRoom];
                    spawnTiles.Shuffle<Vector3Int>();

                    spawnCenter = spawnTiles[0];

                    Tile spawnCenterTile = ScriptableObject.CreateInstance<Tile>();

                    spawnCenterTile.color = Colors.NeonPink;

                    spawnTiles.RemoveAt(0);

                    Debug.Log("max enemies: " + TileMapper.Instance.RoomTiles[spawnRoom].Count / 6);

                    maxEnemies = Mathf.Min(TileMapper.Instance.RoomTiles[spawnRoom].Count / 10,5);
                }
                else
                    failedSpawn = true;
            }

            if (roomEnemyCount[spawnRoom] >= maxEnemies)
            {
                filledRooms.Add(spawnRoom);
                spawnRoom = 0;
                spawnCenter = Vector3Int.zero;
                spawnTiles = null;
                failedSpawn = true;
            }

            if (!failedSpawn)
            { 
                for (int i = 0; i < spawnTiles.Count; i++)
                {
                    if(Utility.ManhattanDistance(spawnCenter, spawnTiles[i]) <= 4)
                    { 
                        spawnPoint = spawnTiles[i];
                        spawnTiles.RemoveAt(i);
                        roomEnemyCount[spawnRoom] += 1;
                        break;
                    }

                    if (i == spawnTiles.Count - 1)
                    { 
                        spawnRoom = 0;
                        failedSpawn = true;
                    }
                }
            }

            if (failedSpawn)
                gameObject.SetActive(false);
            else
            {
                transform.position = new Vector3(spawnPoint.x, spawnPoint.y, -1);
            }
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }

    }
}