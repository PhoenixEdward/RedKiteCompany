using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedKite
{
    public class BattleGrid : MonoBehaviour
    {
        Tile clearTile;
        Tile rangeTile;
        Tile selectTile;
        Tile attackTile;
        Tile assistTile;

        public Level level;

        PathFinder pathFinder = new PathFinder();


        public static Tilemap map;

        public List<Node> withinRange = new List<Node>();
        public List<Node> canMoveTo = new List<Node>();

        public bool isActive;

        // Start is called before the first frame update
        void Start()
        {
            map = GetComponent<Tilemap>();

            clearTile = ScriptableObject.CreateInstance<Tile>();

            rangeTile = ScriptableObject.CreateInstance<Tile>();
            rangeTile.sprite = Resources.Load<Sprite>("UI/Range");

            selectTile = ScriptableObject.CreateInstance<Tile>();
            selectTile.sprite = Resources.Load<Sprite>("UI/Select");

            attackTile = ScriptableObject.CreateInstance<Tile>();
            attackTile.sprite = Resources.Load<Sprite>("UI/Attack");

            assistTile = ScriptableObject.CreateInstance<Tile>();
            assistTile.sprite = Resources.Load<Sprite>("UI/Assist");

            for (int x = 0; x < map.cellBounds.xMax; x++)
            {
                for (int y = 0; y < map.cellBounds.yMax; y++)
                {
                    map.SetTile(new Vector3Int(x, y, 0), clearTile);
                }
            }
        }

        // Update is called once per frame

        public void NewUnitRange(Unit unit)
        {
            isActive = true;

            Dictionary<Vector3Int, PathFinder.TileHighlightType> range = pathFinder.GetRanges(unit);

            foreach(KeyValuePair<Vector3Int, PathFinder.TileHighlightType> entry in range)
            {
                if (entry.Value == PathFinder.TileHighlightType.Attack)
                    map.SetTile(entry.Key, attackTile);
                else if (entry.Value == PathFinder.TileHighlightType.Assist)
                    map.SetTile(entry.Key, assistTile);
                else
                {
                    map.SetTile(entry.Key, rangeTile);
                    withinRange.Add(PathFinder.graph[entry.Key.x, entry.Key.y]);
                    canMoveTo.Add(PathFinder.graph[entry.Key.x, entry.Key.y]);
                }

            }

            map.RefreshAllTiles();
        }


        public void UnitRange(Unit unit)
        {
                //draw grid of valid movement tiles
                //may need to keep an eye out for impassible moving units. could cause issues here.

                if (unit.IsMoving == false & withinRange.Count == 0 & unit.Ready)
                {

                    //formula for area of max unit range is (n+1)^2 + n^2 where n is movement speed.
                    //for now null for withinRange will be a single array with Vector3Int.Zero (default value) indicating a non walkable cell.
                    //this will either need a second array or be converted to a dictionary later to record WHY the cell isn't walkable (enemy unit, chest, wall).
                    //honestly not sure if this is the best place to store that data though.


                    int withinRangeIndex = 0;

                    //changed box from List to Array. May not really be worth it for this small of an operation but it's good practice. Nulls indicate

                    int boxRange = (unit.Movement * 2) + 1;


                    Vector2Int cell;

                    Vector2 startingSpot = new Vector2(unit.Coordinate.x - unit.Movement, unit.Coordinate.y - unit.Movement);

                    for (int i = 0; i < boxRange; i++)
                    {
                        for (int j = 0; j < boxRange; j++)
                        {
                            cell = new Vector2Int((int)startingSpot.x + i, (int)startingSpot.y + j);

                            if (Utility.ManhattanDistance(new Vector3Int((int)unit.Coordinate.x, (int)unit.Coordinate.y,2), new Vector3Int(cell.x, cell.y,2)) <= unit.Movement)
                            {

                                if (cell.x >= 0 & cell.x < TileMapper.Instance.W & cell.y >= 0 & cell.y < TileMapper.Instance.H)
                                {
                                    withinRange.Add(PathFinder.graph[cell.x, cell.y]);

                                }

                                withinRangeIndex++;

                            }
                        }
                    }

                    //remove nodes that are not walkable or obstructed. May be worth while to create a seperate variable.

                    for (int i = 0; i < withinRange.Count; i++)
                    {
                        if (withinRange[i] != null)
                        {
                            if (pathFinder.IsReachable(PathFinder.graph[unit.Coordinate.x, unit.Coordinate.y], withinRange[i], withinRange.ToArray(), unit.Movement))
                            {
                                canMoveTo.Add(withinRange[i]);
                            }
                        }
                    }

                    for (int i = 0; i < canMoveTo.Count; i++)
                    {

                        map.SetTile(canMoveTo[i].cell, rangeTile);
                    }

                    map.RefreshAllTiles();
                }

                else if (unit.IsMoving == true & withinRange != null & canMoveTo != null)
                {

                    for (int i = 0; i < canMoveTo.Count; i++)
                    {
                        if (canMoveTo[i] != null)
                        {
                            map.SetTile(canMoveTo[i].cell, clearTile);
                        }
                    }

                    withinRange = new List<Node>();
                    canMoveTo = new List<Node>();
                }

                map.RefreshAllTiles();

            //kept in case something doesnt work

        }

        //pretty sure if I I make this an array this repetition isn't necessary
        public void HighlightActionables(GameSprite[] actionables, int highlightIndex)
        {

            for (int i = 0; i < actionables.Length; i++)
            {
                if (i == highlightIndex)
                    map.SetTile(actionables[i].Coordinate, selectTile);
                else
                {
                    if (actionables[i] is Enemy)
                        map.SetTile(actionables[i].Coordinate, attackTile);
                    else if (actionables[i] is Hero)
                        map.SetTile(actionables[i].Coordinate, assistTile);
                    else
                        map.SetTile(actionables[i].Coordinate, rangeTile);
                }
            }
            map.RefreshAllTiles();
        }

        public void Clear()
        {
            map.ClearAllTiles();
            map.RefreshAllTiles();
        }

        public void DeactivateUnitRange()
        {
            withinRange = new List<Node>();
            canMoveTo = new List<Node>();

            isActive = false;

            map.ClearAllTiles();

            map.RefreshAllTiles();
        }
    }
}