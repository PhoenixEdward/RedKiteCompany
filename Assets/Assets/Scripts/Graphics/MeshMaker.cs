using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RedKite
{
    [System.Serializable]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshMaker : MonoBehaviour
    {
        public Mesh mesh;
        ///<summary>Submeshes go from Front, Top, Back, Bottom, Left, to Right.</summary>
        public Mesh[] subMeshes = new Mesh[6];
        public int[,,,] graph;
        Vector3 scale;
        Vector3 position;

        public Vector3[] subMeshScales = new Vector3[6];
        public Vector3[] subMeshPositions = new Vector3[6];

        public List<Bounds>[] subMeshBounds = new List<Bounds>[6] { new List<Bounds>(), new List<Bounds>(),
            new List<Bounds>(), new List<Bounds>(), new List<Bounds>(), new List<Bounds>() };


        public void NewMakeMesh(Vector3 _scale, Vector3 _position)
        {
            position = _position;
            scale = _scale;

            subMeshScales[0] = new Vector3(scale.x, scale.y, 0);
            subMeshScales[1] = new Vector3(scale.x, 0, scale.z);
            subMeshScales[2] = new Vector3(scale.x, scale.y, 0);
            subMeshScales[3] = new Vector3(scale.x, 0, scale.z);
            subMeshScales[4] = new Vector3(0, scale.y, scale.z);
            subMeshScales[5] = new Vector3(0, scale.y, scale.z);

            subMeshPositions[0] = new Vector3(position.x, position.y, position.z - (scale.z / 2));
            subMeshPositions[1] = new Vector3(position.x, position.y + (scale.y / 2), position.z);
            subMeshPositions[2] = new Vector3(position.x, position.y, position.z + (scale.z / 2));
            subMeshPositions[3] = new Vector3(position.x, position.y - (scale.y / 2), position.z);
            subMeshPositions[4] = new Vector3(position.x - (scale.x / 2), position.y, position.y);
            subMeshPositions[5] = new Vector3(position.x + (scale.x / 2), position.y, position.z);

            int frontTriCount = ((int)_scale.x * (int)_scale.y) * 6;
            int topTriCount = ((int)_scale.z * (int)_scale.x) * 6;
            int sideTricount = ((int)_scale.y * (int)_scale.z) * 6;

            int frontVertCount = ((int)_scale.x + 1) * (((int)_scale.y + 1) * 2);
            int sideVertCount = ((int)_scale.z + 1) * (((int)_scale.y + 1) * 2);
            int topVertCount = ((int)_scale.x + 1) * (((int)_scale.z + 1) * 2);

            Vector3[] frontVertices = new Vector3[frontVertCount];
            Vector3[] topVertices = new Vector3[topVertCount];
            Vector3[] backVertices = new Vector3[frontVertCount];
            Vector3[] bottomVertices = new Vector3[topVertCount];
            Vector3[] leftVertices = new Vector3[sideVertCount];
            Vector3[] rightVertices = new Vector3[sideVertCount];

            int[] frontTriangles = new int[frontTriCount];
            int[] topTriangles = new int[topTriCount];
            int[] backTriangles = new int[frontTriCount];
            int[] bottomTriangles = new int[topTriCount];
            int[] leftTriangles = new int[sideTricount];
            int[] rightTriangles = new int[sideTricount];

            Vector3 coordOffset = new Vector3(-scale.x / 2, -scale.y / 2, -scale.z / 2);

            //establish 4D cube for tracking indices.
            graph = new int[((int)_scale.x + 1), ((int)_scale.y + 1) * 2, ((int)_scale.z + 1) * 2, 6];

            int index = 0;
            int heightIndex = 0;
            int widthIndex = 0;

            //get vertices on all axis.
            //back vertices
            for (int y = 0; y < ((int)_scale.y + 1); y++)
            {
                for (int x = 0; x < ((int)_scale.x + 1); x++)
                {
                    graph[widthIndex, y, (int)_scale.z, 2] = index;
                    backVertices[index] = new Vector3(x, y, (int)_scale.z) + coordOffset;

                    graph[widthIndex, y, (int)_scale.z, 2] = index;
                    backVertices[index] = new Vector3(x, y, (int)_scale.z) + coordOffset;

                    graph[widthIndex, y, (int)_scale.z, 2] = index;
                    backVertices[index] = new Vector3(x, y, (int)_scale.z) + coordOffset;

                    graph[widthIndex, y, (int)_scale.z, 2] = index;
                    backVertices[index] = new Vector3(x, y, (int)_scale.z) + coordOffset;

                    index += 4;
                }
            }


            index = 0;
            heightIndex = 0;
            widthIndex = 0;
            //front vertices.
            for (int y = 0; y < ((int)_scale.y + 1); y++)
            {
                for (int x = 0; x < ((int)_scale.x + 1); x++)
                {
                    graph[widthIndex, y, 0, 0] = index;
                    frontVertices[index] = new Vector3(x, y, 0) + coordOffset;

                    graph[widthIndex, y, 0, 0] = index;
                    frontVertices[index] = new Vector3(x, y, 0) + coordOffset;

                    graph[x, y, 0, 0] = index;
                    frontVertices[index] = new Vector3(x, y, 0) + coordOffset;

                    graph[x, y, 0, 0] = index;
                    frontVertices[index] = new Vector3(x, y, 0) + coordOffset;

                    index += 4;
                }
            }

            index = 0;
            heightIndex = 0;
            widthIndex = 0;
            //top Vertices
            for (int z = 0; z < ((int)_scale.z + 1); z++)
            {
                for (int x = 0; x < ((int)_scale.x + 1); x++)
                {
                    graph[x, (int)_scale.y, z, 1] = index;
                    topVertices[index] = new Vector3(x, (int)_scale.y, z) + coordOffset;

                    graph[x, (int)_scale.y, z, 1] = index;
                    topVertices[index] = new Vector3(x, (int)_scale.y, z) + coordOffset;

                    graph[x, (int)_scale.y, z, 1] = index;
                    topVertices[index] = new Vector3(x, (int)_scale.y, z) + coordOffset;

                    graph[x, (int)_scale.y, z, 1] = index;
                    topVertices[index] = new Vector3(x, (int)_scale.y, z) + coordOffset;

                    index += 4;
                }
            }

            index = 0;
            heightIndex = 0;
            widthIndex = 0;
            //bottom vertices
            for (int z = 0; z < ((int)_scale.z + 1); z++)
            {
                for (int x = 0; x < ((int)_scale.x + 1); x++)
                {
                    graph[x, 0, z, 3] = index;
                    bottomVertices[index] = new Vector3(x, 0, z) + coordOffset;

                    graph[x, 0, z, 3] = index;
                    bottomVertices[index] = new Vector3(x, 0, z) + coordOffset;

                    graph[x, 0, z, 3] = index;
                    bottomVertices[index] = new Vector3(x, 0, z) + coordOffset;

                    graph[x, 0, z, 3] = index;
                    bottomVertices[index] = new Vector3(x, 0, z) + coordOffset;

                    index += 4;
                }
            }

            index = 0;
            heightIndex = 0;
            widthIndex = 0;
            //left vertices
            for (int y = 0; y < ((int)_scale.y + 1); y++)
            {
                for (int z = 0; z < ((int)_scale.z + 1); z++)
                {
                    graph[0, y, z, 4] = index;
                    leftVertices[index] = new Vector3(0, y, z) + coordOffset;

                    graph[0, y, z, 4] = index;
                    leftVertices[index] = new Vector3(0, y, z) + coordOffset;

                    graph[0, y, z, 4] = index;
                    leftVertices[index] = new Vector3(0, y, z) + coordOffset;

                    graph[0, y, z, 4] = index;
                    leftVertices[index] = new Vector3(0, y, z) + coordOffset;

                    index += 4;
                }
            }

            index = 0;
            heightIndex = 0;
            widthIndex = 0;
            //right vertices
            for (int y = 0; y < ((int)_scale.y + 1); y++)
            {
                for (int z = 0; z < ((int)_scale.z + 1); z++)
                {
                    graph[(int)_scale.x, y, z, 5] = index;
                    rightVertices[index] = new Vector3((int)_scale.x, y, z) + coordOffset;

                    graph[(int)_scale.x, y, z, 5] = index;
                    rightVertices[index] = new Vector3((int)_scale.x, y, z) + coordOffset;

                    graph[(int)_scale.x, y, z, 5] = index;
                    rightVertices[index] = new Vector3((int)_scale.x, y, z) + coordOffset;

                    graph[(int)_scale.x, y, z, 5] = index;
                    rightVertices[index] = new Vector3((int)_scale.x, y, z) + coordOffset;

                    index+=4;
                }
            }


            int triIndex = 0;
            //loop through 3D graph on all surfaces.
            //back triangles.
            for (int y = 0; y < (int)_scale.y * 2; y++)
            {
                for (int x = 0; x < (int)_scale.x * 2; x++)
                {
                    backTriangles[triIndex] = graph[x + 1, y, (int)_scale.z, 2];
                    backTriangles[triIndex + 1] = graph[x + 1, y + 1, (int)_scale.z, 2];
                    backTriangles[triIndex + 2] = graph[x, y, (int)_scale.z, 2];

                    backTriangles[triIndex + 3] = graph[x, y, (int)_scale.z, 2];
                    backTriangles[triIndex + 4] = graph[x + 1, y + 1, (int)_scale.z, 2];
                    backTriangles[triIndex + 5] = graph[x, y + 1, (int)_scale.z, 2];


                    triIndex += 6;
                }
            }

            triIndex = 0;
            //front triangles.
            for (int y = 0; y < (int)_scale.y*2; y++)
            {
                for (int x = 0; x < (int)_scale.x*2; x++)
                {
                    frontTriangles[triIndex] = graph[x + 1, y, 0, 0];
                    frontTriangles[triIndex + 1] = graph[x, y, 0, 0];
                    frontTriangles[triIndex + 2] = graph[x + 1, y + 1, 0, 0];

                    frontTriangles[triIndex + 3] = graph[x + 1, y + 1, 0, 0];
                    frontTriangles[triIndex + 4] = graph[x, y, 0, 0];
                    frontTriangles[triIndex + 5] = graph[x, y + 1, 0, 0];

                    triIndex += 6;
                }
            }

            triIndex = 0;
            //top triangles.
            for (int z = 0; z < (int)_scale.z*2; z++)
            {
                for (int x = 0; x < (int)_scale.x*2; x++)
                {

                    topTriangles[triIndex] = graph[x, (int)_scale.y, z + 1, 1];
                    topTriangles[triIndex + 1] = graph[x + 1, (int)_scale.y, z + 1, 1];
                    topTriangles[triIndex + 2] = graph[x, (int)_scale.y, z, 1];

                    topTriangles[triIndex + 3] = graph[x, (int)_scale.y, z, 1];
                    topTriangles[triIndex + 4] = graph[x + 1, (int)_scale.y, z + 1, 1];
                    topTriangles[triIndex + 5] = graph[x + 1, (int)_scale.y, z, 1];

                    triIndex += 6;
                }
            }

            triIndex = 0;
            //bottom bottomTriangles.
            for (int z = 0; z < (int)_scale.z*2; z++)
            {
                for (int x = 0; x < (int)_scale.x*2; x++)
                {
                    bottomTriangles[triIndex] = graph[x, 0, z + 1, 3];
                    bottomTriangles[triIndex + 1] = graph[x, 0, z, 3];
                    bottomTriangles[triIndex + 2] = graph[x + 1, 0, z + 1, 3];

                    bottomTriangles[triIndex + 3] = graph[x + 1, 0, z + 1, 3];
                    bottomTriangles[triIndex + 4] = graph[x, 0, z, 3];
                    bottomTriangles[triIndex + 5] = graph[x + 1, 0, z, 3];
                    triIndex += 6;
                }
            }

            triIndex = 0;
            //left triangles
            for (int y = 0; y < (int)_scale.y*2; y++)
            {
                for (int z = 0; z < (int)_scale.z*2; z++)
                {
                    leftTriangles[triIndex] = graph[0, y, z, 4];
                    leftTriangles[triIndex + 1] = graph[0, y, z + 1, 4];
                    leftTriangles[triIndex + 2] = graph[0, y + 1, z, 4];

                    leftTriangles[triIndex + 3] = graph[0, y + 1, z, 4];
                    leftTriangles[triIndex + 4] = graph[0, y, z + 1, 4];
                    leftTriangles[triIndex + 5] = graph[0, y + 1, z + 1, 4];

                    triIndex += 6;
                }
            }

            triIndex = 0;
            //right rightTriangles
            for (int y = 0; y < (int)_scale.y*2; y++)
            {
                for (int z = 0; z < (int)_scale.z*2; z++)
                {
                    rightTriangles[triIndex] = graph[(int)_scale.x, y, z + 1, 5];
                    rightTriangles[triIndex + 1] = graph[(int)_scale.x, y, z, 5];
                    rightTriangles[triIndex + 2] = graph[(int)_scale.x, y + 1, z + 1, 5];

                    rightTriangles[triIndex + 3] = graph[(int)_scale.x, y + 1, z + 1, 5];
                    rightTriangles[triIndex + 4] = graph[(int)_scale.x, y, z, 5];
                    rightTriangles[triIndex + 5] = graph[(int)_scale.x, y + 1, z, 5];

                    triIndex += 6;
                }
            }

            //create front submesh

            subMeshes[0] = new Mesh();
            subMeshes[0].Clear();
            subMeshes[0].vertices = frontVertices;
            subMeshes[0].triangles = frontTriangles;
            //subMeshes[0].Optimize();
            //subMeshes[0].RecalculateNormals();

            //create top submesh
            subMeshes[1] = new Mesh();
            subMeshes[1].Clear();
            subMeshes[1].vertices = topVertices;
            subMeshes[1].triangles = topTriangles;
            //subMeshes[1].Optimize();
            //subMeshes[1].RecalculateNormals();

            //create back submesh
            subMeshes[2] = new Mesh();
            subMeshes[2].Clear();
            subMeshes[2].vertices = backVertices;
            subMeshes[2].triangles = backTriangles;
            //subMeshes[2].Optimize();
            //subMeshes[2].RecalculateNormals();

            //create bottom submesh
            subMeshes[3] = new Mesh();
            subMeshes[3].Clear();
            subMeshes[3].vertices = bottomVertices;
            subMeshes[3].triangles = bottomTriangles;
            //subMeshes[3].Optimize();
            //subMeshes[3].RecalculateNormals();

            //create left submesh
            subMeshes[4] = new Mesh();
            subMeshes[4].Clear();
            subMeshes[4].vertices = leftVertices;
            subMeshes[4].triangles = leftTriangles;
            //subMeshes[4].Optimize();
            //subMeshes[4].RecalculateNormals();

            //create right submesh
            subMeshes[5] = new Mesh();
            subMeshes[5].Clear();
            subMeshes[5].vertices = rightVertices;
            subMeshes[5].triangles = rightTriangles;
            //subMeshes[5].Optimize();
            //subMeshes[5].RecalculateNormals();

            //record bounds of individual submeshes (necessary for consitency later down the line even if nothing is merged with it.
            subMeshBounds[0].Add(new Bounds(subMeshes[0].bounds.center + position, subMeshes[0].bounds.size));
            subMeshBounds[1].Add(new Bounds(subMeshes[1].bounds.center + position, subMeshes[1].bounds.size));
            subMeshBounds[2].Add(new Bounds(subMeshes[2].bounds.center + position, subMeshes[2].bounds.size));
            subMeshBounds[3].Add(new Bounds(subMeshes[3].bounds.center + position, subMeshes[3].bounds.size));
            subMeshBounds[4].Add(new Bounds(subMeshes[4].bounds.center + position, subMeshes[4].bounds.size));
            subMeshBounds[5].Add(new Bounds(subMeshes[5].bounds.center + position, subMeshes[5].bounds.size));



        }

        public void NewSetTextures(Renderer renderer, Texture2D[] textures)
        {
            /// <summary>
            /// Textures should be organized as follows in the array: 
            /// top, front, back, left, right, bottom.
            /// </summary>
            Vector2[] frontUVs = new Vector2[subMeshes[0].vertexCount];
            Vector2[] topUVs = new Vector2[subMeshes[1].vertexCount];
            Vector2[] backUVs = new Vector2[subMeshes[2].vertexCount];
            Vector2[] bottomUVs = new Vector2[subMeshes[3].vertexCount];
            Vector2[] leftUVs = new Vector2[subMeshes[4].vertexCount];
            Vector2[] rightUVs = new Vector2[subMeshes[5].vertexCount];

            subMeshes[0].uv = FindUVs3D(subMeshes[0], subMeshBounds[0], 0);
            subMeshes[1].uv = FindUVs3D(subMeshes[1], subMeshBounds[1], 1);
            subMeshes[2].uv = FindUVs3D(subMeshes[2], subMeshBounds[2], 2);
            subMeshes[3].uv = FindUVs3D(subMeshes[3], subMeshBounds[3], 3);
            subMeshes[4].uv = FindUVs3D(subMeshes[4], subMeshBounds[4], 4);
            subMeshes[5].uv = FindUVs3D(subMeshes[5], subMeshBounds[5], 5);


        }


        Vector2[] FindUVs3D(Mesh mesh, List<Bounds> boundsList, int side)
        {
            Vector2[] uvs = new Vector2[mesh.vertexCount];
            int uvIndex = 0;

            int up = 0;
            int down = 0;
            float pix = 1f;
            int overshoot = 0;

            if (side == 0 | side == 2)
            {
                for (float z = mesh.bounds.min.z - down; z <= mesh.bounds.max.z + up; z += pix)
                {
                    for (float y = subMeshes[0].bounds.min.y - down; y <= mesh.bounds.max.y + up; y += pix)
                    {
                        for (float x = subMeshes[0].bounds.min.x - down; x <= mesh.bounds.max.x + up; x += pix)
                        {
                            foreach (Bounds bounds in boundsList)
                            {
                                if (bounds.Contains(new Vector3(x, y, z)) & uvIndex < uvs.Length)
                                {
                                    if (Math.Floor(y) % 2 == 0)
                                    {
                                        if (Math.Floor(x) % 2 == 0)
                                        {
                                            uvs[uvIndex] = new Vector2(0f, 0f);
                                        }
                                        else
                                        {
                                            uvs[uvIndex] = new Vector2(1f, 0f);
                                        }
                                    }
                                    else
                                    {
                                        if (Math.Floor(x) % 2 == 0)
                                        {
                                            uvs[uvIndex] = new Vector2(0f, 1f);
                                        }
                                        else
                                        {
                                            uvs[uvIndex] = new Vector2(1f, 1f);
                                        }
                                    }

                                    uvIndex++;

                                    break;
                                }

                                if (uvIndex >= uvs.Length)
                                {
                                    overshoot++;
                                }
                            }
                        }
                    }
                }
            }
            else if (side == 1 | side == 3)
            {
                // top topUVs
                for (float y = mesh.bounds.min.y - down; y <= mesh.bounds.max.y + up; y += pix)
                {
                    for (float z = mesh.bounds.min.z - down; z <= mesh.bounds.max.z + up; z += pix)
                    {
                        for (float x = mesh.bounds.min.x - down; x <= mesh.bounds.max.x + up; x += pix)
                        {
                            int overLapCount = 0;
                            foreach (Bounds bounds in boundsList)
                            {
                                if (bounds.Contains(new Vector3(x, y, z)) & uvIndex < uvs.Length)
                                {
                                    overLapCount++;

                                    if (overLapCount > 1)
                                        break;

                                    if (Math.Floor(z) % 2 == 0)
                                    {
                                        if (Math.Floor(x) % 2 == 0)
                                        {
                                            uvs[uvIndex] = new Vector2(0f, 0f);
                                        }
                                        else
                                        {
                                            uvs[uvIndex] = new Vector2(1f, 0f);
                                        }
                                    }
                                    else
                                    {
                                        if (Math.Floor(x) % 2 == 0)
                                        {
                                            uvs[uvIndex] = new Vector2(0f, 1f);
                                        }
                                        else
                                        {
                                            uvs[uvIndex] = new Vector2(1f, 1f);
                                        }
                                    }

                                    uvIndex++;

                                    break;
                                }

                                if (uvIndex >= uvs.Length)
                                {
                                    overshoot++;
                                }

                            }
                        }
                    }
                }
            }

            else
            {
                for (float x = mesh.bounds.min.x - down; x <= mesh.bounds.max.x + up; x += pix)
                {
                    for (float y = mesh.bounds.min.y - down; y <= subMeshes[5].bounds.max.y + up; y += pix)
                    {
                        for (float z = mesh.bounds.min.z - down; z <= mesh.bounds.max.z + up; z += pix)
                        {
                            foreach (Bounds bounds in boundsList)
                            {
                                if (bounds.Contains(new Vector3(x, y, z)) & uvIndex < uvs.Length)
                                {
                                    if (Math.Floor(y) % 2 == 0)
                                    {
                                        if (Math.Floor(z) % 2 == 0)
                                        {
                                            uvs[uvIndex] = new Vector2(0f, 0f);
                                        }
                                        else
                                        {
                                            uvs[uvIndex] = new Vector2(1f, 0f);
                                        }
                                    }
                                    else
                                    {
                                        if (Math.Floor(z) % 2 == 0)
                                        {
                                            uvs[uvIndex] = new Vector2(0f, 1f);
                                        }
                                        else
                                        {
                                            uvs[uvIndex] = new Vector2(1f, 1f);
                                        }
                                    }

                                    uvIndex++;

                                    break;
                                }
                            }
                        }
                    }
                }
            }


            Debug.Log("Overshoot: " + overshoot + " On side: " + side);
            Debug.Log("Vertex Count: " + mesh.vertexCount);

            return uvs;
        }


        public void OldMakeMesh(Vector3 _scale, Vector3 _position)
        {
            position = _position;
            scale = _scale;

            subMeshScales[0] = new Vector3(scale.x,scale.y,0);
            subMeshScales[1] = new Vector3(scale.x, 0, scale.z);
            subMeshScales[2] = new Vector3(scale.x, scale.y, 0);
            subMeshScales[3] = new Vector3(scale.x, 0, scale.z);
            subMeshScales[4] = new Vector3(0, scale.y, scale.z);
            subMeshScales[5] = new Vector3(0, scale.y, scale.z);

            subMeshPositions[0] = new Vector3(position.x, position.y, position.z - (scale.z / 2));
            subMeshPositions[1] = new Vector3(position.x, position.y + (scale. y / 2), position.z);
            subMeshPositions[2] = new Vector3(position.x, position.y, position.z + (scale.z / 2));
            subMeshPositions[3] = new Vector3(position.x, position.y - (scale.y / 2), position.z);
            subMeshPositions[4] = new Vector3(position.x - (scale.x / 2), position.y, position.y);
            subMeshPositions[5] = new Vector3(position.x + (scale.x / 2), position.y, position.z);

            int frontTriCount = ((int)_scale.x * (int)_scale.y) * 6;
            int topTriCount = ((int)_scale.z * (int)_scale.x) * 6;
            int sideTricount = ((int)_scale.y * (int)_scale.z) * 6;

            int frontVertCount = ((int)_scale.x + 1) * (((int)_scale.y + 1) * 2);
            int sideVertCount = ((int)_scale.z + 1) * (((int)_scale.y + 1) * 2);
            int topVertCount = ((int)_scale.x + 1) * (((int)_scale.z + 1)* 2);

            Vector3[] frontVertices = new Vector3[frontVertCount];
            Vector3[] topVertices = new Vector3[topVertCount];
            Vector3[] backVertices = new Vector3[frontVertCount];
            Vector3[] bottomVertices = new Vector3[topVertCount];
            Vector3[] leftVertices = new Vector3[sideVertCount];
            Vector3[] rightVertices = new Vector3[sideVertCount];

            int[] frontTriangles = new int[frontTriCount];
            int[] topTriangles = new int[topTriCount];
            int[] backTriangles = new int[frontTriCount];
            int[] bottomTriangles = new int[topTriCount];
            int[] leftTriangles = new int[sideTricount];
            int[] rightTriangles = new int[sideTricount];

            Vector3 coordOffset = new Vector3(-scale.x/2,-scale.y/2,-scale.z/2);

            //establish 4D cube for tracking indices.
            graph = new int[((int)_scale.x + 1), ((int)_scale.y + 1) * 2, ((int)_scale.z + 1) * 2, 6];

            int index = 0;
            int heightIndex = 0;

            //get vertices on all axis.
            //front vertices.
            for (int y = 0; y < ((int)_scale.y + 1); y++)
            {
                for (int x = 0; x < ((int)_scale.x + 1); x++)
                {
                    graph[x, heightIndex, 0, 0] = index;
                    frontVertices[index] = new Vector3(x, y, 0) + coordOffset;

                    graph[x, heightIndex + 1, 0, 0] = index + 1;
                    frontVertices[index + 1] = new Vector3(x, y + 1, 0) + coordOffset;

                    index +=2;
                }
                heightIndex += 2;
            }

            index = 0;
            heightIndex = 0;
            //top Vertices
            for (int z = 0; z < ((int) _scale.z + 1); z++)
            {
                for (int x = 0; x < ((int)_scale.x + 1); x++)
                {
                    graph[x,(int)_scale.y,heightIndex, 1] = index;
                    topVertices[index] = new Vector3(x, (int)_scale.y, z) + coordOffset;

                    graph[x, (int)_scale.y, heightIndex + 1, 1] = index + 1;
                    topVertices[index + 1] = new Vector3(x, (int)_scale.y, z + 1) + coordOffset;

                    index += 2;
                }
                heightIndex += 2;
            }

            index = 0;
            heightIndex = 0;
            //back vertices
            for (int y = 0; y < ((int)_scale.y + 1); y++)
            {
                for (int x = 0; x < ((int)_scale.x + 1); x++)
                {
                    graph[x, heightIndex, (int)_scale.z, 2] = index;
                    backVertices[index] = new Vector3(x, y, (int)_scale.z) + coordOffset;

                    graph[x, heightIndex + 1, (int)_scale.z, 2] = index + 1;
                    backVertices[index + 1] = new Vector3(x, y + 1, (int)_scale.z) + coordOffset;

                    index += 2;
                }
                heightIndex += 2;
            }

            index = 0;
            heightIndex = 0;
            //bottom vertices
            for (int z = 0; z < ((int)_scale.z + 1); z++)
            {
                for (int x = 0; x < ((int)_scale.x + 1); x++)
                {
                    graph[x, 0, heightIndex, 3] = index;
                    bottomVertices[index] = new Vector3(x, 0, z) + coordOffset;

                    graph[x, 0, heightIndex + 1, 3] = index + 1;
                    bottomVertices[index + 1] = new Vector3(x, 0, z + 1) + coordOffset;

                    index += 2;
                }
                heightIndex += 2;
            }

            index = 0;
            heightIndex = 0;
            //left vertices
            for (int y = 0; y < ((int)_scale.y + 1); y++)
            {
                for (int z = 0; z < ((int)_scale.z + 1); z++)
                {
                    graph[0, heightIndex, z, 4] = index;
                    leftVertices[index] = new Vector3(0, y, z) + coordOffset;

                    graph[0, heightIndex + 1, z, 4] = index + 1;
                    leftVertices[index + 1] = new Vector3(0, y + 1, z) + coordOffset;

                    index += 2;
                }
                heightIndex += 2;
            }

            index = 0;
            heightIndex = 0;
            //right vertices
            for (int y = 0; y < ((int)_scale.y + 1); y++)
            {
                for (int z = 0; z < ((int)_scale.z + 1); z++)
                {
                    graph[(int)_scale.x, heightIndex, z, 5] = index;
                    rightVertices[index] = new Vector3((int)_scale.x, y, z) + coordOffset;

                    graph[(int)_scale.x, heightIndex + 1, z, 5] = index + 1;
                    rightVertices[index + 1] = new Vector3((int)_scale.x, y + 1, z) + coordOffset;

                    index += 2;
                }
                heightIndex += 2;
            }


            int triIndex = 0;
            //loop through 3D graph on all surfaces.
            //front triangles.
            for (int y = 0; y < (int)_scale.y * 2; y+=2)
            {
                for (int x = 0; x < (int)_scale.x; x++)
                {
                    frontTriangles[triIndex] = graph[x + 1, y , 0, 0];
                    frontTriangles[triIndex + 1] = graph[x, y, 0, 0];
                    frontTriangles[triIndex + 2] = graph[x + 1, y + 1, 0, 0];

                    frontTriangles[triIndex + 3] = graph[x + 1, y + 1, 0, 0];
                    frontTriangles[triIndex + 4] = graph[x, y, 0, 0];
                    frontTriangles[triIndex + 5] = graph[x, y + 1, 0, 0];

                    triIndex += 6;
                }
            }

            triIndex = 0;
            //top triangles.
            for (int z = 0; z < (int)_scale.z * 2; z+=2)
            {
                for (int x = 0; x < (int)_scale.x; x++)
                {

                    topTriangles[triIndex] = graph[x, (int)_scale.y, z + 1, 1];
                    topTriangles[triIndex + 1] = graph[x + 1, (int)_scale.y, z + 1, 1];
                    topTriangles[triIndex + 2] = graph[x, (int)_scale.y, z, 1];

                    topTriangles[triIndex + 3] = graph[x, (int)_scale.y, z, 1];
                    topTriangles[triIndex + 4] = graph[x + 1, (int)_scale.y, z + 1, 1];
                    topTriangles[triIndex + 5] = graph[x + 1, (int)_scale.y, z, 1];

                    triIndex += 6;
                }
            }

            triIndex = 0;
            //back triangles.
            for (int y = 0; y < (int)_scale.y * 2; y+=2)
            {
                for (int x = 0; x < (int)_scale.x; x++)
                {
                    backTriangles[triIndex] = graph[x + 1, y, (int)_scale.z, 2];
                    backTriangles[triIndex + 1] = graph[x + 1, y + 1, (int)_scale.z, 2];
                    backTriangles[triIndex + 2] = graph[x, y, (int)_scale.z, 2];

                    backTriangles[triIndex + 3] = graph[x, y, (int)_scale.z, 2];
                    backTriangles[triIndex + 4] = graph[x + 1, y + 1, (int)_scale.z, 2];
                    backTriangles[triIndex + 5] = graph[x, y + 1, (int)_scale.z, 2];


                    triIndex += 6;
                }
            }

            triIndex = 0;
            //bottom bottomTriangles.
            for (int z = 0; z < (int)_scale.z * 2; z+=2)
            {
                for (int x = 0; x < (int)_scale.x ; x++)
                { 
                    bottomTriangles[triIndex] = graph[x, 0, z + 1, 3];
                    bottomTriangles[triIndex + 1] = graph[x, 0, z, 3];
                    bottomTriangles[triIndex + 2] = graph[x + 1, 0, z + 1, 3];

                    bottomTriangles[triIndex + 3] = graph[x + 1, 0, z + 1, 3];
                    bottomTriangles[triIndex + 4] = graph[x, 0, z, 3];
                    bottomTriangles[triIndex + 5] = graph[x + 1, 0, z, 3];
                    triIndex += 6;
                }
            }

            triIndex = 0;
            //left triangles
            for (int y = 0; y < (int)_scale.y * 2; y+=2)
            {
                for (int z = 0; z < (int)_scale.z; z++)
                {
                    leftTriangles[triIndex] = graph[0, y, z, 4];
                    leftTriangles[triIndex + 1] = graph[0, y, z + 1, 4];
                    leftTriangles[triIndex + 2] = graph[0, y + 1, z, 4];

                    leftTriangles[triIndex + 3] = graph[0, y + 1, z, 4];
                    leftTriangles[triIndex + 4] = graph[0 , y, z + 1, 4];
                    leftTriangles[triIndex + 5] = graph[0, y + 1, z + 1, 4];

                    triIndex += 6;
                }
            }

            triIndex = 0;
            //right rightTriangles
            for (int y = 0; y < (int)_scale.y * 2; y+=2)
            {
                for (int z = 0; z < (int)_scale.z; z++)
                {
                    rightTriangles[triIndex] = graph[(int)_scale.x, y, z + 1, 5];
                    rightTriangles[triIndex + 1] = graph[(int)_scale.x, y, z, 5];
                    rightTriangles[triIndex + 2] = graph[(int)_scale.x, y + 1, z + 1, 5];

                    rightTriangles[triIndex + 3] = graph[(int)_scale.x, y + 1, z + 1, 5];
                    rightTriangles[triIndex + 4] = graph[(int)_scale.x, y, z, 5];
                    rightTriangles[triIndex + 5] = graph[(int)_scale.x, y + 1, z, 5];

                    triIndex += 6;
                }
            }

            //create front submesh

            subMeshes[0] = new Mesh();
            subMeshes[0].Clear();
            subMeshes[0].vertices = frontVertices;
            subMeshes[0].triangles = frontTriangles;
            //subMeshes[0].Optimize();
            //subMeshes[0].RecalculateNormals();

            //create top submesh
            subMeshes[1] = new Mesh();
            subMeshes[1].Clear();
            subMeshes[1].vertices = topVertices;
            subMeshes[1].triangles = topTriangles;
            //subMeshes[1].Optimize();
            //subMeshes[1].RecalculateNormals();

            //create back submesh
            subMeshes[2] = new Mesh();
            subMeshes[2].Clear();
            subMeshes[2].vertices = backVertices;
            subMeshes[2].triangles = backTriangles;
            //subMeshes[2].Optimize();
            //subMeshes[2].RecalculateNormals();

            //create bottom submesh
            subMeshes[3] = new Mesh();
            subMeshes[3].Clear();
            subMeshes[3].vertices = bottomVertices;
            subMeshes[3].triangles = bottomTriangles;
            //subMeshes[3].Optimize();
            //subMeshes[3].RecalculateNormals();

            //create left submesh
            subMeshes[4] = new Mesh();
            subMeshes[4].Clear();
            subMeshes[4].vertices = leftVertices;
            subMeshes[4].triangles = leftTriangles;
            //subMeshes[4].Optimize();
            //subMeshes[4].RecalculateNormals();

            //create right submesh
            subMeshes[5] = new Mesh();
            subMeshes[5].Clear();
            subMeshes[5].vertices = rightVertices;
            subMeshes[5].triangles = rightTriangles;
            //subMeshes[5].Optimize();
            //subMeshes[5].RecalculateNormals();

            //record bounds of individual submeshes (necessary for consitency later down the line even if nothing is merged with it.
            subMeshBounds[0].Add(new Bounds(subMeshes[0].bounds.center + position, subMeshes[0].bounds.size));
            subMeshBounds[1].Add(new Bounds(subMeshes[1].bounds.center + position, subMeshes[1].bounds.size));
            subMeshBounds[2].Add(new Bounds(subMeshes[2].bounds.center + position, subMeshes[2].bounds.size));
            subMeshBounds[3].Add(new Bounds(subMeshes[3].bounds.center + position, subMeshes[3].bounds.size));
            subMeshBounds[4].Add(new Bounds(subMeshes[4].bounds.center + position, subMeshes[4].bounds.size));
            subMeshBounds[5].Add(new Bounds(subMeshes[5].bounds.center + position, subMeshes[5].bounds.size));



        }

        public void OldSetTextures(Renderer renderer, Texture2D[] textures)
        {
            /// <summary>
            /// Textures should be organized as follows in the array: 
            /// top, front, back, left, right, bottom.
            /// </summary>
            Vector2[] frontUVs = new Vector2[subMeshes[0].vertexCount];
            Vector2[] topUVs = new Vector2[subMeshes[1].vertexCount];
            Vector2[] backUVs = new Vector2[subMeshes[2].vertexCount];
            Vector2[] bottomUVs = new Vector2[subMeshes[3].vertexCount];
            Vector2[] leftUVs = new Vector2[subMeshes[4].vertexCount];
            Vector2[] rightUVs = new Vector2[subMeshes[5].vertexCount];

            subMeshes[0].uv = OldFindUVs(subMeshes[0], subMeshBounds[0], 0);
            subMeshes[1].uv = OldFindUVs(subMeshes[1], subMeshBounds[1], 1);
            subMeshes[2].uv = OldFindUVs(subMeshes[2], subMeshBounds[2], 2);
            subMeshes[3].uv = OldFindUVs(subMeshes[3], subMeshBounds[3], 3);
            subMeshes[4].uv = OldFindUVs(subMeshes[4], subMeshBounds[4], 4);
            subMeshes[5].uv = OldFindUVs(subMeshes[5], subMeshBounds[5], 5);


        }

        Vector2[] OldFindUVs(Mesh mesh, List<Bounds> boundsList, int side)
        {
            Vector2[] uvs = new Vector2[mesh.vertexCount];
            int uvIndex = 0;

            int up = 1;
            int down = 1;
            float pix = 1f;
            int overshoot = 0;

            if (side == 0 | side == 2)
            {
                for (float z = mesh.bounds.min.z - down; z <= mesh.bounds.max.z + up; z += pix)
                {
                    for (float y = subMeshes[0].bounds.min.y - down; y <= mesh.bounds.max.y + up; y += pix)
                    {
                        for (float x = subMeshes[0].bounds.min.x - down; x <= mesh.bounds.max.x + up; x += pix)
                        {
                            foreach (Bounds bounds in boundsList)
                            {
                                if (bounds.Contains(new Vector3(x, y, z)) & uvIndex < uvs.Length)
                                {
                                    if (Math.Floor(x) % 2 == 0)
                                    {
                                        uvs[uvIndex] = new Vector2(0f, 0f);
                                        uvs[uvIndex + 1] = new Vector2(0f, 1f);
                                    }
                                    else
                                    {
                                        uvs[uvIndex] = new Vector2(1f, 0f);
                                        uvs[uvIndex + 1] = new Vector2(1f, 1f);


                                    }

                                    uvIndex += 2;
                                }
                                if (uvIndex >= uvs.Length)
                                {
                                    overshoot++;
                                }
                            }
                        }
                    }
                }
            }
            else if (side == 1 | side == 3)
            {
                // top topUVs
                for (float y = mesh.bounds.min.y - down; y <= mesh.bounds.max.y + up; y += pix)
                {
                    for (float z = mesh.bounds.min.z - down; z <= mesh.bounds.max.z + up; z += pix)
                    {
                        for (float x = mesh.bounds.min.x - down; x <= mesh.bounds.max.x + up; x += pix)
                        {

                            foreach (Bounds bounds in boundsList)
                            {
                                if (bounds.Contains(new Vector3(x, y, z)) & uvIndex < uvs.Length)
                                {
                                    if (Math.Floor(x) % 2 == 0)
                                    {
                                        uvs[uvIndex] = new Vector2(0f, 0f);
                                        uvs[uvIndex + +1] = new Vector2(0f, 1f);


                                    }
                                    else
                                    {
                                        uvs[uvIndex] = new Vector2(1f, 0f);
                                        uvs[uvIndex + 1] = new Vector2(1f, 1f);


                                    }

                                    uvIndex += 2;
                                }
                                if (uvIndex >= uvs.Length)
                                {
                                    overshoot++;
                                }
                            }
                        }
                    }
                }
            }

            else
            {
                for (float x = mesh.bounds.min.x - down; x <= mesh.bounds.max.x + up; x += pix)
                {
                    for (float y = mesh.bounds.min.y - down; y <= subMeshes[5].bounds.max.y + up; y += pix)
                    {
                        for (float z = mesh.bounds.min.z - down; z <= mesh.bounds.max.z + up; z += pix)
                        {
                            foreach (Bounds bounds in boundsList)
                            {
                                if (bounds.Contains(new Vector3(x, y, z)) & uvIndex < uvs.Length)
                                {
                                    if (Math.Floor(z) % 2 == 0)
                                    {
                                        uvs[uvIndex] = new Vector2(0f, 0f);
                                        uvs[uvIndex + 1] = new Vector2(0f, 1f);

                                    }
                                    else
                                    {
                                        uvs[uvIndex] = new Vector2(1f, 0f);
                                        uvs[uvIndex + 1] = new Vector2(1f, 1f);

                                    }

                                    uvIndex += 2;
                                }
                                if (uvIndex >= uvs.Length)
                                {
                                    overshoot++;
                                }
                            }
                        }
                    }
                }
            }

            Debug.Log("Overshoot: " + overshoot + " On side: " + side);
            Debug.Log("Vertex Count: " + mesh.vertexCount);

            return uvs;
        }

        public static MeshMaker CombinePlanes(List<MeshMaker> meshMakers)
        {
            MeshMaker outMesh = new MeshMaker();

            //combine planes of each individual meshmakers 6 sides to create one meshmaker with submeshes filled out by all the combined planes.
            for(int plane = 0; plane < 6; plane++)
            { 
                CombineInstance[] combine = new CombineInstance[meshMakers.Count];

                int i = 0;
                while (i < meshMakers.Count)
                {
                    combine[i].mesh = meshMakers[i].subMeshes[plane];
                    combine[i].transform = Matrix4x4.TRS(meshMakers[i].position, Quaternion.identity, Vector3.one);

                    outMesh.subMeshBounds[plane].AddRange(meshMakers[i].subMeshBounds[plane]);

                    i++;
                }

                outMesh.subMeshes[plane] = new Mesh();
                outMesh.subMeshes[plane].CombineMeshes(combine, true, true);
                outMesh.subMeshes[plane].Optimize();
            }

            return outMesh;
        }

        public void MergeSides()
        {
            CombineInstance[] combine = new CombineInstance[subMeshes.Length];

            int i = 0;
            while (i < subMeshes.Length)
            {
                combine[i].mesh = subMeshes[i];
                combine[i].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                        
                i++;
            }

            mesh = new Mesh();
            mesh.CombineMeshes(combine, true, true);
            mesh.Optimize();
            mesh.RecalculateNormals();

        }

        public void CreateCubeTexture(Texture2D[] textures)
        {
            Texture2D outTexture = new Texture2D(100 * 3, 100 * 4);
            Color[] empty = new Color[100 * 100];

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

            for (int i = 0; i < textures.Length + 6; i++)
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
        }
    }
}
