using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedKite
{ 
    public class Reticle : MonoBehaviour
    {
        Vector3Int temp = Vector3Int.zero;

        [SerializeField]
        Vector3Int highlight;
        Tile highlightTile;
        Tile selectTile;

        Tile clearTile;
        Tile rangeHighlightTile;

        PathFinder pathFinder = new PathFinder();

        Grid grid;

        bool isSelection;

        Node destination;

        List<Unit> units = new List<Unit>();

        public Unit selectedHero;

        Tilemap tilemap;

        

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

            units = GameSpriteManager.Instance.Units;

            grid = FindObjectOfType<Grid>();
        }

        // Update is called once per frame
        void Update()
        {
            TileTracker();
            if (selectedHero != null)
            {
                if (destination != null)
                {
                    if (TileMapper.tiles[destination.cell.x, destination.cell.y].IsWalkable == true & selectedHero.IsMoving == false)
                        if (Utility.ManhattanDistance(new Vector3Int(selectedHero.Coordinate.x, selectedHero.Coordinate.y,2), new Vector3Int(destination.cell.x, destination.cell.y,2)) <= selectedHero.movement)
                        {
                            if (pathFinder.IsReachable(selectedHero, destination, BattleGrid.withinRange))
                            { 
                                selectedHero.Move(destination.cell.x, destination.cell.y);
                                destination = null;
                            }
                        }
                }
            }

        }

        public void TileTracker()
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                highlight = grid.WorldToCell(hit.point);
            }

            tilemap.SetTile(highlight, highlightTile);

            if (selectedHero != null)
            {
                if (Input.GetMouseButtonDown(1) & selectedHero.IsMoving == false)
                {
                    selectedHero = null;
                    destination = null;
                }
            }
            foreach (Unit unit in units)
            {
                if (new Vector2(unit.transform.position.x, unit.transform.position.z) == new Vector2(highlight.x, highlight.y) & selectedHero == null)
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
                    if (selectedHero.IsMoving == false)
                    {

                        if (Input.GetMouseButtonDown(0))
                        {
                            Vector3Int destCoords = highlight;
                            destination = PathFinder.graph[destCoords.x, destCoords.y];

                        }

                        foreach (Node node in BattleGrid.canMoveTo)
                        {
                            if (node != null)
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
    }
}