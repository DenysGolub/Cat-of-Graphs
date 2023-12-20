using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gu.Wpf.DataGrid2D;
using Main.Classes;
using System.Web;
using Xceed.Wpf.Toolkit;
using Main.Windows;
using Main.Enumerators;
using Main.Structs;
using Main.Interfaces;
using System;
using Xceed.Wpf.AvalonDock.Layout;
using System.DirectoryServices.ActiveDirectory;
using System.Xml.Linq;

namespace Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void ChangeGraphType(object sender, MyEventArgs e);

        public delegate void FromCanvasToMatrixDelegate();

        public class MyEventArgs : EventArgs
        {
            public string AdditionalData { get; }

            public MyEventArgs(string additionalData)
            {
                AdditionalData = additionalData;
            }
        }
        public event ChangeGraphType SomethingChanged;
        

        AdjacenceList adjacenceListUndirected = new AdjacenceList(GraphType.Undirected);
        AdjacenceList adjacenceListDirected = new AdjacenceList(GraphType.Directed);
        GraphOperationsCanvas graph_operations = new GraphOperationsCanvas();

        Events.CanvasEvents.Undirected undirected_events = new Events.CanvasEvents.Undirected();
        Events.CanvasEvents.Directed directed_events = new Events.CanvasEvents.Directed();
        Events events = new Events();
        MenuColors colors = new MenuColors();

        public Events.CanvasEvents.Undirected EventsUndir
        {
            get
            {
                return undirected_events;
            }
        }

        public Events.CanvasEvents.Directed EventsDir
        {
            get
            {
                return directed_events;
            }
        }
        public Canvas GraphCanvas
        {
            get
            {
                if (DrawingCanvas_Undirected.Visibility == Visibility.Visible)
                {
                    return DrawingCanvas_Undirected;
                }
                return DrawingCanvas_Directed;
            }
        }
        public AdjacenceList GraphAdjacenceList
        {
            get
            {
                if (DrawingCanvas_Undirected.Visibility == Visibility.Visible)
                {
                    return adjacenceListUndirected;
                }
                return adjacenceListDirected;
            }
        }
        public GraphType graphType { get; set; }

        public MainWindow()
        {

            InitializeComponent();
            graphType = GraphType.Undirected;


        }



        private void IncMatrixWindow_Click(object sender, RoutedEventArgs e)
        {
            MatrixShow win = WindowsInstances.MatrixWindowInst(this);
            win.Owner = this;
            if (DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                new MatrixShow(adjacenceListUndirected, DrawingCanvas_Undirected, GraphType.Undirected).Show();
            }
            else
            {
                new MatrixShow(adjacenceListDirected, DrawingCanvas_Directed, GraphType.Directed).Show();
            }
        }

        private void AdjMatrixWindow_Click(object sender, RoutedEventArgs e)
        {
            MatrixShow win = WindowsInstances.MatrixWindowInst(this);
            win.Owner = this;
            win.AddNodeDelegate += AddNodeToCanvas;
            win.AddEdgeDelegate += AddEdgeToCanvas;
            win.DeleteNodeDelegate += DeleteNodeCanvas;
            win.DeleteEdgeDelegate += DeleteEdgeCanvas;
            if (DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {

                win.Matrix = adjacenceListUndirected;
                win.TypeGraph = GraphType.Undirected;

            }
            else
            {
                win.Matrix = adjacenceListDirected;
                win.TypeGraph = GraphType.Directed;

            }
            win.Show();
        }

        private void DeleteEdgeCanvas(string x, string y)
        {
            Canvas canvas;
            AdjacenceList dict;

            if (DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                adjacenceListUndirected.RemoveEdge(int.Parse(x), int.Parse(y));
                DrawingCanvas_Undirected.Children.Remove(DrawingCanvas_Undirected.Children.OfType<Line>().FirstOrDefault(e => e.Name == $"line_{x}_{y}"));
                DrawingCanvas_Undirected.Children.Remove(DrawingCanvas_Undirected.Children.OfType<Line>().FirstOrDefault(e => e.Name == $"line_{y}_{x}"));

            }
            else
            {
                adjacenceListDirected.RemoveEdge(int.Parse(x), int.Parse(y));
                DrawingCanvas_Directed.Children.Remove(DrawingCanvas_Directed.Children.OfType<Shape>().FirstOrDefault(e => e.Name == $"line_{x}_{y}"));
            }
        }

        private void AddEdgeToCanvas(string f_node, string s_node)
        {
            Canvas canvas;
            AdjacenceList dict;

            if (DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                dict = adjacenceListUndirected;
                canvas = DrawingCanvas_Undirected;
            }
            else
            {
                dict = adjacenceListDirected;
                canvas = DrawingCanvas_Directed;
            }

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


                if (DrawingCanvas_Directed.Visibility == Visibility.Visible)
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
        private void AddNodeToCanvas()
        {
            Canvas canvas;
            AdjacenceList dict;

            if(DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                dict = adjacenceListUndirected;
                canvas = DrawingCanvas_Undirected;
            }
            else
            {
                dict = adjacenceListDirected;
                canvas = DrawingCanvas_Directed;
            }

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

        private void DeleteNodeCanvas(int node, HashSet<string> lines)
        {
            NamesUpdate update = new NamesUpdate();

            if (DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                DrawingCanvas_Undirected.Children.Remove(DrawingCanvas_Undirected.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{node}"));
                DrawingCanvas_Undirected.Children.Remove(DrawingCanvas_Undirected.Children.OfType<TextBlock>().FirstOrDefault(e => e.Name == $"TextEllipse_{node}"));

                foreach (string s in lines)
                {
                    DrawingCanvas_Undirected.Children.Remove(DrawingCanvas_Undirected.Children.OfType<Line>().FirstOrDefault(e => e.Name == s));
                }
                update.UpdateCanvas(ref DrawingCanvas_Undirected, node);



            }
            else
            {
                DrawingCanvas_Directed.Children.Remove(DrawingCanvas_Directed.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == $"Ellipse_{node}"));
                DrawingCanvas_Directed.Children.Remove(DrawingCanvas_Directed.Children.OfType<TextBlock>().FirstOrDefault(e => e.Name == $"TextEllipse_{node}"));

                foreach (string s in lines)
                {
                    DrawingCanvas_Directed.Children.Remove(DrawingCanvas_Directed.Children.OfType<Shape>().FirstOrDefault(e => e.Name == s));
                }
                update.UpdateCanvas(ref DrawingCanvas_Directed, node);




            }
        }
        private double RandomBetween(double min, double max)
        {
            return new Random().NextDouble() * (max - min) + min;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawingCanvas_Undirected.PreviewMouseLeftButtonUp += (sender, e) => undirected_events.PreviewMouseLeftButtonUp(sender, e);
            DrawingCanvas_Undirected.PreviewMouseLeftButtonDown += (sender, e) => undirected_events.PreviewMouseLeftButtonDown(sender, e);
            DrawingCanvas_Undirected.PreviewMouseMove += (sender, e) => undirected_events.PreviewMouseMove(sender, e);

            DrawingCanvas_Directed.PreviewMouseLeftButtonDown += (sender, e) => directed_events.PreviewMouseLeftButtonDown(sender, e);
            DrawingCanvas_Directed.PreviewMouseLeftButtonUp += (sender, e) => directed_events.PreviewMouseLeftButtonUp(sender, e);
            DrawingCanvas_Directed.PreviewMouseMove += (sender, e) => directed_events.PreviewMouseMove(sender, e);

            undirected_events.Canvas = DrawingCanvas_Undirected;
            undirected_events.AdjacenceList = adjacenceListUndirected;
            undirected_events.ColorPicker = ColorUndirected;

            directed_events.Canvas = DrawingCanvas_Directed;
            directed_events.AdjacenceList = adjacenceListDirected;
            directed_events.ColorPicker = ColorDirected;

            undirected_events.SubscribeToChanges(events);
            directed_events.SubscribeToChanges(events);

            Delete.Click += (sender, e) => events.RemoveMode();
            MoveEllipse.Click += (sender, e) => events.MoveMode();
            AddEllipse.Click += (sender, e) => events.AddMode();
            Color_Button.Click += (sender, e) => events.ColorMode();
            


        }

        private void ChangeGraphToDirected(object sender, RoutedEventArgs e)
        {
            DrawingCanvas_Directed.Visibility = Visibility.Visible;
            DrawingCanvas_Undirected.Visibility = Visibility.Collapsed;

            colors.ActiveColor.Opacity = 0.5;
            UndGraph_Button.Background = colors.DisableColor;
            DirGraph_Button.Background = colors.ActiveColor;
            graphType = GraphType.Directed;
            ChangeModeInSecondGraph();
        }

        private void ChangeGraphToUndirected(object sender, RoutedEventArgs e)
        {
            DrawingCanvas_Directed.Visibility = Visibility.Collapsed;
            DrawingCanvas_Undirected.Visibility = Visibility.Visible;

            colors.ActiveColor.Opacity = 0.5;
            UndGraph_Button.Background = colors.ActiveColor;
            DirGraph_Button.Background = colors.DisableColor;
            graphType = GraphType.Undirected;
            ChangeModeInSecondGraph();
        }


        private void ChangeModeInSecondGraph()
        {
            var s_graph = Application.Current.Windows.OfType<SecondGraph>().FirstOrDefault();

            if(s_graph == null)
            {
                return;
            }

            if (DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                
               s_graph.SecondGraphsEventsInstance.ChangeToUndirectedSecond();
            }
            else
            {
                s_graph.SecondGraphsEventsInstance.ChangeToDirectedSecond();
            }
        }

        
        private Dictionary<int, HashSet<int>> CurrentVisibleGraphData(out Canvas canvas, out GraphType g_type)
        {
            if(DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                canvas = DrawingCanvas_Undirected;
                g_type = GraphType.Undirected;
                return adjacenceListUndirected.GetList;
            }
            else
            {
                canvas = DrawingCanvas_Directed;
                g_type= GraphType.Directed;
                return adjacenceListDirected.GetList;
            }
        }

        
        private void Addition_Click(object sender, RoutedEventArgs e)
        {
            var adj_list = CurrentVisibleGraphData(out Canvas canvas, out GraphType g_type);

            SetCanvasForAddition(g_type);
        }

        private void SetCanvasForAddition(GraphType g_type)
        {
            if (g_type == GraphType.Undirected)
            {
                ///При використанні out для класа AdjacenceList під час переміщення вершин на полотні
                ///списки суміжності не мають ребер. out and Class питання
                DrawingCanvas_Undirected = graph_operations.Addition(adjacenceListUndirected.GetList, ref DrawingCanvas_Undirected, g_type, out AdjacenceList adjacenceList);
                DrawingCanvas_Undirected.InvalidateVisual();
                adjacenceListUndirected.GetList = adjacenceList.GetList;

            }
            else if (g_type == GraphType.Directed)
            {
                DrawingCanvas_Directed = graph_operations.Addition(adjacenceListDirected.GetList, ref DrawingCanvas_Directed, g_type, out AdjacenceList adjacenceList);
                DrawingCanvas_Undirected.InvalidateVisual();
                adjacenceListDirected.GetList = adjacenceList.GetList; 
            }
            MatrixShow win = WindowsInstances.MatrixWindowInst(this);
            if (win.Matrix != null)
            {
                win.UpdateMatrix();
            }

        }
        private void Unity_Click(object sender, RoutedEventArgs e)
        {
            SecondGraph window = WindowsInstances.SecondGraphInst();
            window.Title = "Режим об'єднання графів";
            window.CurrentOperation = CurrentGraphOperation.Unity;
            window.Type = graphType;
            window.Owner = this;
            window.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
            {
                App.Current.Windows[intCounter].Close();
            }
        }

        private void Intersection_Click(object sender, RoutedEventArgs e)
        {
            SecondGraph window = WindowsInstances.SecondGraphInst();
            window.Title = "Режим перетину графів";
            window.CurrentOperation = CurrentGraphOperation.Intersection;
            window.Type = graphType;
            window.Owner = this;
            window.Show();
        }

        private void CircleSum_Click(object sender, RoutedEventArgs e)
        {
            SecondGraph window = WindowsInstances.SecondGraphInst();
            window.Title = "Режим кільцевої суми графів";
            window.CurrentOperation = CurrentGraphOperation.CircleSum;
            window.Type = graphType;
            window.Owner = this;
            window.Show();
        }

        private void CartesianProduct_Click(object sender, RoutedEventArgs e)
        {
            SecondGraph window = WindowsInstances.SecondGraphInst();
            window.Title = "Режим декартового добутку графів";
            window.CurrentOperation = CurrentGraphOperation.CartesianProduct;
            window.Type = graphType;
            window.Owner = this;
            window.Show();
        }
    }
}