﻿using System.Windows;
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
using System.Windows.Shapes;
using System.Windows.Media;
using Main.TestingPart;
using System.Text.Json;
using System.Windows.Input;
using System.Xml;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;
using Newtonsoft.Json;
using System.Windows.Media.Media3D;
using System.Data;
using System;
using System.Reflection.Metadata;
using System.Collections;
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
        List<Question> questions = new List<Question>();

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

            QuestionsListBox.ItemsSource = questions;

            
        }


        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            if (e.Key == Key.Enter)
            {
                var currentRowIndex = dataGrid.Items.IndexOf(dataGrid.CurrentItem);
                var currentColumnIndex = dataGrid.CurrentColumn.DisplayIndex;

                // Check if the Enter key was pressed in the last cell of the last row
                if (currentRowIndex == dataGrid.Items.Count - 1 && currentColumnIndex == dataGrid.Columns.Count - 1)
                {
                    // Add a new item to the ItemsSource (assuming it's bound to a collection)
                    (dataGrid.ItemsSource as ObservableCollection<Question>).Add(new Question());

                    // Move the focus to the new row
                    dataGrid.ScrollIntoView(dataGrid.Items[dataGrid.Items.Count - 1]);
                    dataGrid.Focus();
                    dataGrid.SelectedIndex = dataGrid.Items.Count - 1;

                    // Begin editing the first cell of the new row
                    dataGrid.BeginEdit();
                }
            }
        }


        private void InlineEditTextBox(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                TextBox textBox = new TextBox();
                textBox.Text = textBlock.Text;
                textBox.LostFocus += (s, args) =>
                {
                    // Update your data object with the new value
                    textBlock.Text = textBox.Text;
                    // Remove the TextBox and show the TextBlock again
                    (textBlock.Parent as ContentPresenter).Content = textBlock;
                };
                // Replace the TextBlock with the TextBox
                textBlock.Parent.SetValue(ContentPresenter.ContentProperty, textBox);
                textBox.Focus();
                textBox.SelectAll();
            }
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



           /* undir.PreviewMouseLeftButtonUp += (sender, e) => undirected_events.PreviewMouseLeftButtonUp(sender, e);
            undir.PreviewMouseLeftButtonDown += (sender, e) => undirected_events.PreviewMouseLeftButtonDown(sender, e);
            undir.PreviewMouseMove += (sender, e) => undirected_events.PreviewMouseMove(sender, e);

            

            dir.PreviewMouseLeftButtonDown += (sender, e) => directed_events.PreviewMouseLeftButtonDown(sender, e);
            dir.PreviewMouseLeftButtonUp += (sender, e) => directed_events.PreviewMouseLeftButtonUp(sender, e);
            dir.PreviewMouseMove += (sender, e) => directed_events.PreviewMouseMove(sender, e);*/

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

            DrawingCanvas_Undirected.Height = CanvasGrid.ActualHeight;
            DrawingCanvas_Undirected.Width = CanvasGrid.ActualWidth;

            DrawingCanvas_Directed.Height = CanvasGrid.ActualHeight;
            DrawingCanvas_Directed.Width = CanvasGrid.ActualWidth;

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

        void ListViewItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
            XmlElement book = ((ListViewItem)sender).Content as XmlElement;
            //var hitInfo = QuestionsListBox.InputHitTest(e.Location);

            if (book == null)
            {
                return;
            }

            if (book.GetAttribute("Stock") == "out")
            {
                MessageBox.Show("Time to order more copies of " + book["Title"].InnerText);
            }
            else
            {
                MessageBox.Show(book["Title"].InnerText + " is available.");
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
            Main.Windows.AdjacenceMatrix win = WindowsInstances.AdjacenceMatrixWindowInst(this);
            Main.Windows.IncidenceMatrix inc_win = WindowsInstances.MatrixIncidenceWindowInst(this);

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

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*if(CanvasGrid.ActualWidth > 0)
            {
                // Adjust the size of the canvas to match the size of the grid
                GraphCanvas.Width = CanvasGrid.ActualWidth;
                GraphCanvas.Height = CanvasGrid.ActualHeight;

                // Adjust the position of each child element of the canvas
                foreach (var child in GraphCanvas.Children)
                {

                    if (child is UIElement uiElement && child is Ellipse)
                    {
                        // Get the current position of the child element
                        double currentLeft = Canvas.GetLeft(uiElement);
                        double currentTop = Canvas.GetTop(uiElement);

                        // Calculate the new position based on the change in size of the canvas
                        double newLeft = currentLeft * CanvasGrid.ActualWidth / e.PreviousSize.Width;
                        double newTop = currentTop * CanvasGrid.ActualHeight / e.PreviousSize.Height;

                        // Update the position of the child element
                        Canvas.SetLeft(uiElement, newLeft);
                        Canvas.SetTop(uiElement, newTop);
                    }
                    else if(child is UIElement && child is TextBlock)
                    {
                        DataFromGraph.AllignOfText()
                    }
                }

                GraphCanvas.InvalidateVisual();*/

            /*DrawingCanvas_Undirected.Height = CanvasGrid.ActualHeight;
            DrawingCanvas_Undirected.Width = CanvasGrid.ActualWidth;

            DrawingCanvas_Directed.Height = CanvasGrid.ActualHeight;
            DrawingCanvas_Directed.Width = CanvasGrid.ActualWidth;

            undir.InvalidateVisual();

            DrawingCanvas_Undirected.InvalidateMeasure();
            DrawingCanvas_Undirected.InvalidateArrange();
            undir.InvalidateMeasure();
            undir.InvalidateArrange();*/
        }

        private void DrawingCanvas_Undirected_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
           /* var element = (UIElement)sender;
            var position = e.GetPosition(element);
            var transform = (MatrixTransform)element.RenderTransform;
            var matrix = transform.Matrix;
            var scale = e.Delta >= 0 ? 1.1 : (1.0 / 1.1); // choose appropriate scaling factor

            matrix.ScaleAtPrepend(scale, scale, position.X, position.Y);
            transform.Matrix = matrix;*/
        }
      
        private BitmapSource ConvertCanvasToImage(Canvas canvas)
        {
            // Create a RenderTargetBitmap
            var renderBitmap = new RenderTargetBitmap(
                (int)canvas.ActualWidth,
                (int)canvas.ActualHeight,
                96d, 96d,
                PixelFormats.Pbgra32
            );

            // Render the Canvas onto the RenderTargetBitmap
            renderBitmap.Render(canvas);

            // Convert the RenderTargetBitmap to a BitmapSource
            return renderBitmap;
        }

        private void CopyCanvasToImage(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuItem)?.Tag?.ToString() == "GraphAddToFile")
            {
                // Get the selected cell
                var selectedCell = QuestionsListBox.CurrentCell;
                if (selectedCell == null)
                    return;

                // Get the selected question item
                var selectedQuestion = selectedCell.Item as Question;
                if (selectedQuestion == null)
                    return;

                // Convert the GraphCanvas to an image
                BitmapSource imageSource = ConvertCanvasToImage(GraphCanvas);

                // Create a MemoryStream to hold the image data
                MemoryStream ms = new MemoryStream();

                // Create a BitmapEncoder and save the image to the MemoryStream
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageSource));
                encoder.Save(ms);

                // Optionally, save the image to a file
                // File.WriteAllBytes("image.png", ms.ToArray());

                // Set the image source of the selected question
                selectedQuestion.ImageSource = ms.ToArray();

                // Set other properties of the selected question as needed
                selectedQuestion.GraphType = graphType;

                this.QuestionsListBox.CommitEdit();
                this.QuestionsListBox.CommitEdit();

                this.QuestionsListBox.CancelEdit();
                this.QuestionsListBox.CancelEdit();

                QuestionsListBox.Items.Refresh();
                
                try
                {
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void ImgCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var quest = QuestionsListBox.CurrentCell.Item as Question;

            ImageWindow imgwin;
            if(quest!=null && e.ClickCount == 2)
            {
              
                WindowsInstances.ImageWindowInst(LoadImage(quest.ImageSource));
               
            }
            
   
            
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void TestingFileSave_Click(object sender, RoutedEventArgs e)
        {

            string fileName = "C:\\Users\\User\\Desktop\\TestFile.json";
            string jsonString = JsonConvert.SerializeObject(questions);


            File.WriteAllText(fileName, jsonString);

            ObservableCollection<Question> deserializedProduct = JsonConvert.DeserializeObject<ObservableCollection<Question>>(jsonString);

            Console.WriteLine();
        }

        private void TestingFileOpen_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "C:\\Users\\User\\Desktop\\TestFile.json";

            ObservableCollection<Question> deserializedProduct = JsonConvert.DeserializeObject<ObservableCollection<Question>>(File.ReadAllText(fileName));

            // QuestionsListBox.ItemsSource = deserializedProduct;
            questions = new List<Question>(deserializedProduct); ///ДОРОБИТИ ТЕСТУВАЛЬНІ ВІКНА ТА САМ ПРОЦЕС
            QuestionsListBox.ItemsSource = questions;  
            QuestionsListBox.Items.Refresh();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewQuestion_Click(object sender, RoutedEventArgs e)
        {
            var b = QuestionsListBox.Items;
            questions.Add(new Question() { GraphType = graphType, State = new AdjMatrixStateParameter() { State = "AdjMatrix"}
            });
            
            this.QuestionsListBox.CommitEdit();
            this.QuestionsListBox.CommitEdit();

            this.QuestionsListBox.CancelEdit();
            this.QuestionsListBox.CancelEdit();


            CorrAnswTagProp.TagPropertyName = "AdjMCorrect";
            QuestionsListBox.Items.Refresh();
        }

        private void Points_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Handle cell editing ending here
            // You can access the TextBox.Text property to get the edited value
            var textBox = sender as TextBox;
            var editedText = textBox.Text;
            Console.WriteLine($"Edited text: {editedText}");
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            
            var textBox = sender as ComboBox;
            var editedText = textBox.SelectedItem;
            Console.WriteLine($"Edited text: {editedText}");
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
     QuestionsListBox.SelectedItem = ((ComboBox)sender).DataContext;

        private void MatrixLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var quest = QuestionsListBox.CurrentCell.Item as Question;
            if (quest != null && e.ClickCount == 2)
            {
                if ((QuestionsListBox.CurrentCell.Column as Main.TestingPart.CustomDataGridTemplateColumn).TagPropertyName == "IncM")
                {
                    var window = new Main.TestingPart.IncidenceMatrix();
                    window.TypeGraph = quest.GraphType;
                    window.Matrix = quest.IncMatrix; //ЗРОБИТИ ОКРЕМІ ВІКНА МАТРИЦЬ ДЛЯ ТЕСТУВАЛЬНОЇ ЧАСТИНИ
                    window.Matrix.Type = quest.GraphType;
                    window.Show();
                }
                else if ((QuestionsListBox.CurrentCell.Column as Main.TestingPart.CustomDataGridTemplateColumn).TagPropertyName == "AdjM")
                {
                    var window = new Main.TestingPart.AdjacenceMatrix();
                    window.TypeGraph = quest.GraphType;
                    window.Matrix = quest.AdjMatrix;
                    window.Matrix.Type = quest.GraphType;
                    window.Show();
                }
                else if ((QuestionsListBox.CurrentCell.Column as Main.TestingPart.CustomDataGridTemplateColumn).TagPropertyName == "AdjMCorrect")
                {
                    var window = new Main.TestingPart.AdjacenceMatrix();
                    window.TypeGraph = quest.GraphType;
                    window.Matrix = quest.CorrectAnswer;
                   
                    window.Matrix.Type = quest.GraphType;
                    window.Show();
                }
            }
        }

        private void CurrentGraphMatrixToCorrect(object sender, RoutedEventArgs e)
        {
            var quest = QuestionsListBox.CurrentCell.Item as Question;

            if(quest!=null)
            {
              
                quest.CorrectAnswer.GetList = new Dictionary<int, HashSet<int>> (GraphAdjacenceList.GetList);

                this.QuestionsListBox.CommitEdit();
                this.QuestionsListBox.CommitEdit();

                this.QuestionsListBox.CancelEdit();
                this.QuestionsListBox.CancelEdit();

                QuestionsListBox.Items.Refresh();
            }
        }

        private void CurrentGraphMatrixToFile( object sender, RoutedEventArgs e)
        {
            var quest = QuestionsListBox.CurrentCell.Item as Question;

            if (quest != null)
            {

                quest.AdjMatrix.GetList = new Dictionary<int, HashSet<int>>(GraphAdjacenceList.GetList);

                this.QuestionsListBox.CommitEdit();
                this.QuestionsListBox.CommitEdit();

                this.QuestionsListBox.CancelEdit();
                this.QuestionsListBox.CancelEdit();

                quest.GraphType = graphType;
                
                QuestionsListBox.Items.Refresh();
            }
        }

        private void StartTest_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "C:\\Users\\User\\Desktop\\TestFile.json";

            ObservableCollection<Question> deserializedProduct = JsonConvert.DeserializeObject<ObservableCollection<Question>>(File.ReadAllText(fileName));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            double score = 0;
            foreach(var quest in deserializedProduct)
            {
                if(quest.QuestionsType == QuestionsType.ToGraphFromAdjacenceMatrix)
                {
                    if(new TestingPart.QuestionsAnsweringWindows.ToGraphFromAdjMatrixWin(quest).ShowDialog() == true)
                    {
                        score += quest.Points;
                    }
                }
                else if (quest.QuestionsType == QuestionsType.ToAdjacenceMatrixFromGraph)
                {
                    if (new TestingPart.QuestionsAnsweringWindows.ToAdjMatrixFromGraphWin(quest).ShowDialog() == true)
                    {
                        score += quest.Points;
                    }
                }

            }
            TimeSpan elapsedTime = stopwatch.Elapsed;

            int hours = elapsedTime.Hours;
            int minutes = elapsedTime.Minutes;
            int seconds = elapsedTime.Seconds;
            stopwatch.Stop();
            MessageBox.Show($"Набрана кількість балів: {score}\nЧас виконання: {hours:D2}:{minutes:D2}:{seconds:D2}", "Результати тестування");
        }
    }
}   