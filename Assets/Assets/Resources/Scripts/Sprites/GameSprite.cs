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

        public int verticalFrames; 
        public int horizontalFrames;

        public int VerticalRow { get; set; }
        public int HorizontalRow { get; set; }

        public Vector3Int Coordinate = new Vector3Int(0, 0, -2);

        static Grid grid;

        //thse will need to be switched to protected after level editor is done
        public Texture2D spriteLoad;
        public Sprite[,] sprites;

        public static Color fogTint;

        protected SpriteRenderer sr;
        protected Vector2Int FrameDimensions;

        static protected CameraMovement cam;

        static CameraMovement.Facing currentCamFacing;

        Material spriteMask;

        protected Vector3 offset = new Vector3(0, 0, 0);

        //This will be set by fog of war. Unsure about stipulations of set.
        public bool IsVisible { get; set; } = false;

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

            FrameDimensions = FrameDimensions == Vector2.zero ? new Vector2Int(150, 150) : FrameDimensions;

            verticalFrames = spriteLoad.height / FrameDimensions.y;
            horizontalFrames = spriteLoad.width / FrameDimensions.x;

            sprites = new Sprite[horizontalFrames, verticalFrames];

            for (int y = 0; y < verticalFrames; y++)
            {
                for (int x = 0; x < horizontalFrames; x++)
                {
                    Sprite sprite = Sprite.Create(spriteLoad, new Rect(new Vector2(x * FrameDimensions.x, y * FrameDimensions.y), FrameDimensions), new Vector2(0.5f, 0f));
                    sprites[x, y] = sprite;
                }
            }

            sr = gameObject.AddComponent<SpriteRenderer>();

            spriteMask = Resources.Load<Material>("RenderTargets/Materials/SpriteMat");

            sr.material = spriteMask;
            sr.material.SetColor("_FogColor", fogTint);

            sr.sortingLayerName = "Units";

        }

        public virtual void Update()
        {
            if (!IsVisible)
                sr.enabled = false;
            else
                sr.enabled = true;

            sr.material.SetTexture("_MainTex2", GameObject.FindGameObjectWithTag("FogCam").GetComponent<Camera>().activeTexture);
            sr.material.SetTexture("_MainTex3", FindObjectOfType<WallRender>().wallRender);

            if (CameraMovement.facing == CameraMovement.Facing.NE)
            {
                transform.rotation = Quaternion.Euler(0, 45f, 0);

                transform.localPosition = grid.CellToWorld(Coordinate) + new Vector3(0, 0.5f, 0);

                if (horizontalFrames > 3 & !IsMoving)
                {
                    VerticalRow = verticalFrames - 1;
                    HorizontalRow = 3;
                }
            }
            else if (CameraMovement.facing == CameraMovement.Facing.SE)
            {

                transform.rotation = Quaternion.Euler(0, 135f, 0);

                transform.localPosition = grid.CellToWorld(Coordinate) + new Vector3(0f, 0, 1f) + new Vector3(0, 0.5f, 0);


                if (horizontalFrames > 2 & !IsMoving)
                {
                    VerticalRow = verticalFrames - 1;
                    HorizontalRow = 2;
                }
            }
            else if (CameraMovement.facing == CameraMovement.Facing.SW)
            {
                transform.rotation = Quaternion.Euler(0, 225f, 0);


                transform.localPosition = grid.CellToWorld(Coordinate) + new Vector3(1f, 0, 1f) + new Vector3(0, 0.5f, 0);


                if (horizontalFrames > 1 & !IsMoving)
                {
                    VerticalRow = verticalFrames - 1;
                    HorizontalRow = 1;
                }
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 315f, 0);

                transform.localPosition = grid.CellToWorld(Coordinate) + new Vector3(1, 0, 0) + new Vector3(0, 0.5f, 0);

                if (!IsMoving)
                {
                    VerticalRow = verticalFrames - 1;
                    HorizontalRow = 0;
                }
            }
        }

        public void ReStart(string textureName, Texture2D texture)
        {
            spriteName = textureName.Substring(0, textureName.Length - 4);

            spriteLoad = texture;

            FrameDimensions = FrameDimensions == Vector2.zero ? new Vector2Int(150, 150) : FrameDimensions;

            verticalFrames = spriteLoad.height / FrameDimensions.y;
            horizontalFrames = spriteLoad.width / FrameDimensions.x;

            sprites = new Sprite[horizontalFrames, verticalFrames];

            for (int y = 0; y < verticalFrames; y++)
            {
                for (int x = 0; x < horizontalFrames; x++)
                {
                    Sprite sprite = Sprite.Create(spriteLoad, new Rect(new Vector2(x * FrameDimensions.x, y * FrameDimensions.y), FrameDimensions), new Vector2(0.5f, 0f));
                    sprites[x, y] = sprite;
                }
            }

        }
    }
}