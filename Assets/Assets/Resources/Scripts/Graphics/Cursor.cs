using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedKite
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    class Cursor : MonoBehaviour
    {
        Mesh mesh;
        MeshFilter meshFilter;
        public Vector2[] uv;

        public void CreateMesh()
        {
            mesh = new Mesh();

            mesh.vertices = new Vector3[]
            {
                //top
                new Vector3(0,0.25f,0),

                //mid
                new Vector3(0.1f, 0, 0.1f),
                new Vector3(0.1f,0,-0.1f),
                new Vector3(-0.1f,0,-0.1f),
                new Vector3(-0.1f,0,0.1f),

                //bottom
                new Vector3(0,-0.25f,0)

            };

            mesh.triangles = new int[]
            {
                //top
                0,1,2,
                0,2,3,
                0,3,4,
                0,4,1,

                //bottom

                2,1,5,
                3,2,5,
                4,3,5,
                1,4,5
            };

            mesh.uv = new Vector2[]
            {
                //top
                new Vector2(0.5f, 1f),

                //sides
                new Vector2(1f, 1f),
                new Vector2(0f,0f),
                new Vector2(1f, 1f),
                new Vector2(0f,0f),

                //bottom
                new Vector2(0.5f, 1f),

            };

            mesh.RecalculateNormals();

            meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            uv = mesh.uv;
        }

        private void Start()
        {
            CreateMesh();

        }
    }
}
