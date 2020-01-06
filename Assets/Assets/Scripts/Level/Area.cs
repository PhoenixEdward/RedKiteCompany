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
            public Vector3 TrueSW;
            public Vector3 TrueNE;
            public float TrueWidth;
            public float TrueHeight;

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

                TrueWidth = TrueNE.x - TrueSW.x + 1;
                TrueHeight = TrueNE.y - TrueSW.y + 1;

                Center = new Vector3((TrueSW.x + (TrueWidth - 1) / 2), (TrueSW.y + (TrueHeight - 1) / 2), 1);
            }
        }



        public class Wall
        {
            public Orient Orientation;
            public Vector3 Min;
            public Vector3 Max;
            public List<Segment> Paths = new List<Segment>();
            public List<Segment> Segments = new List<Segment>();
            public Vector3 Center;
            public float Thickness = 1;
            public float length;
            public float Height = 2;

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

                Center = Min + (Vector3.Scale((Max - Min) / 2, Orientation.Forward)) + ((Thickness/2) * Orientation.Back);

                if (_orientation == Orient.North | _orientation == Orient.South)
                    length = Mathf.Abs((Max - Min).y);
                else
                    length = Mathf.Abs((Max - Min).x);

                Segments = new List<Segment> { new Segment(Orientation, Min, Max, Height)};

            }
            //will be used to segment walls after hallways have been placed. Everything will be cardinalized 
            //so it shouldn't be looped over twice.
            public void Split()
            {
                List<Segment> newSegments = new List<Segment>();

                List<Vector3> wallRange;

                if(Orientation == Orient.North | Orientation == Orient.South)
                    wallRange = Utility.CoordRange(Min + Vector3.up, Max + Vector3.down).ToList();
                else
                    wallRange = Utility.CoordRange(Min, Max).ToList();

                for (int i = 0; i < Paths.Count; i++)
                {

                    Vector3 first = Paths[i].Min;
                    Vector3 last = Paths[i].Max;

                    if (Orientation == Orient.North | Orientation == Orient.South)
                        wallRange.OrderBy(x => x.x);
                    else
                        wallRange.OrderBy(x => x.y);

                    int firstIndex = wallRange.FindIndex(x => x == first);
                    int lastIndex = wallRange.FindIndex(x => x == last);


                    if (i == 0 & Paths.Count == 1)
                    {
                        if (first != Min & Max != last)
                        {
                            newSegments.Add(new Segment(Orientation, Min, first, Height));
                            newSegments.Add(new Segment(Orientation, last, Max, Height));
                        }
                        else
                            continue;
                    }

                    else if (i == 0 & Paths.Count > 1)
                    {
                        newSegments.Add(new Segment(Orientation,Min,first, Height));
                    }

                    else if (i < Paths.Count - 1)
                    {

                        Vector3 nextFirst = Paths[i + 1].Min;
                        int nextFirstIndex = wallRange.FindIndex(x => x == last);

                        Vector3 start = nextFirstIndex > lastIndex ? last : first;
                        int startIndex = nextFirstIndex > lastIndex ? lastIndex : firstIndex;

                        if (nextFirstIndex > startIndex)
                        {
                            newSegments.Add(new Segment(Orientation, start, nextFirst, Height));

                        }
                        else
                        {
                            newSegments.Add(new Segment(Orientation, nextFirst, start, Height));
                        }

                    }
                    else
                    {
                        if(last != Max)
                            newSegments.Add(new Segment(Orientation, last,Max, Height));
                    }

                }

                if (newSegments.Count > 0)
                    Segments = newSegments;

            }

            public struct Segment
            {
                public bool IsPath;
                public bool IsRemoved;
                public Orient Orientation;
                public Vector3 Min;
                public Vector3 Max;
                public Vector3 Center;
                public float Thickness;
                public float Length;
                public float Height;

                public Segment(Orient _orientation, Vector3 _min, Vector3 _max, float _height,bool _isPath = false , bool _isRemoved  = false)
                {
                    IsPath = _isPath;
                    IsRemoved = _isRemoved;
                    Orientation = _orientation;

                    Min = _min;
                    Max = _max;

                    if(_orientation == Orient.North | _orientation == Orient.South)
                    {
                        Thickness = 1;
                        Length = Max.x - Min.x + 1;
                        Center = Min + Vector3.Scale(((Max - Min) / 2), Vector3.right);
                    }

                    else
                    {
                        Thickness = 1;
                        Length = Max.y - Min.y + 1;
                        Center = Min + Vector3.Scale(((Max - Min) / 2), Vector3.up);
                    }
                    Height = _height;


                }
            }

        }

    }
}