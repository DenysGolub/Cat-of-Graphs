using Main.Enumerators;
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
        public static sbyte[,] ToAdjacenceMatrix(this Dictionary<int, HashSet<int>> adjacence_list)
        {
            sbyte[,] adjacence_matrix = new sbyte[adjacence_list.Keys.Count, adjacence_list.Keys.Count]; 
            /*switch (type)
            {
                case GraphType.Directed:
                    {
                  
                        break;
                    }
                case GraphType.Undirected:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }*/

            for (int height_dict = 0; height_dict < adjacence_list.Keys.Count; height_dict++)
            {
                for (int height_contains = 0; height_contains < adjacence_list.Keys.Count; height_contains++)
                {
                    if (adjacence_list[height_dict].Contains(height_contains))
                    {
                        adjacence_matrix[height_dict, height_contains] = 1;
                    }
                }
            }

            return adjacence_matrix;
        }
       /// <summary>
       /// Converts and adjacence list to Matrix of Incidence
       /// </summary>
       /// <param name="adjacence_list"></param>
       /// <returns>2D array (matrix) from adjacence list</returns>
        public static sbyte[,] ToIncidenceMatrix(this Dictionary<int, HashSet<int>> adjacence_list, GraphType type)
        {
            sbyte[,] incidence_matrix = new sbyte[adjacence_list.Keys.Count, adjacence_list.Values.Sum(hash=>hash.Count)];
            sbyte[,] adjadence_matrix = adjacence_list.ToAdjacenceMatrix();
            int count = 0;

            switch (type)
            {
                case GraphType.Directed:
                    {
                        break;
                    }
                case GraphType.Undirected:
                    {
                        for (int first_node = 0; first_node < adjadence_matrix.GetLength(0); first_node++)
                        {
                            for (int second_node = 0; second_node < adjadence_matrix.GetLength(1); second_node++)
                            {
                                if (adjadence_matrix[first_node, second_node] == 1)
                                {
                                    incidence_matrix[first_node, count] = 1;
                                    incidence_matrix[second_node, count] = 1;
                                    count++;
                                }
                            }
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return incidence_matrix;
        }
    }
}
