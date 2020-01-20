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
                    new Wall(RoomIndex, Orientation, Orient.North,Floor),
                    new Wall(RoomIndex, Orientation, Orient.South,Floor),
                    new Wall(RoomIndex, Orientation, Orient.West,Floor),
                    new Wall(RoomIndex, Orientation, Orient.East,Floor),

                };

        }

 
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
            public Vector3 TrueNW;
            public Vector3 TrueSE;

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

                TrueNW = _orientation == Orient.North ? TopLeft : _orientation == Orient.East ? BottomLeft :
                    _orientation == Orient.South ? BottomRight : TopRight;

                TrueSE = _orientation == Orient.North ? BottomRight : _orientation == Orient.East ? TopRight :
                    _orientation == Orient.South ? TopLeft : BottomLeft;

                TrueScale = new Vector3(TrueNE.x - TrueSW.x + 1, 1, TrueNE.z - TrueSW.z + 1);

                Center = new Vector3((TrueSW.x + (TrueScale.x - 1) / 2), 1, (TrueSW.z + (TrueScale.z - 1) / 2));
            }
        }

        public struct Door
        {
            public Vector3 Min;
            public Vector3 Max;
            bool IsRemoved;

            public Door(Vector3 _min, Vector3 _max, bool _isRemoved)
            {
                Min = _min;
                Max = _max;
                IsRemoved = _isRemoved;
            }
        }

        public class Wall
        {
            public int RoomIndex;
            public Orient Orientation;
            public Vector3 Min;
            public Vector3 Max;
            public List<Wall> ConnectedWalls = new List<Wall>();
            public List<Segment> Overlaps = new List<Segment>();
            public List<Segment> Segments = new List<Segment>();
            public Vector3 Center;
            public float Thickness = 1;
            public float length;
            public float Height = 3;
            public Vector3 Scale;
            public Dictionary<int, Door> Doors = new Dictionary<int, Door>();

            public Wall(int _roomIndex,Orient _roomOrientation, Orient _orientation, Stage _floor)
            {
                RoomIndex = _roomIndex;

                Orientation = _orientation;

                Vector3 cornerTopLeft = _floor.TrueNW + Vector3.forward + Vector3.left;
                Vector3 cornerBottomRight = _floor.TrueSE + Vector3.back + Vector3.right;


                Vector3 cornerBottomLeft = _floor.TrueSW + Vector3.back + Vector3.left;
                Vector3 cornerTopRight = _floor.TrueNE + Vector3.forward + Vector3.right;

                if(Orientation == Orient.North)
                {
                    Max = cornerTopRight;
                    Min = cornerTopLeft;
                }
                else if(Orientation == Orient.South)
                {
                    Max = cornerBottomRight;
                    Min = cornerBottomLeft;
                }
                else if(Orientation == Orient.East)
                {
                    Max = cornerTopRight;
                    Min = cornerBottomRight;
                }
                else
                {
                    Max = cornerTopLeft;
                    Min = cornerBottomLeft;
                }


                Center = Min + (Vector3.Scale((Max - Min) / 2, Orientation.Forward)) + ((Thickness / 2) * Orientation.Back);

                if (_orientation == Orient.North | _orientation == Orient.South)
                    Scale = new Vector3(Max.z - Min.z, Height, 1);
                else
                    Scale = new Vector3(Max.x - Min.x, Height, 1);

            }

            public void FindOverlaps()
            {
                Wall newWall = this;

                Vector3 up = newWall.Orientation == Orient.North | newWall.Orientation == Orient.South ? new Vector3(1, 0, 0) : new Vector3(0, 0, 1);
                Vector3 down = up == new Vector3(0, 0, 1) ? new Vector3(0, 0, -1) : new Vector3(-1, 0, 0);

                //remove corners on connected walls
                foreach (Wall oldWall in ConnectedWalls)
                {
                    if (Utility.DirectedDist(newWall.Min, oldWall.Min) <= 0 & Utility.DirectedDist(newWall.Max, oldWall.Min) >= 0)
                    {
                        newWall.Overlaps.Add(new Segment(newWall.Orientation, oldWall.Min, oldWall.Min, 2, _isRemoved: true , _isCorner: true));
                    }
                    if (Utility.DirectedDist(oldWall.Min, newWall.Min) <= 0 & Utility.DirectedDist(oldWall.Max, newWall.Min) >= 0)
                    {
                        oldWall.Overlaps.Add(new Segment(oldWall.Orientation, newWall.Min, newWall.Min, 2, _isRemoved: true, _isCorner: true));
                    }
                    if (Utility.DirectedDist(newWall.Max, oldWall.Max) >= 0 & (Utility.DirectedDist(newWall.Min, oldWall.Max) <= 0))
                    {
                        newWall.Overlaps.Add(new Segment(newWall.Orientation, oldWall.Max, oldWall.Max, 2, _isRemoved: true, _isCorner: true));
                    }
                    if (Utility.DirectedDist(oldWall.Max, newWall.Max) >= 0 & (Utility.DirectedDist(oldWall.Min, newWall.Max)<= 0))
                    {
                        oldWall.Overlaps.Add(new Segment(oldWall.Orientation, newWall.Max, newWall.Max, 2, _isRemoved: true, _isCorner: true));
                    }


                    //find maxiest min
                    float diff2 = Utility.DirectedDist(newWall.Min, oldWall.Min);
                    Vector3 startCorner = diff2 <= 0 ? oldWall.Min :
                        newWall.Min;

                    Vector3 startCoord = startCorner + up;


                    //find minniest max
                    float diff3 = Utility.DirectedDist(newWall.Max, oldWall.Max);
                    Vector3 endCorner = diff3 >= 0 ? oldWall.Max :
                        newWall.Max;

                    Vector3 endCoord = endCorner + down;

                    //add paths for both. One is declared "removed" to prevent overlapping of tiles

                    oldWall.Overlaps.Add(new Segment(oldWall.Orientation, startCoord, endCoord, 1, _isRemoved: true));

                    if (Doors.Keys.Contains(oldWall.RoomIndex))
                    {
                       Door door = Doors[oldWall.RoomIndex];
                       newWall.Overlaps.Add(new Segment(newWall.Orientation,door.Min,door.Max, 1, _isPath: true));
                    }
                }
            }

            public void Split()
            {
                //establish relative up and down. Will probably move to control flow below when less tired.
                Vector3 up = Orientation == Orient.North | Orientation == Orient.South ? new Vector3(1, 0, 0) : new Vector3(0, 0, 1);
                Vector3 down = up == new Vector3(0, 0, 1) ? new Vector3(0, 0, -1) : new Vector3(-1, 0, 0);

                //every wall needs a minumum of one segment.
                List<Segment> segments = new List<Segment>();

                Overlaps.Add(new Segment(Orientation, Min, Min, Height, _isCorner: true));
                Overlaps.Add(new Segment(Orientation, Max, Max, Height, _isCorner: true));

                //this step is important for the look behind featured in the loop as order is important.
                if (Orientation == Orient.North | Orientation == Orient.South)
                    Overlaps = Overlaps.OrderBy(x => x.Min.x).ToList();
                else
                    Overlaps = Overlaps.OrderBy(x => x.Min.z).ToList();

                Overlaps = Overlaps.DistinctBy(x => new { x.Min, x.Max }).ToList();

                    //note <= for look behind. This also means I no longer have to have a seperate structure for those with 0 paths.
                    for (int i = 0; i < Overlaps.Count - 1; i++)
                    {
                        //instantiate start and end of segment
                        Vector3 segMax;
                        Vector3 segMin;

 
                    //finally we have our standard in between case. We ensure a minimum length of 1 then use the tile after the previous path max and 
                    //compare it to the tile before the current path min.

                    //Debug.Log("Room:" + RoomIndex + "Wall" + Orientation.Name + " " + Overlaps[i].Max + up + " " + Overlaps[i + 1].Max + down);

                        if (Utility.DirectedDist(Overlaps[i].Max + up, Overlaps[i + 1].Min + down) > 0)
                            continue;
                        else
                        {
                            segMin = Overlaps[i].Max + up;
                            segMax = Overlaps[i + 1].Max + down;
                        }

                     segments.Add(new Segment(Orientation, segMin, segMax, Height));
                    }

                Segments = segments;
                
            }
        }
    }
}