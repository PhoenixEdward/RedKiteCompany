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
        protected Hero[] heroes;
        protected Vector2 xBounds;
        protected Vector2 yBounds;

        public static Facing facing;

        //these are all backwards. Should fix.
        public enum Facing
        {
            NE,
            SE,
            SW,
            NW
        }

        void OnEnable()
        {
            heroes = FindObjectsOfType<Hero>();

            transform.position = TileMapper.Instance.Areas[0].Floor.Center + new Vector3(-15f, 15f, -15f);

            xBounds = new Vector2(0, TileMapper.Instance.W);
            yBounds = new Vector2(0, TileMapper.Instance.H);

            Camera.main.transform.rotation = Quaternion.Euler(30, 45, 0);

            CameraMovement.facing = Facing.NE;

        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Q))
            { 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    transform.RotateAround(hit.point, Vector3.up, 90f);

                    if (facing == Facing.NE)
                        facing = Facing.SE;
                    else if (facing == Facing.SE)
                        facing = Facing.SW;
                    else if (facing == Facing.SW)
                        facing = Facing.NW;
                    else
                        facing = Facing.NE;
                }

            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    transform.RotateAround(hit.point, Vector3.down, 90f);

                    if (facing == Facing.NE)
                        facing = Facing.NW;
                    else if (facing == Facing.NW)
                        facing = Facing.SW;
                    else if (facing == Facing.SW)
                        facing = Facing.SE;
                    else
                        facing = Facing.NE;
                }

            }
            

            Vector3 movement = Vector3.zero;

            if(facing == Facing.NE)
            { 
                if (Input.GetKey(KeyCode.W))
                {
                    movement.x += pix * Time.deltaTime;
                    movement.z += pix * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    movement.x -= pix * Time.deltaTime;
                    movement.z -= pix * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.D) & transform.position.x < xBounds.y)
                {
                    movement.x += pix/2 * Time.deltaTime;
                    movement.z -= pix/2 * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    movement.x -= pix/2 * Time.deltaTime;
                    movement.z += pix/2 * Time.deltaTime;
                }
            }

            if (facing == Facing.SE)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    movement.z -= pix * Time.deltaTime;
                    movement.x += pix * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.S))
                {

                    movement.z += pix * Time.deltaTime;
                    movement.x -= pix * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    movement.z -= pix / 2 * Time.deltaTime;
                    movement.x -= pix / 2 * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.A) )
                {
                    movement.z += pix / 2 * Time.deltaTime;
                    movement.x += pix / 2 * Time.deltaTime; 
                }
            }


            if (facing == Facing.SW)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    movement.z -= pix * Time.deltaTime;
                    movement.x -= pix * Time.deltaTime;

                }
                if (Input.GetKey(KeyCode.S))
                {
                    movement.z += pix * Time.deltaTime;
                    movement.x += pix * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    movement.z += pix/2 * Time.deltaTime;
                    movement.x -= pix/2 * Time.deltaTime;

                }
                if (Input.GetKey(KeyCode.A))
                {
                    movement.z -= pix/2 * Time.deltaTime;
                    movement.x += pix/2 * Time.deltaTime;
                }
            }

            if (facing == Facing.NW)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    movement.z += pix * Time.deltaTime;
                    movement.x -= pix * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    movement.z -= pix * Time.deltaTime;
                    movement.x += pix * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    movement.z += pix / 2 * Time.deltaTime;
                    movement.x += pix / 2 * Time.deltaTime;

                }
                if (Input.GetKey(KeyCode.A))
                {
                    movement.z -= pix / 2 * Time.deltaTime;
                    movement.x -= pix / 2 * Time.deltaTime;
                }
            }


            transform.position += movement;

        }
    }
}