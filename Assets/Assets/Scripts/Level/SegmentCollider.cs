using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{ 
    public class SegmentCollider : MonoBehaviour
    {
        public Area.Wall.Segment segment;
        public ContactPoint[] contacts = new ContactPoint[10];

        void Start()
        {
            Collider collider = gameObject.AddComponent<BoxCollider>();
            collider.transform.position = segment.Center;
            collider.transform.localScale = new Vector3(segment.Length, 1, segment.Thickness);
        }
        void OnCollisionEnter(Collision collision)
        {
            collision.GetContacts(contacts);

            foreach(ContactPoint contact in contacts)
            {
                Vector3 pointFrom = contact.otherCollider.transform.position;

                if(pointFrom.z != transform.position.z)
                {
                    segment.Length -= 1;
                }

            }
        }
    }
}