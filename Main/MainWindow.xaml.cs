﻿using System.Text;
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

namespace Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AdjacenceList adjacenceListUndirected = new AdjacenceList();
        AdjacenceList adjacenceListDirected = new AdjacenceList();

        Point DragStartPoint, DragEndPoint, ObjectStartLocation;
        object ClickedObject;

        Ellipse firstEllipse, secondEllipse, selectedEllips;
        string firstEllipseName, secondEllipseName;
        bool isLeftMouseDown = false;

        Line line = null;

        bool vis, edit, add_ellipse, remove, move, color;


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
                    var list_lines = DataFromGraph.GetConnectedEdges(ref DrawingCanvas_Undirected, adjacenceListUndirected, clickedEllips.Name.NodesNames());


                    adjacenceListUndirected.RemoveNode(clickedEllips.Name.NodesNames());

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

                            if (f_node == clickedEllips.Name.NodesNames()|| s_node == clickedEllips.Name.NodesNames())
                            {
                                DrawingCanvas_Undirected.Children.Remove(storedLine);
                            }
                        }
                    }

                        NamesUpdate updating = new NamesUpdate();
                    updating.UpdateNodes(adjacenceListUndirected, clickedEllips.Name.NodesNames());


                    updating.UpdateCanvas(ref DrawingCanvas_Undirected, clickedEllips.Name.NodesNames());


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
                    adjacenceListUndirected.AddNode(AAACircle.Name.NodesNames());


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

                var lineNamesUndirected = DataFromGraph.GetConnectedEdges(ref DrawingCanvas_Undirected, adjacenceListUndirected, c.Name.NodesNames());
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










     /*   private void DrawingCanvas_Directed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var mouseWasDownOn = e.Source as FrameworkElement;

            string elementName = mouseWasDownOn.Name;

            if (color == true && (e.OriginalSource is Ellipse || e.OriginalSource is Shape))
            {
                if (e.Source is Ellipse)
                {
                    var elem = e.Source as Ellipse;

                    if (colorPicker2.SelectedColor.HasValue)
                    {
                        // Set the fill color of the ellipse to the selected color from the ColorPicker
                        elem.Fill = new SolidColorBrush(colorPicker2.SelectedColor.Value);
                    }
                }
                else if (e.Source is Shape)
                {
                    var elem = e.Source as Shape;

                    if (colorPicker2.SelectedColor.HasValue)
                    {
                        // Set the fill color of the ellipse to the selected color from the ColorPicker
                        elem.Stroke = new SolidColorBrush(colorPicker2.SelectedColor.Value);
                        elem.Fill = new SolidColorBrush(colorPicker2.SelectedColor.Value);

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


                            var intersectionPoint1 = (Point)CalculateIntersection(center1, fEllipse.Width / 2, center2);
                            var intersectionPoint2 = (Point)CalculateIntersection(center2, sEllipse.Width / 2, center1);


                            Shape line = DrawLinkArrow(intersectionPoint1, intersectionPoint2);




                            string numb_firs_ellip = firstEllipseName.Substring(firstEllipseName.IndexOf("_") + 1);
                            string numb_second_ellip = secondEllipseName.Substring(secondEllipseName.IndexOf("_") + 1);

                            string lineName = "line_" + numb_firs_ellip + "_" + numb_second_ellip;

                            if (!lineNamesDirected.Contains(lineName))
                            {
                                line.Name = lineName;
                                lineNamesDirected.Add(lineName);
                                DrawingCanvas_Directed.Children.Add(line);
                                adjacence.AddConnection(ref Dictionary_DirectedGraphs, sEllipse.Name, line.Name);
                            }


                            // Reset the variables for the next connection
                            firstEllipseName = null;
                            secondEllipseName = null;
                            DrawingCanvas_Directed.InvalidateVisual();
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

                if (e.OriginalSource is Ellipse && remove == true)
                {
                    Ellipse clickedEllips = (Ellipse)e.OriginalSource;
                    DrawingCanvas_Directed.Children.Remove(clickedEllips);

                    adjacence.RemoveEllipse(ref Dictionary_DirectedGraphs, clickedEllips.Name);
                    //adjacence.ConverterTo2DArray(ref Dictionary_DirectedGraphs, DrawingCanvas_Directed);

                    string textBoxName = "Text" + clickedEllips.Name;  // Name of the TextBox to remove

                    TextBlock textBoxToRemove = DrawingCanvas_Directed.Children.OfType<TextBlock>()
                                                .FirstOrDefault(tb => tb.Name == textBoxName);

                    if (textBoxToRemove != null)
                    {
                        DrawingCanvas_Directed.Children.Remove(textBoxToRemove);
                    }



                    List<string> list_lines = lineNamesDirected.ToList();

                    foreach (string lineName in list_lines)
                    {
                        // Retrieve the line by its name
                        Shape storedLine = DrawingCanvas_Directed.Children.OfType<Shape>().FirstOrDefault(l => l.Name == lineName);

                        string name_el = clickedEllips.Name;
                        string click_el_name = name_el.Substring(name_el.IndexOf("_") + 1);

                        if (storedLine != null)
                        {
                            string name_line = storedLine.Name;

                            string name_of_first_ellipse = name_line.Substring(name_line.IndexOf("_") + 1, name_line.LastIndexOf("_") - name_line.IndexOf("_") - 1);
                            string name_of_second_ellipse = name_line.Substring(name_line.LastIndexOf("_") + 1);

                            if (name_of_first_ellipse == click_el_name || name_of_second_ellipse == click_el_name)
                            {
                                DrawingCanvas_Directed.Children.Remove(storedLine);
                                lineNamesDirected.Remove(storedLine.Name);
                            }
                        }
                    }
                    Dictionary_DirectedGraphs = UpdateName.DictionaryNames(Dictionary_DirectedGraphs, clickedEllips.Name);
                    UpdateName.CanvasNames(clickedEllips.Name, DrawingCanvas_Directed);
                    lineNamesDirected = UpdateName.LineNames(lineNamesDirected, clickedEllips.Name);
                }
                else if (e.OriginalSource is Shape && remove == true) //remove
                {
                    Shape clickedLine = (Shape)e.OriginalSource;
                    lineNamesDirected.Remove(clickedLine.Name);
                    DrawingCanvas_Directed.Children.Remove(clickedLine);
                    adjacence.RemoveConnection(ref Dictionary_DirectedGraphs, clickedLine.Name);
                    lineNamesDirected.Remove(clickedLine.Name);


                }
                else if (add_ellipse == true) //add
                {

                    int availableIndex = GetEllipseCount(DrawingCanvas_Directed);
                    string newEllipseName = $"Ellipse_{availableIndex}";


                    Ellipse AAACircle = new Ellipse()
                    {
                        Height = 50,
                        Width = 50,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Fill = Brushes.White,

                    };


                    int ellipseCount = GetEllipseCount(DrawingCanvas_Directed);

                    AAACircle.Name = "Ellipse_" + availableIndex;

                    int el = AAACircle.Name.IndexOf("_");
                    string el1 = AAACircle.Name.Substring(el + 1);

                    TextBlock textBlock = new TextBlock()
                    {
                        Name = "Text" + AAACircle.Name,
                        Text = el1,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 16,
                        FontFamily = new FontFamily("Arial"),

                    };



                    Canvas.SetLeft(AAACircle, Mouse.GetPosition(DrawingCanvas_Directed).X);
                    Canvas.SetTop(AAACircle, Mouse.GetPosition(DrawingCanvas_Directed).Y);


                    Point center1_for_text = AllignOfText(AAACircle, ellipseCount);



                    Canvas.SetLeft(textBlock, center1_for_text.X);
                    Canvas.SetTop(textBlock, center1_for_text.Y);

                    DrawingCanvas_Directed.Children.Add(AAACircle);
                    DrawingCanvas_Directed.Children.Add(textBlock);
                    adjacence.AddEllipse(ref Dictionary_DirectedGraphs, AAACircle.Name);
                    //adjacence.ConverterTo2DArray(ref Dictionary_DirectedGraphs, DrawingCanvas_Directed);
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

            if (ClickedObject is Ellipse && move == true)
            {
                Ellipse c = ClickedObject as Ellipse;
                Canvas.SetLeft(c, ObjectStartLocation.X + deltaX);
                Canvas.SetTop(c, ObjectStartLocation.Y + deltaY);

                Point center1_for_text = AllignOfText(c, c.Name);



                string textBoxName = "Text" + c.Name;  // Name of the TextBox to remove

                TextBlock textBlock = DrawingCanvas_Directed.Children.OfType<TextBlock>()
                                            .FirstOrDefault(tb => tb.Name == textBoxName);

                if (textBlock != null)
                {
                    Canvas.SetLeft(textBlock, center1_for_text.X);
                    Canvas.SetTop(textBlock, center1_for_text.Y);
                }






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
                        Ellipse fEllipse = DrawingCanvas_Directed.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == "Ellipse_" + name_of_first_ellipse);

                        Point center1 = new Point(Canvas.GetLeft(fEllipse) + fEllipse.Width / 2, Canvas.GetTop(fEllipse) + fEllipse.Height / 2);
                        Point center2 = new Point(Canvas.GetLeft(sEllipse) + sEllipse.Width / 2, Canvas.GetTop(sEllipse) + sEllipse.Height / 2);

                        var intersectionPoint1 = (Point)CalculateIntersection(center1, fEllipse.Width / 2, center2);
                        var intersectionPoint2 = (Point)CalculateIntersection(center2, sEllipse.Width / 2, center1);

                        Brush color_line = storedLine.Fill;

                        // Remove the existing line from the canvas
                        DrawingCanvas_Directed.Children.Remove(storedLine);



                        // Redraw the line with updated coordinates
                        storedLine = DrawLinkArrow(intersectionPoint1, intersectionPoint2);
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

        }*/
    }
}