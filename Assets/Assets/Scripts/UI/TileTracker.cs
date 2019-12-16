using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedKite
{
    public class TileTracker : MonoBehaviour
    {
        //You need references to to the Grid and the Tilemap
        Tilemap tm;
        Grid gd;

        public Vector3 destination = Vector3.zero;

        public Vector3Int highlight;

        public Hero hero;

        Vector3Int temp = Vector3Int.zero;

        Sprite oldTile;

        List<GameSprite> units = new List<GameSprite>();

        void Start()
        {
            gd = FindObjectOfType<Grid>();
            tm = FindObjectOfType<Tilemap>();

            // this will grab all heros. Need to figure out how to implement the selection of different heroes.

            hero = FindObjectOfType<Hero>();
        }

        void Update()
        {

            Vector3 worldPoint1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            highlight = gd.WorldToCell(worldPoint1);

            Debug.Log(worldPoint1.ToString());

            if (temp == Vector3Int.zero)
            {
                oldTile = tm.GetSprite(highlight);

                Tile tempTile1 = ScriptableObject.CreateInstance<Tile>();
                tempTile1.color = Color.red;
                tempTile1.sprite = oldTile;
                tm.SetTile(highlight, tempTile1);
                tm.RefreshTile(highlight);

                temp = highlight;
            }

            if (highlight != temp)
            {
                Tile tempTile = ScriptableObject.CreateInstance<Tile>();
                tempTile.sprite = oldTile;
                tm.SetTile(temp, tempTile);
                tm.RefreshTile(temp);

                oldTile = tm.GetSprite(highlight);

                tempTile = ScriptableObject.CreateInstance<Tile>();
                tempTile.color = Color.red;
                tempTile.sprite = oldTile;
                tm.SetTile(highlight, tempTile);
                tm.RefreshTile(highlight);

                temp = highlight;
            }

            if (Input.GetMouseButtonDown(0) & hero.isMoving == false)
            {
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                destination = gd.WorldToCell(gd.WorldToCell(worldPoint));

                //Shows the cell reference for the grid
            }

            tm.RefreshAllTiles();
        }
    }
}