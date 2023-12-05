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

namespace Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AdjacenceList adjacenceListUndirected = new AdjacenceList(GraphType.Undirected);
        AdjacenceList adjacenceListDirected = new AdjacenceList(GraphType.Directed);

        Events.CanvasEvents.Undirected undirected_events = new Events.CanvasEvents.Undirected();
        Events.CanvasEvents.Directed directed_events = new Events.CanvasEvents.Directed();
        Events events = new Events();

       
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
           
            DrawingCanvas_Undirected.Visibility = Visibility.Collapsed;
            DrawingCanvas_Directed.Visibility = Visibility.Visible;

            var c = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACD1FF"));
            c.Opacity = 0.5;
            DirGraph_Button.Background = c;
            UndGraph_Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        }

        private void ChangeGraphToUndirected(object sender, RoutedEventArgs e)
        {

            DrawingCanvas_Directed.Visibility = Visibility.Collapsed;
            DrawingCanvas_Undirected.Visibility = Visibility.Visible;

            color_active.Opacity = 0.5;
            UndGraph_Button.Background = color_active;
            DirGraph_Button.Background = color_disable;

        }




        SolidColorBrush color_disable = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        SolidColorBrush color_active = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACD1FF"));

     

       

        











     
    }
}