using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RedKite
{  public class QuestMapper : TileMapper
    {

        static QuestMapper _instance;

        //Needs a graph in which to place enemies and props. Can inherit from TileMapper.

        public new static QuestMapper Instance
        {
            get
            {
                _instance = _instance ?? (_instance = new QuestMapper());
                grid = Level.Instance.grid;
                return _instance;
            }
        }

        static Grid grid;
        public List<Enemy> enemies = new List<Enemy>();
        public Dictionary<Vector3Int, string> Props { get; private set; } = new Dictionary<Vector3Int, string>();
        int chestCount;

        Dictionary<int, int> distanceFromSpawn = new Dictionary<int, int>();

        PathFinder pathFinder;

        public override void Generate()
        {
            Props.Clear();
            distanceFromSpawn.Clear();

            chestCount = 5;

            pathFinder = new PathFinder();

            foreach(KeyValuePair<int, Area> area in TileMapper.Instance.Areas.Where(x=> x.Key != 0))
            {
                distanceFromSpawn.Add(area.Key, pathFinder.GeneratePathTo(TileMapper.Instance.Areas[0].Floor.Center,area.Value.Floor.Center).Count());
            }

            for(int i = 0; i < chestCount; i++)
            {
                foreach(KeyValuePair<int,int> area in distanceFromSpawn)
                {
                    double chance = rndState.Next(0, area.Value)/ distanceFromSpawn.Values.Average();

                    if(chance > 0.75d)
                    {
                        List<Vector3> coords = TileMapper.Instance.Areas[area.Key].GetCoords().ToList();

                        coords.Shuffle();

                        foreach(Vector3 coord in coords)
                            if(!Props.ContainsKey(grid.WorldToCell(coord)))
                            { 
                                Props.Add(grid.WorldToCell(coord), "chest");
                                break;
                            }

                        break;
                    }

                }
            }
        }
    }
}

