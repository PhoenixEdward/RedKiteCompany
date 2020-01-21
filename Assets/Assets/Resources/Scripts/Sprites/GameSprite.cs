using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{ 
     public class GameSprite : MonoBehaviour
    {
        public enum SpriteType {
            Tile,
            Character,
            Prop
        }

        static bool isFirstSpawn = true;
        public SpriteType spriteType;
        public string spriteName;

        protected int verticalFrames;
        protected int horizontalFrames;

        protected int VerticalRow { get; set; }
        protected int HorizontalRow { get; set; }

        public Vector3Int Coordinate = new Vector3Int(0, 0, -2);

        static Grid grid;

        //thse will need to be switched to protected after level editor is done
        public Texture2D spriteLoad;
        public Sprite[,] sprites;

        protected SpriteRenderer sr;
        protected Vector2Int FrameDimensions;

        static protected CameraMovement cam;

        protected Vector3 offset = new Vector3(0.35f, 0, 0.35f);

        //This will be set by fog of war. Unsure about stipulations of set.
        public bool IsVisible { get; set; }

        public bool isIso;

        public bool IsMoving;

        public virtual void Start()
        {
            if(isFirstSpawn)
            {
                isFirstSpawn = false;
                grid = FindObjectOfType<Grid>();
            }

            if(spriteLoad == null)
            { 

                if (spriteType == SpriteType.Character)
                    spriteLoad = Resources.Load<Texture2D>("Characters/" + spriteName);
                else if (spriteType == SpriteType.Tile)
                    spriteLoad = Resources.Load<Texture2D>("Tiles/" + spriteName);
                else if (spriteType == SpriteType.Prop)
                    spriteLoad = Resources.Load<Texture2D>("Props/" + spriteName);

            }

            FrameDimensions = FrameDimensions == Vector2.zero ? new Vector2Int(100, 100) : FrameDimensions;

            verticalFrames = spriteLoad.height / FrameDimensions.y;
            horizontalFrames = spriteLoad.width / FrameDimensions.x;

            sprites = new Sprite[horizontalFrames, verticalFrames];

            for (int y = 0; y < verticalFrames; y++)
            {
                for (int x = 0; x < horizontalFrames; x++)
                {
                    Sprite sprite = Sprite.Create(spriteLoad, new Rect(new Vector2(x * FrameDimensions.x, y * FrameDimensions.y), FrameDimensions), new Vector2(0.5f, 0.5f));
                    sprites[x, y] = sprite;
                }
            }

            sr = gameObject.AddComponent<SpriteRenderer>();

            sr.sortingLayerName = "Units";

            //transform.rotation = Quaternion.Euler(0, 45, 0);

        }

        public virtual void Update()
        {
            if(CameraMovement.facing == CameraMovement.Facing.NE)
            {
                transform.rotation = Quaternion.Euler(0, 45f, 0);

                if(isIso)
                    transform.position = grid.CellToWorld(Coordinate) + Vector3.up;

                if (horizontalFrames > 3 & !IsMoving)
                    HorizontalRow = 3;
            }
            else if (CameraMovement.facing == CameraMovement.Facing.NW)
            {
                transform.rotation = Quaternion.Euler(0, 135f, 0);

                if (isIso)
                    transform.position = grid.CellToWorld(Coordinate) + new Vector3(0,0,.65f) + Vector3.up;

                if (horizontalFrames > 2 & !IsMoving)
                    HorizontalRow = 2;
            }
            else if (CameraMovement.facing == CameraMovement.Facing.SW)
            {
                transform.rotation = Quaternion.Euler(0, 225f, 0);

                if (isIso)
                    transform.position = grid.CellToWorld(Coordinate) + new Vector3(.65f, 0, .65f) + Vector3.up;

                if (horizontalFrames > 1 & !IsMoving)
                    HorizontalRow = 1;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 315f, 0);

                if (isIso)
                    transform.position = grid.CellToWorld(Coordinate) + new Vector3(.65f, 0, 0) + Vector3.up;

                if(!IsMoving)
                    HorizontalRow = 0;
            }
        }

        public void ReStart(string textureName, Texture2D texture)
        {
            spriteName = textureName.Substring(0, textureName.Length - 4);

            spriteLoad = texture;

            verticalFrames = spriteLoad.height / 100;
            horizontalFrames = spriteLoad.width / 100;

            sprites = new Sprite[horizontalFrames, verticalFrames];

            for (int y = 0; y < verticalFrames; y++)
            {
                for (int x = 0; x < horizontalFrames; x++)
                {
                    Sprite sprite = Sprite.Create(spriteLoad, new Rect(new Vector2(x * 100, y * 100), new Vector2(100, 100)), new Vector2(0.5f, 0.5f));
                    sprites[x, y] = sprite;
                }
            }

        }
    }
}