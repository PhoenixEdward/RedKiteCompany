using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{ 
    public class Modeler : MonoBehaviour
    {
        static Transform level;

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
            level = GameObject.FindGameObjectWithTag("Level").transform;
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

                GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);

                floor.transform.parent = level;

                var floorRender = floor.GetComponent<Renderer>();

                floor.name = "cube " + area.RoomIndex;

                floorRender.material.SetColor("_Color", colors[area.RoomIndex]);

                floor.transform.localPosition = area.Floor.Center;

                floor.transform.localScale = area.Floor.TrueScale;



                foreach (Area.Wall wall in area.Walls)
                {
                    foreach(Segment seg in wall.Segments)
                    {

                        GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);

                        segment.transform.parent = level;

                        segment.name = "Room " + area.RoomIndex + " " + wall.Orientation.Name + "Wall " + seg.Max; 

                        var segRender = segment.GetComponent<Renderer>();

                        segRender.material.SetColor("_Color", colors[area.RoomIndex]);
                        

                        Segment segInstance = segment.AddComponent<Segment>();

                        segInstance.Instantiate(seg);

                    }
                    if (wall.Overlaps.Count != 0)
                    {
                        foreach (Segment path in wall.Overlaps)
                        {
                            if (path.IsRemoved == true)
                            {
                                GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);

                                segment.transform.parent = level;

                                segment.name = "Room " + area.RoomIndex + " " + wall.Orientation.Name + "Path " + path.Max;

                                var segRender = segment.GetComponent<Renderer>();

                                segRender.material.SetColor("_Color", colors[area.RoomIndex]);

                                segment.transform.localPosition = new Vector3(path.Center.x, 1, path.Center.z);

                                //now for scaling to 3D based on wall orientation

                                segment.transform.localScale = path.Scale;


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