using Main.Enumerators;
using Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Main.Classes
{
    class GraphOperations : IGraphOperations
    {
        public AdjacenceList Addition(Dictionary<int, HashSet<int>> dict, GraphType g_type)
        {
            var addition_dict = new AdjacenceList(g_type);
           
            for(int dict_keys=1; dict_keys<=dict.Keys.Count; dict_keys++)
            {
                addition_dict.AddNode(dict_keys);
                for (int number=1; number<=dict.Keys.Count; number++)
                {
                    
                    if (!dict[dict_keys].Contains(number))
                    {
                        if (number == dict_keys && g_type == GraphType.Directed)
                        {
                            addition_dict[dict_keys].Add(number);
                        }
                        else if(number != dict_keys)
                        {
                            addition_dict[dict_keys].Add(number);
                        }
                    }
                }
            }
            return addition_dict;
        }

        public Dictionary<string, HashSet<string>> CartesianProduct(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2)
        {
            var dekart_dict = GetAllPairsForCartesian(dict_g1, dict_g2);
            
            foreach(var kvp_1 in dekart_dict)
            {
                kvp_1.Key.DoubleNodeName(out int x1, out int y1);
                foreach(var kvp_2 in dekart_dict)
                {
                    kvp_2.Key.DoubleNodeName(out int x2, out int y2);

                    if((x1==x2 && dict_g2[y1].Contains(y2)) || (y1 == y2 && dict_g1[x1].Contains(x2)))
                    {
                        dekart_dict[kvp_1.Key].Add(kvp_2.Key);
                    }
                }
            }

            return dekart_dict;
        }

        public Dictionary<int, HashSet<int>> CircleSum(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2)
        {
            var circle_sum_dict = new Dictionary<int, HashSet<int>>();
            foreach (KeyValuePair<int, HashSet<int>> kvp_g1 in dict_g1.Keys.Count >= dict_g2.Keys.Count ? dict_g1 : dict_g2)
            {
                circle_sum_dict[kvp_g1.Key] = kvp_g1.Value;
                if (dict_g2.ContainsKey(kvp_g1.Key))
                {
                    foreach (var item in dict_g2[kvp_g1.Key])
                    {
                        if(!kvp_g1.Value.Contains(item))
                        {
                            circle_sum_dict[kvp_g1.Key].Add(item);
                        }
                    }
                }


            }

            return circle_sum_dict;
        }

        public Dictionary<int, HashSet<int>> Intersection(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2)
        {
            var intersection_dict = new Dictionary<int, HashSet<int>>();
            foreach (KeyValuePair<int, HashSet<int>> kvp_g1 in dict_g1.Keys.Count <= dict_g2.Keys.Count ? dict_g1 : dict_g2)
            {
                intersection_dict[kvp_g1.Key] = kvp_g1.Value;
                if (dict_g2.ContainsKey(kvp_g1.Key))
                {
                    intersection_dict[kvp_g1.Key].Intersect(dict_g2[kvp_g1.Key]);
                }
                else
                {
                    intersection_dict.Remove(kvp_g1.Key);
                    break;
                }
            }

            return intersection_dict;
        }

        public AdjacenceList Unity(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2, GraphType g_type)
        {
            ///5 вершин у g1 i 2 вершини в другому
            /// 2 вершини
            var unity_dict = new AdjacenceList(g_type);
            foreach (KeyValuePair<int, HashSet<int>> kvp_g1 in dict_g1.Keys.Count>=dict_g2.Keys.Count?dict_g1:dict_g2)
            {
                unity_dict.GetList[kvp_g1.Key] = kvp_g1.Value;
                if (dict_g2.ContainsKey(kvp_g1.Key))
                {
                    unity_dict[kvp_g1.Key].Union(dict_g2[kvp_g1.Key]);
                }
                
            }

            return unity_dict;
        }
        private Dictionary<string, HashSet<string>> GetAllPairsForCartesian(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2)
        {
            var dekart_dict = new Dictionary<string, HashSet<string>>();
            foreach (var kvp_g1 in dict_g1.Keys.Count >= dict_g2.Keys.Count ? dict_g1 : dict_g2)
            {
                foreach (var kvp_g2 in dict_g1.Keys.Count >= dict_g2.Keys.Count ? dict_g2 : dict_g1)
                {
                    dekart_dict[$"{kvp_g1.Key},{kvp_g2.Key}"] = new HashSet<string>();
                }
            }
            return dekart_dict;
        }
    }

    class GraphOperationsCanvas:GraphOperations
    {
     public Canvas Addition(Dictionary<int, HashSet<int>> dict, ref Canvas canvas, GraphType g_type, out AdjacenceList addition)
        {
            addition = Addition(dict, g_type);


            var  deleted_lines = RemoveLines(canvas);

            return AddNewLines(addition, addition.GetList.GetLines(), canvas, g_type, deleted_lines);

        } 
        
     /*   public Canvas Unity(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2, GraphType g_type, out AdjacenceList unity)
        {
            unity = Unity(dict_g1, dict_g2, g_type);


        }*/
        private List<string> RemoveLines(Canvas canvas)
        {
            List<string> deleted_lines = new List<string>();

            foreach(UIElement child in canvas.Children)
            {
                if (child is Line line)
                {
                    deleted_lines.Add(line.Name);

                }
                else if (child is Shape shape && !(child is Ellipse))
                {
                    deleted_lines.Add(shape.Name);
                }
            }

            return deleted_lines;
        }
        private Canvas AddNewLines(AdjacenceList addition, List<string> lines, Canvas canvas, GraphType g_type, List<string> deleted_lines)
        {
            foreach(var edge in deleted_lines)
            {
                canvas.Children.Remove(canvas.Children.OfType<Line>().FirstOrDefault(e => e.Name == edge));
                canvas.Children.Remove(canvas.Children.OfType<Shape>().FirstOrDefault(e => e.Name == edge));
                lines.Remove(edge);

            }

            foreach (var edge in lines)
            {
                if (!deleted_lines.Contains(edge))
                {
                    edge.EdgesNames(out int f_node, out int s_node);
                    Ellipse sEllipse = canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{s_node}");

                    // Find the first ellipse
                    Ellipse fEllipse = canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{f_node}");

                    if (fEllipse != null && sEllipse != null)
                    {
                        Point center1 = new Point(Canvas.GetLeft(fEllipse) + fEllipse.Width / 2, Canvas.GetTop(fEllipse) + fEllipse.Height / 2);
                        Point center2 = new Point(Canvas.GetLeft(sEllipse) + sEllipse.Width / 2, Canvas.GetTop(sEllipse) + sEllipse.Height / 2);


                        var intersectionPoint1 = (Point)DataFromGraph.CalculateIntersection(center1, fEllipse.Width / 2, center2);
                        var intersectionPoint2 = (Point)DataFromGraph.CalculateIntersection(center2, sEllipse.Width / 2, center1);


                        dynamic line = null;
                        if (g_type == GraphType.Undirected)
                        {
                            line = new Line()
                            {
                                X1 = intersectionPoint1.X,
                                Y1 = intersectionPoint1.Y,
                                X2 = intersectionPoint2.X,
                                Y2 = intersectionPoint2.Y,
                                Stroke = System.Windows.Media.Brushes.Black,
                                StrokeThickness = 2,
                                Fill = System.Windows.Media.Brushes.Black,
                            };
                        }
                        else if (g_type == GraphType.Directed)
                        {
                            line = DataFromGraph.DrawLinkArrow(intersectionPoint1, intersectionPoint2);
                        }
                        line.Name = $"line_{f_node}_{s_node}";

                        if (g_type == GraphType.Undirected && f_node!=s_node)
                        {
                            if (!deleted_lines.Contains(line.Name) && !deleted_lines.Contains($"line_{s_node}_{f_node}") &&
                                !canvas.Children.Contains(canvas.Children.OfType<Line>().FirstOrDefault(e => e.Name == $"line_{s_node}_{f_node}")))
                            {
                                canvas.Children.Add(line);
                            }
                        }
                        else if(g_type == GraphType.Directed)
                        {
                            canvas.Children.Add(line);
                        }
                    }
                }
            }
            return canvas;
        }
    }
}
