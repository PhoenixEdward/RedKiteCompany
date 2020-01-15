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
        public MeshMaker meshMakerOut;
        public Vector2[] uvs;
        public Vector3[] verts;
        // Start is called before the first frame update
        void Start()
        {
            topWallTex = Resources.Load<Texture2D>("Tiles/BambooFloor");
            wallTex = Resources.Load<Texture2D>("Tiles/GreenBamboo");
            MeshMaker meshMaker = new MeshMaker();
            meshMaker.MakeMesh(new Vector3(1,1,1), new Vector3(0,0,0));
            MeshMaker meshMaker2 = new MeshMaker();
            meshMaker2.MakeMesh(new Vector3(2, 10, 5), new Vector3(3.5f, 0, 0));

            MeshMaker output = MeshMaker.CombinePlanes(new List<MeshMaker> { meshMaker, meshMaker2 });

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

            Renderer render = gameObject.AddComponent<Renderer>();

            gameObject.AddComponent<MeshRenderer>();

            output.SetTextures(render, textures);

            //uvs = output.subMeshes[0].uv;


            output.MergeSides();

            meshMaker.MergeSides();

            meshFilter.mesh = output.mesh;

            verts = meshMaker.subMeshes[0].vertices;

            render.material.mainTexture = wallTex;

            textures = new Texture2D[6] { topWallTex, wallTex, wallTex, wallTex, wallTex, wallTex };
            //meshMaker.SetTextures(textures);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}