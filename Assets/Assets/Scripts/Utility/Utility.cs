
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

        public static VectorExtrema GetVectorExtrema(Vector3[] vecArray)
        {
            Vector3 tempMax = new Vector3(Vector3.negativeInfinity.x, 0, Vector3.negativeInfinity.z);
            Vector3 tempMin = new Vector3(Vector3.positiveInfinity.x,0,Vector3.positiveInfinity.z);

            foreach (Vector3 vector in vecArray)
            {
                if (tempMax.x < vector.x)
                    tempMax.x = vector.x;
                if (tempMin.x > vector.x)
                    tempMin.x = vector.x;
                if (tempMax.z < vector.z)
                    tempMax.z = vector.z;
                if (tempMin.z > vector.z)
                    tempMin.z = vector.z;
                if (tempMax.y < vector.y)
                    tempMax.y = vector.y;
                if (tempMin.y > vector.y)
                    tempMin.y = vector.y;
            }

            Vector3 outMin = tempMin;
            Vector3 outMax = tempMax;

            return new VectorExtrema(outMin, outMax);

        }

        public static float DirectedDist(Vector3 start, Vector3 end)
        {
            float outDist = 0;

            if (end.x > start.x | end.z > start.z)
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
                if (first.x < second.x)
                    outVectors.Add(Vector3.Lerp(first + (Vector3.right * i), second, 1 / Vector3.Distance(first + (Vector3.right * i), second)));
                else if (second.x < first.x)
                    outVectors.Add(Vector3.Lerp(first + (Vector3.left * i), second, 1 / Vector3.Distance(first + (Vector3.left * i), second)));
                else if (first.z < second.z)
                    outVectors.Add(Vector3.Lerp(first + (Vector3.forward * i), second, 1 / Vector3.Distance(first + (Vector3.forward * i), second)));
                else if (second.z < first.z)
                    outVectors.Add(Vector3.Lerp(first + (Vector3.back * i), second, 1 / Vector3.Distance(first + (Vector3.back * i), second)));

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
            bool tooHigh = point.z >= height;
            bool tooLow = point.z < 0;
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
                return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
            }

        }

        public static Vector3Int CartToIso(Vector3 cartesian)
        {
            return Vector3Int.RoundToInt(new Vector3(cartesian.x - cartesian.y, (cartesian.x + cartesian.y) / 2, cartesian.z));
        }


    }

    public struct VectorExtrema
    {
        public Vector3 min;
        public Vector3 max;
        public float width;
        public float height;
        public VectorExtrema(Vector3 _min, Vector3 _max)
        {
            min = _min;
            max = _max;
            width = _max.x - _min.x + 1;
            height = _max.z - _min.z + 1;
        }

    }
}
