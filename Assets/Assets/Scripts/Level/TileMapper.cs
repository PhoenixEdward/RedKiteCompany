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

        public Hero selectedHero;
        List<Node> currentPath = null;

        TileType[,] tiles = new TileType[H,W];

        Node[,] graph;

        Camera cam;

        public Tile[] tileSprites = new Tile[2];
        public Tilemap tilemap;
        Grid grid;
        public Vector2 spawnPoint;
        public static int index = 0;

        List<Hero> units = new List<Hero>();


        //below are tile tracker variables
        public Vector3 destination = Vector3.zero;

        public Vector3Int highlight;

        Vector3Int temp = Vector3Int.zero;

        Sprite oldTile;

        Dictionary<Vector3Int, Sprite> withinRange = null;

        // Start is called before the first frame update
        void Awake()
        {
            tilemap = GetComponent<Tilemap>();

            //when I start adding UI this might fuck up. Tags might be the solution.

            tileSprites[0] = ScriptableObject.CreateInstance<Tile>();
            tileSprites[0].sprite = Resources.Load<Sprite>("Tiles/DungeonFloor");

            tileSprites[1] = ScriptableObject.CreateInstance<Tile>();
            tileSprites[1].sprite = Resources.Load<Sprite>("Tiles/DungeonWall");

            // generate
            addRoom(start: true);

            for (int j = 0; j < 5000; j++)
            {
                addRoom(start: false);
            }

            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    char c = map[y, x];
                    if (c != '\0' & c != '#' & c != '!')
                    {
                        tiles[y, x] = tileTypes[1];
                        tileSprites[0].color = new Color(roomIndices[y, x], 1, 1);
                        tilemap.SetTile(new Vector3Int(y, x, 0), tileSprites[0]);
                        if (c == '@')
                            spawnPoint = tilemap.CellToWorld(new Vector3Int(y, x, 0));
                    }
                    else if (c == '#' | c == '!')
                    {
                        tiles[y, x] = tileTypes[2];
                        tilemap.SetTile(new Vector3Int(y, x, 0), tileSprites[1]);

                    }
                    else if (c == '\0')
                        tiles[y, x] = tileTypes[0];
                }
            }

            units = GameSpriteManager.Instance.Heroes;
        }

        private void Start()
        {

            GenerateGraph();

            grid = FindObjectOfType<Grid>();

            cam = FindObjectOfType<Camera>();
        }

        private void Update()
        {
            TileTracker();
            UnitRange();
            //selected hero needs to be cached.
            if (selectedHero != null)
            {
                if (destination != Vector3.zero & tiles[(int)destination.x, (int)destination.y].isWalkable == true & selectedHero.isMoving == false)
                    GeneratePathTo((int)destination.x, (int)destination.y);
            }

            //draw debug line
            if(currentPath != null)
            {
                int currNode = 0;

                while(currNode < currentPath.Count -1)
                {
                    Vector3 start = new Vector3(selectedHero.currentPath[currNode].x, selectedHero.currentPath[currNode].y);

                    //needs optimization. selected Hero needs to be cached somehow
                    Vector3 end = new Vector3(selectedHero.currentPath[currNode+1].x, selectedHero.currentPath[currNode + 1].y);

                    Debug.DrawLine(start,end);

                    currNode++;
                }
            }
        }

        const int H = 30;
        const int W = 30;
        static char[,] map = new char[H, W];
        static int[,] roomIndices = new int[H, W];

        static System.Random rndState = new System.Random();
        static int rnd(int x) => rndState.Next() % x;

        static void addRoom(bool start)
        {
            int w = rnd(10) + 5;
            int h = rnd(6) + 3;
            int rx = rnd(W - w - 2) + 1;
            int ry = rnd(H - h - 2) + 1;

            int doorCount = 0, doorX = 0, doorY = 0;

            // generate a door
            if (!start)
            {
                // See if we can process this tile
                for (int y = ry - 1; y < ry + h + 2; y++)
                    for (int x = rx - 1; x < rx + w + 2; x++)
                        if (map[y, x] == '.')
                            return;

                // find candidate tiles for the door
                for (int y = ry - 1; y < ry + h + 2; y++)
                    for (int x = rx - 1; x < rx + w + 2; x++)
                    {
                        bool s = x < rx || x > rx + w;
                        bool t = y < ry || y > ry + h;
                        if ((s ^ t) && map[y, x] == '#')
                        {
                            ++doorCount;
                            if (rnd(doorCount) == 0)
                            {
                                doorX = x;
                                doorY = y;
                            }
                        }
                    }

                if (doorCount == 0)
                    return;
            }

            // generate a room
            for (int y = ry - 1; y < ry + h + 2; y++)
                for (int x = rx - 1; x < rx + w + 2; x++)
                {
                    bool s = x < rx || x > rx + w;
                    bool t = y < ry || y > ry + h;
                    if (s && t)
                        map[y, x] = '!'; // avoid generation of doors at corners
                    else if (s ^ t)
                        map[y, x] = '#';
                    else
                    {
                        map[y, x] = '.';
                        roomIndices[y, x] = index;
                    }
                }

            // place the door
            if (doorCount > 0)
                map[doorY, doorX] = '+';

            if (start)
            {
                map[rnd(h) + ry, rnd(w) + rx] = '@';
            } /* else {
                // place other objects
                for(int j=0; j<(rnd(6)+1); j++)
                    char thing = rnd(4)==0 ? '$' : (char)(65+rnd(62))
                    map[rnd(h)+ry, rnd(w)+rx] = thing;
            } */

            index++;


        }

        float CostToEnterTile(int x, int y)
        {
            TileType tt = tiles[x, y];

            return tt.movementCost;
        }

        public bool HeroCanEnterTile(int x, int y)
        {

            // We could test the unit's walk/hover/fly type against various
            // terrain flags here to see if they are allowed to enter the tile.


            return tiles[x,y].isWalkable;
        }

        void GenerateGraph()
        {
            // Initialize the array
            graph = new Node[W, H];

            // Initialize a Node for each spot in the array
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < W; y++)
                {
                    graph[x, y] = new Node();
                    graph[x, y].x = x;
                    graph[x, y].y = y;
                }
            }

            // Now that all the nodes exist, calculate their neighbours
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < W; y++)
                {

                    if(x > 0)
                        graph[x,y].neighbours.Add( graph[x-1, y] );
                    if(x < W-1)
                        graph[x,y].neighbours.Add( graph[x+1, y] );
                    if(y > 0)
                        graph[x,y].neighbours.Add( graph[x, y-1] );
                    if(y < H-1)
                        graph[x,y].neighbours.Add( graph[x, y+1] );
      
                }
            }
        }

        public void GeneratePathTo(int x, int y)
        {
     		// Clear out our Hero's old path.
		selectedHero.currentPath = null;

		if( HeroCanEnterTile(x,y) == false ) {
			// We probably clicked on a mountain or something, so just quit out.
			return;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();
		
		Node source = graph[
		                    selectedHero.tileX, 
		                    selectedHero.tileY
		                    ];
		
		Node target = graph[
		                    x, 
		                    y
		                    ];
		
		dist[source] = 0;
		prev[source] = null;

		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some nodes CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach(Node v in graph) {
			if(v != source) {
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}

			unvisited.Add(v);
		}

		while(unvisited.Count > 0) {
			// "u" is going to be the unvisited node with the smallest distance.
			Node u = null;

			foreach(Node possibleU in unvisited) {
				if(u == null || dist[possibleU] < dist[u]) {
					u = possibleU;
				}
			}

			if(u == target) {
				break;	// Exit the while loop!
			}

			unvisited.Remove(u);

			foreach(Node v in u.neighbours) {
				//float alt = dist[u] + u.DistanceTo(v);
				float alt = dist[u] + CostToEnterTile(v.x, v.y);
				if( alt < dist[v] ) {
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// If we get there, the either we found the shortest route
		// to our target, or there is no route at ALL to our target.

		if(prev[target] == null) {
			// No route between our target and the source
			return;
		}

		List<Node> currentPath = new List<Node>();

		Node curr = target;

		// Step through the "prev" chain and add it to our path
		while(curr != null) {
			currentPath.Add(curr);
			curr = prev[curr];
		}

		// Right now, currentPath describes a route from out target to our source
		// So we need to invert it!

		currentPath.Reverse();

		selectedHero.currentPath = currentPath;

        }

        public void TileTracker()
        {

            Vector3 worldPoint1 = cam.ScreenToWorldPoint(Input.mousePosition);

            highlight = grid.WorldToCell(worldPoint1);

            if (Input.GetMouseButtonDown(1))
                selectedHero = null;

            foreach(Hero unit in units)
            {
                if (new Vector2(unit.transform.position.x, unit.transform.position.y) == new Vector2(highlight.x, highlight.y) & Input.GetMouseButtonDown(0))
                    selectedHero = unit;

            }

            if (temp == Vector3Int.zero)
            {
                oldTile = tilemap.GetSprite(highlight);

                Tile tempTile1 = ScriptableObject.CreateInstance<Tile>();
                tempTile1.color = Color.red;
                tempTile1.sprite = oldTile;
                tilemap.SetTile(highlight, tempTile1);
                tilemap.RefreshTile(highlight);

                temp = highlight;
            }

            if (highlight != temp)
            {
                Tile tempTile = ScriptableObject.CreateInstance<Tile>();
                tempTile.sprite = oldTile;
                tilemap.SetTile(temp, tempTile);

                oldTile = tilemap.GetSprite(highlight);

                tempTile = ScriptableObject.CreateInstance<Tile>();
                tempTile.color = Color.red;
                tempTile.sprite = oldTile;
                tilemap.SetTile(highlight, tempTile);

                temp = highlight;
            }

            if (selectedHero != null)
            {
                if (Input.GetMouseButtonDown(0) & selectedHero.isMoving == false)
                {
                    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    destination = grid.WorldToCell(worldPoint);

                }
            }

            tilemap.RefreshAllTiles();
        }

        public int ManhattanDistance(Vector2Int a, Vector2Int b)
        {
            checked
            {
                return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
            }
        }



        void UnitRange()
        {
            if(selectedHero != null)
            {
                //draw grid of valid movement tiles
                //may need to keep an eye out for impassible moving units. could cause issues here.

                if (selectedHero.isMoving == false & withinRange == null)
                {
                    withinRange = new Dictionary<Vector3Int, Sprite>();
                    List<Node> box = new List<Node>();
                    int boxRange = (selectedHero.movement * 2) + 1;

                    Vector2 startingSpot = new Vector2(selectedHero.tileX - selectedHero.movement, selectedHero.tileY - selectedHero.movement);

                    for (int i = 0; i < boxRange; i++)
                    {
                        for (int j = 0; j < boxRange; j++)
                        {

                            if (startingSpot.x + i >= 0 & startingSpot.x + i < W - 1 & startingSpot.y + j >= 0 & startingSpot.y + j < H - 1)
                                if (tiles[(int)startingSpot.x + i, (int)startingSpot.y + j].isWalkable)
                                    box.Add(graph[(int)startingSpot.x + i, (int)startingSpot.y + j]);
                        }
                    }

                    Debug.Log(box.Count.ToString());


                    foreach (Node cell in box)
                    {
                        if (ManhattanDistance(new Vector2Int(selectedHero.tileX, selectedHero.tileY), new Vector2Int(cell.x, cell.y)) <= selectedHero.movement)
                            withinRange.Add(grid.WorldToCell(new Vector3Int(cell.x, cell.y, 100)), Sprite.Instantiate(tilemap.GetSprite(grid.WorldToCell(new Vector3Int(cell.x, cell.y, 100)))));

                    }

                    foreach (KeyValuePair<Vector3Int, Sprite> entry in withinRange)
                    {
                        Tile tempTile = ScriptableObject.CreateInstance<Tile>();
                        tempTile.color = Color.green;
                        tempTile.sprite = entry.Value;
                        tilemap.SetTile(entry.Key, tempTile);
                    }
                }
                else if (selectedHero.isMoving == true & withinRange != null)
                {
                    foreach (KeyValuePair<Vector3Int, Sprite> entry in withinRange)
                    {
                        Tile tempTile = ScriptableObject.CreateInstance<Tile>();
                        tempTile.sprite = entry.Value;
                        tilemap.SetTile(entry.Key, tempTile);

                        withinRange = null;
                    }
                }

                tilemap.RefreshAllTiles();

            }


            else if (selectedHero == null & withinRange != null)
            {
                foreach(KeyValuePair<Vector3Int, Sprite> entry in withinRange)
                {
                    Tile tempTile = ScriptableObject.CreateInstance<Tile>();
                    tempTile.sprite = entry.Value;
                    tilemap.SetTile(entry.Key, tempTile);
                }

                tilemap.RefreshAllTiles();

                withinRange = null;
            }

        }
    }

}
