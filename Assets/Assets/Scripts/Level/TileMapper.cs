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
            Colors.AmericanViolet,
            Colors.AmericanRed,
            Colors.AmericanOrange,
            Colors.AmericanYellow,
            Colors.AmericanBrown,
            Colors.AmericanPink,
            Colors.AmericanSilver,
            Colors.AmericanPurple,

        };

        public static int H;
        public static int W;
        public static char[,] map;

        static int areaCount;
        public static Dictionary<int, Area> areas;
        static int notOutOfBounds;
        static int roomIndex = 0;
        static int allRuns;

        static char TILE_VOID = ' ';
        //static char TILE_FLOOR = '.';
        static char TILE_WALL = '#';
        static char TILE_CORNER = '!';
        static char TILE_HALL = '+';
        static char TILE_SPAWN = '@';
        //static char TILE_ROOM_CORNER = '?';
        //static char TILE_VISITED = '$';
        //static char TILE_POPULAR = '&';
        static char TILE_WALL_CORNER = '%';
        static int failed;
        static int failedDoors;
        static int[] roomFailures = new int[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        //everything below is unity related
        public static List<Vector3Int>[] RoomTiles { get; private set; }

        static System.Random rndState = new System.Random();
        static int rnd(int x) => rndState.Next() % x;

        public Cell[,] tiles;

        Tile[] tileSprites;
        public Tilemap tilemap;
        public Vector3Int spawnPoint;
        public static int index = 0;
        //to delete below
        List<Unit> units = new List<Unit>();
        Grid grid;

        void Awake()
        {
            H = rndState.Next(40, 50);
            W = rndState.Next(40, 50);

            map = new char[W, H];

            areaCount = 10;
            areas = new Dictionary<int,Area>();


            tiles = new Cell[W, H];

            tilemap = GetComponent<Tilemap>();

            tilemap.size = new Vector3Int(W, H, 1);

            //check if this even does anything anymore? Probably not necessary now.

            RoomTiles = new List<Vector3Int>[areaCount];

            for (int i = 0; i < RoomTiles.Length; i++)
            {
                RoomTiles[i] = new List<Vector3Int>();
            }

            tileSprites = new Tile[3];

            tileSprites[0] = ScriptableObject.CreateInstance<Tile>();
            tileSprites[0].sprite = Resources.Load<Sprite>("Tiles/DungeonEmpty");

            tileSprites[1] = ScriptableObject.CreateInstance<Tile>();
            tileSprites[1].sprite = Resources.Load<Sprite>("Tiles/DungeonFloor");

            tileSprites[2] = ScriptableObject.CreateInstance<Tile>();
            tileSprites[2].sprite = Resources.Load<Sprite>("Tiles/DungeonWall");


            //instantiate void cells
            for (int y = 0; y < map.GetLength(1); y++)
                for (int x = 0; x < map.GetLength(0); x++)
                    map[x, y] = TILE_VOID;
            
            
            // generate tilemap data

            AddSpawn();
            roomIndex++;

            for (int j = 0; j < 5000; j++)
            {
                if (AddArea())
                {
                    roomIndex++;
                }
                if (roomIndex > areaCount - 1)
                    break;
                if (j == 4999)
                    Debug.Log("Bullshit!");
            }

            SplitAllWalls();

            foreach (Area area in areas.Values)
            {
                foreach(Area.Wall wall in area.Walls)
                {
                    foreach (Area.Wall.Segment segment in wall.Paths.Where(x => x.IsRemoved == false))
                    {
                        Vector3[] tiles = Utility.CoordRange(segment.Min, segment.Max);
                            foreach(Vector3 tile in tiles)
                            {
                                map[(int)tile.x,(int)tile.y] = TILE_HALL;
                            }
                    }
                }

            }

            foreach (Area area in areas.Values)
            {
                foreach (Area.Wall wall in area.Walls)
                {
                    foreach (Area.Wall.Segment segment in wall.Segments)
                    {
                        Vector3[] tiles = Utility.CoordRange(segment.Min, segment.Max);
                        foreach (Vector3 tile in tiles)
                        {
                            map[(int)tile.x, (int)tile.y] = TILE_WALL;
                        }
                    }
                }

            }


            //place tiles
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    char c = map[x, y];
                    if (c != TILE_VOID & c != TILE_WALL & c != TILE_WALL_CORNER)
                    {
                        tiles[x, y] = tileTypes[1];
                        Tile colorTile = ScriptableObject.CreateInstance<Tile>();
                        colorTile.sprite = tileSprites[1].sprite;
                        if(c==TILE_SPAWN)
                        { 
                            colorTile.color = colors[0];
                            RoomTiles[0].Add(new Vector3Int(x, y, 1));
                            spawnPoint = Vector3Int.RoundToInt(tilemap.CellToWorld(new Vector3Int(x, y, 0)));
                        }
                        if ((int)c >= 48 & (int)c <= 57)
                        { 
                            colorTile.color = colors[(int)c - 48];
                            RoomTiles[(int)c-48].Add(new Vector3Int(x, y,1));
                        }
                        tilemap.SetTile(new Vector3Int(x, y, 0), colorTile);
                    }
                    else if (c == TILE_WALL | c == TILE_WALL_CORNER)
                    {
                        tiles[x, y] = tileTypes[2];
                        tilemap.SetTile(new Vector3Int(x, y, 0), tileSprites[2]);

                    }
                    else if (c == TILE_VOID)
                    {
                        tiles[x, y] = tileTypes[0];
                        tilemap.SetTile(new Vector3Int(x, y, 0), tileSprites[0]);
                    }
                }
            }

            Utility.LevelToJSON(areas);

        }

        static void AddSpawn(int maxWidth = 8, int maxHeight = 10)
        {
            //room height and width including walls
            int h = (int)Mathf.Max(rnd(maxHeight) + 5, 8);
            int w = (int)Mathf.Max(rnd(maxWidth) + 5, 8);

            //
            int ry = rndState.Next(1, H - h);
            int rx = rndState.Next(1, W - w);

            Vector3 startPoint = new Vector3(rx, ry, 1);

            Area firstArea = new Area(Orient.North, roomIndex, startPoint, w - 2, h - 2);

            //draw floor tiles
            Vector3[] tileCoords = firstArea.GetCoords();

            foreach (Vector3 coord in tileCoords)
                map[(int)coord.x, (int)coord.y] = (char)(roomIndex + 48);

            firstArea.GenerateWalls();

            //draw wall tile
            Vector3[] wallCoords = firstArea.GetWallCoords();

            foreach (Vector3 coord in wallCoords)
                map[(int)coord.x, (int)coord.y] = TILE_CORNER;

            foreach (Area.Wall wall in firstArea.Walls)
            {
                map[(int)wall.Min.x, (int)wall.Min.y] = TILE_WALL_CORNER;
                map[(int)wall.Max.x, (int)wall.Max.y] = TILE_WALL_CORNER;
            }

            int randoIndex = rndState.Next(0, tileCoords.Length);
            Vector3 spawn = tileCoords[randoIndex];

            map[(int)spawn.x, (int)spawn.y] = TILE_SPAWN;


            areas.Add(roomIndex, firstArea);

        }

        static bool AddArea(int maxWidth = 10, int maxHeight = 10)
        {


            Area foundRoom;

            foundRoom = FindArea();

            if (foundRoom == null)
            {
                failed += 1;
                return false;
            }

            foundRoom.GenerateWalls();

            bool foundPaths = FindPaths(foundRoom);

            if (!foundPaths)
            {
                failedDoors++;
                return false;
            }

            //draw found room first so that halls can overwrite walls till I can add segmentation

            Vector3[] floorCoords = foundRoom.GetCoords();

            foreach (Vector3 coord in floorCoords)
                map[(int)coord.x, (int)coord.y] = (char)(roomIndex + 48);

            Vector3[] wallCoords = foundRoom.GetWallCoords();

            foreach (Vector3 coord in wallCoords)
                if (map[(int)coord.x, (int)coord.y] != TILE_WALL_CORNER)
                    map[(int)coord.x, (int)coord.y] = TILE_CORNER;

            foreach (Area.Wall wall in foundRoom.Walls)
            {
                map[(int)wall.Min.x, (int)wall.Min.y] = TILE_WALL_CORNER;
                map[(int)wall.Max.x, (int)wall.Max.y] = TILE_WALL_CORNER;
            }

            areas.Add(roomIndex, foundRoom);

            return true;

        }

        static void SplitAllWalls()
        {
            foreach (Area area in areas.Values)
                foreach (Area.Wall wall in area.Walls)
                { 
                    wall.Split();
                }
        }

        //SHOULD BE VOID
        static bool FindPaths(Area workingArea)
        {
            //reshuffle areas so we don't necessarily build door off the one we came from.

            List<int> allIndices = Enumerable.Range(0, areas.Count).ToList();

            Utility.Shuffle(allIndices);

            List<Area> foundPaths = new List<Area>();
            Utility.Shuffle(workingArea.Walls);
            for (int nWall = 0; nWall < workingArea.Walls.Count; nWall++)
            {
                Area.Wall newWall = workingArea.Walls[nWall];

                Vector3 up = newWall.Orientation == Orient.North | newWall.Orientation == Orient.South ? Vector3.right : Vector3.up;
                Vector3 down = up == Vector3.up ? Vector3.down : Vector3.left;
                Vector3 right = up == Vector3.up ? Vector3.right : Vector3.up;

                foreach (int area in allIndices)
                {
                    Utility.Shuffle(areas[area].Walls);
                    for (int oWall = 0; oWall < areas[area].Walls.Count; oWall++)
                    {
                        Area.Wall oldWall = areas[area].Walls[oWall];
                        //make sure walls are facing eachther.
                        if (newWall.Orientation.Forward == oldWall.Orientation.Back)
                        {
                            //my left is your right if we are facing eachother. This eliminates the forward and backward dimension
                            //and compares the axis on which the walls are stagnant.
                            if (Vector3.Scale(newWall.Max , right) == Vector3.Scale(oldWall.Max , right))
                            {
                                //https://stackoverflow.com/questions/325933/determine-whether-two-date-ranges-overlap


                                float diff0 = Utility.DirectedDist(newWall.Min, oldWall.Max);
                                float diff1 = Utility.DirectedDist(newWall.Max, oldWall.Min);

                                if (diff0 <= 0 & diff1 >= 0)
                                {

                                    float diff2 = Utility.DirectedDist(newWall.Min, oldWall.Min);
                                    Vector3 startCoord = diff2 <= 0 ? oldWall.Min + up :
                                        newWall.Min + up;


                                    float diff3 = Utility.DirectedDist(newWall.Max, oldWall.Max);
                                    Vector3 endCoord = diff3 >= 0 ? oldWall.Max + down:
                                        newWall.Max + down;

                                    if (Vector3.Distance(startCoord, endCoord) < 3)
                                    {
                                        oldWall.Paths.Add(new Area.Wall.Segment(oldWall.Orientation, startCoord, endCoord, oldWall.Height, true));
                                        newWall.Paths.Add(new Area.Wall.Segment(newWall.Orientation, startCoord, endCoord, newWall.Height, true, true));
                                        break;
                                    }
                                    Vector3[] doorRange = Utility.CoordRange(startCoord, endCoord);

                                    double randoP;

                                    randoP = rndState.Next(2);

                                    int width = 2;

                                    Vector3 doorCoord = Vector3.zero;

                                    if (randoP == 1)
                                    {
                                        int[] randoIndexes = Enumerable.Range(0, doorRange.Length).ToArray();

                                        Utility.Shuffle<int>(randoIndexes);

                                        //don't spawn doors at corners
                                        foreach (int index in randoIndexes)
                                        {
                                            Vector3 checkAhead = doorRange[index] + up;
                                            if (map[(int)doorRange[index].x, (int)doorRange[index].y] != TILE_WALL_CORNER &
                                                map[(int)checkAhead.x, (int)checkAhead.y] != TILE_WALL_CORNER)
                                            {
                                                doorCoord = doorRange[index];
                                                width = 2;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        doorCoord = startCoord;
                                        width = doorRange.Length;
                                    }


                                    if (doorCoord == Vector3.zero)
                                        break;

                                    //overlap = new List<Vector3> { doorCoord, doorCoord  + (width * up)};


                                    //overWrite = new List<Vector3> { startCoord, endCoord};

                                    Debug.Log("Min: "+doorCoord);
                                    Debug.Log("Max: " + (doorCoord + (width * up)));

                                    workingArea.ConnectedAreas.Add(areas[area].RoomIndex);
                                    areas[area].ConnectedAreas.Add(workingArea.RoomIndex);

                                    oldWall.Paths.Add(new Area.Wall.Segment(oldWall.Orientation, doorCoord, doorCoord + ((width - 1)* up), oldWall.Height,true));
                                    newWall.Paths.Add(new Area.Wall.Segment(newWall.Orientation, startCoord, endCoord, newWall.Height,true, true));

                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        static Area FindArea()
        {
            List<int> allIndices = Enumerable.Range(0, areas.Count).ToList();

            Utility.Shuffle(allIndices);

            foreach (int area in allIndices)
            {
                int iter = 0;
                foreach (Area.Wall wall in areas[area].Walls.OrderBy(x => rndState.Next()))
                {

                    bool isLong;

                    //because width and height are relative to orientation.
                    if (wall.Orientation == Orient.North | wall.Orientation == Orient.South)
                    {
                        isLong = areas[area].Floor.TrueNE.y - areas[area].Floor.TrueSW.y >
                            areas[area].Floor.TrueNE.x - areas[area].Floor.TrueSW.x ? false : true;
                    }
                    else
                    {
                        isLong = areas[area].Floor.TrueNE.y - areas[area].Floor.TrueSW.y >
                            areas[area].Floor.TrueNE.x - areas[area].Floor.TrueSW.x ? true : false;
                    }

                    int w = isLong ? rndState.Next(8, 10) : rndState.Next(10, 16);
                    int h = isLong ? rndState.Next(10, 16) : rndState.Next(8, 10);


                    Vector3 roomDims = new Vector3(w - 2, h - 2, 1);

                    //pick random spot on "old wall"

                    int oldIndex = rndState.Next((int)Vector3.Distance(wall.Max, wall.Min) - 2);
                    Vector3[] oldRange = Utility.CoordRange(wall.Min + wall.Orientation.Right, wall.Max + wall.Orientation.Left);
                    Vector3 oldCoord = oldRange[oldIndex];

                    // randomly decide if start point is at the old coord or subtract the width of the floor from the old coord
                    int rando = rndState.Next(1);
                    Vector3 newCoord = rando == 1 ? oldCoord : oldCoord - (roomDims.x * wall.Orientation.Right);

                    //move it out one space from the wall and select as corner of new floor.
                    Vector3 startPoint = newCoord + wall.Orientation.Forward;

                    allRuns++;
                    roomFailures[roomIndex]++;
                    iter++;

                    //Console.WriteLine(iter + " " + area.RoomIndex + " " + wall.Orientation.Name);

                    Area testArea = new Area(wall.Orientation, roomIndex, startPoint, w - 2, h - 2);

                    //if the top right and bottom left are in bounds then the rest should be
                    if (!Utility.WithinBounds(testArea.Floor.BottomLeft + wall.Orientation.Left + wall.Orientation.Back, W, H))
                    {
                        //Console.WriteLine("Top Right: " + testArea.Floor.TopRight + " Bottom Left " + testArea.Floor.BottomLeft + " Continue");
                        continue;
                    }

                    if (!Utility.WithinBounds(testArea.Floor.TopRight + wall.Orientation.Right + wall.Orientation.Forward, W, H))
                    {
                        //Console.WriteLine("Top Right: " + testArea.Floor.TopRight + " Bottom Left " + testArea.Floor.BottomLeft + " Continue");
                        continue;
                    }
                    else
                        //Console.WriteLine("Top Right: " + testArea.Floor.TopRight + " Bottom Left " + testArea.Floor.BottomLeft);


                    notOutOfBounds++;

                    int totalTiles = 0;


                    int timeOut = 1;

                    //this is off by one row;
                    totalTiles = (w - 2) * (h - 2);

                    Vector3[] allCoords = testArea.GetCoords();

                    //make sure no floor tiles come in contact with any existing floor OR door tiles.
                    foreach (Vector3 tileCoord in allCoords)
                    {
                        if (float.IsNaN(tileCoord.x) | float.IsNaN(tileCoord.y))
                            Console.WriteLine("NaN Value");
                        //Console.WriteLine("Old Room: " + area.RoomIndex + " Room: " + roomIndex + " Orient: " + area.Orientation.Name + " Coord: " + tileCoord);
                        if (((int)map[(int)tileCoord.x, (int)tileCoord.y] >= 40 |
                            map[(int)tileCoord.x, (int)tileCoord.y] == TILE_CORNER | map[(int)tileCoord.x, (int)tileCoord.y] == TILE_WALL_CORNER))
                        {
                            break;
                        }
                        if (timeOut < totalTiles)
                            timeOut++;
                        else
                        {
                            //maybe make a function for this? add so it takes a list for things like finding doors?
                            //Console.WriteLine("Room: " + roomIndex);
                            //Console.WriteLine("Old Room: "+ area.RoomIndex +" Wall Orientation: " + wall.Orientation.Name + " Start: " + startPoint + " width: " + roomDims.x + " height: " + roomDims.y);
                            //Console.WriteLine("RangeLength: " + allCoords.Length);
                            //Console.WriteLine("TotalTiles: " + totalTiles);
                            //Console.WriteLine("NOOB: " + notOutOfBounds);
                            //Console.WriteLine("AR: " + allRuns);
                            return testArea;
                        }
                    }
                }
            }

            return null;
        }


    }

}