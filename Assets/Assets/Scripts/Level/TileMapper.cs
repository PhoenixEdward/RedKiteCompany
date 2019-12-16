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
            new TileType("Floor"),
            new TileType("Wall")
        };

        public GameObject selectedHero;
        List<Node> currentPath = null;

        TileType[,] tiles = new TileType[H,W];

        Node[,] graph;

        public Tile[] tileSprites = new Tile[2];
        public Tilemap tilemap;
        Grid grid;
        public Vector2 spawnPoint;
        public static int index = 0;

        List<GameSprite> units = new List<GameSprite>();


        //below are tile tracker variables
        public Vector3 destination = Vector3.zero;

        public Vector3Int highlight;

        Vector3Int temp = Vector3Int.zero;

        Sprite oldTile;

        // Start is called before the first frame update
        void Awake()
        {
            tilemap = GetComponent<Tilemap>();

            //when I start adding UI this might fuck up. Tags might be the solution.
            grid = FindObjectOfType<Grid>();

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
                        tiles[y,x] = tileTypes[0];
                        tileSprites[0].color = new Color(roomIndices[y, x], 1, 1);
                        tilemap.SetTile(new Vector3Int(y, x, 0), tileSprites[0]);
                        if (c == '@')
                            spawnPoint = tilemap.CellToWorld(new Vector3Int(y, x, 0));
                    }
                    else if (c == '#' | c == '!')
                    {
                        tiles[y, x] = tileTypes[1];
                        tilemap.SetTile(new Vector3Int(y, x, 0), tileSprites[1]);

                    }
                }
            }
        }

        private void Start()
        {

            GenerateGraph();
          
        }

        private void Update()
        {
            TileTracker();

            //selected hero needs to be cached.

            if (destination != Vector3.zero && tiles[(int)destination.x, (int)destination.y].isWalkable == true & selectedHero.GetComponent<Hero>().isMoving == false)
                GeneratePathTo((int)destination.x, (int)destination.y);

            if(currentPath != null)
            {
                int currNode = 0;

                while(currNode < currentPath.Count -1)
                {
                    Vector3 start = new Vector3(selectedHero.GetComponent<Hero>().currentPath[currNode].x, selectedHero.GetComponent<Hero>().currentPath[currNode].y);

                    //needs optimization. selected Hero needs to be cached somehow
                    Vector3 end = new Vector3(selectedHero.GetComponent<Hero>().currentPath[currNode+1].x, selectedHero.GetComponent<Hero>().currentPath[currNode + 1].y);

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
		selectedHero.GetComponent<Hero>().currentPath = null;

		if( HeroCanEnterTile(x,y) == false ) {
			// We probably clicked on a mountain or something, so just quit out.
			return;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();
		
		Node source = graph[
		                    selectedHero.GetComponent<Hero>().tileX, 
		                    selectedHero.GetComponent<Hero>().tileY
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

		selectedHero.GetComponent<Hero>().currentPath = currentPath;

        }

        void TileTracker()
        {

            Vector3 worldPoint1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            highlight = grid.WorldToCell(worldPoint1);

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
                tilemap.RefreshTile(temp);

                oldTile = tilemap.GetSprite(highlight);

                tempTile = ScriptableObject.CreateInstance<Tile>();
                tempTile.color = Color.red;
                tempTile.sprite = oldTile;
                tilemap.SetTile(highlight, tempTile);
                tilemap.RefreshTile(highlight);

                temp = highlight;
            }

            if (Input.GetMouseButtonDown(0) & selectedHero.GetComponent<Hero>().isMoving == false)
            {
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                destination = grid.WorldToCell(grid.WorldToCell(worldPoint));

                //Shows the cell reference for the grid
            }

            tilemap.RefreshAllTiles();
        }
    }

}
