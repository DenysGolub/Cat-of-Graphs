using Main.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.InstrumentalPart
{
    static class Degree
    {
        public static Dictionary<int, int> GetDegree(AdjacenceList list)
        {
            var deg = new Dictionary<int, int>();

            for(int i = 1; i<=list.GetList.Keys.Count; i++)
            {
                int count = 0;

                foreach(int j in list[i])
                {
                    count++;
                }
                deg.Add(i, count);
            }

            return deg;
        }
    }
}
