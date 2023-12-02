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
        Dictionary<int, HashSet<int>> Addition(Dictionary<int, HashSet<int>> dict);
        Dictionary<int, HashSet<int>> Unity(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2);
        Dictionary<int, HashSet<int>> Intersection(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2);
        Dictionary<int, HashSet<int>> CircleSum(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2);
        Dictionary<string, HashSet<string>> CartesianProduct(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2);
       
    }
}
