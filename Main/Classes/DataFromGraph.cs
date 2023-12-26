using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using Main.Enumerators;

namespace Main.Classes
{
    static class DataFromGraph
    {
        static public Point? CalculateIntersection(Point circleCenter, double circleRadius, Point lineStart)
        {
            if (Math.Abs(circleCenter.X - lineStart.X) < double.Epsilon)
            {
                if (circleCenter.Y > lineStart.Y)
                {
                    return new Point(circleCenter.X, circleCenter.Y - circleRadius);
                }
                return new Point(circleCenter.X, circleCenter.Y - circleRadius);
            }
            if (Math.Abs(circleCenter.Y - lineStart.Y) < double.Epsilon)
            {
                if (circleCenter.X > lineStart.X)
                {
                    return new Point(circleCenter.X - circleRadius, circleCenter.Y);
                }
                return new Point(circleCenter.X + circleRadius, circleCenter.Y);
            }

            // translate to origin point
            var translate = new Vector(-circleCenter.X, -circleCenter.Y);

            circleCenter = circleCenter + translate;
            lineStart = lineStart + translate;

            // y=kx+t -> kx1+t=y1, kx2+t=y2 
            // k=(y1-y2)/(x1-x2), t=y1-kx1
            var k = (circleCenter.Y - lineStart.Y) / (circleCenter.X - lineStart.X);
            var t = circleCenter.Y - k * circleCenter.X;

            // x^2+y^2=r^2, y=kx+t
            // x^2+(kx+t)^2=r^2  ->  (k^2+1)*x^2+2ktx+(t^2-r^2)=0
            // ax^2+bx+c=0  ->  x1=[-b+sqrt(b^2-4ac)]/2a  x2=[-b-sqrt(b^2-4ac)]/2a

            var r = circleRadius;

            var a = k * k + 1;
            var b = 2 * k * t;
            var c = t * t - r * r;

            var delta = b * b - 4 * a * c;
            if (delta < 0)
            {
                // has no intersection
                return null;
            }

            var sqrt = Math.Sqrt(delta);

            var x1 = (-b + sqrt) / (2 * a);
            var y1 = k * x1 + t;

            var x2 = (-b - sqrt) / (2 * a);
            var y2 = k * x2 + t;

            var point1 = new Point(x1, y1);
            var point2 = new Point(x2, y2);

            if ((point1 - lineStart).Length < (point2 - lineStart).Length)
            {
                return point1 - translate;
            }
            return point2 - translate;
        }

        static public HashSet<string> GetConnectedEdges(ref Canvas canv, AdjacenceList adj, int node, GraphType g_type)
        {
            var temp = new HashSet<string>();

            foreach(var item in adj.GetList[node])
            {
                switch (g_type)
                {
                    case GraphType.Directed:
                        if (canv.Children.Contains(canv.Children.OfType<Shape>().FirstOrDefault(l => l.Name == $"line_{node}_{item}")))
                        {
                            temp.Add($"line_{node}_{item}");
                        }
                        if (canv.Children.Contains(canv.Children.OfType<Shape>().FirstOrDefault(l => l.Name == $"line_{item}_{node}")))
                        {
                            temp.Add($"line_{item}_{node}");
                        }
                        break;
                    case GraphType.Undirected:
                        if (canv.Children.Contains(canv.Children.OfType<Line>().FirstOrDefault(l => l.Name == $"line_{node}_{item}")))
                        {
                            temp.Add($"line_{node}_{item}");
                        }
                        else if (canv.Children.Contains(canv.Children.OfType<Line>().FirstOrDefault(l => l.Name == $"line_{item}_{node}")))
                        {
                            temp.Add($"line_{item}_{node}");
                        }
                        break;
                    default:
                        break;
                }
              
            }
            if(g_type == GraphType.Directed)
            {
                foreach (var key in adj.GetList)
                {
                    if (adj.GetList[key.Key].Contains(node))
                    {
                        temp.Add($"line_{key.Key}_{node}");
                    }
                }
            }

            return temp;
        }
        static public Shape DrawLinkArrow(Point p1, Point p2)
        {
            GeometryGroup lineGroup = new GeometryGroup();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            Point p = new Point(p1.X + ((p2.X - p1.X) / 1), p1.Y + ((p2.Y - p1.Y) / 1));
            pathFigure.StartPoint = p;

            Point lpoint = new Point(p.X + 6, p.Y + 15);
            Point rpoint = new Point(p.X - 6, p.Y + 15);
            LineSegment seg1 = new LineSegment();
            seg1.Point = lpoint;
            pathFigure.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment();
            seg2.Point = rpoint;
            pathFigure.Segments.Add(seg2);

            LineSegment seg3 = new LineSegment();
            seg3.Point = p;
            pathFigure.Segments.Add(seg3);

            pathGeometry.Figures.Add(pathFigure);
            RotateTransform transform = new RotateTransform();
            transform.Angle = theta + 90;
            transform.CenterX = p.X;
            transform.CenterY = p.Y;
            pathGeometry.Transform = transform;
            lineGroup.Children.Add(pathGeometry);

            LineGeometry connectorGeometry = new LineGeometry();
            connectorGeometry.StartPoint = p1;
            connectorGeometry.EndPoint = p2;
            lineGroup.Children.Add(connectorGeometry);
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Data = lineGroup;
            path.StrokeThickness = 2;
            path.Stroke = path.Fill = Brushes.Black;

            return path;
        }
        static public Point AllignOfText(Ellipse AAACircle, string ellipse_name)
        {

            int el = ellipse_name.IndexOf("_");
            string el1 = ellipse_name.Substring(el + 1);
            int ellipse_count = Convert.ToInt32(el1);



            if (ellipse_count >= 0 && ellipse_count <= 9)
            {
                return new Point(Canvas.GetLeft(AAACircle) + AAACircle.Width / 2 - 5, Canvas.GetTop(AAACircle) + AAACircle.Height / 2 - 10);
            }
            else if (ellipse_count >= 9 && ellipse_count <= 99)
            {
                return new Point(Canvas.GetLeft(AAACircle) + AAACircle.Width / 2 - 10, Canvas.GetTop(AAACircle) + AAACircle.Height / 2 - 10);
            }
            else if (ellipse_count >= 99)
            {
                return new Point(Canvas.GetLeft(AAACircle) + AAACircle.Width / 2 - 15, Canvas.GetTop(AAACircle) + AAACircle.Height / 2 - 10);
            }
            return new Point(0, 0);

        }
        public static void CompareTwoCanvas(AdjacenceList list1, AdjacenceList list2, Canvas canvas1, Canvas canvas2, out Canvas bigger, out Canvas smaller)
        {
            bigger = null; smaller = null;
            if(list1.CountNodes>=list2.CountNodes)
            {
                bigger = canvas1; smaller = canvas2; return;
            }
            else if(list1.CountNodes<list2.CountNodes)
            {
                bigger = canvas2; smaller = canvas1; return;
            }
        }

    }
}
