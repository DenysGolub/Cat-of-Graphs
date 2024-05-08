using Main.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.InstrumentalPart
{
    public static class RegularGraph
    {
       public static bool IsRegular(AdjacenceList list, out string sequence)
        {
            List<int> degree_sequence = new List<int>();
            int max_degree = 0;
            sequence = "";
            for (int i = 1; i<list.GetList.Keys.Count; i++)
            {
                int count = 0;
                foreach (var adj in list[i])
                {
                    count++;
                }
                if (count > max_degree)
                {
                    max_degree = count;
                }

                degree_sequence.Add(count);
            }

            foreach(var degree in degree_sequence)
            {
                if(degree != max_degree)
                {
                    sequence = "Граф не є регулярним";
                    return false;
                }
            }

            sequence = $"Граф є {max_degree}-регулярним із степінню {list.GetList.Keys.Count}";

            return true;
        }
    }
}
