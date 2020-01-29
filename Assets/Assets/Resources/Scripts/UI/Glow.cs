using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RedKite
{ 
    public class Glow : MonoBehaviour
    {
        RawImage img;
        List<Unit> units;
        public WallRender wallRender;

        public Camera cam;
        public Shader UnitShader;
        // Start is called before the first frame update
        void Start()
        {
            cam.SetReplacementShader(UnitShader, "RenderType");

            img = GetComponentInChildren<RawImage>();
            img.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

            units = GameSpriteManager.Instance.Units;
        }

        private void Update()
        {
            foreach(Unit unit in units)
            {
                unit.mirrorRender.material.SetTexture("_MainTex2", wallRender.wallRender);

                Vector3 point;

                if (CameraMovement.facing == CameraMovement.Facing.NE)
                    point = unit.transform.position + new Vector3(0.5f, 0, 0.5f);
                else if (CameraMovement.facing == CameraMovement.Facing.SE)
                    point = unit.transform.position + new Vector3(0.5f, 0, -0.5f);
                else if (CameraMovement.facing == CameraMovement.Facing.SW)
                    point = unit.transform.position + new Vector3(-0.5f, 0, -0.5f);
                else
                    point = unit.transform.position + new Vector3(-0.5f, 0, 0.5f);

                Vector3 direction = point - Camera.main.transform.position;
                Ray ray = new Ray(Camera.main.transform.position, direction);
                RaycastHit hit;
                Debug.DrawRay(Camera.main.transform.position, direction, Color.red);
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << 9)))
                {
                    if (Vector3.Distance(Camera.main.transform.position, hit.point) < Vector3.Distance(Camera.main.transform.position, point))
                        unit.mirrorRender.material.SetInt("_Covered", 1);
                    else
                        unit.mirrorRender.material.SetInt("_Covered", 0);

                }
                else
                {
                    unit.mirrorRender.material.SetInt("_Covered", 0);
                }

                img.texture = cam.activeTexture;
            }
        }

        public void Regen()
        {
            units.Clear();
            units = GameSpriteManager.Instance.Units;
        }
    }
}