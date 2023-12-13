using Main.Classes;
using Main.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Main.Windows
{
    /// <summary>
    /// Interaction logic for SecondGraph.xaml
    /// </summary>
    public partial class SecondGraph : Window
    {
        AdjacenceList adjacenceListUndirected = new AdjacenceList(GraphType.Undirected);
        AdjacenceList adjacenceListDirected = new AdjacenceList(GraphType.Directed);
        GraphOperationsCanvas graph_operations = new GraphOperationsCanvas();

        Events.CanvasEvents.Undirected undirected_events = new Events.CanvasEvents.Undirected();
        Events.CanvasEvents.Directed directed_events = new Events.CanvasEvents.Directed();
        Events events = new Events();
        GraphType _type;
        public Events SecondGraphsEventsInstance => events;
        public Canvas SecondGraphCanvas
        {
            get
            {
                if(DrawingCanvas_Undirected.Visibility==Visibility.Visible)
                {
                    return DrawingCanvas_Undirected;
                }
                return DrawingCanvas_Directed;
            }
        }
        public AdjacenceList SecondGraphAdjacenceList
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
        public GraphType Type
        {
            get { return _type; }
            set
            {
                if (value == GraphType.Undirected)
                {
                    DrawingCanvas_Directed.Visibility = Visibility.Collapsed;
                    DrawingCanvas_Undirected.Visibility = Visibility.Visible;
                }
                else if (value == GraphType.Directed)
                {
                    DrawingCanvas_Directed.Visibility = Visibility.Visible;
                    DrawingCanvas_Undirected.Visibility = Visibility.Collapsed;
                }
                _type = value;
            }
        }
        public CurrentGraphOperation CurrentOperation { get; set; }
        public SecondGraph()
        {
            InitializeComponent();
            Closed += (sender, e) => WindowsInstances.WindowClosed(sender, e);

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

            directed_events.Canvas = DrawingCanvas_Directed;
            directed_events.AdjacenceList = adjacenceListDirected;

            undirected_events.SubscribeToChangesInSecondGraphs(events);
            directed_events.SubscribeToChangesInSecondGraphs(events);

            Delete.Click += (sender, e) => events.RemoveMode();
            MoveEllipse.Click += (sender, e) => events.MoveMode();
            AddEllipse.Click += (sender, e) => events.AddMode();

        }

        private void IncMatrixWindow_Click(object sender, RoutedEventArgs e)
        {
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
            if (DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                new MatrixShow(adjacenceListUndirected).Show();
            }
            else
            {
                new MatrixShow(adjacenceListDirected).Show();
            }
        }

        private void ShowResultGraph(object sender, RoutedEventArgs e)
        {
            MainWindow wnd = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            Canvas result_graph=null;
            OperationResult window=WindowsInstances.ResultWindowInst();

           

            if (CurrentOperation==CurrentGraphOperation.Unity)
            {
                Canvas bigger_canvas = wnd.GraphAdjacenceList.GetList.Keys.Count > SecondGraphAdjacenceList.GetList.Keys.Count ? wnd.GraphCanvas : SecondGraphCanvas;
                window.Title = "Результат об'єднання графів";

                result_graph = graph_operations.Unity(bigger_canvas, wnd.GraphAdjacenceList.GetList, SecondGraphAdjacenceList.GetList, Type, out AdjacenceList unity);
            }
            else if(CurrentOperation==CurrentGraphOperation.Intersection)
            {
                Canvas smaller_canvas = wnd.GraphAdjacenceList.GetList.Keys.Count < SecondGraphAdjacenceList.GetList.Keys.Count ? wnd.GraphCanvas : SecondGraphCanvas;
                window.Title = "Результат перетину графів";

                result_graph = graph_operations.Intersection(smaller_canvas, wnd.GraphAdjacenceList.GetList, SecondGraphAdjacenceList.GetList, Type, out AdjacenceList intersection);
            }
            else if(CurrentOperation==CurrentGraphOperation.CircleSum)
            {
                Canvas bigger_canvas = wnd.GraphAdjacenceList.GetList.Keys.Count > SecondGraphAdjacenceList.GetList.Keys.Count ? wnd.GraphCanvas : SecondGraphCanvas;
                window.Title = "Результат кільцевої суми графів";

                result_graph = graph_operations.CircleSum(bigger_canvas, wnd.GraphAdjacenceList.GetList, SecondGraphAdjacenceList.GetList, Type, out AdjacenceList circlesum);
            }
            else if (CurrentOperation == CurrentGraphOperation.CartesianProduct)
            {
                window.Title = "Результат декартового добутку графів";

                result_graph = graph_operations.CartesianProduct(wnd.GraphAdjacenceList.GetList, SecondGraphAdjacenceList.GetList, Type);
            }

            window.SetCanvas(result_graph);
            window.Owner = this;
            window.Show();
        }
    }
}
