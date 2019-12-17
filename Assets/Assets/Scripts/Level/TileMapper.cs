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

        Tile[] tileSprites = new Tile[3];
        public Tilemap tilemap;
        Grid grid;
        public Vector2 spawnPoint;
        public static int index = 0;

        List<Hero> units = new List<Hero>();

        bool isSelection;
        //below are tile tracker variables
        //Should destination be attached to the hero?

        public Node destination;

        public Vector3Int highlight;

        Vector3Int temp = Vector3Int.zero;

        Sprite oldTileSprite;
        Color oldTileColor;

        Node[] withinRange = null;
        Node[] canMoveTo = null;
        Sprite[] withinRangeSprites = null;

        void Awake()
        {
            tilemap = GetComponent<Tilemap>();

            //when I start adding UI this might fuck up. Tags might be the solution.

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

            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    char c = map[y, x];
                    if (c != '\0' & c != '#' & c != '!')
                    {
                        tiles[y, x] = tileTypes[1];
                        Tile colorTile = ScriptableObject.CreateInstance<Tile>();
                        colorTile.color = new Color(roomIndices[y, x], 1, 1);
                        colorTile.sprite = tileSprites[1].sprite;
                        tilemap.SetTile(new Vector3Int(y, x, 0), tileSprites[1]);
                        if (c == '@')
                            spawnPoint = tilemap.CellToWorld(new Vector3Int(y, x, 0));
                    }
                    else if (c == '#' | c == '!')
                    {
                        tiles[y, x] = tileTypes[2];
                        tilemap.SetTile(new Vector3Int(y, x, 0), tileSprites[2]);

                    }
                    else if (c == '\0')
                    { 
                        tiles[y, x] = tileTypes[0];
                        tilemap.SetTile(new Vector3Int(y ,x, 0),tileSprites[0]);
                    }
                }
            }

            units = GameSpriteManager.Instance.Heroes;
        }

        private void Start()
        {

            grid = FindObjectOfType<Grid>();

            cam = FindObjectOfType<Camera>();

            GenerateGraph();

        }

        private void Update()
        {
            TileTracker();
            UnitRange();
            //selected hero needs to be cached.
            if (selectedHero != null)
            {
                if (destination != null)
                { 
                    if(tiles[destination.cell.x, destination.cell.y].isWalkable == true & selectedHero.isMoving == false)
                        if(ManhattanDistance(new Vector2Int(selectedHero.tileX,selectedHero.tileY),new Vector2Int(destination.cell.x,destination.cell.y)) <= selectedHero.movement)
                        {
                            Debug.Log("SHIT");
                            if(IsReachable(destination, withinRange))
                                GeneratePathTo((int)destination.cell.x, (int)destination.cell.y);
                        }
                }
            }

            //draw debug line
            if(currentPath != null)
            {
                int currNode = 0;

                while(currNode < currentPath.Count -1)
                {
                    Vector3 start = new Vector3(selectedHero.currentPath[currNode].cell.x, selectedHero.currentPath[currNode].cell.y);

                    //needs optimization. selected Hero needs to be cached somehow
                    Vector3 end = new Vector3(selectedHero.currentPath[currNode+1].cell.x, selectedHero.currentPath[currNode + 1].cell.y);

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

        void GenerateRangeGraph()
        {
            // Initialize the array
            graph = new Node[W, H];

            // Initialize a Node for each spot in the array
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    graph[x, y] = new Node();
                    graph[x, y].cell.x = x;
                    graph[x, y].cell.y = y;
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
                    graph[x, y].cell = grid.WorldToCell(new Vector3(x, y, 100));
                }
            }

            // Now that all the nodes exist, calculate their neighbours
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < W; y++)
                {

                    if (x > 0)
                        graph[x, y].neighbours.Add(graph[x - 1, y]);
                    if (x < W - 1)
                        graph[x, y].neighbours.Add(graph[x + 1, y]);
                    if (y > 0)
                        graph[x, y].neighbours.Add(graph[x, y - 1]);
                    if (y < H - 1)
                        graph[x, y].neighbours.Add(graph[x, y + 1]);

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
				float alt = dist[u] + CostToEnterTile(v.cell.x, v.cell.y);
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

        public bool IsReachable (Node node, Node[] range)
        {
            if (HeroCanEnterTile((int)node.cell.x, (int)node.cell.y) == false)
            {
                // We probably clicked on a mountain or something, so just quit out.
                return false;
            }

            Dictionary<Node, float> dist = new Dictionary<Node, float>();
            Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

            // Setup the "Q" -- the list of nodes we haven't checked yet.
            List<Node> unvisited = new List<Node>();

            Node source = graph[
                                selectedHero.tileX,
                                selectedHero.tileY
                                ];

            Node target = node;

            dist[source] = 0;
            prev[source] = null;

            // Initialize everything to have INFINITY distance, since
            // we don't know any better right now. Also, it's possible
            // that some nodes CAN'T be reached from the source,
            // which would make INFINITY a reasonable value
            // apply this only to nodes in the withinRange array;

            foreach (Node v in range)
            {
                if(v != null)
                { 
                    if (v != source)
                    {
                        dist[v] = Mathf.Infinity;
                        prev[v] = null;
                    }

                    unvisited.Add(v);
                }
            }

            while (unvisited.Count > 0)
            {
                // "u" is going to be the unvisited node with the smallest distance.
                Node u = null;

                foreach (Node possibleU in unvisited)
                {
                    if (u == null || dist[possibleU] < dist[u])
                    {
                        u = possibleU;
                    }
                }

                if (u == target)
                {
                    break;  // Exit the while loop!
                }

                unvisited.Remove(u);

                foreach (Node v in u.neighbours)
                {
                    if (ManhattanDistance(new Vector2Int(selectedHero.tileX, selectedHero.tileY), new Vector2Int(v.cell.x, v.cell.y)) <= selectedHero.movement)
                    { 
                        float alt = dist[u] + CostToEnterTile(v.cell.x, v.cell.y);
                        if (alt < dist[v] & alt < selectedHero.movement)
                        {
                            dist[v] = alt;
                            prev[v] = u;
                        }
                    }
                }
            }

            // If we get there, the either we found the shortest route
            // to our target, or there is no route at ALL to our target.

            if (prev[target] == null)
            {
                // No route between our target and the source
                return false;
            }
            else
            {
                return true;
            }

        }

        public void TileTracker()
        {

            Vector3 worldPoint1 = cam.ScreenToWorldPoint(Input.mousePosition);

            highlight = grid.WorldToCell(worldPoint1);

            if (Input.GetMouseButtonDown(1))
            {
                selectedHero = null;
                destination = null;
            }

            foreach(Hero unit in units)
            {
                if (new Vector2(unit.transform.position.x, unit.transform.position.y) == new Vector2(highlight.x, highlight.y) & Input.GetMouseButtonDown(0))
                { 
                    selectedHero = unit;
                    isSelection = true;
                }

            }

            if (temp == Vector3Int.zero)
            {
                oldTileSprite = tilemap.GetSprite(highlight);
                oldTileColor = tilemap.GetColor(highlight);

                Tile tempTile1 = ScriptableObject.CreateInstance<Tile>();
                tempTile1.color = Color.red;
                tempTile1.sprite = oldTileSprite;
                tilemap.SetTile(highlight, tempTile1);
                tilemap.RefreshTile(highlight);

                temp = highlight;
            }

            if (highlight != temp)
            {
                Tile tempTile = ScriptableObject.CreateInstance<Tile>();
                tempTile.color = oldTileColor;
                tempTile.sprite = oldTileSprite;
                tilemap.SetTile(temp, tempTile);

                oldTileSprite = tilemap.GetSprite(highlight);
                oldTileColor = tilemap.GetColor(highlight);

                tempTile = ScriptableObject.CreateInstance<Tile>();
                tempTile.color = Color.red;
                tempTile.sprite = oldTileSprite;
                tilemap.SetTile(highlight, tempTile);

                temp = highlight;
            }

            if (selectedHero != null)
            {
                if(!isSelection)
                { 
                    if (Input.GetMouseButtonDown(0) & selectedHero.isMoving == false)
                    {
                        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector3Int destCoords = grid.WorldToCell(worldPoint);
                        destination = graph[destCoords.x, destCoords.y];

                    }
                }
            }

            tilemap.RefreshAllTiles();

            isSelection = false;
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
                    //formula for area of max unit range is (n+1)^2 + n^2 where n is movement speed.
                    //for now null for withinRange will be a single array with Vector3Int.Zero (default value) indicating a non walkable cell.
                    //this will either need a second array or be converted to a dictionary later to record WHY the cell isn't walkable (enemy unit, chest, wall).
                    //honestly not sure if this is the best place to store that data though.

                    canMoveTo = new Node[(int)Mathf.Pow(selectedHero.movement + 1, 2) + (int)Mathf.Pow(selectedHero.movement, 2)];
                    withinRange = new Node[(int)Mathf.Pow(selectedHero.movement+1,2) + (int)Mathf.Pow(selectedHero.movement,2)];
                    withinRangeSprites = new Sprite[(int)Mathf.Pow(selectedHero.movement + 1, 2) + (int)Mathf.Pow(selectedHero.movement, 2)];

                    int withinRangeIndex = 0;

                    //changed box from List to Array. May not really be worth it for this small of an operation but it's good practice. Nulls indicate

                    int boxRange = (selectedHero.movement * 2) + 1;


                    Vector2Int cell;

                    Vector2 startingSpot = new Vector2(selectedHero.tileX - selectedHero.movement, selectedHero.tileY - selectedHero.movement);

                    for (int i = 0; i < boxRange; i++)
                    {
                        for (int j = 0; j < boxRange; j++)
                        {
                            cell = new Vector2Int((int)startingSpot.x + i, (int)startingSpot.y + j);

                            if (ManhattanDistance(new Vector2Int(selectedHero.tileX, selectedHero.tileY), new Vector2Int(cell.x, cell.y)) <= selectedHero.movement)
                            {

                                if (cell.x >= 0 & cell.x < W & cell.y >= 0 & cell.y < H)
                                {

                                    withinRange[withinRangeIndex] = graph[cell.x, cell.y];

                                    withinRangeSprites[withinRangeIndex] = Sprite.Instantiate(tilemap.GetSprite(grid.WorldToCell(new Vector3Int(cell.x, cell.y, 100))));

                                }

                                withinRangeIndex++;

                            }
                        }
                    }

                    //remove nodes that are not walkable or obstructed. May be worth while to create a seperate variable.

                    for (int i = 0; i < withinRange.Length; i++)
                    {
                        if (withinRange[i] != null)
                        {
                            if (!IsReachable(withinRange[i], withinRange))
                            {
                                canMoveTo[i] = null;
                                Debug.Log("Worked!");
                            }
                            else
                            {
                                canMoveTo[i] = withinRange[i];
                            }
                        }
                    }

                    for (int i = 0; i < canMoveTo.Length; i++)
                    {
                        if(canMoveTo[i] != null)
                        { 
                            Tile tempTile = ScriptableObject.CreateInstance<Tile>();
                            tempTile.color = Color.green;
                            tempTile.sprite = withinRangeSprites[i];
                            tilemap.SetTile(canMoveTo[i].cell, tempTile);
                        }
                    }

                    tilemap.RefreshAllTiles();
                }

                else if (selectedHero.isMoving == true & withinRange != null & canMoveTo != null)
                {

                    for (int i = 0; i < canMoveTo.Length; i++)
                    {
                        if (canMoveTo[i] != null)
                        {
                            Tile tempTile = ScriptableObject.CreateInstance<Tile>();
                            tempTile.sprite = withinRangeSprites[i];
                            tilemap.SetTile(canMoveTo[i].cell, tempTile);
                        }
                    }

                    withinRange = null;
                    canMoveTo = null;
                }

                tilemap.RefreshAllTiles();

            }


            else if (selectedHero == null & withinRange != null & canMoveTo != null)
            {

                for (int i = 0; i < withinRange.Length; i++)
                {
                    if (canMoveTo[i] != null)
                    {
                        Tile tempTile = ScriptableObject.CreateInstance<Tile>();
                        tempTile.sprite = withinRangeSprites[i];
                        tilemap.SetTile(canMoveTo[i].cell, tempTile);
                    }
                }

                tilemap.RefreshAllTiles();

                withinRange = null;
                canMoveTo = null;
            }

        }
    }

}
