using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{ 
    public class Modeler : MonoBehaviour
    {
        GameObject[] Floors;

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
            Floors = new GameObject[TileMapper.areas.Count];
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

                var floorRender = floor.GetComponent<Renderer>();

                floor.name = "cube " + area.RoomIndex;

                floorRender.material.SetColor("_Color", colors[area.RoomIndex]);

                floor.transform.position = new Vector3(area.Floor.Center.x,1,area.Floor.Center.z);

                floor.transform.localScale = new Vector3(area.Floor.TrueWidth, 1, area.Floor.TrueHeight);

                /*foreach (Area.Wall wall in area.Walls)
                {
                    foreach(Area.Wall.Segment seg in wall.Segments)
                    {

                        GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);


                        segment.name = "Room " + area.RoomIndex + " " + wall.Orientation.Name + "Wall " + seg.Max; 

                        var segRender = segment.GetComponent<Renderer>();

                        segRender.material.SetColor("_Color", colors[area.RoomIndex]);
                        
                        segment.transform.position = new Vector3(seg.Center.x, 1, seg.Center.z);

                        //now for scaling to 3D based on wall orientation

                        if (wall.Orientation == Orient.North | wall.Orientation == Orient.South)
                        {
                            segment.transform.localScale = new Vector3(seg.Length, seg.Height, seg.Thickness);
                        }
                        else
                        {
                            segment.transform.localScale = new Vector3(seg.Thickness, seg.Height, seg.Length);
                        }

                    }
                    if (wall.Paths.Count != 0)
                    {
                        foreach (Area.Wall.Segment path in wall.Paths)
                        {
                            if(path.IsRemoved == false)
                            { 
                                GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);

                                segment.name = "Room " + area.RoomIndex + " " + wall.Orientation.Name + "Path " + path.Max;

                                var segRender = segment.GetComponent<Renderer>();

                                segRender.material.SetColor("_Color", colors[area.RoomIndex]);

                                segment.transform.position = new Vector3(path.Center.x, 1, path.Center.z);

                                //now for scaling to 3D based on wall orientation

                                segment.transform.localScale = new Vector3(path.Length, 1, path.Thickness);
   

                            }
                        }
                    }

                    if (wall.Corners.Count != 0)
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
                    }
                }*/
            }
        }
    }
}