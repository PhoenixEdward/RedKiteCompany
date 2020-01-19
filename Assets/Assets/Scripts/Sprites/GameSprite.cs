using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{ 
     public class GameSprite : MonoBehaviour
    {
        public enum SpriteType {
            Tile,
            Character
        }

        public enum Setting
        {
            None,
            JuneApartment,
            FirstBlock,
            JitterBean,
            GrabNGo
        }

        protected static Level level;
        static bool isFirstSpawn = true;
        public SpriteType spriteType;
        public string spriteName;

        protected int verticalFrames;
        protected int horizontalFrames;

        protected Texture2D spriteLoad;
        public Sprite[,] sprites;
        protected SpriteRenderer sr;
        protected Vector2 FrameDimensions;

        // Start is called before the first frame update


        public virtual void Start()
        {
            //find a way for only the first unit spawned will do this. You did it for spawn just do it again.
            if(isFirstSpawn)
            { 
                level = FindObjectOfType<Level>();
                isFirstSpawn = false;
            }


            if (spriteType == SpriteType.Character)
                spriteLoad = Resources.Load<Texture2D>("Characters/" + spriteName);
            else if (spriteType == SpriteType.Tile)
                spriteLoad = Resources.Load<Texture2D>("Tiles/" + spriteName);

            Debug.Log(spriteLoad.width);
            
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

            sr = gameObject.AddComponent<SpriteRenderer>();

            sr.sortingLayerName = "Units";

            transform.rotation = Quaternion.Euler(0, 45, 0);

        }

        public void ReStart(Texture2D texture)
        {
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