using Main.Enumerators;
using Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Security.Permissions;

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

            for (int height_dict = 1; height_dict <= adjacence_list.Keys.Count; height_dict++)
            {
                for (int height_contains = 1; height_contains <= adjacence_list.Keys.Count; height_contains++)
                {
                    if (adjacence_list[height_dict].Contains(height_contains))
                    {
                        adjacence_matrix[height_dict-1, height_contains-1] = 1;
                    }
                }
            }

            return adjacence_matrix;
        }
        static private List<string> GetLines(this Dictionary<int, HashSet<int>> adjacence_list, ref Canvas canvas)
        {
            var list = new List<string>();

            var matrix = adjacence_list.ToAdjacenceMatrix();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (canvas.Children.Cast<FrameworkElement>()
                      .Any(x => x.Name != null && x.Name.ToString() == $"line_{i + 1}_{j + 1}"))
                    {
                        list.Add($"line_{i + 1}_{j + 1}");
                    }
                }
            }

            return list;
        }
        static public List<string> GetLines(this Dictionary<int, HashSet<int>> adjacence_list)
        {
            var list = new List<string>();

            var matrix = adjacence_list.ToAdjacenceMatrix();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == 1)
                    {
                        list.Add($"line_{i + 1}_{j + 1}");
                    }
                }
            }

            return list;
        }
        /// <summary>
        /// Converts an adjacence list to Matrix of Incidence
        /// </summary>
        /// <param name="adjacence_list"></param>
        /// <returns>2D array (matrix) from adjacence list</returns>
        public static sbyte[,] ToIncidenceMatrix(this Dictionary<int, HashSet<int>> adjacence_list, GraphType type, ref Canvas canv, out List<string> lineNames)
        {
            lineNames = adjacence_list.GetLines(ref canv);
            sbyte[,] incidence_matrix = new sbyte[adjacence_list.Keys.Count, lineNames.Count];
            int count = 0;

            switch (type)
            {
                case GraphType.Directed:
                    break;
                case GraphType.Undirected:
                    break;
                default:
                    break;
            }

            switch (type)
            {
                case GraphType.Directed:
                    {
                        foreach (string line in lineNames)
                        {
                            line.EdgesNames(out int f_node, out int s_node);

                            if (f_node == s_node)
                            {
                                incidence_matrix[f_node - 1, count] = 2;
                            }
                            else
                            {
                                incidence_matrix[f_node - 1, count] = -1;
                                incidence_matrix[s_node - 1, count] = 1;
                            }
                            count++;
                        }
                        break;
                    }
                case GraphType.Undirected:
                    {
                        foreach (string line in lineNames)
                        {

                            line.EdgesNames(out int f_node, out int s_node);

                            incidence_matrix[f_node - 1, count] = 1;
                            incidence_matrix[s_node - 1, count] = 1;
                            count++;
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
