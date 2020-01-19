using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
namespace RedKite
{ 
    public class SpriteSelection : MonoBehaviour
    {
        public RectTransform panel;
        public Dropdown[] dropdowns;
        Dropdown topWallTex;
        bool isActive = false;
        float menuCoolDown = .10f;
        float timesSinceCoolDown = 0;
        Modeler modeler;
        GameSprite[] units;
        // Start is called before the first frame update
        void Start()
        {
            dropdowns = GetComponentsInChildren<Dropdown>();
            modeler = FindObjectOfType<Modeler>();
            units = FindObjectsOfType<GameSprite>();
            dropdowns = dropdowns.OrderBy(x => Convert.ToInt32(x)).ToArray();
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) & timesSinceCoolDown >= menuCoolDown)
            {
                timesSinceCoolDown = 0;

                if(!isActive)
                {
                    isActive = true;

                    Debug.Log("Run");

                    panel.localPosition += new Vector3(0, 500, 0);
                    string path = Application.dataPath + "\\Sprites\\Tiles";

                    List<string> textures = Directory.GetFiles(path, "*.png").ToList<string>().Select(x => x.Substring(path.Length + 1)).ToList();

                    List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

                    foreach (string file in textures)
                    {
                        Dropdown.OptionData dat = new Dropdown.OptionData(file);
                        options.Add(dat);
                    }

                    options = options.OrderBy(x => x.text).ToList();

                    Debug.Log(options.Count);

                    dropdowns[0].AddOptions(options);
                    dropdowns[1].AddOptions(options);
                    dropdowns[2].AddOptions(options);

                    string path2 = Application.dataPath + "\\Sprites\\UnitSprites";

                    List<string> sprites = Directory.GetFiles(path2, "*.png").ToList<string>().Select(x => x.Substring(path2.Length + 1)).ToList();

                    List<Dropdown.OptionData> spriteOptions = new List<Dropdown.OptionData>();

                    foreach (string file in sprites)
                    {
                        Dropdown.OptionData dat = new Dropdown.OptionData(file);
                        spriteOptions.Add(dat);
                    }

                    spriteOptions = spriteOptions.OrderBy(x => x.text).ToList();

                    dropdowns[3].AddOptions(spriteOptions);
                    dropdowns[4].AddOptions(spriteOptions);

                }

                else if (isActive)
                {
                    Debug.Log("Run");
                    panel.localPosition -= new Vector3(0, 500, 0);
                    isActive = false;
                }
            }



            timesSinceCoolDown += Time.deltaTime;
        }

        public IEnumerator GetTextures(string textureName, int identity)
        {
            string path = "file:///" + Application.dataPath + "\\Sprites";

            if (identity == 3 | identity == 4)
                path += "\\UnitSprites\\" + textureName;
            else
                path += "\\Tiles\\" + textureName;

            Debug.Log("test");
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {                
                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

                if (identity == 0)
                    modeler.SetTopWallTexture(myTexture);
                else if (identity == 1)
                    modeler.SetSideWallTextures(myTexture);
                else if (identity == 2)
                    modeler.SetFloorTexture(myTexture);
                else if (identity == 3)
                    units[0].ReStart(myTexture);
                else
                    units[1].ReStart(myTexture);

            }
        }
    }
}
