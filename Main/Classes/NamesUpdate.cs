using Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Main.Classes
{
    class NamesUpdate : INamesUpdate
    {
        public void UpdateCanvas(ref Canvas canvas, int node_deleted)
        {
          
            foreach (UIElement element in canvas.Children)
            {
                if (element is Ellipse ellipse)
                {
                    int node_current = ellipse.Name.SingleNodeName();

                    if (node_current> node_deleted)
                    {
                        ellipse.Name = $"Ellipse_{Convert.ToInt32(node_current) - 1}";
                    }
                }
                else if (element is TextBlock text)
                {

                    int text_current = text.Name.SingleNodeName();

                    if (text_current > node_deleted)
                    {
                        text.Name = $"TextEllipse_{text_current - 1}";
                        text.Text = $"{text_current - 1}";
                    }
                }
                else if (element is Line line)
                {
                    line.Name.EdgesNames(out int first_node, out int second_node);

                    if (first_node>node_deleted && second_node>node_deleted)
                    {
                        line.Name = $"line_{first_node - 1}_{second_node - 1}";
                    }
                    else if (first_node>node_deleted)
                    {
                        line.Name = $"line_{first_node - 1}_{second_node}";
                    }
                    else if (second_node > node_deleted)
                    {
                        line.Name = $"line_{first_node}_{second_node - 1}";
                    }
                }
                else if (element is Shape shape)
                {
                    shape.Name.EdgesNames(out int first_node, out int second_node);

                    if (first_node > node_deleted && second_node > node_deleted)
                    {
                        shape.Name = $"line_{first_node - 1}_{second_node - 1}";
                    }
                    else if (first_node > node_deleted)
                    {
                        shape.Name = $"line_{first_node - 1}_{second_node}";
                    }
                    else if (second_node > node_deleted)
                    {
                        shape.Name = $"line_{first_node}_{second_node - 1}";
                    }
                }
            }
        }

        public void UpdateNodes(AdjacenceList adj, int node)
        {
            var temp = new Dictionary<int, HashSet<int>>();

            foreach (var item in adj.GetList) 
            {
                if(item.Key > node)
                {
                    temp[item.Key - 1] = GetUpdatedHashset(adj[item.Key], node);
                }
                else
                {
                    temp[item.Key] = GetUpdatedHashset(adj[item.Key], node);
                }

            }
            adj.GetList = temp;
        }

        private HashSet<int> GetUpdatedHashset(HashSet<int> hash, int node)
        {
            var temp = new HashSet<int>();

            foreach(var item in hash)
            {
                if(item>node)
                {
                    temp.Add(item - 1);
                }
                else
                {
                    temp.Add(item);
                }
            }
            return temp;
        }
    }
}
