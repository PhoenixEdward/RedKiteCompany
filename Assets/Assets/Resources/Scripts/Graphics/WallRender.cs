using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{
    [ExecuteInEditMode]
    public class WallRender : MonoBehaviour
    {
        public Camera cam;
        public RenderTexture wallRender;
        public Shader ReplacementShader;

        // Start is called before the first frame update
        void Start()
        {
            cam.targetTexture = wallRender;
            cam.SetReplacementShader(ReplacementShader, "RenderType");
        }

    }
}