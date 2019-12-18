using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedKite
{
    public class BattleGrid : MonoBehaviour
    {
        Tile clearTile;
        Tile highlightTile;
        Tile rangeTile;
        Tile rangeHighlightTile;
        Tile selectTile;

        public Tilemap tilemap;
        Grid grid;

        Node[,] graph;

        public Hero selectedHero;
        List<Node> currentPath = null;

        List<Hero> units = new List<Hero>();

        bool isSelection;

        public Node destination;

        public Vector3Int highlight;

        Vector3Int temp = Vector3Int.zero;

        TileMapper tileMapper;

        Node[] withinRange = null;
        Node[] canMoveTo = null;

        // Start is called before the first frame update
        void Start()
        {
            grid = FindObjectOfType<Grid>();
            tileMapper = FindObjectOfType<TileMapper>();
            tilemap = GetComponent<Tilemap>();

            clearTile = ScriptableObject.CreateInstance<Tile>();

            rangeTile = ScriptableObject.CreateInstance<Tile>();
            rangeTile.sprite = Resources.Load<Sprite>("UI/Range");

            highlightTile = ScriptableObject.CreateInstance<Tile>();
            highlightTile.sprite = Resources.Load<Sprite>("UI/Reticle");

            rangeHighlightTile = ScriptableObject.CreateInstance<Tile>();
            rangeHighlightTile.sprite = Resources.Load<Sprite>("UI/RangeReticle");

            selectTile = ScriptableObject.CreateInstance<Tile>();
            selectTile.sprite = Resources.Load<Sprite>("UI/Select");

            tilemap.size = new Vector3Int(tileMapper.tilemap.size.x,tileMapper.tilemap.size.y,2);

            for (int x = 0; x < tilemap.cellBounds.xMax; x++)
            {
                for (int y = 0; y < tilemap.cellBounds.yMax; y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), clearTile);
                }
            }

            units = GameSpriteManager.Instance.Heroes;

            GenerateGraph();
        }

        // Update is called once per frame
        void Update()
        {

            //unit range must proceed tiletracker so as not to clear on deselect.
            UnitRange();
            TileTracker();
            if (selectedHero != null)
            {
                if (destination != null)
                {
                    if (tileMapper.tiles[destination.cell.x, destination.cell.y].isWalkable == true & selectedHero.isMoving == false)
                        if (ManhattanDistance(new Vector2Int(selectedHero.tileX, selectedHero.tileY), new Vector2Int(destination.cell.x, destination.cell.y)) <= selectedHero.movement)
                        {
                            if (IsReachable(destination, withinRange))
                                GeneratePathTo((int)destination.cell.x, (int)destination.cell.y);
                        }
                }
            }
            //draw debug line
            if (currentPath != null)
            {
                int currNode = 0;

                while (currNode < currentPath.Count - 1)
                {
                    Vector3 start = new Vector3(selectedHero.currentPath[currNode].cell.x, selectedHero.currentPath[currNode].cell.y);

                    //needs optimization. selected Hero needs to be cached somehow
                    Vector3 end = new Vector3(selectedHero.currentPath[currNode + 1].cell.x, selectedHero.currentPath[currNode + 1].cell.y);

                    Debug.DrawLine(start, end);

                    currNode++;
                }
            }
        }

        float CostToEnterTile(int x, int y)
        {
            TileType tt = tileMapper.tiles[x, y];

            return tt.movementCost;
        }

        public bool HeroCanEnterTile(int x, int y)
        {

            // We could test the unit's walk/hover/fly type against various
            // terrain flags here to see if they are allowed to enter the tile.


            return tileMapper.tiles[x, y].isWalkable;
        }


        void GenerateGraph()
        {
            // Initialize the array
            graph = new Node[TileMapper.W, TileMapper.H];

            // Initialize a Node for each spot in the array
            for (int x = 0; x < TileMapper.W; x++)
            {
                for (int y = 0; y < TileMapper.H; y++)
                {
                    graph[x, y] = new Node
                    {
                        cell = grid.WorldToCell(new Vector3(x, y, 100))
                    };
                }
            }

            // Now that all the nodes exist, calculate their neighbours
            for (int x = 0; x < TileMapper.W; x++)
            {
                for (int y = 0; y < TileMapper.H; y++)
                {

                    if (x > 0)
                        graph[x, y].neighbours.Add(graph[x - 1, y]);
                    if (x < TileMapper.W - 1)
                        graph[x, y].neighbours.Add(graph[x + 1, y]);
                    if (y > 0)
                        graph[x, y].neighbours.Add(graph[x, y - 1]);
                    if (y < TileMapper.H - 1)
                        graph[x, y].neighbours.Add(graph[x, y + 1]);

                }
            }
        }

        public void GeneratePathTo(int x, int y)
        {
            // Clear out our Hero's old path.
            selectedHero.currentPath = null;

            if (HeroCanEnterTile(x, y) == false)
            {
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
            foreach (Node v in graph)
            {
                if (v != source)
                {
                    dist[v] = Mathf.Infinity;
                    prev[v] = null;
                }

                unvisited.Add(v);
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
                    float alt = dist[u] + CostToEnterTile(v.cell.x, v.cell.y);
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }

            // If we get there, the either we found the shortest route
            // to our target, or there is no route at ALL to our target.

            if (prev[target] == null)
            {
                // No route between our target and the source
                return;
            }

            List<Node> currentPath = new List<Node>();

            Node curr = target;

            // Step through the "prev" chain and add it to our path
            while (curr != null)
            {
                currentPath.Add(curr);
                curr = prev[curr];
            }

            // Right now, currentPath describes a route from out target to our source
            // So we need to invert it!

            currentPath.Reverse();

            selectedHero.currentPath = currentPath;

        }

        public bool IsReachable(Node node, Node[] range)
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

            if (node == source)
            {
                return true;
            }

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
                if (v != null)
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
                            Debug.Log(v.cell.ToString());
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

            Vector3 worldPoint1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            highlight = grid.WorldToCell(worldPoint1);
            highlight.z = 1;
            tilemap.SetTile(highlight, highlightTile);

            if (Input.GetMouseButtonDown(1))
            {
                selectedHero = null;
                destination = null;
            }

            foreach (Hero unit in units)
            {
                if (new Vector2(unit.transform.position.x, unit.transform.position.y) == new Vector2(highlight.x, highlight.y) & selectedHero == null)
                {
                    if (Input.GetMouseButtonDown(0))
                    { 
                        selectedHero = unit;
                        isSelection = true;
                    }

                    tilemap.SetTile(highlight, selectTile);
                }

            }

            if (temp == Vector3Int.zero)
            {
                temp = highlight;
            }

            if (highlight != temp)
            {

                tilemap.SetTile(temp, clearTile);

                temp = highlight;
            }


            if (selectedHero != null)
            {
                if (!isSelection)
                {
                    if(selectedHero.isMoving == false)
                    { 
                        if (Input.GetMouseButtonDown(0))
                        {
                            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            Vector3Int destCoords = grid.WorldToCell(worldPoint);
                            destCoords.z = 0;
                            destination = graph[destCoords.x, destCoords.y];

                        }

                        foreach (Node node in canMoveTo)
                        {
                            if(node != null)
                            {
                                if (highlight.x == node.cell.x & highlight.y == node.cell.y)
                                {
                                    tilemap.SetTile(highlight, rangeHighlightTile);

                                    break;
                                }
                            }
                        }
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
            if (selectedHero != null)
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
                    withinRange = new Node[(int)Mathf.Pow(selectedHero.movement + 1, 2) + (int)Mathf.Pow(selectedHero.movement, 2)];

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

                                if (cell.x >= 0 & cell.x < TileMapper.W & cell.y >= 0 & cell.y < TileMapper.H )
                                {

                                    withinRange[withinRangeIndex] = graph[cell.x, cell.y];

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
                            }
                            else
                            {
                                canMoveTo[i] = withinRange[i];
                            }
                        }
                    }

                    for (int i = 0; i < canMoveTo.Length; i++)
                    {
                        if (canMoveTo[i] != null)
                        {
                            tilemap.SetTile(canMoveTo[i].cell, rangeTile);
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
                            tilemap.SetTile(canMoveTo[i].cell, clearTile);
                        }
                    }

                    withinRange = null;
                    canMoveTo = null;
                }

                tilemap.RefreshAllTiles();

            }


            else if (selectedHero == null & withinRange != null & canMoveTo != null)
            {

                tilemap.ClearAllTiles();

                tilemap.RefreshAllTiles();

                withinRange = null;
                canMoveTo = null;
            }

        }
    }
}