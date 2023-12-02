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

        Point DragStartPoint, DragEndPoint, ObjectStartLocation;
        object ClickedObject;

        Ellipse firstEllipse, secondEllipse, selectedEllips;
        string firstEllipseName, secondEllipseName;
        bool isLeftMouseDown = false;

        bool vis, edit, add_ellipse, remove, move, color;

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

        private void ChangeGraphToDirected(object sender, RoutedEventArgs e)
        {
           /* OperationWindow operwin_inst = Application.Current.Windows.OfType<OperationWindow>().FirstOrDefault();

            if (operwin_inst != null)
            {
                operwin_inst.UndCanv.Visibility = Visibility.Collapsed;
                operwin_inst.DirCanv.Visibility = Visibility.Visible;
            }*/
            DrawingCanvas_Undirected.Visibility = Visibility.Collapsed;
            DrawingCanvas_Directed.Visibility = Visibility.Visible;
            //DrawingCanvas_Directed.Cursor = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("WpfApp1.Resources.Cursors.cursor_dir.cur"));

/*            click_dir.IsChecked = true;
            click_undir.IsChecked = false;*/
            //MatrixOfIncidentyDirected(lineNamesDirected);
            //adjacence.ConverterTo2DArray(ref Dictionary_DirectedGraphs, DrawingCanvas_Directed);

            var c = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACD1FF"));
            c.Opacity = 0.5;
            DirGraph_Button.Background = c;
            UndGraph_Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
            ClickedObject = null;
        }

        private void ChangeGraphToUndirected(object sender, RoutedEventArgs e)
        {
   /*         OperationWindow operwin_inst = Application.Current.Windows.OfType<OperationWindow>().FirstOrDefault();

            if (operwin_inst != null)
            {
                operwin_inst.DirCanv.Visibility = Visibility.Collapsed;
                operwin_inst.UndCanv.Visibility = Visibility.Visible;
            }*/
            /*click_undir.IsChecked = true;
            click_dir.IsChecked = false;*/

            DrawingCanvas_Directed.Visibility = Visibility.Collapsed;
            DrawingCanvas_Undirected.Visibility = Visibility.Visible;

            color_active.Opacity = 0.5;
            UndGraph_Button.Background = color_active;
            DirGraph_Button.Background = color_disable;

            ClickedObject = null;
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawingCanvas_Undirected_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mouseWasDownOn = e.Source as FrameworkElement;

            string elementName = mouseWasDownOn.Name;


            if (color == true && (e.OriginalSource is Ellipse || e.OriginalSource is Line))
            {
                if (e.Source is Ellipse)
                {
                    var elem = e.Source as Ellipse;

                    if (ColorUndirected.SelectedColor.HasValue)
                    {
                        // Set the fill color of the ellipse to the selected color from the ColorPicker
                        elem.Fill = new SolidColorBrush(ColorUndirected.SelectedColor.Value);
                    }
                }
                else if (e.Source is Line)
                {
                    var elem = e.Source as Line;

                    if (ColorUndirected.SelectedColor.HasValue)
                    {
                        // Set the fill color of the ellipse to the selected color from the ColorPicker
                        elem.Stroke = new SolidColorBrush(ColorUndirected.SelectedColor.Value);
                    }
                }
            }
            else if ((add_ellipse == true || move == true) && e.OriginalSource is Ellipse)
            {
                isLeftMouseDown = true;
                Ellipse c = e.OriginalSource as Ellipse;
                if (add_ellipse == true)
                {
                    if (firstEllipseName == null)
                    {
                        // First ellipse clicked
                        firstEllipseName = c.Name;
                    }
                    else if (secondEllipseName == null)
                    {
                        secondEllipseName = c.Name;

                        bool shapeExistsEllips1 = DrawingCanvas_Undirected.Children.OfType<Shape>().Any(shape => shape.Name == firstEllipseName);

                        bool shapeExistsEllips2 = DrawingCanvas_Undirected.Children.OfType<Shape>().Any(shape => shape.Name == secondEllipseName);
                        if (shapeExistsEllips1 && shapeExistsEllips2)
                        {

                            Ellipse sEllipse = DrawingCanvas_Undirected.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == secondEllipseName);

                            // Find the first ellipse
                            Ellipse fEllipse = DrawingCanvas_Undirected.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == firstEllipseName);

                            Point center1 = new Point(Canvas.GetLeft(fEllipse) + fEllipse.Width / 2, Canvas.GetTop(fEllipse) + fEllipse.Height / 2);
                            Point center2 = new Point(Canvas.GetLeft(sEllipse) + sEllipse.Width / 2, Canvas.GetTop(sEllipse) + sEllipse.Height / 2);


                            var intersectionPoint1 = (Point)DataFromGraph.CalculateIntersection(center1, fEllipse.Width / 2, center2);
                            var intersectionPoint2 = (Point)DataFromGraph.CalculateIntersection(center2, sEllipse.Width / 2, center1);

                            string f_node = firstEllipseName.Substring(firstEllipseName.IndexOf("_") + 1);
                            string s_node = secondEllipseName.Substring(secondEllipseName.IndexOf("_") + 1);
                            Line line = new Line()
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


                            if(ContainingChecker.Edge(adjacenceListUndirected, Convert.ToInt32(f_node), Convert.ToInt32(s_node)) == false && f_node!=s_node)
                            {
                                adjacenceListUndirected.AddEdge(Convert.ToInt32(f_node), Convert.ToInt32(s_node));
                                DrawingCanvas_Undirected.Children.Add(line);
                            }

                           
                            
                            firstEllipseName = null;
                            secondEllipseName = null;
                            DrawingCanvas_Undirected.InvalidateVisual();
                        }
                    }
                    else
                    {
                        firstEllipseName = null;
                        secondEllipseName = null;
                    }
                }

                DragStartPoint.X = e.GetPosition(this).X;
                DragStartPoint.Y = e.GetPosition(this).Y;


                ObjectStartLocation.X = Canvas.GetLeft(c);
                ObjectStartLocation.Y = Canvas.GetTop(c);

                ClickedObject = c;
            }
            else if (remove == true || add_ellipse == true)
            {
                if (e.OriginalSource is Ellipse && remove == true) // remove
                {
                    Ellipse clickedEllips = (Ellipse)e.OriginalSource;
                    DrawingCanvas_Undirected.Children.Remove(clickedEllips);
                    var list_lines = DataFromGraph.GetConnectedEdges(ref DrawingCanvas_Undirected, adjacenceListUndirected, clickedEllips.Name.SingleNodeName(), GraphType.Undirected);


                    adjacenceListUndirected.RemoveNode(clickedEllips.Name.SingleNodeName());

                    string textBoxName = "Text" + clickedEllips.Name;  // Name of the TextBox to remove

                    TextBlock textBoxToRemove = DrawingCanvas_Undirected.Children.OfType<TextBlock>()
                                                .FirstOrDefault(tb => tb.Name == textBoxName);

                    if (textBoxToRemove != null)
                    {
                        DrawingCanvas_Undirected.Children.Remove(textBoxToRemove);
                    }


                    foreach (string lineName in list_lines)
                    {
                        // Retrieve the line by its name
                        Line storedLine = DrawingCanvas_Undirected.Children.OfType<Line>().FirstOrDefault(l => l.Name == lineName);

                        
                        if (storedLine != null)
                        {
                            string name_line = storedLine.Name;

                            name_line.EdgesNames(out int f_node, out int s_node);

                            if (f_node == clickedEllips.Name.SingleNodeName()|| s_node == clickedEllips.Name.SingleNodeName())
                            {
                                DrawingCanvas_Undirected.Children.Remove(storedLine);
                            }
                        }
                    }

                        NamesUpdate updating = new NamesUpdate();
                    updating.UpdateNodes(adjacenceListUndirected, clickedEllips.Name.SingleNodeName());


                    updating.UpdateCanvas(ref DrawingCanvas_Undirected, clickedEllips.Name.SingleNodeName());


                }
                else if (e.OriginalSource is Line && remove == true) //remove
                {
                    Line clickedLine = (Line)e.OriginalSource;
                    clickedLine.Name.EdgesNames(out int f_node, out int s_node);
                    adjacenceListUndirected.RemoveEdge(f_node, s_node);
                    DrawingCanvas_Undirected.Children.Remove(clickedLine);
                   


                }
                else if (add_ellipse == true) //add
                {

                
                    Ellipse AAACircle = new Ellipse()
                    {
                        Name = $"Ellipse_{adjacenceListUndirected.CountNodes+1}",
                        Height = 50,
                        Width = 50,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Fill = Brushes.White,

                    };

                    TextBlock textBlock = new TextBlock()
                    {
                        Name = "Text" + AAACircle.Name,
                        Text = (adjacenceListUndirected.CountNodes+1).ToString(),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 16,
                        FontFamily = new FontFamily("Arial"),

                    };

                    Canvas.SetLeft(AAACircle, Mouse.GetPosition(DrawingCanvas_Undirected).X);
                    Canvas.SetTop(AAACircle, Mouse.GetPosition(DrawingCanvas_Undirected).Y);


                    Point center1_for_text = DataFromGraph.AllignOfText(AAACircle, (adjacenceListUndirected.CountNodes + 1).ToString());



                    Canvas.SetLeft(textBlock, center1_for_text.X);
                    Canvas.SetTop(textBlock, center1_for_text.Y);

                    DrawingCanvas_Undirected.Children.Add(AAACircle);
                    DrawingCanvas_Undirected.Children.Add(textBlock);
                    adjacenceListUndirected.AddNode(AAACircle.Name.SingleNodeName());


                }
            }

        }

        private void DrawingCanvas_Undirected_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (ClickedObject == null)
                return;

            DragEndPoint.X = e.GetPosition(this).X;
            DragEndPoint.Y = e.GetPosition(this).Y;

            double deltaX = DragEndPoint.X - DragStartPoint.X;
            double deltaY = DragEndPoint.Y - DragStartPoint.Y;



            if (move == true && ClickedObject is Ellipse)
            {
                Ellipse c = ClickedObject as Ellipse;
                Canvas.SetLeft(c, ObjectStartLocation.X + deltaX);
                Canvas.SetTop(c, ObjectStartLocation.Y + deltaY);

                Point center1_for_text = DataFromGraph.AllignOfText(c, c.Name);



                string textBoxName = "Text" + c.Name;  // Name of the TextBox to remove

                TextBlock textBlock = DrawingCanvas_Undirected.Children.OfType<TextBlock>()
                                            .FirstOrDefault(tb => tb.Name == textBoxName);

                if (textBlock != null)
                {
                    Canvas.SetLeft(textBlock, center1_for_text.X);
                    Canvas.SetTop(textBlock, center1_for_text.Y);
                }

                var lineNamesUndirected = DataFromGraph.GetConnectedEdges(ref DrawingCanvas_Undirected, adjacenceListUndirected, c.Name.SingleNodeName(), GraphType.Undirected);
                foreach (string lineName in lineNamesUndirected)
                {
                    // Retrieve the line by its name
                    Line storedLine = DrawingCanvas_Undirected.Children.OfType<Line>().FirstOrDefault(l => l.Name == lineName);

                    if (storedLine != null)
                    {
                        string name_line = storedLine.Name;

                        string name_of_first_ellipse = name_line.Substring(name_line.IndexOf("_") + 1, name_line.LastIndexOf("_") - name_line.IndexOf("_") - 1);
                        string name_of_second_ellipse = name_line.Substring(name_line.LastIndexOf("_") + 1);

                        Ellipse sEllipse = DrawingCanvas_Undirected.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == "Ellipse_" + name_of_second_ellipse);

                        // Find the first ellipse
                        Ellipse fEllipse = DrawingCanvas_Undirected.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == "Ellipse_" + name_of_first_ellipse);

                        Point center1 = new Point(Canvas.GetLeft(fEllipse) + fEllipse.Width / 2, Canvas.GetTop(fEllipse) + fEllipse.Height / 2);
                        Point center2 = new Point(Canvas.GetLeft(sEllipse) + sEllipse.Width / 2, Canvas.GetTop(sEllipse) + sEllipse.Height / 2);



                        var intersectionPoint1 = (Point)DataFromGraph.CalculateIntersection(center1, fEllipse.Width / 2, center2);
                        var intersectionPoint2 = (Point)DataFromGraph.CalculateIntersection(center2, sEllipse.Width / 2, center1);

                        storedLine.X1 = intersectionPoint1.X;
                        storedLine.Y1 = intersectionPoint1.Y;
                        storedLine.X2 = intersectionPoint2.X;
                        storedLine.Y2 = intersectionPoint2.Y;
                    }
                }
                DrawingCanvas_Undirected.InvalidateVisual();
            }
            else
            {
                return;
            }
        }

        private void DrawingCanvas_Undirected_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClickedObject = null;
        }



        SolidColorBrush color_disable = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        SolidColorBrush color_active = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACD1FF"));

     

        private void AddEllipse_Click(object sender, RoutedEventArgs e)
        {
            add_ellipse = true;
            remove = false;
            move = false;
            color = false;
            ColorUndirected.Visibility = Visibility.Collapsed;
            //colorPicker2.Visibility = Visibility.Collapsed;

            color_active.Opacity = 0.5;

            AddEllipse.Background = color_active;
            Delete.Background = color_disable;
            MoveEllipse.Background = color_disable;
            Color_Button.Background = color_disable;

        }

        private void MoveEllipse_Click(object sender, RoutedEventArgs e)
        {
            add_ellipse = false;
            remove = false;
            move = true;
            color = false;
            ColorUndirected.Visibility = Visibility.Collapsed;
            //colorPicker2.Visibility = Visibility.Collapsed;

            color_active.Opacity = 0.5;

            AddEllipse.Background = color_disable;
            Delete.Background = color_disable;
            MoveEllipse.Background = color_active;
            Color_Button.Background = color_disable;

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            add_ellipse = false;
            remove = true;
            move = false;
            color = false;
            ColorUndirected.Visibility = Visibility.Collapsed;
            //colorPicker2.Visibility = Visibility.Collapsed;

            color_active.Opacity = 0.5;

            AddEllipse.Background = color_disable;
            Delete.Background = color_active;
            MoveEllipse.Background = color_disable;
            Color_Button.Background = color_disable;
        }










        private void DrawingCanvas_Directed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var mouseWasDownOn = e.Source as FrameworkElement;

            string elementName = mouseWasDownOn.Name;


            if (color == true && (e.OriginalSource is Ellipse || e.OriginalSource is Line))
            {
                if (e.Source is Ellipse)
                {
                    var elem = e.Source as Ellipse;

                    if (ColorDirected.SelectedColor.HasValue)
                    {
                        // Set the fill color of the ellipse to the selected color from the ColorPicker
                        elem.Fill = new SolidColorBrush(ColorDirected.SelectedColor.Value);
                    }
                }
                else if (e.Source is Line)
                {
                    var elem = e.Source as Line;

                    if (ColorDirected.SelectedColor.HasValue)
                    {
                        // Set the fill color of the ellipse to the selected color from the ColorPicker
                        elem.Stroke = new SolidColorBrush(ColorDirected.SelectedColor.Value);
                    }
                }
            }
            else if ((add_ellipse == true || move == true) && e.OriginalSource is Ellipse)
            {
                isLeftMouseDown = true;
                Ellipse c = e.OriginalSource as Ellipse;
                if (add_ellipse == true)
                {
                    if (firstEllipseName == null)
                    {
                        // First ellipse clicked
                        firstEllipseName = c.Name;
                    }
                    else if (secondEllipseName == null)
                    {
                        secondEllipseName = c.Name;

                        bool shapeExistsEllips1 = DrawingCanvas_Directed.Children.OfType<Shape>().Any(shape => shape.Name == firstEllipseName);

                        bool shapeExistsEllips2 = DrawingCanvas_Directed.Children.OfType<Shape>().Any(shape => shape.Name == secondEllipseName);
                        if (shapeExistsEllips1 && shapeExistsEllips2)
                        {

                            Ellipse sEllipse = DrawingCanvas_Directed.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == secondEllipseName);

                            // Find the first ellipse
                            Ellipse fEllipse = DrawingCanvas_Directed.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == firstEllipseName);

                            Point center1 = new Point(Canvas.GetLeft(fEllipse) + fEllipse.Width / 2, Canvas.GetTop(fEllipse) + fEllipse.Height / 2);
                            Point center2 = new Point(Canvas.GetLeft(sEllipse) + sEllipse.Width / 2, Canvas.GetTop(sEllipse) + sEllipse.Height / 2);


                            var intersectionPoint1 = (Point)DataFromGraph.CalculateIntersection(center1, fEllipse.Width / 2, center2);
                            var intersectionPoint2 = (Point)DataFromGraph.CalculateIntersection(center2, sEllipse.Width / 2, center1);

                            string f_node = firstEllipseName.Substring(firstEllipseName.IndexOf("_") + 1);
                            string s_node = secondEllipseName.Substring(secondEllipseName.IndexOf("_") + 1);
                            Shape line = DataFromGraph.DrawLinkArrow(intersectionPoint1, intersectionPoint2);
                            line.Name = $"line_{f_node}_{s_node}";

                            if (!DrawingCanvas_Directed.Children.Cast<FrameworkElement>()
                      .Any(x => x.Name != null && x.Name.ToString() == $"line_{int.Parse(f_node)}_{int.Parse(s_node)}")) 
                      {
                                adjacenceListDirected.AddEdge(Convert.ToInt32(f_node), Convert.ToInt32(s_node));
                                DrawingCanvas_Directed.Children.Add(line);
                            }



                            firstEllipseName = null;
                            secondEllipseName = null;
                            DrawingCanvas_Undirected.InvalidateVisual();
                        }
                    }
                    else
                    {
                        firstEllipseName = null;
                        secondEllipseName = null;
                    }
                }

                DragStartPoint.X = e.GetPosition(this).X;
                DragStartPoint.Y = e.GetPosition(this).Y;


                ObjectStartLocation.X = Canvas.GetLeft(c);
                ObjectStartLocation.Y = Canvas.GetTop(c);

                ClickedObject = c;
            }
            else if (remove == true || add_ellipse == true)
            {
                if (e.OriginalSource is Ellipse && remove == true) // remove
                {
                    Ellipse clickedEllips = (Ellipse)e.OriginalSource;
                    DrawingCanvas_Directed.Children.Remove(clickedEllips);
                    var list_lines = DataFromGraph.GetConnectedEdges(ref DrawingCanvas_Directed, adjacenceListDirected, clickedEllips.Name.SingleNodeName(), GraphType.Directed);


                    adjacenceListDirected.RemoveNode(clickedEllips.Name.SingleNodeName());

                    string textBoxName = "Text" + clickedEllips.Name;  // Name of the TextBox to remove

                    TextBlock textBoxToRemove = DrawingCanvas_Directed.Children.OfType<TextBlock>()
                                                .FirstOrDefault(tb => tb.Name == textBoxName);

                    if (textBoxToRemove != null)
                    {
                        DrawingCanvas_Directed.Children.Remove(textBoxToRemove);
                    }


                    foreach (string lineName in list_lines)
                    {
                        // Retrieve the line by its name
                        Shape storedLine = DrawingCanvas_Directed.Children.OfType<Shape>().FirstOrDefault(l => l.Name == lineName);


                        if (storedLine != null)
                        {

                            DrawingCanvas_Directed.Children.Remove(storedLine);

                        }
                    }

                    NamesUpdate updating = new NamesUpdate();
                    updating.UpdateNodes(adjacenceListDirected, clickedEllips.Name.SingleNodeName());


                    updating.UpdateCanvas(ref DrawingCanvas_Directed, clickedEllips.Name.SingleNodeName());


                }
                else if (e.OriginalSource is Shape && remove == true) //remove
                {
                    Shape clickedLine = (Shape)e.OriginalSource;
                    clickedLine.Name.EdgesNames(out int f_node, out int s_node);
                    adjacenceListDirected.RemoveEdge(f_node, s_node);
                    DrawingCanvas_Directed.Children.Remove(clickedLine);
                }
                else if (add_ellipse == true) //add
                {


                    Ellipse AAACircle = new Ellipse()
                    {
                        Name = $"Ellipse_{adjacenceListDirected.CountNodes + 1}",
                        Height = 50,
                        Width = 50,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Fill = Brushes.White,

                    };

                    TextBlock textBlock = new TextBlock()
                    {
                        Name = "Text" + AAACircle.Name,
                        Text = (adjacenceListDirected.CountNodes + 1).ToString(),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 16,
                        FontFamily = new FontFamily("Arial"),

                    };

                    Canvas.SetLeft(AAACircle, Mouse.GetPosition(DrawingCanvas_Directed).X);
                    Canvas.SetTop(AAACircle, Mouse.GetPosition(DrawingCanvas_Directed).Y);


                    Point center1_for_text = DataFromGraph.AllignOfText(AAACircle, (adjacenceListDirected.CountNodes + 1).ToString());



                    Canvas.SetLeft(textBlock, center1_for_text.X);
                    Canvas.SetTop(textBlock, center1_for_text.Y);

                    DrawingCanvas_Directed.Children.Add(AAACircle);
                    DrawingCanvas_Directed.Children.Add(textBlock);
                    adjacenceListDirected.AddNode(AAACircle.Name.SingleNodeName());


                }
            }

        }

        private void DrawingCanvas_Directed_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (ClickedObject == null)
                return;

            DragEndPoint.X = e.GetPosition(this).X;
            DragEndPoint.Y = e.GetPosition(this).Y;

            double deltaX = DragEndPoint.X - DragStartPoint.X;
            double deltaY = DragEndPoint.Y - DragStartPoint.Y;



            if (move == true && ClickedObject is Ellipse)
            {
                Ellipse c = ClickedObject as Ellipse;
                Canvas.SetLeft(c, ObjectStartLocation.X + deltaX);
                Canvas.SetTop(c, ObjectStartLocation.Y + deltaY);

                Point center1_for_text = DataFromGraph.AllignOfText(c, c.Name);



                string textBoxName = "Text" + c.Name;  // Name of the TextBox to remove

                TextBlock textBlock = DrawingCanvas_Directed.Children.OfType<TextBlock>()
                                            .FirstOrDefault(tb => tb.Name == textBoxName);

                if (textBlock != null)
                {
                    Canvas.SetLeft(textBlock, center1_for_text.X);
                    Canvas.SetTop(textBlock, center1_for_text.Y);
                }

                var lineNamesDirected = DataFromGraph.GetConnectedEdges(ref DrawingCanvas_Directed, adjacenceListDirected, c.Name.SingleNodeName(), GraphType.Directed);
                foreach (string lineName in lineNamesDirected)
                {
                    // Retrieve the line by its name
                    Shape storedLine = DrawingCanvas_Directed.Children.OfType<Shape>().FirstOrDefault(l => l.Name == lineName);

                    if (storedLine != null)
                    {
                        string name_line = storedLine.Name;

                        string name_of_first_ellipse = name_line.Substring(name_line.IndexOf("_") + 1, name_line.LastIndexOf("_") - name_line.IndexOf("_") - 1);
                        string name_of_second_ellipse = name_line.Substring(name_line.LastIndexOf("_") + 1);

                        Ellipse sEllipse = DrawingCanvas_Directed.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == "Ellipse_" + name_of_second_ellipse);

                        // Find the first ellipse
                        Ellipse fEllipse = DrawingCanvas_Directed.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == "Ellipse_" + name_of_first_ellipse);

                        Point center1 = new Point(Canvas.GetLeft(fEllipse) + fEllipse.Width / 2, Canvas.GetTop(fEllipse) + fEllipse.Height / 2);
                        Point center2 = new Point(Canvas.GetLeft(sEllipse) + sEllipse.Width / 2, Canvas.GetTop(sEllipse) + sEllipse.Height / 2);



                        var intersectionPoint1 = (Point)DataFromGraph.CalculateIntersection(center1, fEllipse.Width / 2, center2);
                        var intersectionPoint2 = (Point)DataFromGraph.CalculateIntersection(center2, sEllipse.Width / 2, center1);

                        Brush color_line = storedLine.Fill;

                        // Remove the existing line from the canvas
                        DrawingCanvas_Directed.Children.Remove(storedLine);



                        // Redraw the line with updated coordinates
                        storedLine = DataFromGraph.DrawLinkArrow(intersectionPoint1, intersectionPoint2);
                        storedLine.Name = lineName;
                        storedLine.Fill = color_line;
                        storedLine.Stroke = color_line;

                        // Add the newly drawn line to the canvas
                        DrawingCanvas_Directed.Children.Add(storedLine);
                    }
                }
                DrawingCanvas_Directed.InvalidateVisual();
            }
            else
            {
                return;
            }
        }

        private void DrawingCanvas_Directed_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClickedObject = null;

        }
    }
}