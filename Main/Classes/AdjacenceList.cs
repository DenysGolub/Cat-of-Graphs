using Main.Enumerators;
using Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Classes
{
    internal class AdjacenceList : IAdjacenceList
    {
        Dictionary<int, HashSet<int>> adjacence_list = new Dictionary<int, HashSet<int>>();

        public HashSet<int> this[int index]
        {
            get { return adjacence_list[index]; }
            //set { adjacence_list[index] = value; }
        }

        public int CountNodes => adjacence_list.Count;
        public Dictionary<int, HashSet<int>> GetList { get { return adjacence_list; } set { adjacence_list = value; } }
        public void AddEdge(int first_node, int second_node)
        {
            adjacence_list[first_node].Add(second_node);
            adjacence_list[second_node].Add(first_node);
        }

        public void AddNode(int node)
        {
            adjacence_list[node]=new HashSet<int>();
        }

        public void RemoveEdge(int first_node, int second_node)
        {
            foreach(KeyValuePair<int,HashSet<int>> kvp in adjacence_list)
            {
                if (adjacence_list[kvp.Key].Contains(first_node))
                {
                    adjacence_list[kvp.Key].Remove(first_node);
                }
                else if (adjacence_list[kvp.Key].Contains(second_node))
                {
                    adjacence_list[kvp.Key].Remove(second_node);
                }
            }
        }

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

        /*private void RemoveConnectedEdges(int node)
        {
            for(int i=0; i<adjacence_list.Keys.Count; i++)
            {
                if (adjacence_list[i].Contains(node))
                {
                    adjacence_list[i].Remove(node);
                }
            }
        }*/

        
        public sbyte[,] ToAdjacenceMatrix()
        {
            return adjacence_list.ToAdjacenceMatrix();
        }
        public sbyte[,] ToIncidenceMatrix(GraphType type)
        {
            return adjacence_list.ToIncidenceMatrix(type);
        }

    }

    static class ContainingChecker
    {
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
    }

}
