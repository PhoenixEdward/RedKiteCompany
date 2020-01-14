using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RedKite
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Coalesce : MonoBehaviour
    {
        Material floorMat;
        Texture2D floorTex;

        // Start is called before the first frame update
        void Start()
        {
            floorMat = Resources.Load<Material>("/Tiles/materials/WoodFloor");
            floorTex = Resources.Load<Texture2D>("Tiles/WoodFloor");
            RenderLevel();
            //GetColliders();
            //ombineMeshes();
            transform.position = new Vector3(0.35f, 0, 0.35f);
            //Renderer floorsRender = GetComponent<Renderer>();

            //floorsRender.material.mainTexture = floorTex;
            //floorsRender.material.mainTextureScale = new Vector3(transform.lossyScale.x, transform.lossyScale.z, transform.lossyScale.y);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        void RenderLevel()
        {
            foreach (KeyValuePair<int, Area> entry in TileMapper.areas)
            {
                Area area = entry.Value;

                GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);

                floor.transform.parent = transform;

                var floorRender = floor.GetComponent<Renderer>();

                floorRender.material.mainTexture = floorTex;

                floor.name = "cube " + area.RoomIndex;

                //floorRender.material.SetColor("_Color", colors[area.RoomIndex]);

                floorRender.material.mainTexture = floorTex;
                floorRender.material.mainTextureScale = new Vector3(area.Floor.TrueScale.x,area.Floor.TrueScale.z, area.Floor.TrueScale.y);

                floor.transform.localPosition = area.Floor.Center;

                floor.transform.localScale = area.Floor.TrueScale;

                foreach (Area.Wall wall in area.Walls)
                {
                    if (wall.Overlaps.Count != 0)
                    {
                        foreach (Segment path in wall.Overlaps.Where(x => x.IsCorner == false).ToList())
                        {
                            if (path.IsRemoved == false)
                            {
                                //skip over north and south corners to avoid duplication.
                                if (path.IsCorner & (path.Orientation == Orient.North | path.Orientation == Orient.South))
                                {
                                    continue;
                                }

                                GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);

                                segment.transform.parent = transform;

                                segment.name = "Room " + area.RoomIndex + " " + wall.Orientation.Name + " Path " + path.Max;

                                segment.transform.localPosition = new Vector3(path.Center.x, 1, path.Center.z);

                                //now for scaling to 3D based on wall orientation

                                segment.transform.localScale = path.Scale;


                                var segRender = segment.GetComponent<Renderer>();

                                segRender.material.mainTexture = floorTex;

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

        public void GetColliders()
        {
            BoxCollider[] colliders;
            colliders = GetComponentsInChildren<BoxCollider>();

            foreach(BoxCollider collider in colliders)
            {
                BoxCollider mainCollider = gameObject.AddComponent<BoxCollider>();
                mainCollider.size = collider.size;
            }
        }

        public void CombineMeshes()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);

                i++;
            }
            transform.GetComponent<MeshFilter>().mesh = new Mesh();
            transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
            transform.gameObject.SetActive(true);
        }
    }
}