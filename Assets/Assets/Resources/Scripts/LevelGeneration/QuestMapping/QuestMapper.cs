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
        public Dictionary<Vector3Int, string> Props { get; private set; } = new Dictionary<Vector3Int, string>();
        int chestCount;
        int treeCount;
        System.Random rndState = new System.Random();

        Dictionary<int, int> distanceFromSpawn = new Dictionary<int, int>();

        PathFinder pathFinder;

        public void Generate()
        {
            Props.Clear();
            distanceFromSpawn.Clear();

            chestCount = 5;

            pathFinder = new PathFinder();

            foreach(KeyValuePair<int, Area> area in TileMapper.Instance.Areas.Where(x=> x.Key != 0))
            {
                distanceFromSpawn.Add(area.Key, pathFinder.GeneratePathTo(TileMapper.Instance.Areas[0].Floor.Center,area.Value.Floor.Center, 10000).Count());
            }

            for(int i = 0; i < chestCount; i++)
            {
                foreach(KeyValuePair<int,int> area in distanceFromSpawn)
                {
                    double chance = rndState.Next(0, area.Value)/ distanceFromSpawn.Values.Average();

                    if(chance > 0.65d)
                    {
                        List<Vector3> coords = TileMapper.Instance.Areas[area.Key].GetCoords(true).ToList();

                        coords.Shuffle();

                        foreach(Vector3 coord in coords)
                            if(!Props.ContainsKey(Vector3Int.FloorToInt(coord)))
                            { 
                                Props.Add(Vector3Int.FloorToInt(coord), "chest");
                                break;
                            }

                        break;
                    }

                }
            }

            treeCount = 1;

            for (int i = 0; i < treeCount; i++)
            {
                foreach (KeyValuePair<int, Area> area in TileMapper.Instance.Areas)
                {
                    float chance = (area.Value.Floor.height * area.Value.Floor.width)/30;

                    for(int j = 0; j < chance; j++)
                    { 
                        List<Vector3> coords = TileMapper.Instance.Areas[area.Key].GetCoords(true).ToList();

                        coords.Shuffle();

                        foreach (Vector3 coord in coords)
                        { 
                            if (!Props.ContainsKey(Vector3Int.FloorToInt(coord)))
                            {
                                //needs to prevent blocking paths

                                Props.Add(Vector3Int.FloorToInt(coord), "Evergreen");
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

