using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;

public class SpriteSelection : MonoBehaviour
{
    TMP_Dropdown[] dropDowns;
    TMP_Dropdown topWallTex;
    Dictionary<string,string> textures;
    bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        dropDowns = GetComponentsInChildren<TMP_Dropdown>(); 

        foreach(TMP_Dropdown dropdown in dropDowns)
        {
            if (dropdown.name == "TopWallTexture")
                topWallTex = dropdown;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!isActive)
            {
                isActive = true;

                foreach(TMP_Dropdown dropdown in dropDowns)
                {
                    dropdown.gameObject.SetActive(true);
                }

                string[] topWallTexNames = Directory.GetFiles(Application.dataPath + "\\Sprites\\TopWallTextures","*.png").Select(Path.GetFileNameWithoutExtension).ToArray();

                foreach(string filename in topWallTexNames)
                {
                    TMP_Dropdown.OptionData dat = new TMP_Dropdown.OptionData(filename);

                    Debug.Log(topWallTex.options[0]);

                    if (!topWallTex.options.Contains(dat))
                        topWallTex.options.Add(dat);
                        
                }
            }

            if(isActive)
            {
                foreach (TMP_Dropdown dropdown in dropDowns)
                {
                    dropdown.gameObject.SetActive(false);
                }

            }
        }
    }
}
