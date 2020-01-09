using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;
using UnityEngine;

namespace RedKite
{
    public class Area
    {
        public DateTime creationTime = DateTime.Now;
        public int RoomIndex { get; private set; }
        public Stage Floor { get; private set; }
        public List<Wall> Walls;
        public List<int> ConnectedAreas = new List<int>();

        static int[,] WallGraph = new int[TileMapper.W, TileMapper.H];

        //to keep directions for the dropping and adding of floor dimensions.
        public Orient Orientation;

        //could be expanded to encompass the fact that there will be non-rectangular rooms

        //maybe change from string and wall to enum and wall? might read a little nicer.
        public Area(Orient _orient, int _roomIndex, Vector3 _startPoint, float _width, float _height)
        {
            Orientation = _orient;
            RoomIndex = _roomIndex;
            Floor = new Stage(_startPoint, _width, _height, _orient);
            Walls = new List<Wall>();
        }

        public Vector3[] GetCoords()
        {
            Vector3[][] allCoords = new Vector3[(int)Floor.height][];


            for (int i = 0; i < Floor.height; i++)
            {
                allCoords[i] = Utility.CoordRange(Floor.BottomLeft + (Orientation.Forward * i), Floor.BottomRight + (Orientation.Forward * i));
            }

            return allCoords.SelectMany(x => x).ToArray();

        }

        public Vector3[] GetWallCoords()
        {
            Vector3[][] allCoords = new Vector3[Walls.Count][];


            for (int i = 0; i < Walls.Count; i++)
            {
                allCoords[i] = Utility.CoordRange(Walls[i].Min, Walls[i].Max);
            }

            return allCoords.SelectMany(x => x).ToArray();
        }

        public void GenerateWalls()
        {

            Walls = new List<Wall>()
                {
                    new Wall(Orientation, Orient.North,Floor),
                    new Wall(Orientation, Orient.South,Floor),
                    new Wall(Orientation, Orient.West,Floor),
                    new Wall(Orientation, Orient.East,Floor),

                };

        }

        /*public void Resize(OCipher force)
        {
            Vector3 forceLeft = Orientation == Orient.North ? force.E * Orientation.Right : Orientation == Orient.East ? force.E * Orientation.Back
                : Orientation == Orient.South ? force.E * Orientation.Left : force.E * Orientation.Forward;
            Vector3 forceDown = Orientation == Orient.North ? force.N * Orientation.Back : Orientation == Orient.East ? force.N * Orientation.Right
                : Orientation == Orient.South ? force.N * Orientation.Forward : force.N * Orientation.Left;
            Vector3 forceRight = Orientation == Orient.North ? force.W * Orientation.Right : Orientation == Orient.East ? force.W * Orientation.Forward
                : Orientation == Orient.South ? force.W * Orientation.Left : force.W * Orientation.Back;
            Vector3 forceUp = Orientation == Orient.North ? force.S * Orientation.Forward : Orientation == Orient.East ? force.S * Orientation.Left
                : Orientation == Orient.South ? force.S * Orientation.Back : force.S * Orientation.Right;

            //first drop or add tiles
            Floor.BottomLeft -= forceRight + forceUp;
            Floor.BottomRight -= forceLeft + forceUp;
            Floor.TopRight -= forceLeft + forceDown;
            Floor.TopLeft -= forceRight + forceDown;


            Floor.width = Math.Abs(Floor.BottomRight.X - Floor.BottomLeft.X + 1);
            Floor.height = Math.Abs(Floor.TopLeft.Y - Floor.BottomLeft.Y + 1);

            //adjust walls

            for (int i = 0; i < Walls.Count; i++)
                if (Walls[i].Orientation == Orient.North)
                {
                    Walls[i].Min -= new Vector3(force.E, force.S, 0);
                    Walls[i].Max -= new Vector3(force.W, force.S,0);
                }
                else if (Walls[i].Orientation == Orient.East)
                {
                    Walls[i].Min -= new Vector3(force.S, force.W, 0);
                    Walls[i].Max -= new Vector3(force.N, force.W, 0);
                }
                else if (Walls[i].Orientation == Orient.South)
                {
                    Walls[i].Min -= new Vector3(force.E, force.N, 0);
                    Walls[i].Max -= new Vector3(force.W, force.N, 0);
                }
                else if (Walls[i].Orientation == Orient.West)
                {
                    Walls[i].Min -= new Vector3(force.S, force.E, 0);
                    Walls[i].Max -= new Vector3(force.N, force.E, 0);
                }
                */



        public class Stage
        {
            public Vector3 BottomLeft;
            public Vector3 TopRight;
            public Vector3 BottomRight;
            public Vector3 TopLeft;
            public float width;
            public float height;
            public Vector3 Center;
            public Vector3 TrueScale;
            public Vector3 TrueNE;
            public Vector3 TrueSW;

            public Stage(Vector3 _startPoint, float _width, float _height, Orient _orientation)
            {

                width = _width;
                height = _height;

                BottomLeft = _startPoint;
                TopLeft = _startPoint + ((_height - 1) * _orientation.Forward);
                TopRight = TopLeft + ((_width - 1) * _orientation.Right);
                BottomRight = _startPoint + ((_width - 1) * _orientation.Right);

                TrueSW = _orientation == Orient.North ? BottomLeft : _orientation == Orient.East ? BottomRight :
                    _orientation == Orient.South ? TopRight : TopLeft;

                TrueNE = _orientation == Orient.North ? TopRight : _orientation == Orient.East ? TopLeft :
                    _orientation == Orient.South ? BottomLeft : BottomRight;


                TrueScale = new Vector3(TrueNE.x - TrueSW.x + 1, 1, TrueNE.z - TrueSW.z + 1);

                Center = new Vector3((TrueSW.x + (TrueScale.x - 1) / 2), 1, (TrueSW.z + (TrueScale.z - 1) / 2));
            }
        }



        public class Wall
        {
            public Orient Orientation;
            public Vector3 Min;
            public Vector3 Max;
            public List<Segment> Overlaps = new List<Segment>();
            public List<Segment> Segments = new List<Segment>();
            public Vector3 Center;
            public float Thickness = 1;
            public float length;
            public float Height = 2;
            public Vector3 Scale;

            public Wall(Orient _roomOrientation, Orient _orientation, Stage _floor)
            {
                Orientation = _orientation;

                Vector3 cornerTopLeft = _floor.TopLeft + _roomOrientation.Left + _roomOrientation.Forward;
                Vector3 cornerBottomRight = _floor.BottomRight + _roomOrientation.Back + _roomOrientation.Right;


                Vector3 cornerBottomLeft = _floor.BottomLeft + _roomOrientation.Back + _roomOrientation.Left;
                Vector3 cornerTopRight = _floor.TopRight + _roomOrientation.Forward + _roomOrientation.Right;

                if (_roomOrientation == Orient.North)
                {
                    if (_orientation == Orient.North)
                    {

                        Min = cornerTopLeft;
                        Max = cornerTopRight;
                    }
                    else if (_orientation == Orient.South)
                    {

                        Min = cornerBottomLeft;
                        Max = cornerBottomRight;
                    }
                    else if (_orientation == Orient.West)
                    {

                        Min = cornerBottomLeft;
                        Max = cornerTopLeft;
                    }
                    else if (_orientation == Orient.East)
                    {

                        Min = cornerBottomRight;
                        Max = cornerTopRight;
                    }
                }

                if (_roomOrientation == Orient.South)
                {
                    if (_orientation == Orient.North)
                    {

                        Min = cornerBottomRight;
                        Max = cornerBottomLeft;
                    }
                    else if (_orientation == Orient.South)
                    {

                        Min = cornerTopRight;
                        Max = cornerTopLeft;
                    }
                    else if (_orientation == Orient.West)
                    {

                        Min = cornerTopRight;
                        Max = cornerBottomRight;
                    }
                    else if (_orientation == Orient.East)
                    {

                        Min = cornerTopLeft;
                        Max = cornerBottomLeft;
                    }
                }

                if (_roomOrientation == Orient.East)
                {
                    if (_orientation == Orient.North)
                    {

                        Min = cornerBottomLeft;
                        Max = cornerTopLeft;
                    }
                    else if (_orientation == Orient.South)
                    {

                        Min = cornerBottomRight;
                        Max = cornerTopRight;
                    }
                    else if (_orientation == Orient.West)
                    {

                        Min = cornerBottomRight;
                        Max = cornerBottomLeft;
                    }
                    else if (_orientation == Orient.East)
                    {

                        Min = cornerTopRight;
                        Max = cornerTopLeft;
                    }
                }

                if (_roomOrientation == Orient.West)
                {
                    if (_orientation == Orient.North)
                    {

                        Min = cornerTopRight;
                        Max = cornerBottomRight;
                    }
                    else if (_orientation == Orient.South)
                    {

                        Min = cornerTopLeft;
                        Max = cornerBottomLeft;
                    }
                    else if (_orientation == Orient.West)
                    {

                        Min = cornerTopLeft;
                        Max = cornerTopRight;
                    }
                    else if (_orientation == Orient.East)
                    {

                        Min = cornerBottomLeft;
                        Max = cornerBottomRight;
                    }
                }

                Center = Min + (Vector3.Scale((Max - Min) / 2, Orientation.Forward)) + ((Thickness / 2) * Orientation.Back);

                Segments = new List<Segment> { new Segment(Orientation, Min, Max, Height) };



                if (_orientation == Orient.North | _orientation == Orient.South)
                    Scale = new Vector3(Max.z - Min.z, Height, 1);
                else
                    Scale = new Vector3(Max.x - Min.x, Height, 1);

            }

            public void Split()
            {
                //establish relative up and down. Will probably move to control flow below when less tired.
                Vector3 up = Orientation == Orient.North | Orientation == Orient.South ? new Vector3(1, 0, 0) : new Vector3(0, 0, 1);
                Vector3 down = up == new Vector3(0, 0, 1) ? new Vector3(0, 0, -1) : new Vector3(-1, 0, 0);

                //every wall needs a minumum of one segment.
                List<Segment> segments = new List<Segment>();

                //this step is important for the look behind featured in the loop as order is important.
                if (Orientation == Orient.North | Orientation == Orient.South)
                    Overlaps.OrderBy(x => x.Min.x).ToList();
                else
                    Overlaps.OrderBy(x => x.Min.z).ToList();

                //note <= for look behind. This also means I no longer have to have a seperate structure for those with 0 paths.
                for (int i = 0; i <= Overlaps.Count; i++)
                {
                    //instantiate start and end of segment
                    Vector3 segMax;
                    Vector3 segMin;

                    //if there are no paths then we start from the extremities of the walls minus the corners.
                    if(Overlaps.Count == 0)
                    {
                        segMin = Min + up;
                        segMax = Max + down;
                    }
                    //if this is our first iteration and the count is more than 0 as it was not triggered above, then
                    //we start by using the min as our beginning abd the min of the current path as our max (minus one to not encroach)
                    else if(i == 0)
                    {
                        segMin = Min + up;
                        if (Vector3.Distance(Overlaps[i].Min , Min) < 2)
                            continue;
                        else
                            segMax = Overlaps[i].Min + down;
                    }
                    //if we are at the end of the loop we cannot look to the current iteration. We use the last path in the list, which is
                    //the previous iteration, and compare it to the max. We only accept a distance greater than 1 so that our minimum segment length is 1.
                    else if (i == Overlaps.Count)
                    {
                        segMin = Overlaps[i - 1].Max + up;
                        if (Vector3.Distance(Overlaps[i - 1].Max, Max) < 2)
                            continue;
                        else
                            segMax = Max + down;
                    }
                    //finally we have our standard in between case. We ensure a minimum length of 1 then use the tile after the previous path max and 
                    //compare it to the tile before the current path min.
                    else
                    {
                        if (Vector3.Distance(Overlaps[i - 1].Max, Overlaps[i].Min) < 2)
                            continue;
                        else
                        { 
                            segMin = Overlaps[i - 1].Max + up;
                            segMax = Overlaps[i].Min + down;
                        }

                    }

                    bool segMaxComplete = false;
                    bool segMinComplete = false;

                    //loop half the length of the distance between points plus half a unit to account for odd numbers
                    for (int j = 0; j < Vector3.Distance(segMin, segMax)/2 + .51f; j++)
                    {
                        if (segMin == segMax)
                            break;

                        if (WallGraph[(int)segMin.x, (int)segMin.z] == 0 & segMinComplete == false)
                        {
                            WallGraph[(int)segMin.x, (int)segMin.z] += 1;
                            segMinComplete = true;
                        }
                        else if (segMinComplete == false)
                        {
                             segMin += up;
                        }

                        //keep max and min from crossing.
                        if (segMin == segMax)
                        {
                            //check if valid
                            if(WallGraph[(int)segMin.x,(int)segMin.y] == 0)
                            {
                                WallGraph[(int)segMin.x, (int)segMin.y] += 1;
                                segMinComplete = true;
                                segMaxComplete = true;
                                    
                            }
                            break;
                        }

                        if (WallGraph[(int)segMax.x, (int)segMax.z] == 0 & segMaxComplete == false)
                        {
                            WallGraph[(int)segMax.x, (int)segMax.z] += 1;
                            segMaxComplete = true;
                        }
                        else if (segMaxComplete == false)
                        {
                            segMax += down;
                        }

                    }

                    //if both sides have found a point then we add a segment. else we skip. else really only there for clarity.
                    if (segMinComplete == true & segMaxComplete == true)
                        segments.Add(new Segment(Orientation, segMin, segMax, Height));
                    else
                        continue;
                }

                Segments = segments;
                
            }
        }
    }
}