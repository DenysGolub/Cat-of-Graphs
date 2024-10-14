using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using Main.Windows;

namespace Main.Classes
{
    public delegate void AddNode(Window win);

    public delegate void DelNode(Window win, int node, HashSet<string> lines);

    public delegate void Edge(Window win, string x, string y);
    public delegate void DelEdge(Window win, string x, string y);
    public delegate void UpdateMatrix();
    static class DelegateCanvasEvents
    {
        static public void DeleteEdgeCanvas(MainWindow win, string x, string y)
        {
            Canvas canvas;
            AdjacenceList dict;

            if (win.DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                win.GraphAdjacenceList.RemoveEdge(int.Parse(x), int.Parse(y));
                win.DrawingCanvas_Undirected.Children.Remove(win.DrawingCanvas_Undirected.Children.OfType<Line>().FirstOrDefault(e => e.Name == $"line_{x}_{y}"));
                win.DrawingCanvas_Undirected.Children.Remove(win.DrawingCanvas_Undirected.Children.OfType<Line>().FirstOrDefault(e => e.Name == $"line_{y}_{x}"));
            }
            else
            {
                win.GraphAdjacenceList.RemoveEdge(int.Parse(x), int.Parse(y));
                win.DrawingCanvas_Directed.Children.Remove(win.DrawingCanvas_Directed.Children.OfType<Shape>().FirstOrDefault(e => e.Name == $"line_{x}_{y}"));
            }
        }

        static public void AddEdgeToCanvas(MainWindow win, string f_node, string s_node)
        {
            Canvas canvas = win.GraphCanvas;
            AdjacenceList dict = win.GraphAdjacenceList;

           

            bool shapeExistsEllips1 = canvas.Children.OfType<Shape>().Any(shape => shape.Name == $"Ellipse_{f_node}");

            bool shapeExistsEllips2 = canvas.Children.OfType<Shape>().Any(shape => shape.Name == $"Ellipse_{s_node}");
            if (shapeExistsEllips1 && shapeExistsEllips2)
            {

                Ellipse sEllipse = canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{s_node}");

                // Find the first ellipse
                Ellipse fEllipse = canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{f_node}");

                Point center1 = new Point(Canvas.GetLeft(fEllipse) + fEllipse.Width / 2, Canvas.GetTop(fEllipse) + fEllipse.Height / 2);
                Point center2 = new Point(Canvas.GetLeft(sEllipse) + sEllipse.Width / 2, Canvas.GetTop(sEllipse) + sEllipse.Height / 2);


                var intersectionPoint1 = (Point)DataFromGraph.CalculateIntersection(center1, fEllipse.Width / 2, center2);
                var intersectionPoint2 = (Point)DataFromGraph.CalculateIntersection(center2, sEllipse.Width / 2, center1);
                dynamic line;


                if (canvas.Name=="DrawingCanvas_Directed")
                {
                    line = DataFromGraph.DrawLinkArrow(intersectionPoint1, intersectionPoint2);
                    line.Name = $"line_{f_node}_{s_node}";

                    if (!canvas.Children.Cast<FrameworkElement>()
          .Any(x => x.Name != null && x.Name.ToString() == $"line_{int.Parse(f_node)}_{int.Parse(s_node)}"))
                    {
                        canvas.Children.Add(line);
                        dict.AddEdge(int.Parse(f_node), int.Parse(s_node));

                    }
                }
                else
                {
                    line = new Line()
                    {
                        Name = $"line_{f_node}_{s_node}",
                        X1 = intersectionPoint1.X,
                        Y1 = intersectionPoint1.Y,
                        X2 = intersectionPoint2.X,
                        Y2 = intersectionPoint2.Y,
                        Stroke = System.Windows.Media.Brushes.Black,
                        StrokeThickness = 1,
                        Fill = System.Windows.Media.Brushes.Black,
                    };


                    if (ContainingChecker.Edge(canvas, Convert.ToInt32(f_node), Convert.ToInt32(s_node)) == false && f_node != s_node)
                    {
                        canvas.Children.Add(line);
                        dict.AddEdge(int.Parse(f_node), int.Parse(s_node));
                    }
                }

            }
        }
        static public void AddNodeToCanvas(MainWindow win)
        {
            Canvas canvas=win.GraphCanvas;
            AdjacenceList dict = win.GraphAdjacenceList;

           
            Ellipse AAACircle = new Ellipse()
            {
                Name = $"Ellipse_{dict.CountNodes}",
                Height = 50,
                Width = 50,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = Brushes.White,

            };

            TextBlock textBlock = new TextBlock()
            {
                Name = "Text" + AAACircle.Name,
                Text = (dict.CountNodes).ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 16,
                FontFamily = new FontFamily("Arial"),

            };
            Canvas.SetZIndex(AAACircle, int.MaxValue);
            Canvas.SetZIndex(textBlock, int.MaxValue);



            // The farthest left the dot can be
            double minLeft = 0;
            // The farthest right the dot can be without it going off the screen
            double maxLeft = canvas.ActualWidth - AAACircle.Width;
            // The farthest up the dot can be
            double minTop = 0;
            // The farthest down the dot can be without it going off the screen
            double maxTop = canvas.ActualHeight - AAACircle.Height;


            double left = RandomBetween(minLeft, maxLeft);
            double top = RandomBetween(minTop, maxTop);


            Canvas.SetLeft(AAACircle, left);
            Canvas.SetTop(AAACircle, top);

            Point center1_for_text = DataFromGraph.AllignOfText(AAACircle, (dict.CountNodes).ToString());

            Canvas.SetLeft(textBlock, center1_for_text.X);
            Canvas.SetTop(textBlock, center1_for_text.Y);

            canvas.Children.Add(AAACircle);
            canvas.Children.Add(textBlock);
            canvas.InvalidateVisual();
        }

        static public void DeleteNodeCanvas(MainWindow win, int node, HashSet<string> lines)
        {
            NamesUpdate update = new NamesUpdate();

            if (win.DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                win.DrawingCanvas_Undirected.Children.Remove(win.DrawingCanvas_Undirected.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{node}"));
                win.DrawingCanvas_Undirected.Children.Remove(win.DrawingCanvas_Undirected.Children.OfType<TextBlock>().FirstOrDefault(e => e.Name == $"TextEllipse_{node}"));

                foreach (string s in lines)
                {
                    win.DrawingCanvas_Undirected.Children.Remove(win.DrawingCanvas_Undirected.Children.OfType<Line>().FirstOrDefault(e => e.Name == s));
                }
                update.UpdateCanvas(ref win.DrawingCanvas_Undirected, node);
            }
            else
            {
                win.DrawingCanvas_Directed.Children.Remove(win.DrawingCanvas_Directed.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{node}"));
                win.DrawingCanvas_Directed.Children.Remove(win.DrawingCanvas_Directed.Children.OfType<TextBlock>().FirstOrDefault(e => e.Name == $"TextEllipse_{node}"));

                foreach (string s in lines)
                {
                    win.DrawingCanvas_Directed.Children.Remove(win.DrawingCanvas_Directed.Children.OfType<Shape>().FirstOrDefault(e => e.Name == s));
                }
                update.UpdateCanvas(ref win.DrawingCanvas_Directed, node);

            }
        }
        static private double RandomBetween(double min, double max)
        {
            return new Random().NextDouble() * (max - min) + min;
        }



        static public void DeleteEdgeCanvas(SecondGraph win, string x, string y)
        {
            Canvas canvas;
            AdjacenceList dict;

            if (win.DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                win.SecondGraphAdjacenceList.RemoveEdge(int.Parse(x), int.Parse(y));
                win.DrawingCanvas_Undirected.Children.Remove(win.DrawingCanvas_Undirected.Children.OfType<Line>().FirstOrDefault(e => e.Name == $"line_{x}_{y}"));
                win.DrawingCanvas_Undirected.Children.Remove(win.DrawingCanvas_Undirected.Children.OfType<Line>().FirstOrDefault(e => e.Name == $"line_{y}_{x}"));
            }
            else
            {
                win.SecondGraphAdjacenceList.RemoveEdge(int.Parse(x), int.Parse(y));
                win.DrawingCanvas_Directed.Children.Remove(win.DrawingCanvas_Directed.Children.OfType<Shape>().FirstOrDefault(e => e.Name == $"line_{x}_{y}"));
            }
        }

        static public void AddEdgeToCanvas(SecondGraph win, string f_node, string s_node)
        {
            Canvas canvas = win.SecondGraphCanvas;
            AdjacenceList dict = win.SecondGraphAdjacenceList;



            bool shapeExistsEllips1 = canvas.Children.OfType<Shape>().Any(shape => shape.Name == $"Ellipse_{f_node}");

            bool shapeExistsEllips2 = canvas.Children.OfType<Shape>().Any(shape => shape.Name == $"Ellipse_{s_node}");
            if (shapeExistsEllips1 && shapeExistsEllips2)
            {

                Ellipse sEllipse = canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{s_node}");

                // Find the first ellipse
                Ellipse fEllipse = canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{f_node}");

                Point center1 = new Point(Canvas.GetLeft(fEllipse) + fEllipse.Width / 2, Canvas.GetTop(fEllipse) + fEllipse.Height / 2);
                Point center2 = new Point(Canvas.GetLeft(sEllipse) + sEllipse.Width / 2, Canvas.GetTop(sEllipse) + sEllipse.Height / 2);


                var intersectionPoint1 = (Point)DataFromGraph.CalculateIntersection(center1, fEllipse.Width / 2, center2);
                var intersectionPoint2 = (Point)DataFromGraph.CalculateIntersection(center2, sEllipse.Width / 2, center1);
                dynamic line;


                if (canvas.Name == "DrawingCanvas_Directed")
                {
                    line = DataFromGraph.DrawLinkArrow(intersectionPoint1, intersectionPoint2);
                    line.Name = $"line_{f_node}_{s_node}";

                    if (!canvas.Children.Cast<FrameworkElement>()
          .Any(x => x.Name != null && x.Name.ToString() == $"line_{int.Parse(f_node)}_{int.Parse(s_node)}"))
                    {
                        canvas.Children.Add(line);
                        dict.AddEdge(int.Parse(f_node), int.Parse(s_node));

                    }
                }
                else
                {
                    line = new Line()
                    {
                        Name = $"line_{f_node}_{s_node}",
                        X1 = intersectionPoint1.X,
                        Y1 = intersectionPoint1.Y,
                        X2 = intersectionPoint2.X,
                        Y2 = intersectionPoint2.Y,
                        Stroke = System.Windows.Media.Brushes.Black,
                        StrokeThickness = 2,
                        Fill = System.Windows.Media.Brushes.Black,
                    };


                    if (ContainingChecker.Edge(canvas, Convert.ToInt32(f_node), Convert.ToInt32(s_node)) == false && f_node != s_node)
                    {
                        canvas.Children.Add(line);
                        dict.AddEdge(int.Parse(f_node), int.Parse(s_node));
                    }
                }

            }
        }
        static public void AddNodeToCanvas(SecondGraph win)
        {
            Canvas canvas = win.SecondGraphCanvas;
            AdjacenceList dict = win.SecondGraphAdjacenceList;


            Ellipse AAACircle = new Ellipse()
            {
                Name = $"Ellipse_{dict.CountNodes}",
                Height = 50,
                Width = 50,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = Brushes.White,

            };

            TextBlock textBlock = new TextBlock()
            {
                Name = "Text" + AAACircle.Name,
                Text = (dict.CountNodes).ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 16,
                FontFamily = new FontFamily("Arial"),

            };


            // The farthest left the dot can be
            double minLeft = 0;
            // The farthest right the dot can be without it going off the screen
            double maxLeft = canvas.ActualWidth - AAACircle.Width;
            // The farthest up the dot can be
            double minTop = 0;
            // The farthest down the dot can be without it going off the screen
            double maxTop = canvas.ActualHeight - AAACircle.Height;


            double left = RandomBetween(minLeft, maxLeft);
            double top = RandomBetween(minTop, maxTop);


            Canvas.SetLeft(AAACircle, left);
            Canvas.SetTop(AAACircle, top);

            Point center1_for_text = DataFromGraph.AllignOfText(AAACircle, (dict.CountNodes).ToString());

            Canvas.SetLeft(textBlock, center1_for_text.X);
            Canvas.SetTop(textBlock, center1_for_text.Y);

            canvas.Children.Add(AAACircle);
            canvas.Children.Add(textBlock);
            canvas.InvalidateVisual();
        }

        static public void DeleteNodeCanvas(SecondGraph win, int node, HashSet<string> lines)
        {
            NamesUpdate update = new NamesUpdate();

            if (win.DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                win.DrawingCanvas_Undirected.Children.Remove(win.DrawingCanvas_Undirected.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{node}"));
                win.DrawingCanvas_Undirected.Children.Remove(win.DrawingCanvas_Undirected.Children.OfType<TextBlock>().FirstOrDefault(e => e.Name == $"TextEllipse_{node}"));

                foreach (string s in lines)
                {
                    win.DrawingCanvas_Undirected.Children.Remove(win.DrawingCanvas_Undirected.Children.OfType<Line>().FirstOrDefault(e => e.Name == s));
                }
                update.UpdateCanvas(ref win.DrawingCanvas_Undirected, node);
            }
            else
            {
                win.DrawingCanvas_Directed.Children.Remove(win.DrawingCanvas_Directed.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{node}"));
                win.DrawingCanvas_Directed.Children.Remove(win.DrawingCanvas_Directed.Children.OfType<TextBlock>().FirstOrDefault(e => e.Name == $"TextEllipse_{node}"));

                foreach (string s in lines)
                {
                    win.DrawingCanvas_Directed.Children.Remove(win.DrawingCanvas_Directed.Children.OfType<Shape>().FirstOrDefault(e => e.Name == s));
                }
                update.UpdateCanvas(ref win.DrawingCanvas_Directed, node);

            }
        }
    }
}
