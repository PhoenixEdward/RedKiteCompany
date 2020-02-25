using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RedKite
{  public class QuestMapper
    {

        static QuestMapper _instance;

        //Needs a graph in which to place enemies and props. Can inherit from TileMapper.

        public static QuestMapper Instance
        {
            get
            {
                _instance = _instance ?? (_instance = new QuestMapper());
                grid = Level.Instance.grid;
                return _instance;
            }
        }

        static Grid grid;
        public Quest CurrentQuest { get; private set; }
        public List<Enemy> enemies = new List<Enemy>();
        public Dictionary<Vector3Int, Key> KeyChests = new Dictionary<Vector3Int, Key>();
        public Dictionary<Vector3Int, Skill> LootChests = new Dictionary<Vector3Int, Skill>();
        public Dictionary<Vector3Int, string> Props { get; private set; } = new Dictionary<Vector3Int, string>();
        int chestCount;
        int treeCount;
        System.Random rndState = new System.Random();

        Dictionary<int, int> distanceFromSpawn = new Dictionary<int, int>();

        PathFinder pathFinder;

        public void Generate(string questName)
        {
            CurrentQuest = QuestLoader.Instance.Load(questName);

            CurrentQuest.Instantiate();

            Props.Clear();
            distanceFromSpawn.Clear();

            chestCount = 5;

            pathFinder = new PathFinder();

            foreach(KeyValuePair<int, Area> area in TileMapper.Instance.Areas.Where(x=> x.Key != 0))
            {
                distanceFromSpawn.Add(area.Key, pathFinder.GeneratePathTo(TileMapper.Instance.Areas[0].Floor.Center,area.Value.Floor.Center, 10000).Count());
            }

            for(int i = 0; i < CurrentQuest.Keys.Count; i++)
            {
                //need to remove these from a list once occupied.
                foreach (KeyValuePair<int, int> area in distanceFromSpawn.OrderByDescending(x=> x.Value))
                {
                    Area curArea = TileMapper.Instance.Areas[area.Key];
                    List<Vector3> coords = curArea.GetCoords(true).ToList();
                    coords.Shuffle();

                    foreach (Vector3 coord in coords)
                    {
                        if (TileMapper.Instance.TryUpdateTile(Vector3Int.RoundToInt(coord), Cell.Type.PassableProp, curArea.RoomIndex))
                        {
                            Debug.Log("Key placed");
                            KeyChests.Add(Vector3Int.FloorToInt(coord), CurrentQuest.Keys[i]);
                            break;
                        }
                    }

                    break;
                }
            }

            for (int i = 0; i < CurrentQuest.Loot.Count; i++)
            {
                foreach(KeyValuePair<int,int> area in distanceFromSpawn)
                {
                    Area curArea = TileMapper.Instance.Areas[area.Key];

                    double chance = rndState.Next(0, area.Value)/ distanceFromSpawn.Values.Average();

                    if(chance > 0.65d)
                    {
                        List<Vector3> coords = curArea.GetCoords(true).ToList();

                        coords.Shuffle();

                        foreach(Vector3 coord in coords)
                        { 
                            if(TileMapper.Instance.TryUpdateTile(Vector3Int.RoundToInt(coord), Cell.Type.PassableProp, curArea.RoomIndex))
                            { 
                                LootChests.Add(Vector3Int.FloorToInt(coord), CurrentQuest.Loot[i]);
                                break;
                            }
                        }
                    }

                }
            }

            for (int i = 0; i < CurrentQuest.PropLoads.Count; i++)
            {
                foreach (KeyValuePair<int, Area> area in TileMapper.Instance.Areas)
                {
                    Area curArea = TileMapper.Instance.Areas[area.Key];

                    float chance = (area.Value.Floor.height * area.Value.Floor.width)/CurrentQuest.PropLoads[i].TilesPerUnit;

                    for(int j = 0; j < chance; j++)
                    { 
                        List<Vector3> coords = curArea.GetCoords(true).ToList();

                        coords.Shuffle();

                        foreach (Vector3 coord in coords)
                        { 

                            if (TileMapper.Instance.TryUpdateTile(Vector3Int.RoundToInt(coord), Cell.Type.PassableProp, curArea.RoomIndex))
                            {
                                //needs to prevent blocking paths

                                Props.Add(Vector3Int.FloorToInt(coord), CurrentQuest.PropLoads[i].Name);
                                break;
                            }
                        }
                    }
                }
            }

        }

        public IState LookupState()
        {

            if (CurrentQuest.state == Quest.State.Gather)
                return new GatherState();
            else
                return new IdleState();
        }
    }
}

