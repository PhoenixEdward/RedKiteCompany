using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;

namespace RedKite
{ 
    public class Modeler : MonoBehaviour
    {
        static Transform level;
        Texture2D wallTex;
        Texture2D topWallTex;
        Texture2D floorTex;
        Texture2D[] wallTextures;
        Texture2D[] floorTextures;
        List<MeshMaker> wallMeshes = new List<MeshMaker>();
        List<MeshMaker> floorMeshes = new List<MeshMaker>();
        GameObject walls;
        GameObject floor;

        MeshFilter wallFilter;
        MeshFilter floorFilter;

        MeshRenderer wallRenderer;
        MeshRenderer floorRenderer;

        MeshMaker wallMesh;
        MeshMaker floorMesh;

        public string floorTexName;
        public string topWallTexName;
        public string wallTexName;

        // Start is called before the first frame update
        public void Start()
        {
            floorTexName = floorTexName ?? "WoodFloor";
            topWallTexName = topWallTexName ?? "BambooFloor";
            wallTexName = wallTexName ?? "jFlower";

            floorTex = Resources.Load<Texture2D>("Tiles/" + floorTexName);
            topWallTex = Resources.Load<Texture2D>("Tiles/" + topWallTexName);
            wallTex = Resources.Load<Texture2D>("Tiles/" + wallTexName);
            wallTextures = new Texture2D[6] { wallTex, wallTex, topWallTex, wallTex, wallTex, wallTex };
            floorTextures = new Texture2D[6] { floorTex, floorTex, floorTex, floorTex, floorTex, floorTex };
            walls = new GameObject();
            walls.layer = 9;
            walls.transform.position = new Vector3(0.5f, 0, .5f);

            floor = new GameObject();
            floor.transform.position = new Vector3(0.5f, 0, 0.5f);

            wallFilter = walls.AddComponent<MeshFilter>();
            wallRenderer = walls.AddComponent<MeshRenderer>();

            wallRenderer.material.shader = Shader.Find("Custom/Terrain");

            floorFilter = floor.AddComponent<MeshFilter>();
            floorRenderer = floor.AddComponent<MeshRenderer>();

            floorRenderer.material.shader = Shader.Find("Custom/Terrain");

            RenderWalls(wallFilter,wallRenderer);
            RenderFloor(floorFilter, floorRenderer);

            walls.AddComponent<MeshCollider>();
            floor.AddComponent<MeshCollider>();

        }

        public void Regen()
        {
            wallTextures = wallMesh.SubMeshTextures;
            floorTextures = floorMesh.SubMeshTextures;

            wallFilter.mesh.Clear();
            floorFilter.mesh.Clear();

            wallMeshes.Clear();
            floorMeshes.Clear();

            RenderWalls(wallFilter, wallRenderer);
            RenderFloor(floorFilter, floorRenderer);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void RenderWalls(MeshFilter meshFilter, MeshRenderer meshRenderer)
        {
            foreach(KeyValuePair<int,Area> entry in TileMapper.Instance.Areas)
            {
                Area area = entry.Value;

                foreach (Area.Wall wall in area.Walls)
                {
                    foreach(Segment seg in wall.Segments)
                    {

                        MeshMaker segMesh = new MeshMaker();

                        segMesh.NewMakeMesh(seg.Scale, seg.Center);

                        wallMeshes.Add(segMesh);


                    }
                    if (wall.Overlaps.Count != 0)
                    {
                        foreach (Segment path in wall.Overlaps.Where(x => x.IsCorner).ToList())
                        {
                            if (path.IsRemoved == false)
                            {
                                //skip over north and south corners to avoid duplication.

                                MeshMaker cornerMesh = new MeshMaker();

                                cornerMesh.NewMakeMesh(path.Scale, path.Center);

                                wallMeshes.Add(cornerMesh);
                            }
                        }
                    }
                }
            }

            wallMesh = MeshMaker.CombinePlanes(wallMeshes);

            wallMesh.SetTextures(meshRenderer, wallTextures, new bool[] { false, false, false, false, false, false });

            wallMesh.MergeSides();

            meshFilter.mesh = wallMesh.Mesh;
        }


        void RenderFloor(MeshFilter meshFilter, MeshRenderer meshRenderer)
        {
            foreach (KeyValuePair<int, Area> entry in TileMapper.Instance.Areas)
            {
                Area area = entry.Value;

                MeshMaker floorMesh = new MeshMaker();

                floorMesh.NewMakeMesh(area.Floor.TrueScale, area.Floor.Center);

                floorMeshes.Add(floorMesh);
            }

            floorMesh = MeshMaker.CombinePlanes(floorMeshes);

            floorMesh.SetTextures(meshRenderer, floorTextures, new bool[] { false, false, false, false, false, false });

            floorMesh.MergeSides();

            meshFilter.mesh = floorMesh.Mesh;
        }

        public void SetTopWallTexture(string texName, Texture2D myTexture, bool isMirrored)
        {
            topWallTexName = texName;    

            wallFilter.mesh.Clear();

            wallMesh.UpdateOneTexture(wallRenderer, myTexture, 2, isMirrored);

            wallMesh.MergeSides();

            wallFilter.mesh = wallMesh.Mesh;
        }

        public void SetSideWallTextures(string texName, Texture2D myTexture, bool isMirrored)
        {
            wallTexName = texName;

            wallFilter.mesh.Clear();

            wallMesh.UpdateSideTextures(wallRenderer, myTexture, isMirrored);

            wallMesh.MergeSides();

            wallFilter.mesh = wallMesh.Mesh;
        }
        public void SetFloorTexture(string texName, Texture2D myTexture, bool isMirrored)
        {
            floorTexName = texName;

            floorFilter.mesh.Clear();

            floorMesh.SetTextures(floorRenderer, new Texture2D[] { myTexture, myTexture, myTexture, myTexture, myTexture, myTexture },
                new bool[] { isMirrored, isMirrored, isMirrored, isMirrored, isMirrored, isMirrored });

            floorMesh.MergeSides();

            floorFilter.mesh = floorMesh.Mesh;
        }
    }
}