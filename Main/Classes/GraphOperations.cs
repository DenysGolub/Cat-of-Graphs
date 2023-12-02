using Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Main.Classes
{
    class GraphOperations : IGraphOperations
    {
        public Dictionary<int, HashSet<int>> Addition(Dictionary<int, HashSet<int>> dict)
        {
            var addition_dict = new Dictionary<int, HashSet<int>>();
           
            for(int dict_keys=1; dict_keys<=dict.Keys.Count; dict_keys++)
            {
                addition_dict[dict_keys] = new HashSet<int>();
                for (int number=1; number<=dict.Keys.Count; number++)
                {
                    if (!dict[dict_keys].Contains(number))
                    {
                        addition_dict[dict_keys].Add(number);
                    }
                }
            }
            return addition_dict;
        }

        public Dictionary<string, HashSet<string>> CartesianProduct(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2)
        {
            var dekart_dict = GetAllPairsForCartesian(dict_g1, dict_g2);
            
            foreach(var kvp_1 in dekart_dict)
            {
                kvp_1.Key.DoubleNodeName(out int x1, out int y1);
                foreach(var kvp_2 in dekart_dict)
                {
                    kvp_2.Key.DoubleNodeName(out int x2, out int y2);

                    if((x1==x2 && dict_g2[y1].Contains(y2)) || (y1 == y2 && dict_g1[x1].Contains(x2)))
                    {
                        dekart_dict[kvp_1.Key].Add(kvp_2.Key);
                    }
                }
            }

            return dekart_dict;
        }

        public Dictionary<int, HashSet<int>> CircleSum(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2)
        {
            var circle_sum_dict = new Dictionary<int, HashSet<int>>();
            foreach (KeyValuePair<int, HashSet<int>> kvp_g1 in dict_g1.Keys.Count >= dict_g2.Keys.Count ? dict_g1 : dict_g2)
            {
                circle_sum_dict[kvp_g1.Key] = kvp_g1.Value;
                if (dict_g2.ContainsKey(kvp_g1.Key))
                {
                    foreach (var item in dict_g2[kvp_g1.Key])
                    {
                        if(!kvp_g1.Value.Contains(item))
                        {
                            circle_sum_dict[kvp_g1.Key].Add(item);
                        }
                    }
                }


            }

            return circle_sum_dict;
        }

        public Dictionary<int, HashSet<int>> Intersection(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2)
        {
            var intersection_dict = new Dictionary<int, HashSet<int>>();
            foreach (KeyValuePair<int, HashSet<int>> kvp_g1 in dict_g1.Keys.Count <= dict_g2.Keys.Count ? dict_g1 : dict_g2)
            {
                intersection_dict[kvp_g1.Key] = kvp_g1.Value;
                if (dict_g2.ContainsKey(kvp_g1.Key))
                {
                    intersection_dict[kvp_g1.Key].Intersect(dict_g2[kvp_g1.Key]);
                }
                else
                {
                    break;
                }
            }

            return intersection_dict;
        }

        public Dictionary<int, HashSet<int>> Unity(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2)
        {
            ///5 вершин у g1 i 2 вершини в другому
            /// 2 вершини
            var unity_dict = new Dictionary<int, HashSet<int>>();  
            foreach(KeyValuePair<int, HashSet<int>> kvp_g1 in dict_g1.Keys.Count>=dict_g2.Keys.Count?dict_g1:dict_g2)
            {
                unity_dict[kvp_g1.Key] = kvp_g1.Value;
                if (dict_g2.ContainsKey(kvp_g1.Key))
                {
                    unity_dict[kvp_g1.Key].Union(dict_g2[kvp_g1.Key]);
                }
                
            }

            return unity_dict;
        }
        private Dictionary<string, HashSet<string>> GetAllPairsForCartesian(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2)
        {
            var dekart_dict = new Dictionary<string, HashSet<string>>();
            foreach (var kvp_g1 in dict_g1.Keys.Count >= dict_g2.Keys.Count ? dict_g1 : dict_g2)
            {
                foreach (var kvp_g2 in dict_g1.Keys.Count >= dict_g2.Keys.Count ? dict_g2 : dict_g1)
                {
                    dekart_dict[$"{kvp_g1.Key},{kvp_g2.Key}"] = new HashSet<string>();
                }
            }
            return dekart_dict;
        }

    }
}
