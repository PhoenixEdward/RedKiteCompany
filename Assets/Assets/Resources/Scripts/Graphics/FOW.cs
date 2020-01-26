using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{ 
    public class FOW : MonoBehaviour
    {
        System.Random rnd = new System.Random();
        SpriteRenderer sr;
        public float rando;
        // Start is called before the first frame update
        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            rando += rando + 1/(float)rnd.Next(0,1) > 1 ? rando - 1 / (float)rnd.Next(0, 1) : rando - 1 / (float)rnd.Next(-1, 1);
            sr.material.SetFloat("_Rando",rando);

        }
    }
}