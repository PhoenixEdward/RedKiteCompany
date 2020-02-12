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

        public List<Node> GeneratePathTo(Vector3 startCoord, Vector3 destCoord, int maxDistance, bool nextTo = false)
        {
            if (!HeroCanEnterTile((int)startCoord.x, (int)destCoord.y, (int)destCoord.x, (int)destCoord.y) |
                TileMapper.Instance.Tiles[(int)destCoord.x, (int)destCoord.y].TileType == Cell.Type.OccupiedAlly |
                TileMapper.Instance.Tiles[(int)destCoord.x, (int)destCoord.y].TileType == Cell.Type.OccupiedEnemy)
            {
                return null;
            }

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

            if (nextTo)
                currentPath.RemoveAt(currentPath.Count - 1);

            //the + 1 is to account for the fact that the source is the first node in the list.
            if (currentPath.Count - 1 > maxDistance + 1)
                currentPath = currentPath.Take(maxDistance).ToList();

            return currentPath;

        }

        public bool IsReachable(Node _start, Node _target, Node[] range, int maxDistance)
        {
            if (HeroCanEnterTile((int)_start.cell.x, (int)_start.cell.y, (int)_target.cell.x, (int)_target.cell.y) == false)
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
                        float alt = dist[u] + CostToEnterTile(v.cell.x, v.cell.y);

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

        public List<Node> AIGeneratePathTo(Vector3 startCoord, Vector3 destCoord, int maxDistanceFromStart, int maxDistanceFromTarget, bool nextTo = false)
        {
            List<List<Node>> potentialPaths = new List<List<Node>>();

            Node[] range = Utility.GenerateBoxRange(Vector3Int.FloorToInt(startCoord), maxDistanceFromStart);

            Node source = graph[
                                (int)startCoord.x,
                                (int)startCoord.y
                                ];

            for (int iter = 0; iter < range.Length; iter++)
            { 
                Dictionary<Node, float> dist = new Dictionary<Node, float>();
                Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

                // Setup the "Q" -- the list of nodes we haven't checked yet.
                List<Node> unvisited = new List<Node>();


                Node target = range[iter];

                dist[source] = 0;
                prev[source] = null;

                // Initialize everything to have INFINITY distance, since
                // we don't know any better right now. Also, it's possible
                // that some nodes CAN'T be reached from the source,
                // which would make INFINITY a reasonable value
                foreach (Node v in range)
                {
                    if (v != source)
                    {
                        dist[v] = Mathf.Infinity;
                        prev[v] = null;
                    }

                    unvisited.Add(v);
                }

                Dictionary<int, DistanceStruct> potentialRouteDistances = new Dictionary<int, DistanceStruct>();
                Dictionary<int, Dictionary<Node, Node>> potentialRoutes = new Dictionary<int, Dictionary<Node, Node>>();

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
                        if (Utility.ManhattanDistance(new Vector3Int(source.cell.x, source.cell.y, 2), new Vector3Int(v.cell.x, v.cell.y, 2)) <= maxDistanceFromStart &
                            Utility.WithinBounds(new Vector3(source.cell.x, 2, source.cell.y), TileMapper.Instance.W, TileMapper.Instance.H))
                        {

                            float alt = dist[u] + CostToEnterTile(v.cell.x, v.cell.y);
                            if (alt < dist[v])
                            {
                                dist[v] = alt;
                                prev[v] = u;
                            }
                        }
                    }
                }

                // If we get there, the either we found the shortest route
                // to our target, or there is no route at ALL to our target.

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


                if (currentPath.Sum(x => x.MovementCost) > maxDistanceFromStart | 
                    TileMapper.Instance.Tiles[currentPath[currentPath.Count - 1].cell.x, currentPath[currentPath.Count - 1].cell.y].TileType == Cell.Type.OccupiedEnemy)
                {
                    for (int i = maxDistanceFromStart; i > -1; i--)
                    {
                        if (!(currentPath.Take(i).Sum(x => x.MovementCost) > maxDistanceFromStart) |
                            !(TileMapper.Instance.Tiles[currentPath[currentPath.Count - 1].cell.x, currentPath[currentPath.Count - 1].cell.y].TileType == Cell.Type.OccupiedEnemy))
                            currentPath = currentPath.Take(i).ToList();
                    }
                }

                // Right now, currentPath describes a route from out target to our source
                // So we need to invert it!

                if (currentPath.Count < 2)
                    continue;

                if (nextTo & currentPath[currentPath.Count - 1].cell.x == destCoord.x & currentPath[currentPath.Count - 1].cell.y == destCoord.y)
                    currentPath.RemoveAt(currentPath.Count - 1);

                //this will need to be tested for ranged units
                if (maxDistanceFromTarget < currentPath.Count)
                    currentPath = currentPath.Take(currentPath.Count - (maxDistanceFromTarget - 1)).ToList();

                potentialPaths.Add(currentPath);
            }

            float minDistanceToTarget = potentialPaths.Min(x=> Utility.ManhattanDistance(x[x.Count-1].cell, new Vector2(destCoord.x, destCoord.y)));

            List<Node> finalPath = potentialPaths.FirstOrDefault(x=> Utility.ManhattanDistance(x[x.Count - 1].cell, new Vector2(destCoord.x, destCoord.y)) == minDistanceToTarget);

            return finalPath ?? new List<Node> { source };

        }


        struct DistanceStruct
        {
            public float DistanceFromStart;
            public float DistanceFromTarget;
            
            public DistanceStruct(float _distanceFromStart, float _distanceFromTarget)
            {
                DistanceFromStart = _distanceFromStart;
                DistanceFromTarget = _distanceFromTarget;
            }
        }

        float CostToEnterTile(int x, int y)
        {
            Cell tt = TileMapper.Instance.Tiles[x, y];

            return tt.movementCost;
        }

        float CostToEnterTile(int x, int y, List<Vector2> termedPoints)
        {
            if (termedPoints.Contains(new Vector2(x, y)))
                return 100;

            Cell tt = TileMapper.Instance.Tiles[x, y];

            return tt.movementCost;
        }

        bool HeroCanEnterTile(int x0, int y0, int x1, int y1)
        {

            // We could test the unit's walk/hover/fly type against various
            // terrain flags here to see if they are allowed to enter the tile.
            if (TileMapper.Instance.Tiles[x1, y1].IsWalkable == false |
                ((TileMapper.Instance.Tiles[x0, y0].TileType == Cell.Type.OccupiedAlly & TileMapper.Instance.Tiles[x1, y1].TileType == Cell.Type.OccupiedEnemy) |
                (TileMapper.Instance.Tiles[x0, y0].TileType == Cell.Type.OccupiedEnemy & TileMapper.Instance.Tiles[x1, y1].TileType == Cell.Type.OccupiedAlly)))
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