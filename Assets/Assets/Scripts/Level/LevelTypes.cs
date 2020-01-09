using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RedKite
{
    public struct OCipher
    {
        public float N;
        public float E;
        public float S;
        public float W;

        public OCipher(int n, int e, int s, int w)
        {
            N = n;
            E = e;
            S = s;
            W = w;
        }

        public static OCipher Zero = new OCipher(0, 0, 0, 0);

        public static OCipher operator +(OCipher a) => a;
        public static OCipher operator -(OCipher a) => -a;
        public OCipher this[int key]
        {
            get => this[key];
            set => this[key] = value;
        }

        public static OCipher operator +(OCipher a, OCipher b)
        {
            a.N += b.N;
            a.E += b.E;
            a.S += b.S;
            a.W += b.W;

            return a;
        }
        public static OCipher operator -(OCipher a, OCipher b)
        {
            a.N -= b.N;
            a.E -= b.E;
            a.S -= b.S;
            a.W -= b.W;

            return a;
        }

        public static OCipher operator +(OCipher a, Vector3 v)
        {
            if (v.x < 0)
                a.E += v.x;
            else if (v.x > 0)
                a.W += v.x;

            if (v.z < 0)
                a.S += v.z;
            else if (v.z > 0)
                a.N += v.z;

            return a;
        }

        public static OCipher operator -(OCipher a, Vector3 v)
        {
            if (v.x < 0)
                a.E -= v.x;
            else if (v.x > 0)
                a.W -= v.x;

            if (v.z < 0)
                a.S -= v.z;
            else if (v.z > 0)
                a.N -= v.z;

            return a;
        }

    }
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
            Forward = Vector3.forward;
            Back = Vector3.back;
            Right = Vector3.right;
            Left = Vector3.left;

            if (_side == Side.North)
            {
                Name = "North";
                Forward = Vector3.forward;
                Back = Vector3.back;
                Right = Vector3.right;
                Left = Vector3.left;
            }
            else if (_side == Side.South)
            {
                Name = "South";
                Forward = Vector3.back;
                Back = Vector3.forward;
                Right = Vector3.left;
                Left = Vector3.right;
            }
            else if (_side == Side.West)
            {
                Name = "West";
                Forward = Vector3.left;
                Back = Vector3.right;
                Right = Vector3.forward;
                Left = Vector3.back;
            }
            else
            {
                Name = "East";
                Forward = Vector3.right;
                Back = Vector3.left;
                Right = Vector3.back;
                Left = Vector3.forward;
            }

            foreUnit = Forward.x == 0 ? Forward.z : Forward.x;
            rightUnit = Right.x == 0 ? Right.z : Right.x;

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
