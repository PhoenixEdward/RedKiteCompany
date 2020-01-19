using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RedKite
{

    class PathFinder : Level
    {
        public static Node[,] graph;

        public Unit selectedHero;

        Reticle reticle;

        public List<Node> GeneratePathTo(Unit unit, int x, int y)
        {
            // Clear out our Hero's old path.

            if (HeroCanEnterTile(x, y) == false)
            {
                // We probably clicked on a mountain or something, so just quit out.
                return null;
            }

            Dictionary<Node, float> dist = new Dictionary<Node, float>();
            Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

            // Setup the "Q" -- the list of nodes we haven't checked yet.
            List<Node> unvisited = new List<Node>();

            Node source = graph[
                                (int)unit.Coordinate.x,
                                (int)unit.Coordinate.y
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

            return currentPath;

        }

        public bool IsReachable(Unit unit, Node node, Node[] range)
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
                                (int)unit.Coordinate.x,
                                (int)unit.Coordinate.y
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
                    if (Utility.ManhattanDistance(new Vector3Int((int)unit.Coordinate.x,(int)unit.Coordinate.y,2), new Vector3Int(v.cell.x, v.cell.y,2)) <= unit.movement & 
                        Utility.WithinBounds(new Vector3(unit.Coordinate.x,2,unit.Coordinate.y),TileMapper.W,TileMapper.H))
                    {
                        float alt = dist[u] + CostToEnterTile(v.cell.x, v.cell.y);
                        if (alt < dist[v] & alt < unit.movement)
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

        float CostToEnterTile(int x, int y)
        {
            Cell tt = TileMapper.tiles[x, y];

            return tt.movementCost;
        }

        bool HeroCanEnterTile(int x, int y)
        {

            // We could test the unit's walk/hover/fly type against various
            // terrain flags here to see if they are allowed to enter the tile.

            return TileMapper.tiles[x, y].IsWalkable;
        }


        public void GenerateGraph()
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
                        cell = new Vector3Int(x, y, 0)
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
    }
}