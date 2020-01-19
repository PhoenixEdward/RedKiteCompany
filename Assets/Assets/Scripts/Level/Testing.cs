using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using UnityEngine.Networking;

namespace RedKite
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Testing : MonoBehaviour
    {
        Texture2D wallTex;
        Texture2D topWallTex;
        Texture2D[] textures;
        public Vector2[] uvs;
        public Vector3[] verts;
        public int[] tris;

        public int[] copyTris;
        public Vector3[] copyVerts;

        public List<string> wallFiles = new List<string>();
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        MeshMaker cubeMesh;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log(Application.dataPath);

            meshFilter = gameObject.GetComponent<MeshFilter>();
            meshRenderer = gameObject.GetComponent<MeshRenderer>();

            topWallTex = Resources.Load<Texture2D>("Tiles/BambooFloor");
            wallTex = Resources.Load<Texture2D>("Tiles/WoodFloor");
            MeshMaker meshMaker = new MeshMaker();
            meshMaker.NewMakeMesh(new Vector3(1,1,1), new Vector3(0,0,0));
            MeshMaker meshMaker2 = new MeshMaker();
            meshMaker2.NewMakeMesh(new Vector3(10, 5, 2), new Vector3(7.5f, 0, 0));

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            cubeMesh = MeshMaker.CombinePlanes(new List<MeshMaker> { meshMaker });

            copyTris = cube.GetComponent<MeshFilter>().mesh.triangles;
            copyVerts = cube.GetComponent<MeshFilter>().mesh.vertices;

            textures = new Texture2D[] { wallTex, wallTex, topWallTex, wallTex, wallTex, wallTex };

            cubeMesh.SetTextures(meshRenderer, textures, new bool[] { false, false, false, false, false, false});

            cubeMesh.MergeSides();

            tris = cubeMesh.mesh.triangles;
            verts = cubeMesh.mesh.vertices;
            uvs = cubeMesh.mesh.uv;

            meshFilter.mesh = cubeMesh.mesh;

        }

        IEnumerator GetTopTexture(MeshRenderer meshRenderer, MeshFilter meshFilter , MeshMaker meshMaker, string textureName)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + Application.dataPath + "/GreenBamboo.png");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                meshFilter.mesh.Clear();

                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

                meshMaker.UpdateOneTexture(meshRenderer, myTexture, 2, false);

                meshMaker.MergeSides();

                meshFilter.mesh = meshMaker.mesh;
            }

        }

        IEnumerator GetSideTextures(MeshRenderer meshRenderer, MeshFilter meshFilter, MeshMaker meshMaker, string textureName)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + Application.dataPath + "/GreenBamboo.png");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                meshFilter.mesh.Clear();

                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

                meshMaker.UpdateSideTextures(meshRenderer, myTexture);

                meshMaker.MergeSides();

                meshFilter.mesh = meshMaker.mesh;
            }
        }

            // Update is called once per frame
            void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                string textureName = Directory.GetFiles(Application.dataPath, "*.png")[0];
                StartCoroutine(GetTopTexture(meshRenderer, meshFilter, cubeMesh, textureName));
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                string textureName = Directory.GetFiles(Application.dataPath, "*.png")[0];
                StartCoroutine(GetSideTextures(meshRenderer, meshFilter, cubeMesh, textureName));
            }

        }
    }
}