using Main.Classes;
using Main.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Main.Interfaces
{
    internal interface IGraphOperations
    {
        private protected AdjacenceList Addition(Dictionary<int, HashSet<int>> dict, GraphType type);
        private protected AdjacenceList Unity(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2, GraphType g_type);
        Dictionary<int, HashSet<int>> Intersection(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2);
        Dictionary<int, HashSet<int>> CircleSum(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2);
        Dictionary<string, HashSet<string>> CartesianProduct(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2);
       
    }
}
