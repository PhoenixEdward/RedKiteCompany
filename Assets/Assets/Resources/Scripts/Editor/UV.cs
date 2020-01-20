using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RedKite
{ 
    public class UV : Editor
    {
        [CustomEditor(typeof(Testing))]
        void OnSceneGUI()
        {
            Handles.BeginGUI();
            Handles.color = Color.blue;

            Testing cubeController = (Testing)target;

            Mesh mesh = cubeController.gameObject.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            int i = 0;
            while (i < vertices.Length)
            {
                Vector3 position = vertices[i] * (1 + i * 0.05f);

                Handles.Label(position, "[" + i + "]");
                i++;
            }



            Handles.EndGUI();
        }
    }
}