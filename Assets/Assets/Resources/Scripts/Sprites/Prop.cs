using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{ 
    public class Prop : GameSprite
    {
        public bool IsInteractable { get; protected set; }

        public override void Start()
        {
            spriteType = SpriteType.Prop;

            base.Start();

            Instantiate(spriteName, JobClass.Bard, 1);
            Spawn();
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();

            sr.sprite = sprites[HorizontalRow, VerticalRow];
        }
    }
}