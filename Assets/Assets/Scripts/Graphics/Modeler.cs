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
            texture2Ds = new Texture2D[6] {wallTex, wallTex, wallTex, wallTex, wallTex, topWallTex };
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

                        segment.transform.parent = level;

                        segment.transform.localPosition = new Vector3(seg.Center.x, 1, seg.Center.z);

                        MeshMaker segMesh = segment.AddComponent<MeshMaker>();

                        //segMesh.Instantiate(seg.Scale);

                        segMesh.SetTextures(texture2Ds);

                        segment.name = "Room " + area.RoomIndex + " " + wall.Orientation.Name + "Wall " + seg.Max; 

                        /*
                        var segRender = segment.GetComponent<Renderer>();

                        segRender.material.mainTexture = wallTex;
                        //segRender.material.SetColor("_Color", colors[area.RoomIndex]);
                        segRender.material.mainTextureScale = new Vector3(seg.Scale.x, seg.Scale.z,seg.Scale.y);

                        Segment segInstance = segment.AddComponent<Segment>();

                        segInstance.Instantiate(seg);*/

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

                                GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);

                                segment.transform.parent = level;

                                segment.name = "Room " + area.RoomIndex + " " + wall.Orientation.Name + "Corner " + path.Max;

                                segment.transform.localPosition = new Vector3(path.Center.x, 1, path.Center.z);

                                //now for scaling to 3D based on wall orientation

                                segment.transform.localScale = path.Scale;


                                var segRender = segment.GetComponent<Renderer>();

                                segRender.material.mainTexture = wallTex;

                                segRender.material.mainTextureScale = new Vector3(path.Scale.x, path.Scale.z, path.Scale.y);
                                //segRender.material.SetColor("_Color", colors[area.RoomIndex]);



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