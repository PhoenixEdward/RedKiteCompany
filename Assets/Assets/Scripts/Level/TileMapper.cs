using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;


namespace RedKite
{
    public class TileMapper : MonoBehaviour
    {
        public TileType[] tileTypes =
        {
            new TileType("Empty"),
            new TileType("Floor"),
            new TileType("Wall")
        };

        public static int H;
        public static int W;
        static char[,] map;
        static int[,] roomIndices;

        static System.Random rndState = new System.Random();
        static int rnd(int x) => rndState.Next() % x;

        public TileType[,] tiles;

        Tile[] tileSprites;
        public Tilemap tilemap;
        public Vector2 spawnPoint;
        public static int index = 0;
        //to delete below
        List<Hero> units = new List<Hero>();
        Grid grid;

        void Awake()
        {
            H = rndState.Next(25, 50);
            W = rndState.Next(25,50);
            roomIndices = new int[W, H];

            map = new char[W, H];

            tiles = new TileType[W, H];

            tilemap = GetComponent<Tilemap>();

            tilemap.size = new Vector3Int(W, H, 1);

            //when I start adding UI this might fuck up. Tags might be the solution.

            tileSprites = new Tile[3];

            tileSprites[0] = ScriptableObject.CreateInstance<Tile>();
            tileSprites[0].sprite = Resources.Load<Sprite>("Tiles/DungeonEmpty");

            tileSprites[1] = ScriptableObject.CreateInstance<Tile>();
            tileSprites[1].sprite = Resources.Load<Sprite>("Tiles/DungeonFloor");

            tileSprites[2] = ScriptableObject.CreateInstance<Tile>();
            tileSprites[2].sprite = Resources.Load<Sprite>("Tiles/DungeonWall");

            // generate
            addRoom(start: true);

            for (int j = 0; j < 5000; j++)
            {
                addRoom(start: false);
            }

            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    char c = map[x, y];
                    if (c != '\0' & c != '#' & c != '!')
                    {
                        tiles[x, y] = tileTypes[1];
                        Tile colorTile = ScriptableObject.CreateInstance<Tile>();
                        colorTile.color = new Color(roomIndices[x, y]/roomIndices.Length, 1, 1);
                        colorTile.sprite = tileSprites[1].sprite;
                        tilemap.SetTile(new Vector3Int(x, y, 0), tileSprites[1]);
                        if (c == '@')
                            spawnPoint = tilemap.CellToWorld(new Vector3Int(x, y, 0));
                    }
                    else if (c == '#' | c == '!')
                    {
                        tiles[x, y] = tileTypes[2];
                        tilemap.SetTile(new Vector3Int(x, y, 0), tileSprites[2]);

                    }
                    else if (c == '\0')
                    { 
                        tiles[x, y] = tileTypes[0];
                        tilemap.SetTile(new Vector3Int(x ,y, 0),tileSprites[0]);
                    }
                }
            }


        }

        private void Update()
        {
            
        }

        static void addRoom(bool start)
        {
            int h = rnd(10) + 5;
            int w = rnd(6) + 3;
            int ry = rnd(H - h - 2) + 1;
            int rx = rnd(W - w - 2) + 1;

            int doorCount = 0, doorY = 0, doorX = 0;

            // generate a door
            if (!start)
            {
                // See if we can process this tile
                for (int x = rx - 1; x < rx + w + 2; x++)
                    for (int y = ry - 1; y < ry + h + 2; y++)
                        if (map[x, y] == '.')
                            return;

                // find candidate tiles for the door
                for (int x = rx - 1; x < rx + w + 2; x++)
                    for (int y = ry - 1; y < ry + h + 2; y++)
                    {
                        bool s = y < ry || y > ry + h;
                        bool t = x < rx || x > rx + w;
                        if ((s ^ t) && map[x, y] == '#')
                        {
                            ++doorCount;
                            if (rnd(doorCount) == 0)
                            {
                                doorY = y;
                                doorX = x;
                            }
                        }
                    }

                if (doorCount == 0)
                    return;
            }

            // generate a room
            for (int x = rx - 1; x < rx + w + 2; x++)
                for (int y = ry - 1; y < ry + h + 2; y++)
                {
                    bool s = y < ry || y > ry + h;
                    bool t = x < rx || x > rx + w;
                    if (s && t)
                        map[x, y] = '!'; // avoid generation of doors at corners
                    else if (s ^ t)
                        map[x, y] = '#';
                    else
                    {
                        map[x, y] = '.';
                        roomIndices[x, y] = index;
                    }
                }

            // place the door
            if (doorCount > 0)
                map[doorX, doorY] = '+';

            if (start)
            {
                map[rnd(w) + rx, rnd(h) + ry] = '@';
            } /* else {
                // place other objects
                for(int j=0; j<(rnd(6)+1); j++)
                    char thing = rnd(4)==0 ? '$' : (char)(65+rnd(62))
                    map[rnd(h)+ry, rnd(w)+rx] = thing;
            } */

            index++;


        }

    }

}
