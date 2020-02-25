using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

namespace RedKite
{ 
    public class Reticle : MonoBehaviour
    {
        Vector3Int temp = Vector3Int.zero;

        [SerializeField]
        public Vector3Int highlight;
        Tile highlightTile;
        Tile selectTile;

        Tile clearTile;
        Tile rangeHighlightTile;

        PathFinder pathFinder = new PathFinder();

        Grid grid;
        CombatMenu combatMenu;

        bool isSelection;
        bool deselect = false;

        Node destination;

        List<Hero> heroes = new List<Hero>();
        List<Unit> units = new List<Unit>();

        public Unit selectedHero;

        Tilemap tilemap;

        TileMapper map;

        SpriteSelection menu;

        StatSheet statSheet;

        BattleGrid battleGrid;

        GameObject cursor;
        public Material cursorMat;

        // Start is called before the first frame update
        void Start()
        {
            highlightTile = ScriptableObject.CreateInstance<Tile>();
            highlightTile.sprite = Resources.Load<Sprite>("UI/Reticle");

            rangeHighlightTile = ScriptableObject.CreateInstance<Tile>();
            rangeHighlightTile.sprite = Resources.Load<Sprite>("UI/RangeReticle");

            selectTile = ScriptableObject.CreateInstance<Tile>();
            selectTile.sprite = Resources.Load<Sprite>("UI/Select");

            tilemap = GetComponent<Tilemap>();

            grid = FindObjectOfType<Grid>();

            menu = FindObjectOfType<SpriteSelection>();

            combatMenu = FindObjectOfType<CombatMenu>();

            statSheet = FindObjectOfType<StatSheet>();

            battleGrid = FindObjectOfType<BattleGrid>();

            map = TileMapper.Instance;

            Generate();

            cursor = new GameObject();

            cursor.AddComponent<Cursor>();

            cursor.GetComponent<MeshRenderer>().material = cursorMat;
        }

        public void Generate()
        {
            heroes = GameSpriteManager.Instance.Heroes;
            units = GameSpriteManager.Instance.Units;
        }

        // Update is called once per frame
        void Update()
        {
            if (!menu.isActive)
            {
                if (CombatMenu.IsActive == false)
                    TileTracker();

                UnitData();
                if(!BattleClock.Instance.IsEnemyTurn)
                { 
                    if (selectedHero != null)
                    {
                        if (!selectedHero.Ready & deselect)
                        {
                            deselect = false;
                            selectedHero = null;
                            return;
                        }

                        if (selectedHero.IsMoving == false & battleGrid.isActive == false & selectedHero.Ready)
                            battleGrid.NewUnitRange(selectedHero);

                        if (destination != null & CombatMenu.IsActive == false)
                        {
                            if (TileMapper.Instance.Tiles[destination.cell.x, destination.cell.y].IsWalkable == true & selectedHero.IsMoving == false)
                            {
                                if (Utility.ManhattanDistance(new Vector3Int((int)selectedHero.Coordinate.x, (int)selectedHero.Coordinate.y, 2), new Vector3Int(destination.cell.x, destination.cell.y, 2)) <= selectedHero.Movement)
                                {
                                    if (battleGrid.withinRange.Contains(destination) &
                                        (TileMapper.Instance.Tiles[destination.cell.x, destination.cell.y].TileType != Cell.Type.OccupiedAlly | destination.cell == selectedHero.Coordinate))
                                    {
                                        deselect = true;

                                        List<Node> path = pathFinder.GeneratePathTo(selectedHero.Coordinate, destination.cell, selectedHero.Movement);

                                        highlight = new Vector3Int(path[path.Count - 1].cell.x, path[path.Count - 1].cell.y, -2);

                                        UpdateHighlight(true);

                                        tilemap.RefreshAllTiles();

                                        combatMenu.ActivatePopUp(selectedHero, path[path.Count - 1].cell);

                                        destination = null;

                                    }
                                }
                            }
                        }

                        if (Input.GetMouseButtonDown(1))
                        {
                            selectedHero = null;

                            combatMenu.Deactivate();
                            battleGrid.DeactivateUnitRange();
                        }
                    }
                    else
                    {
                        battleGrid.DeactivateUnitRange();
                    }
                    //this needs to be reworked. The above should be a function. A later problem.
                }
                else
                {
                    battleGrid.DeactivateUnitRange();
                }
            }

        }

        public void UnitData()
        {
            if(selectedHero != null)
            { 
                statSheet.Activate(selectedHero);
            }
            else
            {
                statSheet.Deactivate();
            }
        }

        public void TileTracker()
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << 0)))
            {
                highlight = grid.WorldToCell(hit.point);
            }

            tilemap.SetTile(highlight, highlightTile);
            cursorMat.mainTexture = highlightTile.sprite.texture;

            foreach (Unit unit in units)
            {
                if (new Vector2(unit.Coordinate.x, unit.Coordinate.y) == new Vector2(highlight.x, highlight.y) & selectedHero == null)
                {
                    if (Input.GetMouseButtonDown(0)) // & unit.Ready)
                    {
                        selectedHero = unit;
                        isSelection = true;
                    }
                    else
                    {
                        //create flash message class to communicate that unit is not ready to move and other such UI bull shit.
                    }

                }

            }

            //here is where I can put in all the highlight logic;
            UpdateHighlight();

        }

        public void UpdateHighlight(bool isAction = false)
        {
            if(highlight.x < map.Tiles.GetLength(0) & highlight.y < map.Tiles.GetLength(1))
            { 
                if (map.Tiles[highlight.x, highlight.y].TileType == Cell.Type.OccupiedEnemy | map.Tiles[highlight.x, highlight.y].TileType == Cell.Type.OccupiedAlly)
                {
                    tilemap.SetTile(highlight, selectTile);
                    cursorMat.mainTexture = selectTile.sprite.texture;
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


                //this needs to be moved somewhere else 
                if (selectedHero != null)
                {
                    if (!isSelection)
                    {
                        if (selectedHero.IsMoving == false)
                        {

                            if (Input.GetMouseButtonDown(0) & !isAction)
                            {
                                Vector3Int destCoords = highlight;
                                destination = PathFinder.Graph[destCoords.x, destCoords.y];

                            }

                            foreach (Node node in battleGrid.canMoveTo)
                            {

                                if (highlight.x == node.cell.x & highlight.y == node.cell.y)
                                {
                                    if(!isAction)
                                    { 
                                        tilemap.SetTile(highlight, rangeHighlightTile);

                                        cursorMat.mainTexture = rangeHighlightTile.sprite.texture;
                                    }
                                    else
                                    {
                                        tilemap.SetTile(highlight, selectTile);
                                        cursorMat.mainTexture = selectTile.sprite.texture;
                                    }

                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (selectedHero.IsMoving == false)
                        {
                            foreach (Node node in battleGrid.canMoveTo)
                            {

                                if (highlight.x == node.cell.x & highlight.y == node.cell.y)
                                {

                                    tilemap.SetTile(highlight, rangeHighlightTile);

                                    cursorMat.mainTexture = rangeHighlightTile.sprite.texture;

                                    break;
                                }
                            }
                        }
                    }
                }

                isSelection = false;

                tilemap.RefreshAllTiles();

                cursor.transform.position = grid.CellToWorld(highlight) + new Vector3(0.5f, 2, 0.5f);
                cursor.transform.Rotate(new Vector3(0, 1, 0));
            }
        }
    }
}