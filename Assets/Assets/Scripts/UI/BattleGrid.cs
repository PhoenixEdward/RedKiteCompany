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

        public Tilemap tilemap;

        PathFinder pathFinder = new PathFinder();


        public static TileMapper tileMapper;

        public static Node[] withinRange = null;
        public static Node[] canMoveTo = null;

        Reticle reticle;


        // Start is called before the first frame update
        void Start()
        {
            tileMapper = FindObjectOfType<TileMapper>();
            tilemap = GetComponent<Tilemap>();

            clearTile = ScriptableObject.CreateInstance<Tile>();

            rangeTile = ScriptableObject.CreateInstance<Tile>();
            rangeTile.sprite = Resources.Load<Sprite>("UI/Range");


            for (int x = 0; x < tilemap.cellBounds.xMax; x++)
            {
                for (int y = 0; y < tilemap.cellBounds.yMax; y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), clearTile);
                }
            }

            reticle = FindObjectOfType<Reticle>();
        }

        // Update is called once per frame
        void Update()
        {
            //unit range must proceed tiletracker so as not to clear on deselect.
            UnitRange();

        }



        void UnitRange()
        {
            if (reticle.selectedHero != null)
            {
                //draw grid of valid movement tiles
                //may need to keep an eye out for impassible moving units. could cause issues here.

                if (reticle.selectedHero.IsMoving == false & withinRange == null)
                {

                    //formula for area of max unit range is (n+1)^2 + n^2 where n is movement speed.
                    //for now null for withinRange will be a single array with Vector3Int.Zero (default value) indicating a non walkable cell.
                    //this will either need a second array or be converted to a dictionary later to record WHY the cell isn't walkable (enemy unit, chest, wall).
                    //honestly not sure if this is the best place to store that data though.

                    canMoveTo = new Node[(int)Mathf.Pow(reticle.selectedHero.movement + 1, 2) + (int)Mathf.Pow(reticle.selectedHero.movement, 2)];
                    withinRange = new Node[(int)Mathf.Pow(reticle.selectedHero.movement + 1, 2) + (int)Mathf.Pow(reticle.selectedHero.movement, 2)];


                    int withinRangeIndex = 0;

                    //changed box from List to Array. May not really be worth it for this small of an operation but it's good practice. Nulls indicate

                    int boxRange = (reticle.selectedHero.movement * 2) + 1;


                    Vector2Int cell;

                    Vector2 startingSpot = new Vector2(reticle.selectedHero.Coordinate.x - reticle.selectedHero.movement, reticle.selectedHero.Coordinate.y - reticle.selectedHero.movement);

                    for (int i = 0; i < boxRange; i++)
                    {
                        for (int j = 0; j < boxRange; j++)
                        {
                            cell = new Vector2Int((int)startingSpot.x + i, (int)startingSpot.y + j);

                            if (Utility.ManhattanDistance(new Vector3Int(reticle.selectedHero.Coordinate.x, reticle.selectedHero.Coordinate.y,2), new Vector3Int(cell.x, cell.y,2)) <= reticle.selectedHero.movement)
                            {

                                if (cell.x >= 0 & cell.x < TileMapper.W & cell.y >= 0 & cell.y < TileMapper.H)
                                {
                                    withinRange[withinRangeIndex] = PathFinder.graph[cell.x, cell.y];

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
                            if (!pathFinder.IsReachable(reticle.selectedHero, withinRange[i], withinRange))
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

                else if (reticle.selectedHero.IsMoving == true & withinRange != null & canMoveTo != null)
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


            else if (reticle.selectedHero == null & withinRange != null & canMoveTo != null)
            {

                tilemap.ClearAllTiles();

                tilemap.RefreshAllTiles();

                withinRange = null;
                canMoveTo = null;
            }

        }
    }
}