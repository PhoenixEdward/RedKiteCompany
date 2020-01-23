using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{ 
    public class SpriteRender : MonoBehaviour
    {
        public Camera cam;
        public RenderTexture unitRender;
        Shader UnitShader;
        List<GameObject> mirrors = new List<GameObject>();
        // Start is called before the first frame update
        void Start()
        {
            cam = GetComponent<Camera>();

            unitRender = new RenderTexture(Screen.width, Screen.height, 1);

            cam.targetTexture = unitRender;

            cam.SetReplacementShader(UnitShader, "RenderType");
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}