using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RedKite
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TextFX : MonoBehaviour
    {
        Sprite[] sprites;
        SpriteRenderer sr;
        // Start is called before the first frame update
        public void Activate(int amount, bool isDamage)
        {
            //should move this to FX folder probably.
            sprites = Resources.LoadAll<Sprite>("UI/NumberSprites");
            sr = GetComponent<SpriteRenderer>();

            if (isDamage)
                sr.color = Color.red;
            else
                sr.color = Color.green;

            if (amount == 0)
                sr.color = Color.white;

            sr.sprite = sprites[amount];
            sr.sortingLayerName = "UI";
        }

        // add falling animation later.
        void Update()
        {
        
        }
    }
}