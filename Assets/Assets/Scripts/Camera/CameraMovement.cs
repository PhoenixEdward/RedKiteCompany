using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedKite
{
    public class CameraMovement : MonoBehaviour
    {
        protected float timeSinceLastMove = 0;
        protected float secondsPerMove = 0.0167f;
        protected float pix = 32f/8;
        protected Hero hero;
        protected Tilemap map;
        protected Vector2 xBounds;
        protected Vector2 yBounds;

        void Start()
        {
            hero = FindObjectOfType<Hero>();

            map = FindObjectOfType<Tilemap>();


            transform.position = new Vector3(hero.transform.position.x, hero.transform.position.y, -10);

            xBounds = new Vector2(map.cellBounds.xMin, map.cellBounds.xMax);
            yBounds = new Vector2(map.cellBounds.yMin, map.cellBounds.yMax);
        }

        void Update()
        {

            Vector3 pos = transform.position;

            if (Input.GetKey(KeyCode.W) & pos.y < yBounds.y)
            {
                pos.y += pix * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S) & pos.y > yBounds.x)
            {
                pos.y -= pix * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D) & pos.x < xBounds.y)
            {
                pos.x += pix * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A) & pos.x > xBounds.x)
            {
                pos.x -= pix * Time.deltaTime;
            }

            transform.position = pos;

        }
    }
}