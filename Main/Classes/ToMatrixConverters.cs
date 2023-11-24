using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Classes
{
    static class ToMatrixConverters
    {
        /// <summary>
        /// Converts an adjacence list to Matrix of Adjacence
         /// </summary>
        /// <param name="adjacence_list"></param>
        /// <returns>2D array (matrix) from adjacence list</returns>
        public static int[,] To2DArray(this Dictionary<int, int> adjacence_list)
        {

        }
        /// <summary>
        /// Converts an edges list to Matrix of Incidence
        /// </summary>
        /// <param name="edges_list"></param>
        /// <returns>2D array (matrix) from edges list</returns>
        public static int[,] To2DArray(this HashSet<string> edges_list)
        {

        }
    }
}
