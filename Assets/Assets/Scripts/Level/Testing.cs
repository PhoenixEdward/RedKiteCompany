using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{ 
    public class Testing : MonoBehaviour
    {
        Texture2D wallTex;
        Texture2D topWallTex;
        Texture2D[] textures;
        // Start is called before the first frame update
        void Start()
        {
            topWallTex = Resources.Load<Texture2D>("Tiles/BambooFloor");
            wallTex = Resources.Load<Texture2D>("Tiles/GreenBamboo");
            MeshMaker meshMaker = gameObject.AddComponent<MeshMaker>();
            meshMaker.CreateInstance(new Vector3(1,1,1));
            textures = new Texture2D[6] { topWallTex, wallTex, wallTex, wallTex, wallTex, wallTex };
            meshMaker.SetTextures(textures);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}