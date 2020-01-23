using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRender : MonoBehaviour
{
    public Camera cam;
    RenderTexture wallRender;
    public Shader UnitShader;
    // Start is called before the first frame update
    void OnEnable()
    {
        cam = GetComponent<Camera>();

        //wallRender = new RenderTexture(Screen.width, Screen.height, 1);

        //cam.targetTexture = wallRender;

        cam.SetReplacementShader(UnitShader, "RenderType");
    }

    private void OnDisable()
    {
        cam.ResetReplacementShader();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
