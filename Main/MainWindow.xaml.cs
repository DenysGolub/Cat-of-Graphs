using System.Windows;
using System.Windows.Controls;
using Gu.Wpf.DataGrid2D;
using Main.Classes;
using Main.Windows;
using Main.Enumerators;
using Main.Structs;
using Main.Interfaces;
using System.Windows.Interop;
using System.IO;
using System.Windows.Markup;
using Microsoft.Win32;
using Infralution.Localization.Wpf;
//using DynamicLocalization;
using System.Windows.Documents;
using System.Diagnostics;
namespace Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void ChangeGraphType(object sender, MyEventArgs e);

        public delegate void ChangeCulture(string code);
        
        public class MyEventArgs : EventArgs
        {
            public string AdditionalData { get; }

            public MyEventArgs(string additionalData)
            {
                AdditionalData = additionalData;
            }
        }
        public event ChangeGraphType SomethingChanged;
        public event ChangeCulture CultureChanged;

        AdjacenceList adjacenceListUndirected = new AdjacenceList(GraphType.Undirected);
        AdjacenceList adjacenceListDirected = new AdjacenceList(GraphType.Directed);
        GraphOperationsCanvas graph_operations = new GraphOperationsCanvas();

        Events.CanvasEvents.Undirected undirected_events = new Events.CanvasEvents.Undirected();
        Events.CanvasEvents.Directed directed_events = new Events.CanvasEvents.Directed();
        Events events = new Events();
        MenuColors colors = new MenuColors();

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
            set
            {
                if (DrawingCanvas_Undirected.Visibility == Visibility.Visible)
                {
                    adjacenceListUndirected = value;
                    return;
                }
                adjacenceListDirected = value;
            }
        }
        public GraphType graphType { get; set; }

        public MainWindow()
        {
            

            InitializeComponent();
            foreach (MenuItem item in menuItemLanguages.Items)
            {
                if (item.Tag.ToString() == CultureManager.UICulture.Name)
                {
                    item.IsChecked = true; break;
                }
            }

            
            graphType = GraphType.Undirected;
            undirected_events.ClassOwner = this;
            directed_events.ClassOwner = this;

            this.Loaded += (s, e) =>
            {
                MainWindow.WindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
                HwndSource.FromHwnd(MainWindow.WindowHandle)?.AddHook(new HwndSourceHook(HandleMessages));
            };


           /* LocUtil.SetDefaultLanguage(this);

            // Adjust checked language menu item
            foreach (MenuItem item in menuItemLanguages.Items)
            {
                if (item.Tag.ToString().Equals(LocUtil.GetCurrentCultureName(this)))
                    item.IsChecked = true;
            }*/

        }

        public static IntPtr WindowHandle { get; private set; }

        internal static void HandleParameter(string[] args)
        {
            if (args.Length == 1)
            {
                string p = args[0];
                if (Application.Current?.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.Load(p);
                }
            }
            if (args.Length > 1)
            {
                string p = string.Join(" ", args);
                if (Application.Current?.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.Load(p);
                }
            }
            ///ДОРОБИТИ ФАЙЛОВУ СИСТЕМУ. ПЕРЕВОД ЗА МОЖЛИВОСТІ ТЕЖ
        }

        private static IntPtr HandleMessages
        (IntPtr handle, int message, IntPtr wParameter, IntPtr lParameter, ref Boolean handled)
        {
            var data = UnsafeNative.GetMessage(message, lParameter);
            if (data != null)
            {
                if (Application.Current.MainWindow == null)
                    return IntPtr.Zero;

                if (Application.Current.MainWindow.WindowState == System.Windows.WindowState.Minimized)
                {
                    Application.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
                }

                UnsafeNative.SetForegroundWindow(new WindowInteropHelper
                                                (Application.Current.MainWindow).Handle);

                var args = data.Split(' ');
                HandleParameter(args);
                handled = true;
            }

            return IntPtr.Zero;
        }

        private void IncMatrixWindow_Click(object sender, RoutedEventArgs e)
        {
            MatrixController.Incidence(this);
        }

        private void AdjMatrixWindow_Click(object sender, RoutedEventArgs e)
        {
            MatrixController.Adjacence(this);
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

            UndGraph_Button.Background = colors.DisableColor;
            DirGraph_Button.Background = colors.ActiveColor;
            graphType = GraphType.Directed;
            if (ColorUndirected.Visibility == Visibility.Visible)
            {
                ColorUndirected.Visibility = Visibility.Collapsed;
                ColorDirected.Visibility = Visibility.Visible;
            }
            ChangeModeInSecondGraph();

            if (WindowsInstances.AdjacenceMatrixWindowExist(this, out int ind))
            {
                MatrixController.Adjacence(this);
            }

            if (WindowsInstances.MatrixIncidenceWindowExist(this, out int ind1))
            {
                MatrixController.Incidence(this);
            }
        }

        private void ChangeGraphToUndirected(object sender, RoutedEventArgs e)
        {
            DrawingCanvas_Directed.Visibility = Visibility.Collapsed;
            DrawingCanvas_Undirected.Visibility = Visibility.Visible;

            UndGraph_Button.Background = colors.ActiveColor;
            DirGraph_Button.Background = colors.DisableColor;
            graphType = GraphType.Undirected;

            if (ColorDirected.Visibility == Visibility.Visible)
            {
                ColorDirected.Visibility = Visibility.Collapsed;
                ColorUndirected.Visibility = Visibility.Visible;
            }
            ChangeModeInSecondGraph();

            if(WindowsInstances.AdjacenceMatrixWindowExist(this, out int ind)) 
            {
                MatrixController.Adjacence(this);
            }

            if(WindowsInstances.MatrixIncidenceWindowExist(this, out int ind1)) 
            {
                MatrixController.Incidence(this);
            }
        }

        private void Load(string path)
        {
            Canvas canv = null;
            AdjacenceList list = null;
            if (path.Contains(".cogu"))
            {
                DrawingCanvas_Undirected.Visibility = Visibility.Visible;
                DrawingCanvas_Directed.Visibility = Visibility.Collapsed;
                UndGraph_Button.Background = colors.ActiveColor;
                DirGraph_Button.Background = colors.DisableColor;
                canv = DrawingCanvas_Undirected;
                list = adjacenceListUndirected;
                graphType = GraphType.Undirected;
            }
            else if (path.Contains(".cogd"))
            {
                DrawingCanvas_Undirected.Visibility = Visibility.Collapsed;
                DrawingCanvas_Directed.Visibility = Visibility.Visible;
                UndGraph_Button.Background = colors.DisableColor;
                DirGraph_Button.Background = colors.ActiveColor;
                canv = DrawingCanvas_Directed;
                list = adjacenceListDirected;
                graphType = GraphType.Directed;
            }
            else
            {
                return;
            }

            FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);

            Canvas savedCanvas = XamlReader.Load(fs) as Canvas;
            fs.Close();

            FileSystem.Load(ref canv, savedCanvas, ref list);

            if (WindowsInstances.AdjacenceMatrixWindowExist(this, out int ind))
            {
                MatrixController.Adjacence(this);
            }

            if (WindowsInstances.MatrixIncidenceWindowExist(this, out int ind1))
            {
                MatrixController.Incidence(this);
            }
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

        private GraphType CurrentVisibleGraphData()
        {
            if(DrawingCanvas_Undirected.Visibility == Visibility.Visible)
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

        private void Addition_Click(object sender, RoutedEventArgs e)
        {
            SetCanvasForAddition(CurrentVisibleGraphData());
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
            AdjacenceMatrix win = WindowsInstances.AdjacenceMatrixWindowInst(this);
            IncidenceMatrix inc_win = WindowsInstances.MatrixIncidenceWindowInst(this);

            if (win.Matrix != null)
            {
                win.UpdateMatrix();
            }

            if (inc_win.Matrix != null)
            {
                inc_win.UpdateMatrix();
            }

        }
        private void Unity_Click(object sender, RoutedEventArgs e)
        {
            SetWindowInfo.SecGraphWindow(CurrentGraphOperation.Unity, graphType);
        }

        private void Intersection_Click(object sender, RoutedEventArgs e)
        {
            SetWindowInfo.SecGraphWindow(CurrentGraphOperation.Intersection, graphType);
        }

        private void CircleSum_Click(object sender, RoutedEventArgs e)
        {
            SetWindowInfo.SecGraphWindow(CurrentGraphOperation.CircleSum, graphType);
        }

        private void CartesianProduct_Click(object sender, RoutedEventArgs e)
        {
            SetWindowInfo.SecGraphWindow(CurrentGraphOperation.CartesianProduct, graphType);
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            AdjacenceList list = CurrentVisibleGraphData(out Canvas canvas);
            FileSystem.NullData(ref canvas, ref list);
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файли графа (*.cogu; *.cogd)|*.cogu; *.cogd|Всі файли (*.*)|*.*"; // Filter files by extension

            if(openFileDialog.ShowDialog() == true)
            {
                Load(openFileDialog.FileName);
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            CurrentVisibleGraphData(out Canvas canvas);
            FileSystem.Save(canvas, CurrentVisibleGraphData());
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
            {
                App.Current.Windows[intCounter].Close();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            SetWindowInfo.AboutWindow();
        }

        private void DFS_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<int, int> visited = new Dictionary<int, int>();

            foreach (var item in GraphAdjacenceList.GetList.Keys)
            {
                visited.Add(item, 0);
            }

            //var canvas = GraphCanvas;

            // Use Dispatcher to call the recursive method on the UI thread
            SearchAlgorithms.DFS(GraphAdjacenceList, GraphCanvas, 1, visited);

        }

        private void Lang_Click(object sender, RoutedEventArgs e)
        {
            foreach (MenuItem item in menuItemLanguages.Items)
            {
                item.IsChecked = false;
            }

            MenuItem mi = sender as MenuItem; //Console.WriteLine("menu tag: " + mi.Tag.ToString());
            mi.IsChecked = true;
            //SwitchLanguage(mi.Tag.ToString());
            string lang = mi.Tag.ToString();

            CultureManager.UICulture = new System.Globalization.CultureInfo(lang);

            if (CultureChanged != null)
            {
                CultureChanged(lang);
            }
        }

        private void Cycle_Click(object sender, RoutedEventArgs e)
        {
            bool isCyclic = false;
            if (graphType == GraphType.Directed)
            {
                isCyclic = SearchAlgorithms.IsCycleExistInDirectedGraph(GraphAdjacenceList, 1, new HashSet<int>(), new HashSet<int>());
            }
            else
            {
                //isCyclic = SearchAlgorithms.IsCycleExistInUndirectedGraph(GraphAdjacenceList, 1, new HashSet<int>());
            }
            MessageBox.Show(isCyclic ? "True" : "False");


        }
    }
}