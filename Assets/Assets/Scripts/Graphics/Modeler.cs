using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace RedKite
{ 
    public class Modeler : MonoBehaviour
    {
        static Transform level;
        Texture2D wallTex;
        Texture2D topWallTex;
        GameObject roomFloors;
        Texture2D[] texture2Ds;
        List<MeshMaker> wallMeshes = new List<MeshMaker>();
        MeshFilter meshFilter;
        MeshRenderer meshRender;

        Color[] colors = new Color[10]
        {
            Colors.AmericanBlue,
            Colors.AmericanGreen,
            Colors.AmericanViolet,
            Colors.AmericanRed,
            Colors.AmericanOrange,
            Colors.AmericanYellow,
            Colors.AmericanBrown,
            Colors.AmericanPink,
            Colors.AmericanSilver,
            Colors.AmericanPurple,

        };


        // Start is called before the first frame update
        void Start()
        {
            topWallTex = Resources.Load<Texture2D>("Tiles/BambooFloor");
            wallTex = Resources.Load<Texture2D>("Tiles/jFlower");
            texture2Ds = new Texture2D[6] { wallTex, wallTex, topWallTex, wallTex, wallTex, wallTex };

            meshFilter = GetComponent<MeshFilter>();

            meshRender = GetComponent<MeshRenderer>();

            RenderLevel();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void RenderLevel()
        {
            foreach(KeyValuePair<int,Area> entry in TileMapper.areas)
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
                                if (path.IsCorner & (path.Orientation == Orient.North | path.Orientation == Orient.South))
                                {
                                    continue;
                                }

                                MeshMaker cornerMesh = new MeshMaker();

                                cornerMesh.NewMakeMesh(path.Scale, path.Center);

                                wallMeshes.Add(cornerMesh);
                            }
                            else
                                Debug.Log(area.RoomIndex + " " + path.Orientation.Name + " " + path.Min + " " + path.Max);
                        }
                    }
                }
            }

            MeshMaker walls = MeshMaker.CombinePlanes(wallMeshes);

            walls.SetTextures(meshRender, texture2Ds, new bool[] { false, false, false, false, false, false });

            walls.MergeSides();

            meshFilter.mesh = walls.mesh;
        }
    }
}