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

        public Texture2D[] subMeshTextures = new Texture2D[6];

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

            int frontVertCount = ((int)_scale.x * 2) * ((int)_scale.y * 2);
            int sideVertCount = ((int)_scale.z * 2) * ((int)_scale.y * 2); ;
            int topVertCount = ((int)_scale.x  * 2) * ((int)_scale.z * 2);

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
            graph = new int[((int)_scale.x + 1) * 2, ((int)_scale.y + 1) * 2, ((int)_scale.z + 1) * 2, 6];

            int index = 0;
            int heightIndex = 0;
            int widthIndex = 0;



            //get vertices on all axis.
            //back vertices
            for (int y = 0; y < ((int)_scale.y); y++)
            {
                widthIndex = 0;
                for (int x = 0; x < ((int)_scale.x); x++)
                {
                    graph[widthIndex + 1, heightIndex, (int)_scale.z, 0] = index;
                    backVertices[index] = new Vector3(x + 1, y, (int)_scale.z) + coordOffset;

                    graph[widthIndex, heightIndex, (int)_scale.z, 0] = index + 1;
                    backVertices[index + 1] = new Vector3(x, y, (int)_scale.z) + coordOffset;

                    graph[widthIndex + 1, heightIndex + 1, (int)_scale.z, 0] = index + 2;
                    backVertices[index + 2] = new Vector3(x + 1, y + 1, (int)_scale.z) + coordOffset;

                    graph[widthIndex, heightIndex + 1, (int)_scale.z, 0] = index + 3;
                    backVertices[index + 3] = new Vector3(x, y + 1, (int)_scale.z) + coordOffset;

                    widthIndex += 2;
                    index += 4;
                }
                heightIndex += 2;
            }

            index = 0;
            heightIndex = 0;
            //front vertices.
            for (int y = 0; y < ((int)_scale.y); y++)
            {
                widthIndex = 0;
                for (int x = 0; x < ((int)_scale.x); x++)
                {
                    graph[widthIndex + 1, heightIndex + 1, 0, 1] = index;
                    frontVertices[index] = new Vector3(x + 1, y + 1, 0) + coordOffset;

                    graph[widthIndex, heightIndex + 1, 0, 1] = index + 1;
                    frontVertices[index + 1] = new Vector3(x, y + 1, 0) + coordOffset;

                    graph[widthIndex + 1, heightIndex, 0, 1] = index + 2;
                    frontVertices[index + 2] = new Vector3(x + 1, y, 0) + coordOffset;

                    graph[widthIndex, heightIndex, 0, 1] = index + 3;
                    frontVertices[index + 3] = new Vector3(x, y, 0) + coordOffset;

                    widthIndex += 2;
                    index += 4;
                }
                heightIndex += 2;
            }

            index = 0;
            heightIndex = 0;
            //top Vertices
            for (int z = 0; z < ((int)_scale.z); z++)
            {
                widthIndex = 0;
                for (int x = 0; x < ((int)_scale.x); x++)
                {
                    graph[widthIndex + 1, (int)_scale.y, heightIndex + 1, 2] = index;
                    topVertices[index] = new Vector3(x + 1, (int)_scale.y, z + 1) + coordOffset;

                    graph[widthIndex, (int)_scale.y, heightIndex + 1, 2] = index + 1;
                    topVertices[index + 1] = new Vector3(x, (int)_scale.y, z + 1) + coordOffset;

                    graph[widthIndex + 1, (int)_scale.y, heightIndex, 2] = index + 2;
                    topVertices[index + 2] = new Vector3(x + 1, (int)_scale.y, z) + coordOffset;

                    graph[widthIndex, (int)_scale.y, heightIndex, 2] = index + 3;
                    topVertices[index + 3] = new Vector3(x, (int)_scale.y, z) + coordOffset;

                    widthIndex += 2;
                    index += 4;
                }
                heightIndex += 2;
            }

            index = 0;
            heightIndex = 0;
            //bottom vertices
            for (int z = 0; z < ((int)_scale.z); z++)
            {
                widthIndex = 0;
                for (int x = 0; x < ((int)_scale.x); x++)
                {
                    graph[widthIndex + 1, 0, heightIndex, 3] = index;
                    bottomVertices[index] = new Vector3(x + 1, 0, z) + coordOffset;

                    graph[widthIndex + 1, 0, heightIndex + 1, 3] = index + 1;
                    bottomVertices[index + 1] = new Vector3(x + 1, 0, z + 1) + coordOffset;

                    graph[widthIndex, 0, heightIndex + 1, 3] = index + 2;
                    bottomVertices[index + 2] = new Vector3(x, 0, z + 1) + coordOffset;

                    graph[widthIndex, 0, heightIndex, 3] = index + 3;
                    bottomVertices[index + 3] = new Vector3(x, 0, z) + coordOffset;

                    widthIndex += 2;
                    index += 4;
                }
                heightIndex += 2;
            }

            index = 0;
            heightIndex = 0;
            //left vertices
            for (int y = 0; y < ((int)_scale.y); y++)
            {
                widthIndex = 0;
                for (int z = 0; z < ((int)_scale.z); z++)
                {
                    graph[0, heightIndex, widthIndex + 1, 4] = index;
                    leftVertices[index] = new Vector3(0, y, z + 1) + coordOffset;

                    graph[0, heightIndex + 1, widthIndex + 1, 4] = index + 1;
                    leftVertices[index + 1] = new Vector3(0, y + 1, z + 1) + coordOffset;

                    graph[0, heightIndex + 1, widthIndex, 4] = index + 2;
                    leftVertices[index + 2] = new Vector3(0, y + 1, z) + coordOffset;

                    graph[0, heightIndex, widthIndex, 4] = index + 3;
                    leftVertices[index + 3] = new Vector3(0, y, z) + coordOffset;

                    widthIndex += 2;
                    index += 4;
                }
                heightIndex += 2;
            }

            index = 0;
            heightIndex = 0;
            //right vertices
            for (int y = 0; y < ((int)_scale.y); y++)
            {
                widthIndex = 0;
                for (int z = 0; z < ((int)_scale.z); z++)
                {
                    graph[(int)_scale.x, heightIndex, widthIndex, 5] = index;
                    rightVertices[index] = new Vector3((int)_scale.x, y, z) + coordOffset;

                    graph[(int)_scale.x, heightIndex + 1, widthIndex, 5] = index + 1;
                    rightVertices[index + 1] = new Vector3((int)_scale.x, y + 1, z) + coordOffset;

                    graph[(int)_scale.x, heightIndex + 1, widthIndex + 1, 5] = index + 2;
                    rightVertices[index + 2] = new Vector3((int)_scale.x, y + 1, z + 1) + coordOffset;

                    graph[(int)_scale.x, heightIndex, widthIndex + 1, 5] = index + 3;
                    rightVertices[index + 3] = new Vector3((int)_scale.x, y, z + 1) + coordOffset;

                    widthIndex += 2;
                    index+=4;
                }
                heightIndex += 2;
            }


            int triIndex = 0;
            //loop through 3D graph on all surfaces.
            //back triangles.
            for (int y = 0; y < (int)_scale.y * 2; y+=2)
            {
                for (int x = 0; x < (int)_scale.x * 2; x+=2)
                {
                    backTriangles[triIndex] = graph[x + 1, y, (int)_scale.z, 0];
                    backTriangles[triIndex + 1] = graph[x + 1, y + 1, (int)_scale.z, 0];
                    backTriangles[triIndex + 2] = graph[x, y + 1, (int)_scale.z, 0];

                    backTriangles[triIndex + 3] = graph[x + 1, y, (int)_scale.z, 0];
                    backTriangles[triIndex + 4] = graph[x, y + 1, (int)_scale.z, 0];
                    backTriangles[triIndex + 5] = graph[x, y, (int)_scale.z, 0];


                    triIndex += 6;
                }
            }

            triIndex = 0;
            //front triangles.
            for (int y = 0; y < (int)_scale.y*2; y+=2)
            {
                for (int x = 0; x < (int)_scale.x*2; x+=2)
                {
                    frontTriangles[triIndex] = graph[x + 1, y + 1, 0, 1];
                    frontTriangles[triIndex + 1] = graph[x + 1, y, 0, 1];
                    frontTriangles[triIndex + 2] = graph[x, y, 0, 1];

                    frontTriangles[triIndex + 3] = graph[x + 1, y + 1, 0, 1];
                    frontTriangles[triIndex + 4] = graph[x, y, 0, 1];
                    frontTriangles[triIndex + 5] = graph[x, y + 1, 0, 1];

                    triIndex += 6;
                }
            }

            triIndex = 0;
            //top triangles.
            for (int z = 0; z < (int)_scale.z*2; z += 2)
            {
                for (int x = 0; x < (int)_scale.x*2; x += 2)
                {

                    topTriangles[triIndex] = graph[x + 1, (int)_scale.y, z + 1, 2];
                    topTriangles[triIndex + 1] = graph[x + 1, (int)_scale.y, z, 2];
                    topTriangles[triIndex + 2] = graph[x, (int)_scale.y, z, 2];

                    topTriangles[triIndex + 3] = graph[x + 1, (int)_scale.y, z + 1, 2];
                    topTriangles[triIndex + 4] = graph[x, (int)_scale.y, z, 2];
                    topTriangles[triIndex + 5] = graph[x, (int)_scale.y, z + 1, 2];

                    triIndex += 6;
                }
            }

            triIndex = 0;
            //bottom bottomTriangles.
            for (int z = 0; z < (int)_scale.z*2; z += 2)
            {
                for (int x = 0; x < (int)_scale.x*2; x += 2)
                {
                    bottomTriangles[triIndex] = graph[x + 1, 0, z, 3];
                    bottomTriangles[triIndex + 1] = graph[x + 1, 0, z + 1, 3];
                    bottomTriangles[triIndex + 2] = graph[x, 0, z + 1, 3];

                    bottomTriangles[triIndex + 3] = graph[x + 1, 0, z, 3];
                    bottomTriangles[triIndex + 4] = graph[x, 0, z + 1, 3];
                    bottomTriangles[triIndex + 5] = graph[x, 0, z, 3];
                    triIndex += 6;
                }
            }

            triIndex = 0;
            //left triangles
            for (int y = 0; y < (int)_scale.y*2; y += 2)
            {
                for (int z = 0; z < (int)_scale.z*2; z += 2)
                {
                    leftTriangles[triIndex] = graph[0, y, z + 1, 4];
                    leftTriangles[triIndex + 1] = graph[0, y + 1, z + 1, 4];
                    leftTriangles[triIndex + 2] = graph[0, y + 1, z, 4];

                    leftTriangles[triIndex + 3] = graph[0, y, z + 1, 4];
                    leftTriangles[triIndex + 4] = graph[0, y + 1, z, 4];
                    leftTriangles[triIndex + 5] = graph[0, y, z, 4];

                    triIndex += 6;
                }
            }

            triIndex = 0;
            //right rightTriangles
            for (int y = 0; y < (int)_scale.y*2; y += 2)
            {
                for (int z = 0; z < (int)_scale.z*2; z += 2)
                {
                    rightTriangles[triIndex] = graph[(int)_scale.x, y, z, 5];
                    rightTriangles[triIndex + 1] = graph[(int)_scale.x, y + 1, z, 5];
                    rightTriangles[triIndex + 2] = graph[(int)_scale.x, y + 1, z + 1, 5];

                    rightTriangles[triIndex + 3] = graph[(int)_scale.x, y, z, 5];
                    rightTriangles[triIndex + 4] = graph[(int)_scale.x, y + 1, z + 1, 5];
                    rightTriangles[triIndex + 5] = graph[(int)_scale.x, y, z + 1, 5];

                    triIndex += 6;
                }
            }

            //create back submesh

            subMeshes[0] = new Mesh();
            subMeshes[0].Clear();
            subMeshes[0].vertices = backVertices;
            subMeshes[0].triangles = backTriangles;
            //subMeshes[0].Optimize();
            //subMeshes[0].RecalculateNormals();

            //create front submesh
            subMeshes[1] = new Mesh();
            subMeshes[1].Clear();
            subMeshes[1].vertices = frontVertices;
            subMeshes[1].triangles = frontTriangles;
            //subMeshes[1].Optimize();
            //subMeshes[1].RecalculateNormals();

            //create top submesh
            subMeshes[2] = new Mesh();
            subMeshes[2].Clear();
            subMeshes[2].vertices = topVertices;
            subMeshes[2].triangles = topTriangles;
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

        public void SetTextures(Renderer renderer, Texture2D[] textures, bool[] isMirrored)
        {
            /// <summary>
            /// Textures should be organized as follows in the array: 
            /// top, front, back, left, right, bottom.
            /// </summary>

            subMeshTextures[0] = textures[0];
            subMeshTextures[1] = textures[1];
            subMeshTextures[2] = textures[2];
            subMeshTextures[3] = textures[3];
            subMeshTextures[4] = textures[4];
            subMeshTextures[5] = textures[5];

            subMeshes[0].uv = FindUVs(subMeshes[0], subMeshBounds[0], 0, isMirrored[0]);
            subMeshes[1].uv = FindUVs(subMeshes[1], subMeshBounds[1], 1, isMirrored[1]);
            subMeshes[2].uv = FindUVs(subMeshes[2], subMeshBounds[2], 2, isMirrored[2]);
            subMeshes[3].uv = FindUVs(subMeshes[3], subMeshBounds[3], 3, isMirrored[3]);
            subMeshes[4].uv = FindUVs(subMeshes[4], subMeshBounds[4], 4, isMirrored[4]);
            subMeshes[5].uv = FindUVs(subMeshes[5], subMeshBounds[5], 5, isMirrored[5]);

            Texture2D cubeTex = CreateCubeTexture(subMeshTextures);

            renderer.material.mainTexture = cubeTex;

        }

        public void UpdateOneTexture(Renderer renderer, Texture2D texture, int side, bool isMirrored)
        {
            /// <summary>
            /// Textures should be organized as follows in the array: 
            /// top, front, back, left, right, bottom.
            /// </summary>

            subMeshTextures[side] = texture;

            subMeshes[side].uv = FindUVs(subMeshes[side], subMeshBounds[side], side, isMirrored);

            Texture2D updateTexture = CreateCubeTexture(subMeshTextures);

            renderer.material.mainTexture = updateTexture;
        }

        public void UpdateSideTextures(Renderer renderer, Texture2D texture, bool isMirrored)
        {
            /// <summary>
            /// Textures should be organized as follows in the array: 
            /// top, front, back, left, right, bottom.
            /// </summary>

            subMeshTextures[0] = texture;
            subMeshTextures[1] = texture;
            subMeshTextures[3] = texture;
            subMeshTextures[4] = texture;
            subMeshTextures[5] = texture;

            subMeshes[1].uv = FindUVs(subMeshes[1], subMeshBounds[1], 1, isMirrored);
            subMeshes[2].uv = FindUVs(subMeshes[2], subMeshBounds[2], 2, isMirrored);
            subMeshes[3].uv = FindUVs(subMeshes[3], subMeshBounds[3], 3, isMirrored);
            subMeshes[4].uv = FindUVs(subMeshes[4], subMeshBounds[4], 4, isMirrored);
            subMeshes[5].uv = FindUVs(subMeshes[5], subMeshBounds[5], 5, isMirrored);


            Texture2D updateTexture = CreateCubeTexture(subMeshTextures);



            renderer.material.mainTexture = updateTexture;
        }

        public Vector2[] FindUVs(Mesh mesh,List<Bounds> boundsList ,int side, bool isMirrored)
        {

            Vector2[] uvs = new Vector2[mesh.vertexCount];

            int[][][] rotOffset = new int[2][][];

            if (!isMirrored)
            { 
                for(int i = 0; i < rotOffset.GetLength(0); i++)
                {
                    rotOffset[i] = new int[4][];
                    for (int j = 0; j < rotOffset[i].Length; j++)
                        rotOffset[i][j] = new int[] {0,0};
                }
            }
            else
            {
                //potentially look at front and top. They are slightly different because first index starts on top.
                if(side == 0)
                {
                    rotOffset[0] = new int[][] { new int[] { 0, 1}, new int[] { 0, -1}, new int[] { 0, 1 }, new int[] { 0, -1} };
                    rotOffset[1] = new int[][] { new int[] { 2, 3 }, new int[] { 2, 1 }, new int[] { -2, -1 }, new int[] { -2, -3} };
                }
                if (side == 1)
                {
                    rotOffset[0] = new int[][] { new int[] { 0, 1 }, new int[] { 0, -1 }, new int[] { 0, 1 }, new int[] { 0, -1 } };
                    rotOffset[1] = new int[][] { new int[] { 2, 3 }, new int[] { 2, 1 }, new int[] { -2, -1 }, new int[] { -2, -3 } };
                }
                if (side == 2)
                {
                    rotOffset[0] = new int[][] { new int[] { 0, 1 }, new int[] { 0, -1 }, new int[] { 0, 1 }, new int[] { 0, -1 } };
                    rotOffset[1] = new int[][] { new int[] { 2, 3 }, new int[] { 2, 1 }, new int[] { -2, -1 }, new int[] { -2, -3 } };
                }
                if (side == 3)
                {
                    rotOffset[0] = new int[][] { new int[] { 0, 3 }, new int[] { 0, 1 }, new int[] { 0, -1 }, new int[] { 0, -3 } };
                    rotOffset[1] = new int[][] { new int[] { 1, 2 }, new int[] { -1, 2 }, new int[] { 1, -2 }, new int[] { -1, -2 } };

                }
                if (side == 4)
                {
                    rotOffset[0] = new int[][] { new int[] { 0, 3 }, new int[] { 0, 1 }, new int[] { 0, -1 }, new int[] { 0, -3 } };
                    rotOffset[1] = new int[][] { new int[] { 1, 2 }, new int[] { -1, 2 }, new int[] { 1, -2 }, new int[] { -1, -2 } };
                }
                if (side == 5)
                {
                    rotOffset[0] = new int[][] { new int[] { 0, 3 }, new int[] { 0, 1 }, new int[] { 0, -1 }, new int[] { 0, -3 } };
                    rotOffset[1] = new int[][] { new int[] { 1, 2 }, new int[] { -1, 2 }, new int[] { 1, -2 }, new int[] { -1, -2 } };
                }
            }

            int rotV;
            int rotH;
            int index = 0;

            if(side == 0)
            {
                //backUVs
                while (index < mesh.vertexCount)
                {
                    if (Math.Floor(mesh.vertices[index].y) % 2 == 0)
                        rotV = 0;
                    else
                        rotV = 1;
                    if (Math.Floor(mesh.vertices[index].x) % 2 == 0)
                        rotH = 0;
                    else
                        rotH = 1;

                    Debug.Log(rotV + " " + rotH);
                    //bottom right
                    uvs[index + rotOffset[rotV][0][rotH]] = new Vector2(0.65f, 0);
                    //bottom left
                    uvs[index + 1 + rotOffset[rotV][1][rotH]] = new Vector2(0.33f, 0);
                    //top right
                    uvs[index + 2 + rotOffset[rotV][2][rotH]] = new Vector2(0.66f, 0.24f);
                    //top left
                    uvs[index + 3 + rotOffset[rotV][3][rotH]] = new Vector2(0.33f, 0.25f);

                    index += 4;

                }

            }
            
            else if(side == 1)
            { 
                //front uvs
                while(index < mesh.vertexCount)
                {
                    if (Math.Floor(mesh.vertices[index].y) % 2 == 0)
                        rotV = 0;
                    else
                        rotV = 1;
                    if (Math.Floor(mesh.vertices[index].x) % 2 == 0)
                        rotH = 0;
                    else
                        rotH = 1;

                    //top right
                    uvs[index + rotOffset[rotV][0][rotH]] = new Vector3(0.65f,0.74f);
                    // top left
                    uvs[index + 1 + rotOffset[rotV][1][rotH]] = new Vector3(0.33f, 0.74f);
                    // bottom right;
                    uvs[index + 2 + rotOffset[rotV][2][rotH]] = new Vector2(0.65f, 0.50f);
                    //bottom left
                    uvs[index + 3 + rotOffset[rotV][3][rotH]] = new Vector2(0.33f, 0.50f);

                        index += 4;

                }
            }
            
            else if(side == 2)
            {
                //top uvs
                while (index < mesh.vertexCount)
                {
                    if (Math.Floor(mesh.vertices[index].z) % 2 == 0)
                        rotV = 0;
                    else
                        rotV = 1;
                    if (Math.Floor(mesh.vertices[index].x) % 2 == 0)
                        rotH = 0;
                    else
                        rotH = 1;

                    //top right
                    uvs[index + rotOffset[rotV][0][rotH]] = new Vector3(0.64f, 1);
                    //top left
                    uvs[index + 1 + rotOffset[rotV][1][rotH]] = new Vector3(0.33f, 1f);
                    //bottom right
                    uvs[index + 2 + rotOffset[rotV][2][rotH]] = new Vector3(0.64f, 0.75f);
                    //bottom left
                    uvs[index + 3 + rotOffset[rotV][3][rotH]] = new Vector3(0.33f, 0.75f);

                    index += 4;

                }

            }

            else if(side == 3)
            {
                //bottom uvs
                while (index < mesh.vertexCount)
                {

                    if (Math.Floor(mesh.vertices[index].z) % 2 == 0)
                        rotV = 0;
                    else
                        rotV = 1;
                    if (Math.Floor(mesh.vertices[index].x) % 2 == 0)
                        rotH = 0;
                    else
                        rotH = 1;

                    //bottom right
                    uvs[index + rotOffset[rotV][0][rotH]] = new Vector3(0.64f, 0.25f);
                    //top right
                    uvs[index + 1 + rotOffset[rotV][1][rotH]] = new Vector3(0.64f, 0.49f);
                    //top left
                    uvs[index + 2 + rotOffset[rotV][2][rotH]] = new Vector3(0.33f, 0.49f);
                    //bottom left
                    uvs[index + 3 + rotOffset[rotV][3][rotH]] = new Vector3(0.33f, 0.25f);

                    index += 4;

                }
            }

            else if(side == 4)
            {
                //left uvs
                while (index < mesh.vertexCount)
                {

                    if (Math.Floor(mesh.vertices[index].y) % 2 == 0)
                        rotV = 0;
                    else
                        rotV = 1;
                    if (Math.Floor(mesh.vertices[index].z) % 2 == 0)
                        rotH = 0;
                    else
                        rotH = 1;

                    //bottom left
                    uvs[index + rotOffset[rotV][0][rotH]] = new Vector3(0, 0.5f);
                    //top left
                    uvs[index + 1 + rotOffset[rotV][1][rotH]] = new Vector3(0, 0.74f);
                    //top right
                    uvs[index + 2 + rotOffset[rotV][2][rotH]] = new Vector3(0.32f, 0.74f);
                    //bottom right
                    uvs[index + 3 + rotOffset[rotV][3][rotH]] = new Vector3(0.32f, 0.5f);

                    index += 4;
                }
            }

            else
            {
                //right uvs
                while (index < mesh.vertexCount)
                {
                    if (Math.Floor(mesh.vertices[index].y) % 2 == 0)
                        rotV = 0;
                    else
                        rotV = 1;
                    if (Math.Floor(mesh.vertices[index].z) % 2 == 0)
                        rotH = 0;
                    else
                        rotH = 1;

                    //bottom left
                    uvs[index + rotOffset[rotV][0][rotH]] = new Vector3(0.66f, 0.5f);
                    //top left
                    uvs[index + 1 + rotOffset[rotV][1][rotH]] = new Vector3(0.66f, 0.74f); 
                    //top right
                    uvs[index + 2 + rotOffset[rotV][2][rotH]] = new Vector3(1f, 0.74f);
                    //bottom right
                    uvs[index + 3 + rotOffset[rotV][3][rotH]] = new Vector3(1f, 0.5f);

                    index += 4;
                }
            }

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
                //outMesh.subMeshes[plane].Optimize();
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
            //mesh.Optimize();
            mesh.RecalculateNormals();

        }

        public Texture2D CreateCubeTexture(Texture2D[] textures)
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

            int[] texOrder = new int[] {2,4,1,5,3,0};
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
                    Color[] pixels = textures[texOrder[texIndex]].GetPixels();
                    outTexture.SetPixels(
                        texCoords[i].x, texCoords[i].y, 100, 100,
                        pixels);
                    texIndex++;
                }
            }

            outTexture.Apply();

            return outTexture;
        }
    }
}
