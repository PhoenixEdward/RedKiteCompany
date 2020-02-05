using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using UnityEditor;

namespace RedKite
{ 
    public class SpriteSelection : MonoBehaviour
    {
        public RectTransform panel;
        public Dropdown[] dropdowns;
        public GameObject propCreator;
        GameObject propCreateInstance;
        Dropdown topWallTex;
        public bool isActive = false;
        public bool texturerActive = false;
        public bool propCreatorActive = false;
        float menuCoolDown = .10f;
        float timesSinceCoolDown = 0;
        Modeler modeler;
        Hero[] units;
        Reticle reticle;
        Canvas canvas;
        float scaleFactor;
        float timeSinceDeselect;
        Grid grid;
        // Start is called before the first frame update
        void OnEnable()
        {
            canvas = FindObjectOfType<Canvas>();
            scaleFactor = canvas.scaleFactor;

            dropdowns = GetComponentsInChildren<Dropdown>();
            modeler = FindObjectOfType<Modeler>();
            units = GameSpriteManager.Instance.Heroes.ToArray();
            dropdowns = dropdowns.OrderBy(x => Convert.ToInt32(x)).ToArray();
            reticle = FindObjectOfType<Reticle>();
            grid = FindObjectOfType<Grid>();
        }

        // Update is called once per frame
        void Update()
        {
            canvas.transform.rotation = Camera.main.transform.rotation;

            if(Input.GetKeyDown(KeyCode.Space) & timesSinceCoolDown >= menuCoolDown & !propCreatorActive)
            {
                timesSinceCoolDown = 0;

                if(!isActive)
                {
                    isActive = true;
                    texturerActive = true;

                    panel.localPosition = new Vector3(0 + Screen.width/2, 0 + Screen.height/2, 0);
                    string path = Application.dataPath + "\\Sprites\\Tiles";

                    List<string> textures = Directory.GetFiles(path, "*.png").ToList<string>().Select(x => x.Substring(path.Length + 1)).ToList();

                    List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

                    foreach (string file in textures)
                    {
                        Dropdown.OptionData dat = new Dropdown.OptionData(file);
                        options.Add(dat);
                    }

                    options = options.OrderBy(x => x.text).ToList();

                    dropdowns[0].ClearOptions();
                    dropdowns[1].ClearOptions();
                    dropdowns[2].ClearOptions();

                    dropdowns[0].AddOptions(options);
                    dropdowns[1].AddOptions(options);
                    dropdowns[2].AddOptions(options);

                    try
                    {
                        dropdowns[0].SetValueWithoutNotify(dropdowns[0].options.FindIndex(x => x.text == modeler.topWallTexName + ".png"));
                        dropdowns[1].SetValueWithoutNotify(dropdowns[1].options.FindIndex(x => x.text == modeler.wallTexName + ".png"));
                        dropdowns[2].SetValueWithoutNotify(dropdowns[2].options.FindIndex(x => x.text == modeler.floorTexName + ".png"));
                    }
                    catch
                    {
                        Debug.LogError("Default Sprite Has Been Removed");
                    };

                    string path2 = Application.dataPath + "\\Sprites\\UnitSprites";

                    List<string> sprites = Directory.GetFiles(path2, "*.png").ToList<string>().Select(x => x.Substring(path2.Length + 1)).ToList();

                    List<Dropdown.OptionData> spriteOptions = new List<Dropdown.OptionData>();

                    foreach (string file in sprites)
                    {
                        Dropdown.OptionData dat = new Dropdown.OptionData(file);
                        spriteOptions.Add(dat);
                    }

                    spriteOptions = spriteOptions.OrderBy(x => x.text).ToList();

                    dropdowns[3].ClearOptions();
                    dropdowns[4].ClearOptions();

                    dropdowns[3].AddOptions(spriteOptions);
                    dropdowns[4].AddOptions(spriteOptions);

                    try
                    {
                        dropdowns[3].SetValueWithoutNotify(dropdowns[3].options.FindIndex(x => x.text == units[0].spriteName + ".png"));
                        dropdowns[4].SetValueWithoutNotify(dropdowns[4].options.FindIndex(x => x.text == units[1].spriteName + ".png"));
                    }
                    catch
                    {
                        Debug.LogError("Default Sprite Has Been Removed");
                    };


                }

                else if (isActive)
                {
                    panel.localPosition = new Vector3(0 + Screen.width / 2, - Screen.height/2, 0);
                    isActive = false;
                    texturerActive = false;
                }
            }

            if (reticle.selectedHero == null)
            {
                timeSinceDeselect += Time.deltaTime;

                if (Input.GetMouseButtonDown(1) & timeSinceDeselect > menuCoolDown)
                {
                    if (!texturerActive & !isActive)
                    { 
                        isActive = true;
                        propCreatorActive = true;

                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                        {
                            if (propCreateInstance == null)
                            {
                                propCreateInstance = Instantiate(propCreator);
                            }

                            propCreateInstance.transform.SetParent(transform);
                            propCreateInstance.transform.localScale = Vector3.one;
                            Vector3 pos = Camera.main.WorldToScreenPoint(hit.point);
                            pos = new Vector2(pos.x - Screen.width/2, pos.y - Screen.height/2);
                            propCreateInstance.GetComponent<RectTransform>().localPosition = pos;

                            string path3 = Application.dataPath + "\\Sprites\\PropSprites";

                            List<string> sprites = Directory.GetFiles(path3, "*.png").ToList<string>().Select(x => x.Substring(path3.Length + 1)).ToList();

                            List<Dropdown.OptionData> spriteOptions = new List<Dropdown.OptionData>();

                            foreach (string file in sprites)
                            {
                                Dropdown.OptionData dat = new Dropdown.OptionData(file);
                                spriteOptions.Add(dat);
                            }

                            UIPropCreator propCreate = propCreateInstance.transform.GetChild(2).gameObject.AddComponent<UIPropCreator>();
                            propCreate.position = new Vector3Int(reticle.highlight.x, reticle.highlight.y, -1);

                            UIPropCreator propDelete = propCreateInstance.transform.GetChild(1).gameObject.AddComponent<UIPropCreator>();
                            propDelete.position = new Vector3Int(reticle.highlight.x, reticle.highlight.y, -1);

                            propCreateInstance.GetComponentInChildren<Dropdown>().AddOptions(spriteOptions);

                        }

                    }
                    else
                    {
                        Destroy(propCreateInstance);
                        propCreatorActive = false;
                        isActive = false;
                    }
                }

            }
            else
            {
                timeSinceDeselect = 0;
            }

            timesSinceCoolDown += Time.deltaTime;
        }

        public IEnumerator GetTextures(string textureName, int identity, bool isMirrored = false)
        {
            string path = "file:///" + Application.dataPath + "\\Sprites";

            if (identity == 3 | identity == 4)
                path += "\\UnitSprites\\" + textureName;
            else
                path += "\\Tiles\\" + textureName;

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
                    modeler.SetTopWallTexture(textureName, myTexture, isMirrored);
                else if (identity == 1)
                    modeler.SetSideWallTextures(textureName, myTexture, isMirrored);
                else if (identity == 2)
                    modeler.SetFloorTexture(textureName, myTexture, isMirrored);
                else if (identity == 3)
                    units[0].ReStart(textureName, myTexture);
                else if(identity == 4)
                    units[1].ReStart(textureName, myTexture);

            }
        }

        public IEnumerator GetPropTextures(string textureName, Vector3Int position, bool isIso)
        {
            string path = "file:///" + Application.dataPath + "\\Sprites";

            path += "\\PropSprites\\" + textureName;


            UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

                Level.Instance.AddProp(position, textureName.Substring(0, textureName.Length - 4), myTexture, isIso);
            }
        }
    }
}
