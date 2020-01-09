using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RedKite
{
    public class Segment : MonoBehaviour
    {
        public bool IsPath;
        public bool IsRemoved;
        public Orient Orientation;
        public Vector3 Min;
        public Vector3 Max;
        public Vector3 Center;
        public Vector3 Scale;
        public float Height;

        static int[,] space = new int[TileMapper.W,TileMapper.H];

        public Segment(Orient _orientation, Vector3 _min, Vector3 _max, float _height, bool _isPath = false, bool _isRemoved = false)
        {
            IsPath = _isPath;
            IsRemoved = _isRemoved;
            Orientation = _orientation;

            Min = _min;
            Max = _max;


            if (_orientation == Orient.North | _orientation == Orient.South)
            {
                Scale = new Vector3(Max.x - Min.x + 1, _height, 1);
                Center = Min + Vector3.Scale(((Max - Min) / 2), Vector3.right);
            }
            else
            {
                Scale = new Vector3(1, _height, Max.z - Min.z + 1);
                Center = Min + Vector3.Scale(((Max - Min) / 2), Vector3.forward);
            }
            Height = _height;

        }

        void OnCollisionEnter(Collision collision)
        {

            Debug.Log("Ran");

            foreach (ContactPoint contact in collision.contacts)
            {
                if (transform.localScale.z > transform.localScale.x)
                {
                    if (transform.position.x > contact.otherCollider.transform.position.x)
                    {

                        transform.localScale -= Vector3.right;
                        transform.position -= new Vector3(.5f, 0, 0);
                    }
                    else
                    {

                        transform.localScale -= Vector3.left;
                        transform.position += new Vector3(.5f, 0, 0);
                        
                    }

                }
                if (transform.localScale.z < transform.localScale.x)
                {
                    if (transform.position.z > contact.otherCollider.transform.position.z)
                    {

                        transform.localScale -= Vector3.forward;
                        transform.position -= new Vector3(.5f, 0, 0);
                        
                    }
                    else
                    {
                        transform.localScale -= Vector3.back;
                        transform.position += new Vector3(.5f, 0, 0);
                    }
                }
            }
        }

        

        public void Instantiate(Segment segment)
        {
            IsPath = segment.IsPath;
            IsRemoved = segment.IsRemoved;
            Orientation = segment.Orientation;

            Min = segment.Min;
            Max = segment.Max;


            if (segment.Orientation == Orient.North | segment.Orientation == Orient.South)
            {
                Scale = new Vector3(Max.x - Min.x + 1, segment.Height, 1);
                Center = Min + Vector3.Scale(((Max - Min) / 2), Vector3.right);
            }
            else
            {
                Scale = new Vector3(1, segment.Height, Max.z - Min.z + 1);
                Center = Min + Vector3.Scale(((Max - Min) / 2), Vector3.forward);
            }
            Height = segment.Height;

            transform.localPosition = segment.Center;

            //now for scaling to 3D based on wall orientation

            transform.localScale = segment.Scale;



        }
    }
}
