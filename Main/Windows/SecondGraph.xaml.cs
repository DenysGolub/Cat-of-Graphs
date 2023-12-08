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

        Events.CanvasEvents.Undirected undirected_events = new Events.CanvasEvents.Undirected();
        Events.CanvasEvents.Directed directed_events = new Events.CanvasEvents.Directed();
        Events events = new Events();

        public Events SecondGraphsEventsInstance => events;
        public SecondGraph()
        {
            InitializeComponent();
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

    }
}
