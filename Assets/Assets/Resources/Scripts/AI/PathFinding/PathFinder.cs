using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RedKite
{

    public class PathFinder
    {
        public static Node[,] graph;

        public Unit selectedHero;

        Reticle reticle;

        public List<Node> GeneratePathTo(Vector3 startCoord, Vector3 destCoord, int maxDistance, bool isHero = true, bool isEnemy = false)
        { 
            Dictionary<Node, float> dist = new Dictionary<Node, float>();
            Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

            // Setup the "Q" -- the list of nodes we haven't checked yet.
            List<Node> unvisited = new List<Node>();

            Node source = graph[
                                (int)startCoord.x,
                                (int)startCoord.y
                                ];

            Node target = graph[
                                (int)destCoord.x,
                                (int)destCoord.y
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
                    float alt;

                    if (v != target)
                        alt = dist[u] + CostToEnterTile(v.cell.x, v.cell.y, isHero: isHero, isEnemy: isEnemy);
                    else
                        alt = dist[u] + CostToEnterTile(v.cell.x, v.cell.y, isHero: isHero, isEnemy: isEnemy, true);

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
                return null;
            }

            List<Node> currentPath = new List<Node>();

            Node curr = target;

            currentPath.Add(curr);

            // Step through the "prev" chain and add it to our path
            while (curr != null)
            {
                currentPath.Add(curr);
                curr = prev[curr];
            }

            // Right now, currentPath describes a route from out target to our source
            // So we need to invert it!

            currentPath.Reverse();

            //the + 1 is to account for the fact that the source is the first node in the list.
            if (dist[target] > maxDistance + 1)
                return null;

            return currentPath;

        }

        public bool IsReachable(Node _start, Node _target, Node[] range, int maxDistance, bool isHero = true, bool isEnemy = false, bool isFinal = false)
        {
            if (HeroCanEnterTile((int)_target.cell.x, (int)_target.cell.y, isHero: isHero, isEnemy: isEnemy) == false)
            {
                return false;
            }

            Dictionary<Node, float> dist = new Dictionary<Node, float>();
            Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

            // Setup the "Q" -- the list of nodes we haven't checked yet.
            List<Node> unvisited = new List<Node>();

            Node source = _start;

            if (_target == source)
            {
                return true;
            }

            Node target = _target;

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
                    if (Utility.ManhattanDistance(new Vector3Int(source.cell.x, source.cell.y, 2), new Vector3Int(v.cell.x, v.cell.y, 2)) <= maxDistance &
                        Utility.WithinBounds(new Vector3(source.cell.x, 2, source.cell.y), TileMapper.Instance.W, TileMapper.Instance.H))
                    {
                        float alt = dist[u] + CostToEnterTile(v.cell.x, v.cell.y, isHero: isHero, isEnemy: isEnemy, isFinal: isFinal);

                        if (alt < dist[v] & alt <= maxDistance)
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

        public enum TileHighlightType
        {
            Move,
            Attack,
            Assist
        }

        public Dictionary<Vector3Int, TileHighlightType> GetRanges(GameSprite unit)
        {
            Dictionary<Vector3Int, TileHighlightType> allPoints = new Dictionary<Vector3Int, TileHighlightType>();

            int maxDistance = unit.Movement + Mathf.Max(unit.MaxAttackRange, unit.MaxAssistRange);
            int maxAttackRange = unit.Movement + unit.MaxAttackRange;
            int maxAssistRange = unit.Movement + unit.MaxAssistRange;

            Node[] range = Utility.GenerateBoxRange(unit.Coordinate, maxDistance);

            Node source = graph[
                    unit.Coordinate.x,
                    unit.Coordinate.y
                    ];


            Dictionary<Node, float> dist = new Dictionary<Node, float>();
            Dictionary<Node, float> rangeDist = new Dictionary<Node, float>();
            Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

            // Setup the "Q" -- the list of nodes we haven't checked yet.
            List<Node> unvisited = new List<Node>();


            dist[source] = 0;
            rangeDist[source] = 0;
            prev[source] = null;

            foreach (Node v in range)
            {
                if (v != source)
                {
                    dist[v] = Mathf.Infinity;
                    rangeDist[v] = Mathf.Infinity;
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

                unvisited.Remove(u);

                foreach (Node v in u.neighbours)
                {
                    if (Utility.ManhattanDistance(new Vector3Int(source.cell.x, source.cell.y, 2), new Vector3Int(v.cell.x, v.cell.y, 2)) <= maxDistance)
                       {
                        float alt = dist[u] + CostToEnterTile(v.cell.x, v.cell.y, true);
                        float rangeAlt = dist[u] + RangeCostToEnterTile(v.cell.x, v.cell.y);

                        if (alt < dist[v])
                        {
                            dist[v] = alt;
                            rangeDist[v] = rangeAlt;
                            prev[v] = u;
                        }
                    }
                }
            }

            for (int i = 0; i < range.Length; i++)
            {
                float totalMovementCost = dist[range[i]];
                float totalRangeCost = rangeDist[range[i]];

                if (totalRangeCost > unit.Movement & totalRangeCost <= maxDistance)
                {
                    if (totalRangeCost <= maxAttackRange)
                        allPoints.Add(range[i].cell, TileHighlightType.Attack);
                    else if (totalRangeCost <= maxAssistRange)
                        allPoints.Add(range[i].cell, TileHighlightType.Assist);
                }
                else if (totalMovementCost <= unit.Movement)
                {
                    allPoints.Add(range[i].cell, TileHighlightType.Move);
                }
                else if ((TileMapper.Instance.Tiles[range[i].cell.x, range[i].cell.y].TileType == Cell.Type.OccupiedEnemy) &
                    totalRangeCost <= maxAttackRange)
                {
                    allPoints.Add(range[i].cell, TileHighlightType.Attack);
                }
                else if ((TileMapper.Instance.Tiles[range[i].cell.x, range[i].cell.y].TileType == Cell.Type.PassableProp) &
                    totalRangeCost <= unit.Movement)
                {
                    allPoints.Add(range[i].cell, TileHighlightType.Move);
                }

            }

            Debug.Log("AllPoints: " + allPoints.Count);

            return allPoints;

        }


        public List<Node> AIGeneratePathTo(Vector3 startCoord, Vector3 destCoord, int maxDistanceFromStart, int maxDistanceFromTarget)
        {
            List<List<Node>> potentialPaths = new List<List<Node>>();

            Node[] range = Utility.GenerateBoxRange(Vector3Int.FloorToInt(startCoord), maxDistanceFromStart);

            Node source = graph[
                                (int)startCoord.x,
                                (int)startCoord.y
                                ];

            Debug.Log("Dest Coord: " + destCoord);

            for (int iter = 0; iter < range.Length; iter++)
            {
                Node target = range[iter];

                if (TileMapper.Instance.Tiles[target.cell.x, target.cell.y].TileType == Cell.Type.OccupiedEnemy | !HeroCanEnterTile((int)target.cell.x, (int)target.cell.y, false, true)) 
                {
                    continue;
                }

                Dictionary<Node, float> dist = new Dictionary<Node, float>();
                Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

                // Setup the "Q" -- the list of nodes we haven't checked yet.
                List<Node> unvisited = new List<Node>();


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
                        if (Utility.ManhattanDistance(new Vector3Int(source.cell.x, source.cell.y, 2), new Vector3Int(v.cell.x, v.cell.y, 2)) <= maxDistanceFromStart)
                        {
                            float alt = dist[u] + CostToEnterTile(v.cell.x, v.cell.y, isEnemy: true);

                            if (alt < dist[v] & alt <= maxDistanceFromStart)
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
                    continue;
                }


                List<Node> currentPath = new List<Node>();

                Node curr = target;

                currentPath.Add(curr);

                // Step through the "prev" chain and add it to our path
                while (curr != null)
                {
                    currentPath.Add(curr);
                    curr = prev[curr];

                }

                currentPath.Reverse();

                if (dist[currentPath[currentPath.Count - 1]] > maxDistanceFromStart)
                    continue;

                // Right now, currentPath describes a route from out target to our source
                // So we need to invert it!
                if (currentPath.Count < 2)
                    continue;

                potentialPaths.Add(currentPath);
            }

            Debug.Log("Path count: " + potentialPaths.Count);

            int curMin = 1000;

            foreach(List<Node> x in potentialPaths)
            {
                int curDist = Utility.ManhattanDistance(x[x.Count - 1].cell, graph[(int)destCoord.x, (int)destCoord.y].cell);
                curMin = curDist < curMin ? curDist : curMin;
            }


            //issue with it defaulting to attempting to walk through a wall as shortest path. Need to account for future movement cost. Maybe redo box range to incorporate complete distance to target.
            List<Node> finalPath = potentialPaths.FirstOrDefault(x => Utility.ManhattanDistance(x[x.Count - 1].cell, graph[(int)destCoord.x, (int)destCoord.y].cell) == curMin);

            return finalPath ?? new List<Node> { source };

        }

        float CostToEnterTile(int x, int y, bool isHero = false, bool isEnemy = false, bool isFinal = false)
        {

            int tt = (int)TileMapper.Instance.Tiles[x, y].movementCost;

            if (isHero == true & TileMapper.Instance.Tiles[x, y].TileType == Cell.Type.OccupiedEnemy | isHero == true & TileMapper.Instance.Tiles[x, y].TileType == Cell.Type.PassableProp)
                tt = 100;

            if (isEnemy == true & TileMapper.Instance.Tiles[x, y].TileType == Cell.Type.OccupiedAlly | isEnemy == true & TileMapper.Instance.Tiles[x, y].TileType == Cell.Type.PassableProp)
                tt = 100;

            if (isFinal & (TileMapper.Instance.Tiles[x, y].TileType == Cell.Type.OccupiedAlly | TileMapper.Instance.Tiles[x, y].TileType == Cell.Type.OccupiedEnemy | TileMapper.Instance.Tiles[x, y].TileType == Cell.Type.PassableProp))
                tt = 100;

            return tt;
        }

        float RangeCostToEnterTile(int x, int y, bool isHero = false, bool isEnemy = false, bool isFinal = false)
        {

            int tt = (int)TileMapper.Instance.Tiles[x, y].rangeCost;

            return tt;
        }


        bool HeroCanEnterTile(int x1, int y1, bool isHero, bool isEnemy)
        {

            // We could test the unit's walk/hover/fly type against various
            // terrain flags here to see if they are allowed to enter the tile.
            if (TileMapper.Instance.Tiles[x1, y1].IsWalkable == false |
                ((isHero & TileMapper.Instance.Tiles[x1, y1].TileType == Cell.Type.OccupiedEnemy) |
                (isEnemy & TileMapper.Instance.Tiles[x1, y1].TileType == Cell.Type.OccupiedAlly)))
                return false;
            else
                return true;


        }


        public void GenerateGraph()
        {
            // Initialize the array
            graph = new Node[TileMapper.Instance.W, TileMapper.Instance.H];

            // Initialize a Node for each spot in the array
            for (int x = 0; x < TileMapper.Instance.W; x++)
            {
                for (int y = 0; y < TileMapper.Instance.H; y++)
                {
                    graph[x, y] = new Node
                    {
                        cell = new Vector3Int(x, y, -1)
                    };
                }
            }

            // Now that all the nodes exist, calculate their neighbours
            for (int x = 0; x < TileMapper.Instance.W; x++)
            {
                for (int y = 0; y < TileMapper.Instance.H; y++)
                {

                    if (x > 0)
                        graph[x, y].neighbours.Add(graph[x - 1, y]);
                    if (x < TileMapper.Instance.W - 1)
                        graph[x, y].neighbours.Add(graph[x + 1, y]);
                    if (y > 0)
                        graph[x, y].neighbours.Add(graph[x, y - 1]);
                    if (y < TileMapper.Instance.H - 1)
                        graph[x, y].neighbours.Add(graph[x, y + 1]);

                }
            }
        }
    }
}