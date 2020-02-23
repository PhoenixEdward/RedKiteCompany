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
        Tile readyTile;

        public Level level;

        PathFinder pathFinder = new PathFinder();


        public static Tilemap map;

        public List<Node> withinRange = new List<Node>();
        public List<Node> canMoveTo = new List<Node>();

        public bool isActive;

        List<Hero> heroes;

        Reticle reticle;

        // Start is called before the first frame update
        void Start()
        {
            reticle = FindObjectOfType<Reticle>();

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

            readyTile = ScriptableObject.CreateInstance<Tile>();
            readyTile.sprite = Resources.Load<Sprite>("UI/Ready");

            for (int x = 0; x < map.cellBounds.xMax; x++)
            {
                for (int y = 0; y < map.cellBounds.yMax; y++)
                {
                    map.SetTile(new Vector3Int(x, y, 0), clearTile);
                }
            }

        }

        private void Update()
        {
            heroes = GameSpriteManager.Instance.Heroes;

            if (!isActive)
            {
                foreach (Hero hero in heroes)
                {
                    if (hero.Ready)
                    {
                        map.SetTile(hero.Coordinate, readyTile);
                    }
                    else
                        map.SetTile(hero.Coordinate, clearTile);
                }
            }
            map.RefreshAllTiles();
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

                withinRange.Add(PathFinder.graph[entry.Key.x, entry.Key.y]);
            }

            map.RefreshAllTiles();
        }

        //pretty sure if I I make this an array this repetition isn't necessary
        public void HighlightActionables(GameSprite[] actionables, int highlightIndex)
        {

            for (int i = 0; i < actionables.Length; i++)
            {
                if (i == highlightIndex)
                {
                    reticle.highlight = actionables[i].Coordinate + new Vector3Int(0, 0, -1);

                    reticle.UpdateHighlight();
                }
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