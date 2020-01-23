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
        int northWall;
        int southWall;
        int eastWall;
        int westWall;
        // Start is called before the first frame update
        void Start()
        {
            cam = GetComponent<Camera>();
            cam.SetReplacementShader(ReplacementShader, "RenderType");

            northWall = LayerMask.NameToLayer("NorthWall");
            southWall = LayerMask.NameToLayer("SouthWall");
            eastWall = LayerMask.NameToLayer("EastWall");
            westWall = LayerMask.NameToLayer("WestWall");

            cam.cullingMask = (1 << southWall) | (1 << westWall) | (1 << northWall) | (1 << eastWall);


            //wallRender = new RenderTexture(Screen.width, Screen.height, 1);

            //cam.targetTexture = wallRender;

            cam.depthTextureMode = DepthTextureMode.Depth;
        }

        // Update is called once per frame
        void Update()
        {
            /*if(CameraMovement.facing == CameraMovement.Facing.NE)
            {
                cam.cullingMask = (1 << southWall) | (1 << westWall);
            }
            else if(CameraMovement.facing == CameraMovement.Facing.NW)
            {
                cam.cullingMask = (1 << southWall) | (1 << eastWall);

            }
            else if (CameraMovement.facing == CameraMovement.Facing.SW)
            {
                cam.cullingMask = (1 << northWall) | (1 << eastWall);
            }
            else
            {
                cam.cullingMask = (1 << northWall) | (1 << westWall);
            }*/

        }
    }
}