﻿using Main.Enumerators;
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
    public class AdjacenceList : IAdjacenceList
    {
        Dictionary<int, HashSet<int>> adjacence_list = new Dictionary<int, HashSet<int>>();
        GraphType type;
        public AdjacenceList()
        {

        }
        public AdjacenceList(GraphType type)
        {
            this.type = type;
        }
        public HashSet<int> this[int index]
        {
            get { return adjacence_list[index]; }
            set { adjacence_list[index] = value; }
        }

        public int CountNodes => adjacence_list.Count;

        
        public Dictionary<int, HashSet<int>> GetList { get { return adjacence_list; } set { adjacence_list = value; } }
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

        public void AddNode(int node)
        {
            adjacence_list[node]=new HashSet<int>();
        }

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

        public sbyte[,] ToAdjacenceMatrix()
        {
            return adjacence_list.ToAdjacenceMatrix();
        }
        public sbyte[,] ToIncidenceMatrix(GraphType type, ref Canvas canv, out List<string> lines)
        {
            return adjacence_list.ToIncidenceMatrix(type, ref canv, out lines);
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
