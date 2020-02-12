
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
//using CsvHelper;
using Newtonsoft.Json;
using System.IO;

namespace RedKite
{
    static class Utility
    {
        static System.Random rand = new System.Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static Node[] GenerateBoxRange(Vector3Int _startingSpot, int _distance)
        {
            List<Node> range = new List<Node>();

            Vector2 startingSpot = new Vector2(_startingSpot.x - _distance, _startingSpot.y - _distance);

            int distance = (_distance * 2) + 1;

            Vector3Int cell;

            Debug.Log("GenerateBoxRangeStart: " + _startingSpot);

            for (int i = 0; i < distance; i++)
            {
                for (int j = 0; j < distance; j++)
                {
                    cell = new Vector3Int((int)startingSpot.x + i, (int)startingSpot.y + j, -1);

                    if (cell.x >= 0 & cell.x < TileMapper.Instance.W & cell.y >= 0 & cell.y < TileMapper.Instance.H)
                    {
                        Debug.Log("GenerateBoxRange: " + cell);

                        range.Add(PathFinder.graph[cell.x, cell.y]);
                    }
                }
            }

            return range.ToArray();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
        (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }


        public static Vector3 Invert(this Vector3 vec)
        {
            return new Vector3(1 / vec.x, 1 / vec.y, 1 / vec.z);
        }

        public static float DirectedDist(Vector3 start, Vector3 end)
        {
            float outDist = 0;

            if (end.x < start.x | end.y < start.y)
                outDist = -Vector3.Distance(start, end);
            else
                outDist = Vector3.Distance(start, end);

            return outDist;
        }


        public static Vector3[] CoordRange(Vector3 first, Vector3 second)
        {
            List<Vector3> outVectors = new List<Vector3>();

            outVectors.Add(first);

            for (int i = 0; i < Vector3.Distance(first, second); i++)
            {
                outVectors.Add(Vector3.Lerp(first, second, (Vector3.Distance(first, second) - i) / Vector3.Distance(first, second)));

            }


            return outVectors.ToArray();
        }


        public static void LevelToJSON(Dictionary<int, Area> areas)
        {
            //var json = JsonConvert.SerializeObject(areas.Values, Newtonsoft.Json.Formatting.Indented);

            //File.WriteAllText(@"C:\Users\phoen\UnitySource\Red Kite Company\Assets\Data\LevelData.json", json);
        }

        /*public static void LevelToCSV(char[,] map)
        {

            FileStream fs = null;
            List<Dictionary<int, char>> mapExpo = new List<Dictionary<int, char>>();

            for (int y = 0; y < TileMapper.H; y++)
            {
                Dictionary<int, char> next = new Dictionary<int, char>();
                for (int x = 0; x < TileMapper.W; x++)
                {
                    next[x] = map[x, y];
                }
                mapExpo.Add(next);
            }



            try
            {
                fs = new FileStream(@"C:\Users\phoen\source\repos\SandBox\Data\LevelData.csv", FileMode.OpenOrCreate);
                using (var writer = new StreamWriter(fs, Encoding.UTF8))
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField("x,y");

                    foreach (KeyValuePair<int, char> record in mapExpo[0].OrderBy(x => x.Key))
                    {
                        csv.WriteField(record.Key);
                    }

                    csv.NextRecord();


                    for (int i = TileMapper.H - 1; i > -1; i--)
                    {
                        csv.WriteField(i);
                        foreach (KeyValuePair<int, char> record in mapExpo[i].OrderBy(x => x.Key))
                        {
                            csv.WriteField(record.Value);
                        }
                        csv.NextRecord();
                    }
                }
            }
            finally
            {
                if (fs != null)
                    fs.Dispose();
            }
        }
        */
        public static bool WithinBounds(Vector3 point, int width, int height)
        {
            bool tooHigh = point.y >= height;
            bool tooLow = point.y < 0;
            bool tooEast = point.x >= width;
            bool tooWest = point.x < 0;

            bool OOB = tooHigh | tooLow | tooEast | tooWest ? false : true;

            return OOB;
        }

        public static int ExclusiveRandom(int rangeStart, int rangeEnd, HashSet<int> exclude)
        {
            var range = Enumerable.Range(rangeStart, rangeEnd + rangeStart).Where(i => !exclude.Contains(i));

            int index = rand.Next(0, rangeEnd - exclude.Count);

            Debug.Log("range: " + range.ToList().Count);
            Debug.Log("index: " + index);
            return range.ElementAt(index);
        }

        public static int ManhattanDistance(Vector3Int a, Vector3Int b)
        {
            checked
            {
                return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
            }

        }

        public static int ManhattanDistance(Vector3 a, Vector3 b)
        {
            checked
            {
                return (int)Mathf.Abs(a.x - b.x) + (int)Mathf.Abs(a.y - b.y);
            }

        }

        public static Vector2 CartToIso(Vector2 cartesian)
        {
            return new Vector2(cartesian.x - cartesian.y, (cartesian.x + cartesian.y) / 2);
        }


    }
}
