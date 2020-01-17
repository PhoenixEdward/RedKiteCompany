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
        public int[] tris;
        public Vector2[] copyUVs;
        public Vector3[] copyVerts;
        public int[] copyTris;

        // Start is called before the first frame update
        void Start()
        {
            topWallTex = Resources.Load<Texture2D>("Tiles/BambooFloor");
            wallTex = Resources.Load<Texture2D>("Tiles/GreenBamboo");
            MeshMaker meshMaker = new MeshMaker();
            meshMaker.NewMakeMesh(new Vector3(5,10,2), new Vector3(0,0,0));
            MeshMaker meshMaker2 = new MeshMaker();
            meshMaker2.NewMakeMesh(new Vector3(1, 1, 1), new Vector3(5f, 0, 0));

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

            //go.transform.localScale = new Vector3(5, 5, 5);

            Mesh copy = go.GetComponent<MeshFilter>().mesh;

            copyVerts = copy.vertices;
            copyTris = copy.triangles;
            copyUVs = copy.uv;


            MeshMaker output = MeshMaker.CombinePlanes(new List<MeshMaker> { meshMaker});

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

            MeshRenderer render = gameObject.AddComponent<MeshRenderer>();

            output.NewSetTextures(render, textures);

            output.MergeSides();

            tris = meshMaker.subMeshes[5].triangles;
            verts = meshMaker.subMeshes[5].vertices;
            uvs = output.mesh.uv;

            meshFilter.mesh = output.mesh;

            render.material.mainTexture = wallTex;

            textures = new Texture2D[6] { topWallTex, wallTex, wallTex, wallTex, wallTex, wallTex };
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}