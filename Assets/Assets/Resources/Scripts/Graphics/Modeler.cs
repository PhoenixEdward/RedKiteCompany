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
        Texture2D[] texture2Ds;
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

        // Start is called before the first frame update
        public void Start()
        {

            floorTex = Resources.Load<Texture2D>("Tiles/WoodFloor");
            topWallTex = Resources.Load<Texture2D>("Tiles/BambooFloor");
            wallTex = Resources.Load<Texture2D>("Tiles/jFlower");
            texture2Ds = new Texture2D[6] { wallTex, wallTex, topWallTex, wallTex, wallTex, wallTex };

            walls = new GameObject();
            walls.transform.position = new Vector3(.35f, 0, .35f);

            floor = new GameObject();
            floor.transform.position = new Vector3(.35f, 0, .35f);

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
            foreach(KeyValuePair<int,Area> entry in TileMapper.Instance.areas)
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

            wallMesh.SetTextures(meshRenderer, texture2Ds, new bool[] { false, false, false, false, false, false });

            wallMesh.MergeSides();

            meshFilter.mesh = wallMesh.mesh;
        }


        void RenderFloor(MeshFilter meshFilter, MeshRenderer meshRenderer)
        {
            foreach (KeyValuePair<int, Area> entry in TileMapper.Instance.areas)
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

            floorMesh = MeshMaker.CombinePlanes(floorMeshes);

            floorMesh.SetTextures(meshRenderer, new Texture2D[] { floorTex, floorTex, floorTex, floorTex, floorTex, floorTex }, new bool[] { false, false, false, false, false, false });

            floorMesh.MergeSides();

            meshFilter.mesh = floorMesh.mesh;
        }

        public void SetTopWallTexture(Texture2D myTexture, bool isMirrored)
        {
                wallFilter.mesh.Clear();

                wallMesh.UpdateOneTexture(wallRenderer, myTexture, 2, isMirrored);

                wallMesh.MergeSides();

                wallFilter.mesh = wallMesh.mesh;
        }

        public void SetSideWallTextures(Texture2D myTexture, bool isMirrored)
        {
                wallFilter.mesh.Clear();

                wallMesh.UpdateSideTextures(wallRenderer, myTexture, isMirrored);

                wallMesh.MergeSides();

                wallFilter.mesh = wallMesh.mesh;
        }
        public void SetFloorTexture(Texture2D myTexture, bool isMirrored)
        {
            floorFilter.mesh.Clear();

            floorMesh.SetTextures(floorRenderer, new Texture2D[] { myTexture, myTexture, myTexture, myTexture, myTexture, myTexture },
                new bool[] { isMirrored, isMirrored, isMirrored, isMirrored, isMirrored, isMirrored });

            floorMesh.MergeSides();

            floorFilter.mesh = floorMesh.mesh;
        }
    }
}