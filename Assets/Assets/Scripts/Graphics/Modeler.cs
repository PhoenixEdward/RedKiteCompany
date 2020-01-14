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
            roomFloors = GameObject.Find("RoomFloor");
            wallTex = Resources.Load<Texture2D>("Tiles/GreenBamboo");
            level = GameObject.FindGameObjectWithTag("Level").transform;
            texture2Ds = new Texture2D[6] { topWallTex, wallTex, wallTex, wallTex, wallTex, wallTex };
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

                        GameObject segment = new GameObject();

                        MeshMaker segMesh = segment.AddComponent<MeshMaker>();

                        segMesh.MakeMesh(seg.Scale);

                        segMesh.SetTextures(texture2Ds);

                        segment.name = "Room " + area.RoomIndex + " " + wall.Orientation.Name + "Wall " + seg.Max;

                        segment.transform.parent = level;

                        Bounds segBounds = segMesh.mesh.bounds;

                        Vector3 offsetHorizontal = segment.transform.position - segment.transform.TransformPoint(segBounds.center);

                        segment.transform.localPosition = new Vector3(seg.Center.x, 1, seg.Center.z) + offsetHorizontal + Vector3.up - new Vector3(0,0,0.5f); 
                        

                    }
                    if (wall.Overlaps.Count != 0)
                    {
                        foreach (Segment path in wall.Overlaps.Where(x => x.IsCorner).ToList())
                        {
                            if (path.IsRemoved == false)
                            {
                                //skip over north and south corners to avoid duplication.
                                if(path.IsCorner & (path.Orientation == Orient.North | path.Orientation == Orient.South))
                                {
                                    continue;
                                }

                                GameObject segment = new GameObject();

                                MeshMaker segMesh = segment.AddComponent<MeshMaker>();

                                segMesh.MakeMesh(path.Scale);

                                segMesh.SetTextures(texture2Ds);

                                segment.transform.parent = level;

                                segment.name = "Room " + area.RoomIndex + " " + wall.Orientation.Name + "Corner " + path.Max;

                                Bounds segBounds = segMesh.mesh.bounds;

                                segment.transform.parent = level;

                                Vector3 offsetHorizontal = segment.transform.position - segment.transform.TransformPoint(segBounds.center);

                                segment.transform.localPosition = new Vector3(path.Center.x, 1, path.Center.z) + offsetHorizontal + Vector3.up;


                            }
                            else
                                Debug.Log(area.RoomIndex + " " + path.Orientation.Name + " " + path.Min + " " + path.Max);
                        }
                    }

                    /*if (wall.Corners.Count != 0)
                    {
                        foreach (Vector3 corner in wall.Corners)
                        {

                            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);

                            segment.name = "Room " + area.RoomIndex + " " + wall.Orientation.Name + "Corner " + corner.x;

                            var segRender = segment.GetComponent<Renderer>();

                            segRender.material.SetColor("_Color", colors[area.RoomIndex]);

                            segment.transform.position = new Vector3(corner.x, 1, corner.z);

                            //now for scaling to 3D based on wall orientation

                            segment.transform.localScale = new Vector3(1, 2, 1);


                        }
                    }*/

                }
            }
        }
    }
}