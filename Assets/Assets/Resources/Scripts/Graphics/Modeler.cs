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
        Texture2D sideFloorTex;
        Texture2D[] wallTextures;
        Texture2D[] floorTextures;
        List<MeshMaker> wallMeshes = new List<MeshMaker>();
        List<MeshMaker> floorMeshes = new List<MeshMaker>();
        List<MeshMaker> oobMeshes = new List<MeshMaker>();
        GameObject walls;
        GameObject floor;
        GameObject OOB;

        MeshFilter wallFilter;
        MeshFilter floorFilter;
        MeshFilter oobFilter;

        MeshRenderer wallRenderer;
        MeshRenderer floorRenderer;
        MeshRenderer oobRenderer;

        MeshMaker wallMesh;
        MeshMaker floorMesh;
        MeshMaker oobMesh;

        public string floorTexName;
        public string topWallTexName;
        public string wallTexName;
        public string sideFloorTexName;

        // Start is called before the first frame update
        public void Start()
        {
            floorTexName = floorTexName ?? "Snow";
            topWallTexName = topWallTexName ?? "Snow";
            wallTexName = wallTexName ?? "SnowDirt";
            sideFloorTexName = sideFloorTexName ?? "Dirt";

            floorTex = Resources.Load<Texture2D>("Tiles/" + floorTexName);
            topWallTex = Resources.Load<Texture2D>("Tiles/" + topWallTexName);
            wallTex = Resources.Load<Texture2D>("Tiles/" + wallTexName);
            sideFloorTex = Resources.Load<Texture2D>("Tiles/" + sideFloorTexName);
            wallTextures = new Texture2D[6] { wallTex, wallTex, topWallTex, wallTex, wallTex, wallTex };
            floorTextures = new Texture2D[6] { sideFloorTex, sideFloorTex, floorTex, sideFloorTex, sideFloorTex, sideFloorTex };

            walls = new GameObject();
            walls.name = "Walls";
            walls.layer = 9;
            walls.transform.position = new Vector3(0.5f, 0, .5f);

            floor = new GameObject();
            floor.name = "Floor";
            floor.transform.position = new Vector3(0.5f, 0, 0.5f);

            OOB = new GameObject();
            OOB.name = "OOB";
            OOB.transform.position = new Vector3(0.5f, 0, 0.5f);

            wallFilter = walls.AddComponent<MeshFilter>();
            wallRenderer = walls.AddComponent<MeshRenderer>();

            //wallRenderer.material.shader = Shader.Find("Custom/Terrain");

            floorFilter = floor.AddComponent<MeshFilter>();
            floorRenderer = floor.AddComponent<MeshRenderer>();

            oobFilter = OOB.AddComponent<MeshFilter>();
            oobRenderer = OOB.AddComponent<MeshRenderer>();

            //floorRenderer.material.shader = Shader.Find("Custom/Terrain");

            RenderWalls(walls, wallFilter, wallRenderer);
            RenderFloor(floor, floorFilter, floorRenderer);
            RenderOOB(OOB, oobFilter, oobRenderer);


            walls.transform.SetParent(transform);
            floor.transform.SetParent(transform);
            OOB.transform.SetParent(transform);
        }

        public void Regen()
        {
            wallTextures = wallMesh.SubMeshTextures;
            floorTextures = floorMesh.SubMeshTextures;

            wallFilter.mesh.Clear();
            floorFilter.mesh.Clear();

            wallMeshes.Clear();
            floorMeshes.Clear();

            RenderWalls(walls, wallFilter, wallRenderer);
            RenderFloor(floor, floorFilter, floorRenderer);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void RenderWalls(GameObject go, MeshFilter meshFilter, MeshRenderer meshRenderer)
        {
            foreach (KeyValuePair<int, Area> entry in TileMapper.Instance.Areas)
            {
                Area area = entry.Value;

                foreach (Area.Wall wall in area.Walls)
                {
                    foreach (Segment seg in wall.Segments)
                    {
                        MeshMaker segMesh = new MeshMaker();

                        if (seg.Scale.x < 0 | seg.Scale.y < 0 | seg.Scale.z < 0)
                            continue;

                        segMesh.NewMakeMesh(new Vector3(seg.Scale.x, seg.Scale.z, seg.Scale.y), new Vector3(seg.Center.x, seg.Center.z, seg.Center.y) + Vector3.up);

                        wallMeshes.Add(segMesh);

                    }
                }
            }

            wallMesh = MeshMaker.CombinePlanes(wallMeshes);

            wallMesh.SetTextures(meshRenderer, wallTextures, new bool[] { false, false, false, false, false, false });

            wallMesh.MergeSides();

            meshFilter.mesh = wallMesh.Mesh;

            MeshCollider meshCollider;

            if (go.TryGetComponent<MeshCollider>(out meshCollider))
                meshCollider.sharedMesh = meshFilter.mesh;
            else
                go.AddComponent<MeshCollider>();
        }


        void RenderFloor(GameObject go, MeshFilter meshFilter, MeshRenderer meshRenderer)
        {
            foreach (KeyValuePair<int, Area> entry in TileMapper.Instance.Areas)
            {
                Area area = entry.Value;

                MeshMaker floorMesh = new MeshMaker();

                floorMesh.NewMakeMesh(new Vector3(area.Floor.TrueScale.x, area.Floor.TrueScale.z, area.Floor.TrueScale.y), new Vector3(area.Floor.Center.x, area.Floor.Center.z, area.Floor.Center.y));

                floorMeshes.Add(floorMesh);

                //should consider storing pathways somewhere else. In walls seems a little bizarre.
                foreach (Area.Wall wall in area.Walls)
                {
                    if (wall.Overlaps.Count != 0)
                    {
                        foreach (Segment path in wall.Overlaps.Where(x => x.IsPath == true & x.IsRemoved == false).ToList())
                        {

                            MeshMaker pathMesh = new MeshMaker();

                            pathMesh.NewMakeMesh(new Vector3(path.Scale.x, path.Scale.z, path.Scale.y), new Vector3(path.Center.x, path.Center.z, path.Center.y));

                            floorMeshes.Add(pathMesh);
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

                        segMesh.NewMakeMesh(new Vector3(seg.Scale.x, seg.Scale.z, seg.Scale.y), new Vector3(seg.Center.x, seg.Center.z, seg.Center.y));

                        floorMeshes.Add(segMesh);


                    }
                }
            }

            floorMesh = MeshMaker.CombinePlanes(floorMeshes);

            floorMesh.SetTextures(meshRenderer, floorTextures, new bool[] { false, false, false, false, false, false });

            floorMesh.MergeSides();

            meshFilter.mesh = floorMesh.Mesh;

            MeshCollider meshCollider;

            if (go.TryGetComponent<MeshCollider>(out meshCollider))
                meshCollider.sharedMesh = meshFilter.mesh;
            else
                go.AddComponent<MeshCollider>();
        }

        void RenderOOB(GameObject go, MeshFilter meshFilter, MeshRenderer meshRenderer)
        {
            Cell[,] map = TileMapper.Instance.Tiles;
            
            /*
            List<Vector4> endPoints = new List<Vector4>();

            int x = 0;
            int y = 0;

            int firstX = x;
            int firstY = y;

            //-1 so that they double as a bool
            int lastY;
            int lastX = -1;


            while (y < TileMapper.Instance.H)
            {
                while (x < TileMapper.Instance.W)
                {
                    if (map[x, y].TileType != Cell.Type.Empty)
                    {
                        if (lastX < 0)
                            lastX = x - 1;
                        if (x < lastX + 1)
                        {
                            lastY = y - 1;

                            endPoints.Add(new Vector4(firstX, firstY, lastX, lastY));

                            while(map[x, y].TileType != Cell.Type.Empty)
                            {
                                if(x >= TileMapper.Instance.W)
                                {
                                    x = 0;
                                    y++;

                                    if (y >= TileMapper.Instance.H)
                                        break;
                                }
                                else
                                {
                                    x++;
                                }
                            }

                            firstX = x;
                            firstY = y;

                            //-1 so that they double as a bool
                            lastX = -1;
                        }
                    }

                    x++;
                }

                if (lastX < 0)
                    lastX = x;

                if(y >= TileMapper.Instance.H - 1)
                {
                    lastY = y;

                    endPoints.Add(new Vector4(firstX, firstY, lastX, lastY));
                }

                y++;
                x = 0;
            }

            foreach(Vector4 points in endPoints)
            {
                Debug.Log(points.x + " " + points.y + " "  + points.z + " " + points.w);

                Debug.Log("Blahh");

                int scaleX = ((int)points.z - (int)points.x) + 1;
                int scaleZ = ((int)points.w - (int)points.y) + 1;

                float posX = points.x + ((scaleX  - 1)/ 2);
                float posZ = points.y + ((scaleZ - 1 )/ 2);

                MeshMaker mesh = new MeshMaker();

                mesh.NewMakeMesh(new Vector3(scaleX, 1, scaleZ), new Vector3(posX, 1, posZ));

                oobMeshes.Add(mesh);
            }*/

            for(int y = 0; y < map.GetLength(1); y++)
            {
                for(int x = 0; x < map.GetLength(0); x++)
                {
                    if(map[x,y].TileType == Cell.Type.Empty)
                    {
                        if (x == 0)
                        {
                            MeshMaker sideMesh = new MeshMaker();

                            sideMesh.NewMakeMesh(new Vector3(1, 1, 1), new Vector3(x, 1, y));

                            oobMeshes.Add(sideMesh);

                        }
                        else if (x == map.GetLength(0) - 1)
                        {
                            MeshMaker sideMesh = new MeshMaker();

                            sideMesh.NewMakeMesh(new Vector3(1, 1, 1), new Vector3(x, 1, y));

                            oobMeshes.Add(sideMesh);

                        }

                        else if (y == 0)
                        {
                            MeshMaker sideMesh = new MeshMaker();

                            sideMesh.NewMakeMesh(new Vector3(1, 1, 1), new Vector3(x, 1, y));

                            oobMeshes.Add(sideMesh);
                        }
                        else if (y == map.GetLength(1) - 1)
                        {
                            MeshMaker sideMesh = new MeshMaker();

                            sideMesh.NewMakeMesh(new Vector3(1, 1, 1), new Vector3(x, 1, y));

                            oobMeshes.Add(sideMesh);
                        }
                        else { 
                            MeshMaker topMesh = new MeshMaker();

                            topMesh.NewMakeMesh(new Vector3(1, 0, 1), new Vector3(x, 1.5f, y));

                            oobMeshes.Add(topMesh);

                        }

                    }

                    if (x == 0 & y == 0 & map[x,y].TileType == Cell.Type.OccupiedAlly)
                    {
                        MeshMaker sideMesh = new MeshMaker();

                        sideMesh.NewMakeMesh(new Vector3(1, 1, 1), new Vector3(x, 1, y));

                        oobMeshes.Add(sideMesh);
                    }
                }
            }

            oobMesh = MeshMaker.CombinePlanes(oobMeshes);

            oobMesh.SetTextures(meshRenderer, floorTextures, new bool[] { false, false, false, false, false, false });

            oobMesh.MergeSides();

            meshFilter.mesh = oobMesh.Mesh;

            MeshCollider meshCollider;

            if (go.TryGetComponent(out meshCollider))
                meshCollider.sharedMesh = meshFilter.mesh;
            else
                go.AddComponent<MeshCollider>();

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