using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

namespace RedKite
{ 
    public class FOW : MonoBehaviour
    {
        Camera cam;
        public Shader spriteColorShader;

        char[,] roomMap;

        char[,] fogMap;
        List<int> activeRooms;
        bool newRoom = false;
        char TILE_FOG = 'F';
        char TILE_CLEAR = 'C';
        char TILE_EDGE = 'E';
        char TILE_WALL = '#';

        public Shader fogShader;

        List<Hero> heroes;
        List<Enemy> enemies;
        List<Prop> props;

        Patch[,] patches;

        public Material partMaterial;

        System.Random rnd = new System.Random();

        //hide immediately if first spawn
        bool firstSpawn = true;

        // Start is called before the first frame update
        void Start()
        {
            cam = GameObject.FindGameObjectWithTag("FogCam").GetComponent<Camera>();
            cam.SetReplacementShader(spriteColorShader, "RenderType");
            cam.targetTexture = new RenderTexture(Screen.width, Screen.height, 1);

            roomMap = TileMapper.Instance.map;

            fogMap = new char[TileMapper.Instance.W, TileMapper.Instance.H];

            patches = new Patch[TileMapper.Instance.W, TileMapper.Instance.H];

            //instantiate the grid with fog cells and place them on tilemap
            for (int y = 0; y < TileMapper.Instance.H; y++)
            { 
                for (int x = 0; x < TileMapper.Instance.W; x++)
                {
                    if(roomMap[x,y] != TILE_WALL)
                    { 
                        fogMap[x, y] = TILE_FOG;
                        patches[x, y] = new Patch(rnd, new Vector3Int(x, 0, y), transform, partMaterial);
                    }
                }
            }



            heroes = GameSpriteManager.Instance.Heroes;
            enemies = GameSpriteManager.Instance.Enemies;
            props = GameSpriteManager.Instance.Props;

            activeRooms = new List<int>();
            
            //place inital fog tiles on tilemap
        }

        // Update is called once per frame
        void Update()
        {
            foreach(Patch patch in patches)
            {
                if(patch != null)
                { 
                    patch.Rotate();

                    if (patch.isHidden & !patch.hasDissipated)
                        patch.Dissipate(true);
                    else if (!patch.isHidden & patch.hasDissipated)
                        patch.Dissipate(false);
                }
            }

            List<int> currentRooms = new List<int>();

            foreach (Hero hero in heroes)
            {
                Vector3Int coord = hero.Coordinate;

                if((int)roomMap[coord.x, coord.y] - 48 >= 0 & (int)roomMap[coord.x, coord.y] - 48 <= 9)
                {
                    currentRooms.Add((int)roomMap[coord.x, coord.y] - 48);

                    if(!activeRooms.Contains((int)roomMap[coord.x, coord.y] - 48))
                    { 
                        activeRooms.Add((int)roomMap[coord.x, coord.y] - 48);
                    }

                }
            }



            foreach(int roomIndex in activeRooms)
            {
                Vector3[] edgeCoords = TileMapper.Instance.Areas[roomIndex].GetWallCoords();

                foreach (Vector3 coord in edgeCoords)
                {
                    if (currentRooms.Contains(roomIndex))
                        fogMap[(int)coord.x, (int)coord.z] = TILE_EDGE;
                    else
                    {
                        fogMap[(int)coord.x, (int)coord.z] = TILE_FOG;
                    }
                }
                Vector3[] clearCoords = TileMapper.Instance.Areas[roomIndex].GetCoords();

                foreach (Vector3 coord in clearCoords)
                {
                    if(currentRooms.Contains(roomIndex))
                    { 
                        fogMap[(int)coord.x, (int)coord.z] = TILE_CLEAR;


                        foreach (Prop prop in props)
                            if (prop.Coordinate.x == coord.x & prop.Coordinate.y == coord.z)
                                prop.IsVisible = true;
                    }
                    else
                    { 
                        fogMap[(int)coord.x, (int)coord.z] = TILE_FOG;
                    }
                }
            }


            for (int y = 1; y < TileMapper.Instance.H - 1; y++)
            {
                for (int x = 1; x < TileMapper.Instance.W - 1; x++)
                {
                    if(patches[x,y] != null)
                    { 
                        if (fogMap[x, y] == TILE_FOG)
                        {
                            patches[x, y].isHidden = false;
                        }
                        else if (fogMap[x, y] == TILE_EDGE &
                            !new char[] { fogMap[x, y + 1], fogMap[x, y - 1], fogMap[x + 1, y], fogMap[x - 1, y] }.All(q => q == TILE_EDGE | q == TILE_CLEAR))
                        {
                            patches[x, y].isHidden = false;
                        }
                        else
                        {
                            if (firstSpawn == true)
                            { 
                                patches[x, y].Hide(true);
                            }
                            else
                                patches[x, y].isHidden = true;
                        }
                    }

                }

            }

            if (firstSpawn == true)
                firstSpawn = false;
        }

        class Patch
        {
            public GameObject[] puffs;

            Vector3 coordinate;
            Vector3[] offsets;

            Vector3 dissapateScale = new Vector3(0.05f, 0.05f, 0);

            public bool isHidden = false;
            public bool hasDissipated = false;

            public Patch(System.Random rnd,Vector3Int _coordinate, Transform transform, Material partMaterial)
            {
                puffs = new GameObject[2];
                offsets = new Vector3[2];

                coordinate = new Vector3(_coordinate.x, 2.5f, _coordinate.z);

                puffs[0] = GameObject.CreatePrimitive(PrimitiveType.Quad);
                puffs[0].transform.SetParent(transform);
                puffs[0].transform.localScale = new Vector3(2f, 2f, 0);

                //give position random offset. subtract half from intial point so range for tile becomes somewhere between what the tile coords would be face down
                offsets[0] = new Vector3((float)rnd.NextDouble(), 0, (float)rnd.NextDouble());
                puffs[0].transform.position = coordinate + offsets[0];

                puffs[0].GetComponent<MeshRenderer>().material = partMaterial;

                puffs[0].layer = 10;


                puffs[1] = GameObject.CreatePrimitive(PrimitiveType.Quad);
                puffs[1].transform.SetParent(transform);
                puffs[1].transform.localScale = new Vector3(1.5f, 1.5f, 0);

                //give position random offset. subtract half from intial point so range for tile becomes somewhere between what the tile coords would be face down
                offsets[1] = new Vector3((float)rnd.NextDouble(), 0, (float)rnd.NextDouble());
                puffs[1].transform.position = coordinate + offsets[1];


                puffs[1].GetComponent<MeshRenderer>().material = partMaterial;

                puffs[1].layer = 10;
            }

            public void Hide(bool _hide)
            {
                puffs[0].GetComponent<MeshRenderer>().enabled = !_hide;
                puffs[1].GetComponent<MeshRenderer>().enabled = !_hide;

                if (_hide)
                    isHidden = true;
                else
                    isHidden = false;
            }

            public void Rotate()
            {
                if (CameraMovement.facing == CameraMovement.Facing.NE)
                {
                    puffs[0].transform.rotation = Quaternion.Euler(0, 45f, 0);
                    puffs[1].transform.rotation = Quaternion.Euler(0, 45f, 0);

                    puffs[0].transform.position = coordinate + offsets[0];
                    puffs[1].transform.position = coordinate + offsets[1];
                }
                else if (CameraMovement.facing == CameraMovement.Facing.SE)
                {
                    puffs[0].transform.rotation = Quaternion.Euler(0, 135f, 0);
                    puffs[1].transform.rotation = Quaternion.Euler(0, 135f, 0);

                    puffs[0].transform.position = coordinate + offsets[0] + new Vector3(0,0,1f);
                    puffs[1].transform.position = coordinate + offsets[1] + new Vector3(0, 0, 1f);
                }
                else if (CameraMovement.facing == CameraMovement.Facing.SW)
                {
                    puffs[0].transform.rotation = Quaternion.Euler(0, 225f, 0);
                    puffs[1].transform.rotation = Quaternion.Euler(0, 225f, 0);


                    puffs[0].transform.position = coordinate + offsets[0] + new Vector3(1f, 0, 1f);
                    puffs[1].transform.position = coordinate + offsets[1] + new Vector3(1f, 0, 1f);

                }
                else
                {
                    puffs[0].transform.rotation = Quaternion.Euler(0, 315f, 0);
                    puffs[1].transform.rotation = Quaternion.Euler(0, 315f, 0);

                    puffs[0].transform.position = coordinate + offsets[0] + new Vector3(1f, 0, 0);
                    puffs[1].transform.position = coordinate + offsets[1] + new Vector3(1f, 0, 0);

                }
            }

            public void Shrink(bool _isEdge)
            {
                if(_isEdge)
                { 
                    puffs[0].transform.localScale = new Vector3(1f, 1f, 1f);
                    puffs[1].transform.localScale = new Vector3(1f, 1f, 1f);
                }
                else
                {
                    puffs[0].transform.localScale = new Vector3(2f, 2f, 1f);
                    puffs[1].transform.localScale = new Vector3(2f, 2f, 1f);
                }

            }

            public void UpdateShader(Texture renderTarget)
            {
                puffs[0].GetComponent<MeshRenderer>().material.SetTexture("_MainTex3",renderTarget);
                puffs[1].GetComponent<MeshRenderer>().material.SetTexture("_MainTex3", renderTarget);
            }

            public void Dissipate(bool _dissipate)
            {
                bool dis1 = false;
                bool dis2 = false;

                if(_dissipate)
                { 
                    if (puffs[0].transform.localScale.x > 0 | puffs[0].transform.localScale.y > 0)
                        puffs[0].transform.localScale -= dissapateScale;
                    else
                        dis1 = true;

                    if (puffs[1].transform.localScale.x > 0 | puffs[1].transform.localScale.y > 0)
                        puffs[1].transform.localScale -= dissapateScale;
                    else
                        dis2 = true;

                    if (dis1 & dis2)
                        Hide(true);

                    hasDissipated = dis1 == true & dis2 == true ? true : false;
                }
                else
                {
                    Hide(false);

                    if (puffs[0].transform.localScale.x < 2 | puffs[0].transform.localScale.y < 2)
                        puffs[0].transform.localScale += dissapateScale;
                    else
                        dis1 = true;

                    if (puffs[1].transform.localScale.x < 2 | puffs[1].transform.localScale.y < 2)
                        puffs[1].transform.localScale += dissapateScale;
                    else
                        dis2 = true;  

                    hasDissipated = dis1 == true & dis2 == true ? false : true;
                }
            }
        }
    }
}