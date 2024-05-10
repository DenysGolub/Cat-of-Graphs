using Main.Enumerators;
using Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Main.Classes
{
    /// <summary>
    /// Class that imitates adjacence list of graph. Supports adding or deleting node (edge). Also
    /// has converters to Matrix of Adjacence or Matrix of Incidence
    /// </summary>
    public class AdjacenceList : IAdjacenceList
    {
        Dictionary<int, HashSet<int>> adjacence_list = new Dictionary<int, HashSet<int>>();
        GraphType type;
        public AdjacenceList()
        {

        }


        public AdjacenceList(AdjacenceList old_list)
        {
            this.type = old_list.Type;
            this.adjacence_list = new Dictionary<int, HashSet<int>>(old_list.GetList);
        }

        public AdjacenceList(GraphType type)
        {
            this.type = type;
        }
        /// <summary>
        /// Indexator to get connected edges of current node by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public HashSet<int> this[int index]
        {
            get { return adjacence_list[index]; }
            set { adjacence_list[index] = value; }
        }

        public int CountNodes => adjacence_list.Count;

        public GraphType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        public Dictionary<int, HashSet<int>> GetList { get { return adjacence_list; } set { adjacence_list = value; } }
        /// <summary>
        /// Adding edge (connection) between two nodes
        /// </summary>
        /// <param name="first_node"></param>
        /// <param name="second_node"></param>
        public void AddEdge(int first_node, int second_node)
        {
            if (type == GraphType.Undirected)
            {
                adjacence_list[first_node].Add(second_node);
                adjacence_list[second_node].Add(first_node);
            }
            else if(type==GraphType.Directed)
            {
                adjacence_list[first_node].Add(second_node);
            }
        }
       /// <summary>
       /// Adding new node to list
       /// </summary>
       /// <param name="node"></param>
        public void AddNode(int node)
        {
            adjacence_list[node]=new HashSet<int>();
        }

        /// <summary>
        /// Remove edge (connection) between two nodes in list.
        /// </summary>
        /// <param name="first_node"></param>
        /// <param name="second_node"></param>
        public void RemoveEdge(int first_node, int second_node)
        {
            if (type == GraphType.Undirected)
            {
                if (adjacence_list[first_node].Contains(second_node))
                {
                    adjacence_list[first_node].Remove(second_node);
                    adjacence_list[second_node].Remove(first_node);
                }
            }
            else if (type == GraphType.Directed)
            {
                adjacence_list[first_node].Remove(second_node);
            }
        }

        /// <summary>
        /// Remove node and all connection that contains it from list
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(int node)
        {
            adjacence_list.Remove(node);

            foreach (KeyValuePair<int, HashSet<int>> kvp in adjacence_list)
            {
                if (adjacence_list[kvp.Key].Contains(node))
                {
                    adjacence_list[kvp.Key].Remove(node);
                }
            }
        }

        /// <summary>
        /// Converts current AdjacenceList to Matrix of Adjacence
        /// </summary>
        /// <returns>2D array of matrix</returns>
        public sbyte[,] ToAdjacenceMatrix()
        {
            return adjacence_list.ToAdjacenceMatrix();
        }

        /// <summary>
        /// Converts current AdjacenceList to Matrix of Incidence
        /// </summary>
        /// <returns>2D array of matrix</returns>
        public sbyte[,] ToIncidenceMatrix(GraphType type, ref Canvas canv, out List<string> lines)
        {
            return adjacence_list.ToIncidenceMatrix(type, ref canv, out lines);
        }

    }

    /// <summary>
    /// Class that check if edge exist in some situations for undirected graph
    /// </summary>
    static class ContainingChecker
    {
        /// <summary>
        /// Check if edge exist in AdjacenceList
        /// </summary>
        /// <param name="adj"></param>
        /// <param name="first_node"></param>
        /// <param name="second_node"></param>
        /// <returns>true - if exist, false - if no</returns>
        public static bool Edge(AdjacenceList adj, int first_node, int second_node)
        {
           if(adj.GetList[first_node].Contains(second_node))
           {
                return true;
           } 
           else if(adj.GetList[second_node].Contains(first_node))
           {
                return true;
           }
            return false;
        }

        /// <summary>
        /// Check if edge exist on Canvas
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="first_node"></param>
        /// <param name="second_node"></param>
        /// <returns>true - if exist, false - if no</returns>
        public static bool Edge(Canvas canvas, int first_node, int second_node)
        {
            if(canvas.Children.Contains(canvas.Children.OfType<Line>().FirstOrDefault(e => e.Name == $"line_{first_node}_{second_node}")))
            {
                return true;
            }
            else if(canvas.Children.Contains(canvas.Children.OfType<Line>().FirstOrDefault(e => e.Name == $"line_{second_node}_{first_node}")))
            {
                return true;
            }
            return false;
        }
    }

}
