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

        public SpriteType spriteType;
        public string spriteName;
        public Setting setting;
        public bool simpleCollision;
        public float spriteDepth;
        public List<Rect> relativeCollisionBoxes;
        public List<Rect> CollisionBoxes { get; protected set; } = new List<Rect>();

        public bool CanTalk;

        protected Sprite[] spritesLoad;
        protected Sprite[,] sprites;
        protected SpriteRenderer sr;
        protected Vector2 FrameDimensions;

        // Start is called before the first frame update
        public virtual void Start()
        {
            if(spriteType == SpriteType.Character)
                spritesLoad = Resources.LoadAll<Sprite>("Characters/" + spriteName);
            else if(spriteType == SpriteType.Tile)
                spritesLoad = Resources.LoadAll<Sprite>("Tiles/"+ setting  + "/" + spriteName);

            sr = GetComponent<SpriteRenderer>();

            sprites = new Sprite[8, spritesLoad.Length / 8];

            int loop = 0;

            FrameDimensions = spritesLoad[0].rect.size/100;

            for (int i = 0; i < (spritesLoad.Length / 8); i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    sprites[j, i] = spritesLoad[loop];
                    loop++;
                }
            }

            //potentially uninvert the transform.position.y

            if (simpleCollision)
            {
                relativeCollisionBoxes.Add(new Rect(0, FrameDimensions.y - spriteDepth, FrameDimensions.x, spriteDepth));
                CollisionBoxes.Add(new Rect(transform.position.x, -transform.position.y + FrameDimensions.y - spriteDepth, FrameDimensions.x, spriteDepth));
            }
            else
            {
                for (int i = 0; i < relativeCollisionBoxes.Count; i++)
                    CollisionBoxes.Add(new Rect(transform.position.x + relativeCollisionBoxes[i].x, -transform.position.y + relativeCollisionBoxes[i].y, relativeCollisionBoxes[i].width, relativeCollisionBoxes[i].height));
            }

            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y - FrameDimensions.y);

        }

        public void UpdateCollision()
        {
            for(int i = 0; i < relativeCollisionBoxes.Count; i++)
                CollisionBoxes[i] = new Rect(transform.position.x + relativeCollisionBoxes[i].x, -transform.position.y + relativeCollisionBoxes[i].y, relativeCollisionBoxes[i].width, relativeCollisionBoxes[i].height);
        }

    }
}