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
        public Cell[] tileTypes =
        {
            new Cell(Cell.Type.Empty),
            new Cell(Cell.Type.Floor),
            new Cell(Cell.Type.Wall)
        };

        Color[] colors = new Color[10]
        {
            Colors.AmericanBlue,
            Colors.AmericanGreen,
            Colors.AmericanPurple,
            Colors.AmericanRed,
            Colors.AmericanOrange,
            Colors.AmericanYellow,
            Colors.AmericanBrown,
            Colors.AmericanGold,
            Colors.AmericanPink,
            Colors.AmericanSilver,

        };

        public static int H;
        public static int W;
        static char[,] map;

        static int roomCount;
        static int roomIndex = 0;

        static List<Vector2>[] roomTiles;

        static System.Random rndState = new System.Random();
        static int rnd(int x) => rndState.Next() % x;

        public Cell[,] tiles;

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
            W = rndState.Next(25, 50);

            map = new char[W, H];

            roomCount = rndState.Next(4, 10);

            tiles = new Cell[W, H];

            tilemap = GetComponent<Tilemap>();

            tilemap.size = new Vector3Int(W, H, 1);

            //create array of lists to store tile coordinates for each room 
            //and loop through to create lists for the given number of rooms

            roomTiles = new List<Vector2>[roomCount];

            for (int i = 0; i < roomTiles.Length; i++)
            {
                roomTiles[i] = new List<Vector2>();
            }

            tileSprites = new Tile[3];

            tileSprites[0] = ScriptableObject.CreateInstance<Tile>();
            tileSprites[0].sprite = Resources.Load<Sprite>("Tiles/DungeonEmpty");

            tileSprites[1] = ScriptableObject.CreateInstance<Tile>();
            tileSprites[1].sprite = Resources.Load<Sprite>("Tiles/DungeonFloor");

            tileSprites[2] = ScriptableObject.CreateInstance<Tile>();
            tileSprites[2].sprite = Resources.Load<Sprite>("Tiles/DungeonWall");

            // generate
            bool nothing = addRoom(start: true);
            roomIndex++;
            for (int j = 0; j < 5000; j++)
            {
                if (addRoom(start: false))
                    roomIndex++;
                if (roomIndex > roomCount - 1)
                    break;
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
                        colorTile.sprite = tileSprites[1].sprite;
                        if(c=='@')
                        { 
                            colorTile.color = colors[0];
                            roomTiles[0].Add(new Vector2(x, y));
                        }
                        if ((int)c >= 48 & (int)c <= 57)
                        { 
                            colorTile.color = colors[(int)c - 48];
                            roomTiles[(int)c-48].Add(new Vector2(x, y));
                        }
                        tilemap.SetTile(new Vector3Int(x, y, 0), colorTile);
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
                        tilemap.SetTile(new Vector3Int(x, y, 0), tileSprites[0]);
                    }
                }
            }


        }


        static bool addRoom(bool start)
        {
            int h = rnd(10) + 5;
            int w = rnd(6) + 3;
            int ry = rnd(H - h - 2) + 1;
            int rx = rnd(W - w - 2) + 1;

            int doorCount = 0, doorY = 0, doorX = 0;

            // generate a doorX
            if (!start)
            {
                // See if we can process this tile
                for (int x = rx - 1; x < rx + w + 2; x++)
                    for (int y = ry - 1; y < ry + h + 2; y++)
                        if (((int)map[x, y] >= 48 & (int)map[x, y] <= 57))
                            return false;

                // find candidate tiles for the doorX
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
                    return false;
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
                        map[x, y] = (char)(roomIndex + 48);
                }

            // place the doorX
            if (doorCount > 0)
            {
                map[doorX, doorY] = '+';
                return true;
            }

            if (start)
            {
                map[rnd(w) + rx, rnd(h) + ry] = '@';
                return true;
            } /* else {
            // place other objects
            for(int j=0; j<(rnd(6)+1); j++)
                char thing = rnd(4)==0 ? '$' : (char)(65+rnd(62))
                map[rnd(w)+rx, rnd(h)+ry] = thing;
        } */

            return false;
        }

    }

}