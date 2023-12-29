using Main.Enumerators;
using Main.Interfaces;
using Main.Windows;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Xps.Serialization;
using System.Xml.Linq;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Main.Classes
{
    class GraphOperations : IGraphOperations
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="g_type"></param>
        /// <returns>AdjacenceList instance which is addition to input AdjacenceList</returns>
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

                    if (dict_g2.ContainsKey(y1))
                    {
                        if (x1 == x2 && dict_g2[y1].Contains(y2))
                        {
                            dekart_dict[kvp_1.Key].Add(kvp_2.Key);
                        }
                        else if (dict_g1.ContainsKey(x1))
                        {
                            if ((y1 == y2 && dict_g1[x1].Contains(x2)))
                            {
                                dekart_dict[kvp_1.Key].Add(kvp_2.Key);
                            }
                        }
                    }

                }
            }

            return dekart_dict;
        }

        public AdjacenceList CircleSum(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2, GraphType g_type)
        {
            ///5 вершин у g1 i 2 вершини в другому
            /// 2 вершини
            var cirle_sum_dict = new AdjacenceList(g_type);
            Dictionary<int, HashSet<int>> most_heights = null;
            Dictionary<int, HashSet<int>> min_heights = null;

            if (dict_g1.Keys.Count >= dict_g2.Keys.Count)
            {
                most_heights = new Dictionary<int, HashSet<int>>(dict_g1);
                min_heights = new Dictionary<int, HashSet<int>>(dict_g2);
            }
            else if (dict_g1.Keys.Count < dict_g2.Keys.Count)
            {
                most_heights = new Dictionary<int, HashSet<int>>(dict_g2);
                min_heights = new Dictionary<int, HashSet<int>>(dict_g1);
            }

            for (int key = 1; key <= most_heights.Keys.Count; key++)
            {
                cirle_sum_dict.GetList[key] = new HashSet<int>(most_heights[key]);

                if (min_heights.ContainsKey(key))
                {
                    foreach (int item_min in min_heights[key])
                    {
                        
                        if(cirle_sum_dict[key].Contains(item_min))
                        {
                            cirle_sum_dict[key].Remove(item_min);
                        }
                        else if (!cirle_sum_dict[key].Contains(item_min))
                        {
                            cirle_sum_dict[key].Add(item_min);
                        }
                    }
                }

            }

            return cirle_sum_dict;
        }

        public AdjacenceList Intersection(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2, GraphType g_type)
        {
            var intersection_dict = new AdjacenceList(g_type);

            Dictionary<int, HashSet<int>> most_heights = null;
            Dictionary<int, HashSet<int>> min_heights = null;

            if (dict_g1.Keys.Count >= dict_g2.Keys.Count)
            {
                most_heights = new Dictionary<int, HashSet<int>>(dict_g1);
                min_heights = new Dictionary<int, HashSet<int>>(dict_g2);
            }
            else if (dict_g1.Keys.Count < dict_g2.Keys.Count)
            {
                most_heights = new Dictionary<int, HashSet<int>>(dict_g2);
                min_heights = new Dictionary<int, HashSet<int>>(dict_g1);
            }

            foreach (KeyValuePair<int, HashSet<int>> kvp_g1 in min_heights)
            {
                intersection_dict[kvp_g1.Key] = new HashSet<int>(kvp_g1.Value);
                if (most_heights.ContainsKey(kvp_g1.Key))
                {
                    intersection_dict[kvp_g1.Key].IntersectWith(most_heights[kvp_g1.Key]);
                }
                else
                {
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
            Dictionary<int, HashSet<int>> most_heights=null;
            Dictionary<int, HashSet<int>> min_heights=null;

            if(dict_g1.Keys.Count >= dict_g2.Keys.Count) 
            {
                most_heights = new Dictionary<int, HashSet<int>>(dict_g1);
                min_heights = new Dictionary<int, HashSet<int>>(dict_g2);
            }
            else if(dict_g1.Keys.Count < dict_g2.Keys.Count)
            {
                most_heights = new Dictionary<int, HashSet<int>>(dict_g2);
                min_heights = new Dictionary<int, HashSet<int>>(dict_g1);
            }

            for(int key=1; key<=most_heights.Keys.Count; key++)
            {
                unity_dict.GetList[key] = new HashSet<int>(most_heights[key]);
                if (min_heights.ContainsKey(key))
                {
                    foreach(int item in min_heights[key])
                    {
                        unity_dict[key].Add(item);
                    }
                }
            }
            return unity_dict;
        }
        private Dictionary<string, HashSet<string>> GetAllPairsForCartesian(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2)
        {
            var dekart_dict = new Dictionary<string, HashSet<string>>();

            Dictionary<int, HashSet<int>> most_heights = null;
            Dictionary<int, HashSet<int>> min_heights = null;

            if (dict_g1.Keys.Count >= dict_g2.Keys.Count)
            {
                most_heights = new Dictionary<int, HashSet<int>>(dict_g1);
                min_heights = new Dictionary<int, HashSet<int>>(dict_g2);
            }
            else if (dict_g1.Keys.Count < dict_g2.Keys.Count)
            {
                most_heights = new Dictionary<int, HashSet<int>>(dict_g2);
                min_heights = new Dictionary<int, HashSet<int>>(dict_g1);
            }


            foreach (var kvp_g1 in most_heights)
            {
                foreach (var kvp_g2 in min_heights)
                {
                    dekart_dict[$"{kvp_g1.Key}_{kvp_g2.Key}"] = new HashSet<string>();
                }
            }
            return dekart_dict;
        }

        ~GraphOperations()
        {
            GC.Collect(); // find finalizable objects
            GC.WaitForPendingFinalizers(); // wait until finalizers executed
            GC.Collect(); // collect finalized objects
        }
    }

    class GraphOperationsCanvas: GraphOperations
    {
        public Canvas Addition(Dictionary<int, HashSet<int>> dict, ref Canvas canvas, GraphType g_type, out AdjacenceList addition)
        {
            addition = Addition(dict, g_type);


            var  deleted_lines = RemoveLines(canvas);

            return AddNewLines(addition, addition.GetList.GetLines(), canvas, g_type, deleted_lines);

        }

        public Canvas Unity(Canvas canvas, Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2, GraphType g_type, out AdjacenceList unity)
        {
            unity = Unity(dict_g1, dict_g2, g_type);


            return AddNewLines(unity, unity.GetList.GetLines(), canvas, g_type);
        }

        public Canvas Intersection(Canvas canvas, Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2, GraphType g_type, out AdjacenceList intersection)
        {
            intersection = Intersection(dict_g1, dict_g2, g_type);


            return AddNewLines(intersection, intersection.GetList.GetLines(), canvas, g_type);
        }

        public Canvas CircleSum(Canvas canvas, Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2, GraphType g_type, out AdjacenceList circle_sum)
        {
            circle_sum = CircleSum(dict_g1, dict_g2, g_type);

            return AddNewLines(circle_sum, circle_sum.GetList.GetLines(), canvas, g_type);
        }
        
        private HashSet<string> GetLinesCartesian(Dictionary<string, HashSet<string>> dict, GraphType type)
        {
            var lines = new HashSet<string>();

            foreach(var node in dict.Keys)
            {
                foreach(var node_2 in dict[node])
                {
                    string edge = $"{node}_dek_{node_2}";
                    if (type==GraphType.Undirected && !lines.Contains($"{node_2}_dek_{node}") && node!=node_2)
                    {
                        lines.Add(edge);
                    }
                    else if(type == GraphType.Directed)
                    {
                        lines.Add(edge);
                    }
                }
            }

            return lines;

        }
        public Canvas CartesianProduct(Dictionary<int, HashSet<int>> dict_g1, Dictionary<int, HashSet<int>> dict_g2, GraphType g_type)
        {
            var cartesian_dict = CartesianProduct(dict_g1, dict_g2);

            AddNodesCartesian(cartesian_dict, out Canvas cartesian_canvas);
            return GetNewCanvasCartesianProduct(cartesian_canvas, cartesian_dict, g_type);
        }

        private Canvas GetNewCanvasCartesianProduct(Canvas cartes_nodes, Dictionary<string, HashSet<string>> cartesian, GraphType g_type)
        {
            var edges = GetLinesCartesian(cartesian, g_type);

            foreach(var edge in edges)
            {
                edge.DoubleEdgeName(out string f_node, out string s_node);

                Ellipse fEllipse = cartes_nodes.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_dek_{f_node}");
                Ellipse sEllipse = cartes_nodes.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_dek_{s_node}");


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

                    if (g_type == GraphType.Undirected && f_node != s_node)
                    {
                        if (!cartes_nodes.Children.Contains(cartes_nodes.Children.OfType<Line>().FirstOrDefault(e => e.Name == $"line_{s_node}_{f_node}")))
                        {
                            cartes_nodes.Children.Add(line);
                        }
                    }
                    else if (g_type == GraphType.Directed)
                    {
                        cartes_nodes.Children.Add(line);
                    }
                }
            }
            return cartes_nodes;
        }

        private void AddNodesCartesian(Dictionary<string, HashSet<string>> cartesian, out Canvas cartesian_product)
        {
            cartesian_product = new Canvas();
            double height=0;
            double width=0;

            try{
                width = WindowsInstances.ResultWindowInst().DisplayingData.ActualWidth;
                height = WindowsInstances.ResultWindowInst().DisplayingData.ActualHeight;
            }
            catch(Exception ex)
            {
                
            }

            if(width == 0 && height == 0)
            {
                width = WindowsInstances.ResultWindowInst().Width;
                height = WindowsInstances.ResultWindowInst().Height;
            }

            foreach (var key in cartesian.Keys)
            {
               

                Ellipse AAACircle = new Ellipse()
                {
                    Name = $"Ellipse_dek_{key}",
                    Height = 50,
                    Width = 50,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = Brushes.White,
                };


                TextBlock textBlock = new TextBlock()
                {
                    Text = key.Replace("_", ","),
                    Name = "Text" + AAACircle.Name,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 16,
                    FontFamily = new FontFamily("Arial"),
                };

                /*Random rand_height = new Random();
                Random rand_width = new Random();*/



                // The farthest left the dot can be
                double minLeft = 0;
                // The farthest right the dot can be without it going off the screen
                double maxLeft = width - AAACircle.Width;
                // The farthest up the dot can be
                double minTop = 0;
                // The farthest down the dot can be without it going off the screen
                double maxTop = height - AAACircle.Height;


                double left = RandomBetween(minLeft, maxLeft);
                double top = RandomBetween(minTop, maxTop);
                //AAACircle.Margin = new Thickness(left, top, 0, 0);


                Canvas.SetTop(AAACircle, top);
                Canvas.SetLeft(AAACircle, left);


                Point center1_for_text = new Point(Canvas.GetLeft(AAACircle) + AAACircle.Width / 2 - 10, Canvas.GetTop(AAACircle) + AAACircle.Height / 2 - 10);



                Canvas.SetLeft(textBlock, center1_for_text.X);
                Canvas.SetTop(textBlock, center1_for_text.Y);
                cartesian_product.Children.Add(AAACircle);
                cartesian_product.Children.Add(textBlock);
            }
        }

        private double RandomBetween(double min, double max)
        {
            Random random = new Random();
            return random.NextDouble() * (max - min) + min;
        }
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

        private Canvas AddNodes(Canvas old_canvas, Canvas canvas, AdjacenceList dict)
        {
            for (int i = 1; i <= dict.CountNodes; i++)
            {
                Ellipse node = new Ellipse()
                {
                    Name = $"Ellipse_{i}",
                    Height = 50,
                    Width = 50,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = Brushes.White,
                };

                var old_node = old_canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{i}");

                Canvas.SetLeft(node, Canvas.GetLeft(old_node));
                Canvas.SetTop(node, Canvas.GetTop(old_node));

                TextBlock textBlock = new TextBlock()
                {
                    Name = $"TextEllipse_{i}",
                    Text =i.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 16,
                    FontFamily = new FontFamily("Arial"),

                };

                var old_text = old_canvas.Children.OfType<TextBlock>().FirstOrDefault(e => e.Name == $"TextEllipse_{i}");

                Canvas.SetLeft(textBlock, Canvas.GetLeft(old_text));
                Canvas.SetTop(textBlock, Canvas.GetTop(old_text));

                canvas.Children.Add(node);
                canvas.Children.Add(textBlock);
            }
            return canvas;


        }
        private Canvas AddNewLines(AdjacenceList dict, List<string> lines, Canvas canvas, GraphType g_type) 
        {
            Canvas result_canvas = new Canvas();

            result_canvas = AddNodes(canvas, result_canvas, dict);
            foreach (var edge in lines)
            {
                edge.EdgesNames(out int f_node, out int s_node);

                Ellipse fEllipse = result_canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{f_node}");
                Ellipse sEllipse = result_canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{s_node}");


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

                    if (g_type == GraphType.Undirected && f_node != s_node)
                    {
                        if (!result_canvas.Children.Contains(result_canvas.Children.OfType<Line>().FirstOrDefault(e => e.Name == $"line_{s_node}_{f_node}")))
                        {
                            result_canvas.Children.Add(line);
                        }
                    }
                    else if (g_type == GraphType.Directed)
                    {
                        result_canvas.Children.Add(line);
                    }
                }
            }
            return result_canvas;
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


        ~GraphOperationsCanvas()
        {
            GC.Collect(); // find finalizable objects
            GC.WaitForPendingFinalizers(); // wait until finalizers executed
            GC.Collect(); // collect finalized objects
        }

       
    }
}
