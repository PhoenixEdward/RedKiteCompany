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

        public Level level;

        PathFinder pathFinder = new PathFinder();


        public static Tilemap map;

        public List<Node> withinRange = new List<Node>();
        public List<Node> canMoveTo = new List<Node>();

        // Start is called before the first frame update
        void Start()
        {
            map = GetComponent<Tilemap>();

            clearTile = ScriptableObject.CreateInstance<Tile>();

            rangeTile = ScriptableObject.CreateInstance<Tile>();
            rangeTile.sprite = Resources.Load<Sprite>("UI/Range");


            for (int x = 0; x < map.cellBounds.xMax; x++)
            {
                for (int y = 0; y < map.cellBounds.yMax; y++)
                {
                    map.SetTile(new Vector3Int(x, y, 0), clearTile);
                }
            }
        }

        // Update is called once per frame



        public void UnitRange(Unit unit)
        {
                //draw grid of valid movement tiles
                //may need to keep an eye out for impassible moving units. could cause issues here.

                if (unit.IsMoving == false & withinRange.Count == 0)
                {

                    Debug.Log("Rand");

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
                            if (pathFinder.IsReachable(unit, withinRange[i], withinRange.ToArray()))
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

        public void HighlightActionables(List<Enemy> actionables)
        {

            foreach (GameSprite actionable in actionables)
                map.SetTile(actionable.Coordinate, rangeTile);

            map.RefreshAllTiles();
        }
        public void HighlightActionables(List<Hero> actionables)
        {
            Debug.Log("Assist " + actionables.Count);
            foreach (GameSprite actionable in actionables)
                map.SetTile(actionable.Coordinate, rangeTile);

            map.RefreshAllTiles();
        }
        public void HighlightActionables(List<Prop> actionables)
        {
            foreach (GameSprite actionable in actionables)
                map.SetTile(actionable.Coordinate, rangeTile);

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


            map.ClearAllTiles();

            map.RefreshAllTiles();
        }
    }
}