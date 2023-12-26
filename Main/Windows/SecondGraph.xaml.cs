using Main.Classes;
using Main.Enumerators;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
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

            undirected_events.ClassOwner = this;
            directed_events.ClassOwner = this;

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
            MatrixController.Incidence(this);
        }

        private void AdjMatrixWindow_Click(object sender, RoutedEventArgs e)
        {
            MatrixController.Adjacence(this);
        }

        private void ShowResultGraph(object sender, RoutedEventArgs e)
        {
            MainWindow wnd = WindowsInstances.MainWindowInst;

            DataFromGraph.CompareTwoCanvas(wnd.GraphAdjacenceList, SecondGraphAdjacenceList, wnd.GraphCanvas, SecondGraphCanvas, out Canvas bigger, out Canvas smaller);

            SetWindowInfo.ResultWindow(CurrentOperation, Type, bigger, smaller, wnd.GraphAdjacenceList, SecondGraphAdjacenceList);
          
        }



        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            AdjacenceList list = CurrentVisibleGraphData(out Canvas canvas);
            FileSystem.NullData(ref canvas, ref list);
        }

        private void Load(string path)
        {
            Canvas canv = null;
            AdjacenceList list = null;
            if (path.Contains(".cogu") && Type == GraphType.Undirected)
            {
                DrawingCanvas_Undirected.Visibility = Visibility.Visible;
                DrawingCanvas_Directed.Visibility = Visibility.Collapsed;
                canv = DrawingCanvas_Undirected;
                list = adjacenceListUndirected;
            }
            else if (path.Contains(".cogd") && Type == GraphType.Directed)
            {
                DrawingCanvas_Undirected.Visibility = Visibility.Collapsed;
                DrawingCanvas_Directed.Visibility = Visibility.Visible;
                canv = DrawingCanvas_Directed;
                list = adjacenceListDirected;
            }
            else
            {
                return;
            }

            FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);

            Canvas savedCanvas = XamlReader.Load(fs) as Canvas;
            fs.Close();

            FileSystem.Load(ref canv, savedCanvas, ref list);
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файли графа (*.cogu; *.cogd)|*.cogu; *.cogd|Всі файли (*.*)|*.*"; // Filter files by extension

            if (openFileDialog.ShowDialog() == true)
            {
                Load(openFileDialog.FileName);
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            CurrentVisibleGraphData(out Canvas canvas);
            FileSystem.Save(canvas, CurrentVisibleGraphData());
        }

        private GraphType CurrentVisibleGraphData()
        {
            if (DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                return GraphType.Undirected;
            }
            return GraphType.Directed;
        }

        private AdjacenceList CurrentVisibleGraphData(out Canvas canv)
        {
            if (DrawingCanvas_Undirected.Visibility == Visibility.Visible)
            {
                canv = DrawingCanvas_Undirected;
                return adjacenceListUndirected;
            }

            canv = DrawingCanvas_Directed;
            return adjacenceListDirected;
        }
    }
}
