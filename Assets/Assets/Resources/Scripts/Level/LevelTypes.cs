using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RedKite
{
    public struct Orient
    {
        enum Side
        {
            North,
            South,
            East,
            West
        }

        public string Name;
        public Vector3 Forward { get; private set; }
        public Vector3 Back { get; private set; }
        public Vector3 Left { get; private set; }
        public Vector3 Right { get; private set; }
        public float foreUnit;
        public float rightUnit;

        //static members
        public static Orient North = new Orient(Side.North);
        public static Orient South = new Orient(Side.South);
        public static Orient East = new Orient(Side.East);
        public static Orient West = new Orient(Side.West);



        Orient(Side _side)
        {
            Forward = Vector3.up;
            Back = Vector3.down;
            Right = Vector3.right;
            Left = Vector3.left;

            if (_side == Side.North)
            {
                Name = "North";
                Forward = Vector3.up;
                Back = Vector3.down;
                Right = Vector3.right;
                Left = Vector3.left;
            }
            else if (_side == Side.South)
            {
                Name = "South";
                Forward = Vector3.down;
                Back = Vector3.up;
                Right = Vector3.left;
                Left = Vector3.right;
            }
            else if (_side == Side.West)
            {
                Name = "West";
                Forward = Vector3.left;
                Back = Vector3.right;
                Right = Vector3.up;
                Left = Vector3.down;
            }
            else
            {
                Name = "East";
                Forward = Vector3.right;
                Back = Vector3.left;
                Right = Vector3.down;
                Left = Vector3.up;
            }

            foreUnit = Forward.x == 0 ? Forward.y : Forward.x;
            rightUnit = Right.x == 0 ? Right.y : Right.x;

        }

        public static bool operator ==(Orient a, Orient b)
        {
            bool match;

            match = a.Forward == b.Forward & a.Back == b.Back & a.Left == b.Left & a.Right == b.Right;

            return match;
        }

        public static bool operator !=(Orient a, Orient b)
        {
            bool match;

            match = a.Forward != b.Forward & a.Back != b.Back & a.Left != b.Left & a.Right != b.Right;

            return match;
        }

    }
}
