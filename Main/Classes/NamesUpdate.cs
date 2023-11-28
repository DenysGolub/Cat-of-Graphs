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
        public void UpdateCanvas(ref Canvas canvas, int node)
        {
            Regex regex = new Regex(@"_(\d*)");

          
            foreach (UIElement element in canvas.Children)
            {
                if (element is Ellipse ellipse)
                {
                    Match m_current = regex.Match(ellipse.Name);

                    string ellips_current = m_current.Groups[1].Value;

                    if (Convert.ToInt32(ellips_current) > node)
                    {
                        ellipse.Name = $"Ellipse_{Convert.ToInt32(ellips_current) - 1}";
                    }
                }
                else if (element is TextBlock text)
                {
                    Match text_m_current = regex.Match(text.Name);

                    string text_current = text_m_current.Groups[1].Value;

                    if (Convert.ToInt32(text_current) > Convert.ToInt32(node))
                    {
                        text.Name = $"TextEllipse_{Convert.ToInt32(text_current) - 1}";
                        text.Text = $"{Convert.ToInt32(text_current) - 1}";
                    }
                }
                else if (element is Line line)
                {
                    Regex line_m_current = new Regex(@"_(\d*)_(\d*)");

                    string name_line = line.Name;
                    Match match_line_name = line_m_current.Match(name_line);

                    string first_el = match_line_name.Groups[1].Value;
                    string second_el = match_line_name.Groups[2].Value;

                    if ((Convert.ToInt32(first_el) > node) && (Convert.ToInt32(second_el) > node))
                    {
                        line.Name = $"line_{Convert.ToInt32(first_el) - 1}_{Convert.ToInt32(second_el) - 1}";
                    }
                    else if (Convert.ToInt32(first_el) > node)
                    {
                        line.Name = $"line_{Convert.ToInt32(first_el) - 1}_{second_el}";
                    }
                    else if (Convert.ToInt32(second_el) > node)
                    {
                        line.Name = $"line_{first_el}_{Convert.ToInt32(second_el) - 1}";
                    }
                }
                else if (element is Shape shape)
                {
                    Regex line_m_current = new Regex(@"_(\d*)_(\d*)");

                    string name_line = shape.Name;
                    Match match_line_name = line_m_current.Match(name_line);

                    string first_el = match_line_name.Groups[1].Value;
                    string second_el = match_line_name.Groups[2].Value;

                    if ((Convert.ToInt32(first_el) > node) && (Convert.ToInt32(second_el) > node))
                    {
                        shape.Name = $"line_{Convert.ToInt32(first_el) - 1}_{Convert.ToInt32(second_el) - 1}";
                    }
                    else if (Convert.ToInt32(first_el) > node)
                    {
                        shape.Name = $"line_{Convert.ToInt32(first_el) - 1}_{second_el}";
                    }
                    else if (Convert.ToInt32(second_el) > node)
                    {
                        shape.Name = $"line_{first_el}_{Convert.ToInt32(second_el) - 1}";
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
