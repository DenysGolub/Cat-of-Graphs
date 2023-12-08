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

namespace Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void ChangeGraphType(object sender, MyEventArgs e);

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

       
        public MainWindow()
        {

            InitializeComponent();
            
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
            ChangeModeInSecondGraph();
        }

        private void ChangeGraphToUndirected(object sender, RoutedEventArgs e)
        {
            DrawingCanvas_Directed.Visibility = Visibility.Collapsed;
            DrawingCanvas_Undirected.Visibility = Visibility.Visible;

            colors.ActiveColor.Opacity = 0.5;
            UndGraph_Button.Background = colors.ActiveColor;
            DirGraph_Button.Background = colors.DisableColor;
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
        }
        private void Unity_Click(object sender, RoutedEventArgs e)
        {
            SecondGraph window = new SecondGraph();
            window.Owner = this;
            window.Show();
            ChangeModeInSecondGraph();
        }
    }
}