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
        List<MeshMaker>[] wallMeshes = new List<MeshMaker>[4];
        List<MeshMaker> floorMeshes = new List<MeshMaker>();
        GameObject[] walls = new GameObject[4];
        GameObject floor;

        MeshFilter wallFilter0;
        MeshFilter wallFilter1;
        MeshFilter wallFilter2;
        MeshFilter wallFilter3;
        MeshFilter floorFilter;

        MeshRenderer wallRenderer0;
        MeshRenderer wallRenderer1;
        MeshRenderer wallRenderer2;
        MeshRenderer wallRenderer3;
        MeshRenderer floorRenderer;

        MeshMaker northWallMesh;
        MeshMaker southWallMesh;
        MeshMaker eastWallMesh;
        MeshMaker westWallMesh;

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

            wallMeshes[0] = new List<MeshMaker>();
            wallMeshes[1] = new List<MeshMaker>();
            wallMeshes[2] = new List<MeshMaker>();
            wallMeshes[3] = new List<MeshMaker>();


            walls[0] = new GameObject();
            walls[0].layer = 9;

            walls[1] = new GameObject();
            walls[1].layer = 10;

            walls[2] = new GameObject();
            walls[2].layer = 11;

            walls[3] = new GameObject();
            walls[3].layer = 9;

            walls[0].transform.position = new Vector3(0.5f, 0, .5f);
            walls[1].transform.position = new Vector3(0.5f, 0, .5f);
            walls[2].transform.position = new Vector3(0.5f, 0, .5f);
            walls[3].transform.position = new Vector3(0.5f, 0, .5f);


            floor = new GameObject();
            floor.transform.position = new Vector3(0.5f, 0, 0.5f);

            wallFilter0 = walls[0].AddComponent<MeshFilter>();
            wallRenderer0 = walls[0].AddComponent<MeshRenderer>();

            wallFilter1 = walls[1].AddComponent<MeshFilter>();
            wallRenderer1 = walls[1].AddComponent<MeshRenderer>();

            wallFilter2 = walls[2].AddComponent<MeshFilter>();
            wallRenderer2 = walls[2].AddComponent<MeshRenderer>();

            wallFilter3 = walls[3].AddComponent<MeshFilter>();
            wallRenderer3 = walls[3].AddComponent<MeshRenderer>();

            wallRenderer0.material.shader = Shader.Find("Custom/Terrain");
            wallRenderer1.material.shader = Shader.Find("Custom/Terrain");
            wallRenderer2.material.shader = Shader.Find("Custom/Terrain");
            wallRenderer3.material.shader = Shader.Find("Custom/Terrain");


            floorFilter = floor.AddComponent<MeshFilter>();
            floorRenderer = floor.AddComponent<MeshRenderer>();

            floorRenderer.material.shader = Shader.Find("Custom/Terrain");

            RenderWalls(wallFilter0, wallRenderer0, northWallMesh, 0, Orient.North);
            RenderWalls(wallFilter1, wallRenderer1, southWallMesh, 1, Orient.South);
            RenderWalls(wallFilter2, wallRenderer2, eastWallMesh, 2, Orient.East);
            RenderWalls(wallFilter3, wallRenderer3, westWallMesh, 3, Orient.West);

            RenderFloor(floorFilter, floorRenderer);

            walls[0].AddComponent<MeshCollider>();
            walls[1].AddComponent<MeshCollider>();
            walls[2].AddComponent<MeshCollider>();
            walls[3].AddComponent<MeshCollider>();

            floor.AddComponent<MeshCollider>();

        }

        public void Regen()
        {
            wallTextures = northWallMesh.SubMeshTextures;
            floorTextures = floorMesh.SubMeshTextures;

            wallFilter0.mesh.Clear();
            wallFilter1.mesh.Clear();
            wallFilter2.mesh.Clear();
            wallFilter3.mesh.Clear();

            floorFilter.mesh.Clear();

            wallMeshes[0].Clear();
            wallMeshes[1].Clear();
            wallMeshes[2].Clear();
            wallMeshes[3].Clear();

            floorMeshes.Clear();

            RenderWalls(wallFilter0, wallRenderer0, northWallMesh, 0, Orient.North);
            RenderWalls(wallFilter1, wallRenderer1, southWallMesh, 1, Orient.South);
            RenderWalls(wallFilter2, wallRenderer2, eastWallMesh, 2, Orient.East);
            RenderWalls(wallFilter3, wallRenderer3, westWallMesh, 3, Orient.West);

            RenderFloor(floorFilter, floorRenderer);
        }

        // Update is called once per frame
        void Update()
        {

        }

        //can probably pass in a mesh rather than an int representing the side;
        void RenderWalls(MeshFilter meshFilter, MeshRenderer meshRenderer, MeshMaker wallMesh, int side, Orient orienation)
        {
            foreach(KeyValuePair<int,Area> entry in TileMapper.Instance.Areas)
            {
                Area area = entry.Value;

                foreach (Area.Wall wall in area.Walls)
                {
                    foreach(Segment seg in wall.Segments.Where(x=> x.Orientation == orienation))
                    {

                        MeshMaker segMesh = new MeshMaker();

                        segMesh.NewMakeMesh(seg.Scale, seg.Center + Vector3.up);

                        wallMeshes[side].Add(segMesh);





                    }
                    if (wall.Overlaps.Count != 0)
                    {
                        foreach (Segment path in wall.Overlaps.Where(x => x.IsCorner & x.Orientation == orienation).ToList())
                        {
                            if (path.IsRemoved == false)
                            {
                                //skip over north and south corners to avoid duplication.

                                MeshMaker cornerMesh = new MeshMaker();

                                cornerMesh.NewMakeMesh(path.Scale, path.Center + Vector3.up);

                                wallMeshes[side].Add(cornerMesh);

                            }
                        }
                    }
                }
            }

            wallMesh = MeshMaker.CombinePlanes(wallMeshes[side]);

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

                //should consider storing pathways somewhere else. In walls seems a little bizarre.
                foreach (Area.Wall wall in area.Walls)
                {
                    if (wall.Overlaps.Count != 0)
                    {
                        foreach (Segment path in wall.Overlaps.Where(x => x.IsCorner == false).ToList())
                        {
                            if (path.IsRemoved == false)
                            {
                                MeshMaker pathMesh = new MeshMaker();

                                pathMesh.NewMakeMesh(path.Scale, path.Center);

                                floorMeshes.Add(pathMesh);
                            }
                        }
                    }
                }
            }

            foreach (KeyValuePair<int, Area> entry in TileMapper.Instance.Areas)
            {
                Area area = entry.Value;

                foreach (Area.Wall wall in area.Walls)
                {
                    foreach (Segment seg in wall.Segments)
                    {

                        MeshMaker segMesh = new MeshMaker();

                        segMesh.NewMakeMesh(seg.Scale, seg.Center);

                        floorMeshes.Add(segMesh);


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

                                floorMeshes.Add(cornerMesh);
                            }
                        }
                    }
                }
            }

            floorMesh = MeshMaker.CombinePlanes(floorMeshes);

            floorMesh.SetTextures(meshRenderer, floorTextures, new bool[] { false, false, false, false, false, false });

            floorMesh.MergeSides();

            meshFilter.mesh = floorMesh.Mesh;
        }

        public void SetTopWallTexture(string texName, Texture2D myTexture, bool isMirrored)
        {
            topWallTexName = texName;    

            wallFilter0.mesh.Clear();
            wallFilter1.mesh.Clear();
            wallFilter2.mesh.Clear();
            wallFilter3.mesh.Clear();


            northWallMesh.UpdateOneTexture(wallRenderer0, myTexture, 2, isMirrored);
            northWallMesh.UpdateOneTexture(wallRenderer1, myTexture, 2, isMirrored);
            northWallMesh.UpdateOneTexture(wallRenderer2, myTexture, 2, isMirrored);
            northWallMesh.UpdateOneTexture(wallRenderer3, myTexture, 2, isMirrored);


            northWallMesh.MergeSides();

            wallFilter0.mesh = northWallMesh.Mesh;
            wallFilter1.mesh = northWallMesh.Mesh;
            wallFilter2.mesh = northWallMesh.Mesh;
            wallFilter3.mesh = northWallMesh.Mesh;

        }

        public void SetSideWallTextures(string texName, Texture2D myTexture, bool isMirrored)
        {
            wallTexName = texName;

            wallFilter0.mesh.Clear();
            wallFilter1.mesh.Clear();
            wallFilter2.mesh.Clear();
            wallFilter3.mesh.Clear();


            northWallMesh.UpdateSideTextures(wallRenderer0, myTexture, isMirrored);
            northWallMesh.UpdateSideTextures(wallRenderer1, myTexture, isMirrored);
            northWallMesh.UpdateSideTextures(wallRenderer2, myTexture, isMirrored);
            northWallMesh.UpdateSideTextures(wallRenderer3, myTexture, isMirrored);


            northWallMesh.MergeSides();

            wallFilter0.mesh = northWallMesh.Mesh;
            wallFilter1.mesh = northWallMesh.Mesh;
            wallFilter2.mesh = northWallMesh.Mesh;
            wallFilter3.mesh = northWallMesh.Mesh;

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