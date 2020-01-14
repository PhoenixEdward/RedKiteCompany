﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RedKite
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshMaker : MonoBehaviour
    {
        public Vector3[] vertices;
        public Vector2[] uvs;
        public int[] triangles;
        public Mesh mesh;
        public int[,,,] graph;

        public void MakeMesh(Vector3 scale)
        {
            //int totalFaceCount = (((int)scale.x * (int)scale.y) * 2) + (((int)scale.z * (int)scale.y) * 2);

            int frontTriCount = ((int)scale.x * (int)scale.y) * 6;
            int topTriCount = ((int)scale.z * (int)scale.x) * 6;
            int sideTricount = ((int)scale.y * (int)scale.z) * 6;

            int frontVertCount = ((int)scale.x + 1) * (((int)scale.y + 1) * 2);
            int sideVertCount = ((int)scale.z + 1) * (((int)scale.y + 1) * 2);
            int topVertCount = ((int)scale.x + 1) * (((int)scale.z + 1)* 2);

            triangles = new int[(frontTriCount * 2) + (topTriCount * 2) + (sideTricount * 2)];
            int triIndex = 0;

            vertices = new Vector3[(frontVertCount * 2) + (topVertCount * 2) + (sideVertCount * 2)];

            uvs = new Vector2[(frontVertCount * 2) + (topVertCount * 2) + (sideVertCount * 2)];

            //establish 3D cube for tracking indices.
            graph = new int[((int)scale.x + 1), ((int)scale.y + 1) * 2, ((int)scale.z + 1) * 2, 6];

            int index = 0;
            int heightIndex = 0;

            //get vertices on all axis.
            //front vertices.
            for (int y = 0; y < ((int)scale.y + 1); y++)
            {
                for (int x = 0; x < ((int)scale.x + 1); x++)
                {
                    graph[x, heightIndex, 0, 0] = index;
                    vertices[index] = new Vector3(x, y, 0);

                    graph[x, heightIndex + 1, 0, 0] = index + 1;
                    vertices[index + 1] = new Vector3(x, y + 1, 0);

                    index +=2;
                }
                heightIndex += 2;
            }

            heightIndex = 0;
            //top Vertices
            for (int z = 0; z < ((int) scale.z + 1); z++)
            {
                for (int x = 0; x < ((int)scale.x + 1); x++)
                {
                    graph[x,(int)scale.y,heightIndex, 1] = index;
                    vertices[index] = new Vector3(x, (int)scale.y, z);

                    graph[x, (int)scale.y, heightIndex + 1, 1] = index + 1;
                    vertices[index + 1] = new Vector3(x, (int)scale.y, z + 1);

                    index += 2;
                }
                heightIndex += 2;
            }

            heightIndex = 0;
            //back vertices
            for (int y = 0; y < ((int)scale.y + 1); y++)
            {
                for (int x = 0; x < ((int)scale.x + 1); x++)
                {
                    graph[x, heightIndex, (int)scale.z, 2] = index;
                    vertices[index] = new Vector3(x, y, (int)scale.z);

                    graph[x, heightIndex + 1, (int)scale.z, 2] = index + 1;
                    vertices[index + 1] = new Vector3(x, y + 1, (int)scale.z);

                    index += 2;
                }
                heightIndex += 2;
            }

            heightIndex = 0;
            //bottom vertices
            for (int z = 0; z < ((int)scale.z + 1); z++)
            {
                for (int x = 0; x < ((int)scale.x + 1); x++)
                {
                    graph[x, 0, heightIndex, 3] = index;
                    vertices[index] = new Vector3(x, 0, z);

                    graph[x, 0, heightIndex + 1, 3] = index + 1;
                    vertices[index + 1] = new Vector3(x, 0, z + 1);

                    index += 2;
                }
                heightIndex += 2;
            }

            heightIndex = 0;
            //left vertices
            for (int y = 0; y < ((int)scale.y + 1); y++)
            {
                for (int z = 0; z < ((int)scale.z + 1); z++)
                {
                    graph[0, heightIndex, z, 4] = index;
                    vertices[index] = new Vector3(0, y, z);

                    graph[0, heightIndex + 1, z, 4] = index + 1;
                    vertices[index + 1] = new Vector3(0, y + 1, z);

                    index += 2;
                }
                heightIndex += 2;
            }

            heightIndex = 0;
            //right vertices
            for (int y = 0; y < ((int)scale.y + 1); y++)
            {
                for (int z = 0; z < ((int)scale.z + 1); z++)
                {
                    graph[(int)scale.x, heightIndex, z, 5] = index;
                    vertices[index] = new Vector3((int)scale.x, y, z);

                    graph[(int)scale.x, heightIndex + 1, z, 5] = index + 1;
                    vertices[index + 1] = new Vector3((int)scale.x, y + 1, z);

                    index += 2;
                }
                heightIndex += 2;
            }



            //loop through 3D graph on all surfaces.
            //front triangles.
            for (int y = 0; y < (int)scale.y * 2; y+=2)
            {
                for (int x = 0; x < (int)scale.x; x++)
                {
                    triangles[triIndex] = graph[x + 1, y , 0, 0];
                    triangles[triIndex + 1] = graph[x, y, 0, 0];
                    triangles[triIndex + 2] = graph[x + 1, y + 1, 0, 0];

                    triangles[triIndex + 3] = graph[x + 1, y + 1, 0, 0];
                    triangles[triIndex + 4] = graph[x, y, 0, 0];
                    triangles[triIndex + 5] = graph[x, y + 1, 0, 0];

                    triIndex += 6;
                }
            }

            //top triangles.
            for (int z = 0; z < (int)scale.z * 2; z+=2)
            {
                for (int x = 0; x < (int)scale.x; x++)
                {

                    triangles[triIndex] = graph[x, (int)scale.y, z + 1, 1];
                    triangles[triIndex + 1] = graph[x + 1, (int)scale.y, z + 1, 1];
                    triangles[triIndex + 2] = graph[x, (int)scale.y, z, 1];

                    triangles[triIndex + 3] = graph[x, (int)scale.y, z, 1];
                    triangles[triIndex + 4] = graph[x + 1, (int)scale.y, z + 1, 1];
                    triangles[triIndex + 5] = graph[x + 1, (int)scale.y, z, 1];

                    triIndex += 6;
                }
            }

            //back triangles.
            for (int y = 0; y < (int)scale.y * 2; y+=2)
            {
                for (int x = 0; x < (int)scale.x; x++)
                {
                    triangles[triIndex] = graph[x + 1, y, (int)scale.z, 2];
                    triangles[triIndex + 1] = graph[x + 1, y + 1, (int)scale.z, 2];
                    triangles[triIndex + 2] = graph[x, y, (int)scale.z, 2];

                    triangles[triIndex + 3] = graph[x, y, (int)scale.z, 2];
                    triangles[triIndex + 4] = graph[x + 1, y + 1, (int)scale.z, 2];
                    triangles[triIndex + 5] = graph[x, y + 1, (int)scale.z, 2];


                    triIndex += 6;
                }
            }

            //bottom triangles.
            for (int z = 0; z < (int)scale.z * 2; z+=2)
            {
                for (int x = 0; x < (int)scale.x ; x++)
                { 
                    triangles[triIndex] = graph[x, 0, z + 1, 3];
                    triangles[triIndex + 1] = graph[x, 0, z, 3];
                    triangles[triIndex + 2] = graph[x + 1, 0, z + 1, 3];

                    triangles[triIndex + 3] = graph[x + 1, 0, z + 1, 3];
                    triangles[triIndex + 4] = graph[x, 0, z, 3];
                    triangles[triIndex + 5] = graph[x + 1, 0, z, 3];
                    triIndex += 6;
                }
            }

            //left triangles
            for (int y = 0; y < (int)scale.y * 2; y+=2)
            {
                for (int z = 0; z < (int)scale.z; z++)
                {
                    triangles[triIndex] = graph[0, y, z, 4];
                    triangles[triIndex + 1] = graph[0, y, z + 1, 4];
                    triangles[triIndex + 2] = graph[0, y + 1, z, 4];

                    triangles[triIndex + 3] = graph[0, y + 1, z, 4];
                    triangles[triIndex + 4] = graph[0 , y, z + 1, 4];
                    triangles[triIndex + 5] = graph[0, y + 1, z + 1, 4];

                    triIndex += 6;
                }
            }

            //right triangles
            for (int y = 0; y < (int)scale.y * 2; y+=2)
            {
                for (int z = 0; z < (int)scale.z; z++)
                {
                    triangles[triIndex] = graph[(int)scale.x, y, z + 1, 5];
                    triangles[triIndex + 1] = graph[(int)scale.x, y, z, 5];
                    triangles[triIndex + 2] = graph[(int)scale.x, y + 1, z + 1, 5];

                    triangles[triIndex + 3] = graph[(int)scale.x, y + 1, z + 1, 5];
                    triangles[triIndex + 4] = graph[(int)scale.x, y, z, 5];
                    triangles[triIndex + 5] = graph[(int)scale.x, y + 1, z, 5];

                    triIndex += 6;
                }
            }

            //get uv coordinates
            //front uvs
            int uvIndex = 0;

            //front uvs
            for (int y = 0; y < ((int)scale.y + 1); y++)
            {
                for (int x = 0; x < ((int)scale.x + 1); x ++)
                {
                    if (x % 2 == 0)
                    { 
                        uvs[uvIndex] = new Vector2(0.33f, 0.5f);
                        uvs[uvIndex + + 1] = new Vector2(0.33f, 0.74f);
                    }
                    else
                    { 
                        uvs[uvIndex] = new Vector2(0.65f, 0.5f);
                        uvs[uvIndex + 1] = new Vector2(0.65f, 0.74f);
                    }

                    uvIndex+=2;
                }
            }

            // top uvs
            for (int z = 0; z < ((int)scale.z + 1); z++)
            {
                for (int x = 0; x < ((int)scale.x + 1); x++)
                {
                    if (x % 2 == 0)
                    {
                        uvs[uvIndex] = new Vector2(0.34f, 0.76f);
                        uvs[uvIndex + +1] = new Vector2(0.34f, .99f);
                    }
                    else
                    {
                        uvs[uvIndex] = new Vector2(0.65f, 0.76f);
                        uvs[uvIndex + 1] = new Vector2(0.65f, .99f);
                    }

                    uvIndex +=2;
                }
            }

            //back uvs
            for (int y = 0; y < ((int)scale.y + 1); y++)
            {
                for (int x = 0; x < ((int)scale.x + 1); x++)
                {
                    if (x % 2 == 0)
                    { 
                        uvs[uvIndex] = new Vector2(0.34f, .01f);
                        uvs[uvIndex + 1] = new Vector2(.34f, 0.24f);
                    }
                    else
                    { 
                        uvs[uvIndex] = new Vector2(0.66f, 0f);
                        uvs[uvIndex + 1] = new Vector2(0.66f, 0.24f);
                    }

                    uvIndex+=2;
                }
            }

            //bottom uvs
            for (int z = 0; z < ((int)scale.z + 1); z++)
            {
                for (int x = 0; x < ((int)scale.x + 1); x++)
                {
                    if (x % 2 == 0)
                    {
                        uvs[uvIndex] = new Vector2(0.34f, 0.25f);
                        uvs[uvIndex + 1] = new Vector2(.34f, 0.49f);
                    }
                    else
                    {
                        uvs[uvIndex] = new Vector2(0.64f, 0.25f);
                        uvs[uvIndex + 1] = new Vector2(0.64f, 0.49f);
                    }

                    uvIndex += 2;
                }
            }


            //right uv
            for (int y = 0; y < ((int)scale.y + 1); y++)
            {
                for (int z = 0; z < ((int)scale.z + 1); z++)
                {
                    if (z % 2 == 0)
                    { 
                        uvs[uvIndex] = new Vector2(0.66f, 0.5f);
                        uvs[uvIndex + 1] = new Vector2(0.66f, 0.74f);
                    }
                    else
                    { 
                        uvs[uvIndex] = new Vector2(0.99f, 0.5f);
                        uvs[uvIndex + 1] = new Vector2(0.99f, 0.74f);
                    }

                    uvIndex += 2;
                }
            }

            for (int y = 0; y < ((int)scale.y + 1); y++)
            {
                for (int z = 0; z < ((int)scale.z + 1); z++)
                {
                    if (z % 2 == 0)
                    { 
                        uvs[uvIndex] = new Vector2(0, 0.5f);
                        uvs[uvIndex + 1] = new Vector2(0, 0.74f);
                    }
                    else
                    { 
                        uvs[uvIndex] = new Vector2(0.32f, 0.5f);
                        uvs[uvIndex + 1] = new Vector2(0.32f, 0.74f);
                    }

                    uvIndex += 2;
                }
            }



            mesh = GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.Optimize();
            mesh.RecalculateNormals();

        }

        public void SetTextures(Texture2D[] textures)
        {
            /// <summary>
            /// Textures should be organized as follows in the array: 
            /// top, front, back, left, right, bottom.
            /// </summary>

            Texture2D outTexture = new Texture2D(100 * 3, 100 * 4);
            Color[] empty = new Color[100*100];

            //get empty color array
            for (int i = 0; i < empty.Length; i++)
                empty[i] = Color.clear;

            Vector2Int[] texCoords = new Vector2Int[]
            {
                new Vector2Int(0,300),
                new Vector2Int(100,300),
                new Vector2Int(200,300),

                new Vector2Int(0,200),
                new Vector2Int(100,200),
                new Vector2Int(200,200),

                new Vector2Int(0,100),
                new Vector2Int(100,100),
                new Vector2Int(200,100),

                new Vector2Int(0,0),
                new Vector2Int(100,0),
                new Vector2Int(200,0),

            };
            
            //creates indexes for skipping over
            int[] empties = new int[6] { 0, 2, 6, 8, 9, 11 };

            //to increment the texture array seperately.
            int texIndex = 0;

            for(int i = 0; i < textures.Length + 6; i++)
            {
                if (empties.Contains(i))
                {
                    outTexture.SetPixels(texCoords[i].x, texCoords[i].y, 100, 100, empty);
                }
                else
                {
                    Color[] pixels = textures[texIndex].GetPixels();
                    Debug.Log(pixels.Length);
                    outTexture.SetPixels(
                        texCoords[i].x, texCoords[i].y, 100, 100, 
                        pixels);
                    texIndex++;
                }
            }

            outTexture.Apply();
            //Get render to set texture

            Renderer renderer = GetComponent<Renderer>();

            renderer.material.mainTexture = outTexture;
            renderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;


        }
    }
}
