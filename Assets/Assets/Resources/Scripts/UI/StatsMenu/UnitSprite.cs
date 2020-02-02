using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RedKite
{ 
    public class UnitSprite : MonoBehaviour
    {
        public Unit selectedUnit { get; set; }
        Image image;
        // Start is called before the first frame update
        void Start()
        {
            image = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            if (selectedUnit != null)
                image.sprite = selectedUnit.sprites[0, 0];
        }
    }
}