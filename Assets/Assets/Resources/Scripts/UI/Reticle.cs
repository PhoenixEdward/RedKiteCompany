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

        Node destination;

        List<Hero> units = new List<Hero>();

        public Hero selectedHero;

        Tilemap tilemap;

        SpriteSelection menu;

        StatSheet statSheet;

        BattleGrid battleGrid;

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

            Generate();
        }

        public void Generate()
        {
            units = GameSpriteManager.Instance.Heroes;

        }

        // Update is called once per frame
        void Update()
        {
            if (!menu.isActive)
            {
                if (combatMenu.IsActive == false)
                    TileTracker();

                UnitData();
                if (selectedHero != null)
                {
                    if (!selectedHero.Ready)
                    {
                        selectedHero = null;
                        return;
                    }

                    if (selectedHero.IsMoving == false & combatMenu.IsActive == false)
                        battleGrid.UnitRange(selectedHero);
                    else if (battleGrid.withinRange != null & battleGrid.canMoveTo != null)
                        battleGrid.DeactivateUnitRange();

                    if (destination != null & combatMenu.IsActive == false)
                    {
                        if (TileMapper.Instance.Tiles[destination.cell.x, destination.cell.y].IsWalkable == true & selectedHero.IsMoving == false)
                        {
                            if (Utility.ManhattanDistance(new Vector3Int((int)selectedHero.Coordinate.x, (int)selectedHero.Coordinate.y, 2), new Vector3Int(destination.cell.x, destination.cell.y, 2)) <= selectedHero.Movement)
                            {
                                if (pathFinder.IsReachable(selectedHero, destination, battleGrid.withinRange.ToArray()))
                                {
                                    {
                                        combatMenu.ActivatePopUp(selectedHero, destination.cell);
                                        destination = null;
                                    }
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
                //this needs to be reworked. The above should be a function. A later problem.
            }
        }

        public void UnitData()
        {
            Unit highlightedUnit = null;

            foreach (Unit unit in units)
            {
                if (new Vector2(unit.Coordinate.x, unit.Coordinate.y) == new Vector2(highlight.x, highlight.y))
                {
                    highlightedUnit = unit;
                }
            }

            if(highlightedUnit != null)
            {
                statSheet.Activate(highlightedUnit);
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

            foreach (Hero unit in units)
            {
                if (new Vector2(unit.Coordinate.x, unit.Coordinate.y) == new Vector2(highlight.x, highlight.y) & selectedHero == null)
                {
                    if (Input.GetMouseButtonDown(0) & unit.Ready)
                    {
                        selectedHero = unit;
                        isSelection = true;
                    }
                    else
                    {
                        //create flash message class to communicate that unit is not ready to move and other such UI bull shit.
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

            //this needs to be moved somewhere else 
            if (selectedHero != null)
            {
                if (!isSelection)
                {
                    if (selectedHero.IsMoving == false)
                    {

                        if (Input.GetMouseButtonDown(0))
                        {
                            Vector3Int destCoords = highlight;
                            destination = PathFinder.graph[destCoords.x, destCoords.y];

                        }

                        foreach (Node node in battleGrid.canMoveTo)
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

            tilemap.RefreshAllTiles();

            isSelection = false;
        }
    }
}