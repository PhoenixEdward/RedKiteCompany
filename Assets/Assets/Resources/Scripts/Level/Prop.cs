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

            offset = isIso ? Vector3.zero : offset;

            transform.position = Coordinate + offset + Vector3.up;

            sr.sprite = sprites[0, 0];
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }
    }
}