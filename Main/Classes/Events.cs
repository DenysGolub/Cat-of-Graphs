using Main.Enumerators;
using Main.Interfaces;
using Main.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Point = System.Windows.Point;

namespace Main.Classes
{
    public delegate void ChangeModeEventHandler(object sender, MyEventArgs e);

    public class MyEventArgs : EventArgs
    {
        public string AdditionalData { get; }

        public MyEventArgs(string additionalData)
        {
            AdditionalData = additionalData;
        }
    }

    public class Events //Class B
    {
       

        public event ChangeModeEventHandler SomethingChanged;



        public void AddMode()
        {
            OnSomethingChanged(new MyEventArgs("AddMode"));
        }

        public void MoveMode()
        {
            OnSomethingChanged(new MyEventArgs("MoveMode"));
        }

        public void RemoveMode()
        {
            OnSomethingChanged(new MyEventArgs("DeleteMode"));
        }
        public void ColorMode()
        {
            OnSomethingChanged(new MyEventArgs("ColorMode"));
        }

        public void ChangeToUndirectedSecond()
        {
            OnSomethingChanged(new MyEventArgs("ChangeToUndirectedGraph"));
            MainWindow wnd = (MainWindow)Application.Current.MainWindow;

           
        }
        public void ChangeToDirectedSecond()
        {
            OnSomethingChanged(new MyEventArgs("ChangeToDirectedGraph"));

            MainWindow wnd = (MainWindow)Application.Current.MainWindow;

        }
            
        protected virtual void OnSomethingChanged(MyEventArgs e)
        {
            SomethingChanged?.Invoke(this, e);
        }

        public class CanvasEvents //Class A
        {
            SolidColorBrush color_disable = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
            SolidColorBrush color_active = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACD1FF"));


            public void SubscribeToChanges(Events classB)
            {
                // Subscribe to the event
                classB.SomethingChanged += HandleSomethingChanged;
            }

            public void SubscribeToChangesInSecondGraphs(Events wnd)
            {
                wnd.SomethingChanged += HandleChangedInSecondGraph;
            } 

            private void HandleSomethingChanged(object sender, MyEventArgs e)
            {
                MainWindow wnd = (MainWindow)Application.Current.MainWindow;

                if (e.AdditionalData == "AddMode")
                {
                    Edit = true;
                    Remove = false;
                    Move = false;
                    Color = false;
                    wnd.ColorUndirected.Visibility = Visibility.Collapsed;
                    wnd.ColorDirected.Visibility = Visibility.Collapsed;

                    color_active.Opacity = 0.5;

                    wnd.AddEllipse.Background = color_active;
                    wnd.Delete.Background = color_disable;
                    wnd.MoveEllipse.Background = color_disable;
                    wnd.Color_Button.Background = color_disable;
                }
                else if (e.AdditionalData == "MoveMode")
                {
                    Edit = false;
                    Remove = false;
                    Move = true;
                    Color = false;
                    wnd.ColorUndirected.Visibility = Visibility.Collapsed;
                    wnd.ColorDirected.Visibility = Visibility.Collapsed;

                    color_active.Opacity = 0.5;

                    wnd.AddEllipse.Background = color_disable;
                    wnd.Delete.Background = color_disable;
                    wnd.MoveEllipse.Background = color_active;
                    wnd.Color_Button.Background = color_disable;

                }
                else if(e.AdditionalData == "DeleteMode")
                {
                    Edit = false;
                    Remove = true;
                    Move = false;
                    Color = false;
                    wnd.ColorUndirected.Visibility = Visibility.Collapsed;
                    wnd.ColorDirected.Visibility = Visibility.Collapsed;

                    color_active.Opacity = 0.5;

                    wnd.AddEllipse.Background = color_disable;
                    wnd.Delete.Background = color_active;
                    wnd.MoveEllipse.Background = color_disable;
                    wnd.Color_Button.Background = color_disable;
                }
                else if(e.AdditionalData == "ColorMode")
                {
                    Edit = false;
                    Remove = false;
                    Move = false;
                    Color = true;

                   

                    color_active.Opacity = 0.5;

                    wnd.AddEllipse.Background = color_disable;
                    wnd.Delete.Background = color_disable;
                    wnd.MoveEllipse.Background = color_disable;
                    wnd.Color_Button.Background = color_active;

                    if(wnd.DrawingCanvas_Directed.Visibility == Visibility.Visible)
                    {
                        wnd.ColorUndirected.Visibility = Visibility.Collapsed;
                        wnd.ColorDirected.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        wnd.ColorDirected.Visibility = Visibility.Collapsed;
                        wnd.ColorUndirected.Visibility = Visibility.Visible;
                    }
                }
            }

            private void HandleChangedInSecondGraph(object sender, MyEventArgs e)
            {
                SecondGraph wnd = Application.Current.Windows.OfType<SecondGraph>().FirstOrDefault();

                if (e.AdditionalData == "AddMode")
                {
                    Edit = true;
                    Remove = false;
                    Move = false;
                    Color = false;

                    color_active.Opacity = 0.5;

                    wnd.AddEllipse.Background = color_active;
                    wnd.Delete.Background = color_disable;
                    wnd.MoveEllipse.Background = color_disable;
                }
                else if (e.AdditionalData == "MoveMode")
                {
                    Edit = false;
                    Remove = false;
                    Move = true;
                    Color = false;


                    color_active.Opacity = 0.5;

                    wnd.AddEllipse.Background = color_disable;
                    wnd.Delete.Background = color_disable;
                    wnd.MoveEllipse.Background = color_active;

                }
                else if (e.AdditionalData == "DeleteMode")
                {
                    Edit = false;
                    Remove = true;
                    Move = false;
                    Color = false;

                    color_active.Opacity = 0.5;

                    wnd.AddEllipse.Background = color_disable;
                    wnd.Delete.Background = color_active;
                    wnd.MoveEllipse.Background = color_disable;
                }
                else if (e.AdditionalData == "ChangeToDirectedGraph")
                {

                    wnd.DrawingCanvas_Directed.Visibility = Visibility.Visible;
                    wnd.DrawingCanvas_Undirected.Visibility = Visibility.Collapsed;
                    wnd.Type = GraphType.Directed;
                }
                else if (e.AdditionalData == "ChangeToUndirectedGraph")
                {

                    wnd.DrawingCanvas_Directed.Visibility = Visibility.Collapsed;
                    wnd.DrawingCanvas_Undirected.Visibility = Visibility.Visible;
                    wnd.Type = GraphType.Undirected;

                }




            }



            bool add_ellipse, remove, move, color;

            public bool Edit { get => add_ellipse; set => add_ellipse = value; }
            public bool Remove { get => remove; set => remove = value; }
            public bool Move { get => move; set => move = value; }
            public bool Color { get => color; set => color = value; }


            Point DragStartPoint, DragEndPoint, ObjectStartLocation;
            object ClickedObject;

            Ellipse firstEllipse, secondEllipse, selectedEllips;
            string firstEllipseName, secondEllipseName;
            bool isLeftMouseDown = false;






            public class Directed : CanvasEvents, ICanvasEvents
            {
                Window owner;  
                Canvas canvas;
                AdjacenceList _adjacenceList;
                ColorPicker _colorpicker;

                public Canvas Canvas { get => canvas; set => canvas = value; }

                public AdjacenceList AdjacenceList { get => _adjacenceList; set => _adjacenceList = value; }
                public ColorPicker ColorPicker { get => _colorpicker; set => _colorpicker = value; }
                public Window ClassOwner { get => owner; set => owner = value; }


                public void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
                {
                    AdjacenceMatrix win = WindowsInstances.AdjacenceMatrixWindowInst(ClassOwner);

                    IncidenceMatrix inc_win = WindowsInstances.MatrixIncidenceWindowInst(ClassOwner);

                    var mouseWasDownOn = e.Source as FrameworkElement;

                    string elementName = mouseWasDownOn.Name;


                    if (color == true && (e.OriginalSource is Ellipse || e.OriginalSource is Shape))
                    {
                        if (e.Source is Ellipse)
                        {
                            var elem = e.Source as Ellipse;

                            if (ColorPicker.SelectedColor.HasValue)
                            {
                                // Set the fill color of the ellipse to the selected color from the ColorPicker
                                elem.Fill = new SolidColorBrush(ColorPicker.SelectedColor.Value);
                            }
                        }
                        else if (e.Source is Shape)
                        {
                            var elem = e.Source as Shape;

                            if (ColorPicker.SelectedColor.HasValue)
                            {
                                // Set the fill color of the ellipse to the selected color from the ColorPicker
                                elem.Stroke = new SolidColorBrush(ColorPicker.SelectedColor.Value);
                                elem.Fill = new SolidColorBrush(ColorPicker.SelectedColor.Value);
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

                                bool shapeExistsEllips1 = Canvas.Children.OfType<Shape>().Any(shape => shape.Name == firstEllipseName);

                                bool shapeExistsEllips2 = Canvas.Children.OfType<Shape>().Any(shape => shape.Name == secondEllipseName);
                                if (shapeExistsEllips1 && shapeExistsEllips2)
                                {

                                    Ellipse sEllipse = Canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == secondEllipseName);

                                    // Find the first ellipse
                                    Ellipse fEllipse = Canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == firstEllipseName);

                                    Point center1 = new Point(Canvas.GetLeft(fEllipse) + fEllipse.Width / 2, Canvas.GetTop(fEllipse) + fEllipse.Height / 2);
                                    Point center2 = new Point(Canvas.GetLeft(sEllipse) + sEllipse.Width / 2, Canvas.GetTop(sEllipse) + sEllipse.Height / 2);


                                    var intersectionPoint1 = (Point)DataFromGraph.CalculateIntersection(center1, fEllipse.Width / 2, center2);
                                    var intersectionPoint2 = (Point)DataFromGraph.CalculateIntersection(center2, sEllipse.Width / 2, center1);

                                    string f_node = firstEllipseName.Substring(firstEllipseName.IndexOf("_") + 1);
                                    string s_node = secondEllipseName.Substring(secondEllipseName.IndexOf("_") + 1);
                                    Shape line = DataFromGraph.DrawLinkArrow(intersectionPoint1, intersectionPoint2);
                                    line.Name = $"line_{f_node}_{s_node}";

                                    if (!Canvas.Children.Cast<FrameworkElement>()
                              .Any(x => x.Name != null && x.Name.ToString() == $"line_{int.Parse(f_node)}_{int.Parse(s_node)}"))
                                    {
                                        _adjacenceList.AddEdge(Convert.ToInt32(f_node), Convert.ToInt32(s_node));
                                        Canvas.Children.Add(line);
                                    }



                                    firstEllipseName = null;
                                    secondEllipseName = null;
                                    Canvas.InvalidateVisual();
                                }
                            }
                            else
                            {
                                firstEllipseName = null;
                                secondEllipseName = null;
                            }
                        }

                        DragStartPoint.X = e.GetPosition(Canvas).X;
                        DragStartPoint.Y = e.GetPosition(Canvas).Y;


                        ObjectStartLocation.X = Canvas.GetLeft(c);
                        ObjectStartLocation.Y = Canvas.GetTop(c);

                        ClickedObject = c;
                    }
                    else if (remove == true || add_ellipse == true)
                    {
                        if (e.OriginalSource is Ellipse && remove == true) // remove
                        {
                            Ellipse clickedEllips = (Ellipse)e.OriginalSource;
                            Canvas.Children.Remove(clickedEllips);
                            var list_lines = DataFromGraph.GetConnectedEdges(ref canvas, _adjacenceList, clickedEllips.Name.SingleNodeName(), GraphType.Directed);


                            _adjacenceList.RemoveNode(clickedEllips.Name.SingleNodeName());

                            string textBoxName = "Text" + clickedEllips.Name;  // Name of the TextBox to remove

                            TextBlock textBoxToRemove = Canvas.Children.OfType<TextBlock>()
                                                        .FirstOrDefault(tb => tb.Name == textBoxName);

                            if (textBoxToRemove != null)
                            {
                                Canvas.Children.Remove(textBoxToRemove);
                            }


                            foreach (string lineName in list_lines)
                            {
                                // Retrieve the line by its name
                                Shape storedLine = Canvas.Children.OfType<Shape>().FirstOrDefault(l => l.Name == lineName);


                                if (storedLine != null)
                                {

                                    Canvas.Children.Remove(storedLine);

                                }
                            }

                            NamesUpdate updating = new NamesUpdate();
                            updating.UpdateNodes(_adjacenceList, clickedEllips.Name.SingleNodeName());


                            updating.UpdateCanvas(ref canvas, clickedEllips.Name.SingleNodeName());


                        }
                        else if (e.OriginalSource is Shape && remove == true) //remove
                        {
                            Shape clickedLine = (Shape)e.OriginalSource;
                            clickedLine.Name.EdgesNames(out int f_node, out int s_node);
                            _adjacenceList.RemoveEdge(f_node, s_node);
                            Canvas.Children.Remove(clickedLine);
                        }
                        else if (add_ellipse == true) //add
                        {


                            Ellipse AAACircle = new Ellipse()
                            {
                                Name = $"Ellipse_{_adjacenceList.CountNodes + 1}",
                                Height = 50,
                                Width = 50,
                                Stroke = Brushes.Black,
                                StrokeThickness = 1,
                                Fill = Brushes.White,

                            };

                            TextBlock textBlock = new TextBlock()
                            {
                                Name = "Text" + AAACircle.Name,
                                Text = (_adjacenceList.CountNodes + 1).ToString(),
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                FontSize = 16,
                                FontFamily = new FontFamily("Arial"),

                            };

                            Canvas.SetLeft(AAACircle, Mouse.GetPosition(Canvas).X);
                            Canvas.SetTop(AAACircle, Mouse.GetPosition(Canvas).Y);


                            Point center1_for_text = DataFromGraph.AllignOfText(AAACircle, (_adjacenceList.CountNodes + 1).ToString());



                            Canvas.SetLeft(textBlock, center1_for_text.X);
                            Canvas.SetTop(textBlock, center1_for_text.Y);

                            Canvas.Children.Add(AAACircle);
                            Canvas.Children.Add(textBlock);
                            _adjacenceList.AddNode(AAACircle.Name.SingleNodeName());


                        }
                    }
                    if (win.Matrix != null)
                    {
                        win.UpdateMatrix();
                    }

                    if (inc_win.Matrix != null)
                    {
                        inc_win.UpdateMatrix();
                    }
                }

                public void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
                {
                    ClickedObject = null;
                }

                public void PreviewMouseMove(object sender, MouseEventArgs e)
                {
                    if (ClickedObject == null)
                        return;

                    DragEndPoint.X = e.GetPosition(canvas).X;
                    DragEndPoint.Y = e.GetPosition(canvas).Y;

                    double deltaX = DragEndPoint.X - DragStartPoint.X;
                    double deltaY = DragEndPoint.Y - DragStartPoint.Y;



                    if (move == true && ClickedObject is Ellipse)
                    {
                        Ellipse c = ClickedObject as Ellipse;
                        Canvas.SetLeft(c, ObjectStartLocation.X + deltaX);
                        Canvas.SetTop(c, ObjectStartLocation.Y + deltaY);

                        Point center1_for_text = DataFromGraph.AllignOfText(c, c.Name);



                        string textBoxName = "Text" + c.Name;  // Name of the TextBox to remove

                        TextBlock textBlock = Canvas.Children.OfType<TextBlock>()
                                                    .FirstOrDefault(tb => tb.Name == textBoxName);

                        if (textBlock != null)
                        {
                            Canvas.SetLeft(textBlock, center1_for_text.X);
                            Canvas.SetTop(textBlock, center1_for_text.Y);
                        }

                        var lineNamesDirected = DataFromGraph.GetConnectedEdges(ref canvas, _adjacenceList, c.Name.SingleNodeName(), GraphType.Directed);
                        foreach (string lineName in lineNamesDirected)
                        {
                            // Retrieve the line by its name
                            Shape storedLine = Canvas.Children.OfType<Shape>().FirstOrDefault(l => l.Name == lineName);

                            if (storedLine != null)
                            {
                                string name_line = storedLine.Name;

                                string name_of_first_ellipse = name_line.Substring(name_line.IndexOf("_") + 1, name_line.LastIndexOf("_") - name_line.IndexOf("_") - 1);
                                string name_of_second_ellipse = name_line.Substring(name_line.LastIndexOf("_") + 1);

                                Ellipse sEllipse = Canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == "Ellipse_" + name_of_second_ellipse);

                                // Find the first ellipse
                                Ellipse fEllipse = Canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == "Ellipse_" + name_of_first_ellipse);

                                Point center1 = new Point(Canvas.GetLeft(fEllipse) + fEllipse.Width / 2, Canvas.GetTop(fEllipse) + fEllipse.Height / 2);
                                Point center2 = new Point(Canvas.GetLeft(sEllipse) + sEllipse.Width / 2, Canvas.GetTop(sEllipse) + sEllipse.Height / 2);



                                var intersectionPoint1 = (Point)DataFromGraph.CalculateIntersection(center1, fEllipse.Width / 2, center2);
                                var intersectionPoint2 = (Point)DataFromGraph.CalculateIntersection(center2, sEllipse.Width / 2, center1);

                                Brush color_line = storedLine.Fill;

                                // Remove the existing line from the canvas
                                Canvas.Children.Remove(storedLine);



                                // Redraw the line with updated coordinates
                                storedLine = DataFromGraph.DrawLinkArrow(intersectionPoint1, intersectionPoint2);
                                storedLine.Name = lineName;
                                storedLine.Fill = color_line;
                                storedLine.Stroke = color_line;

                                // Add the newly drawn line to the canvas
                                Canvas.Children.Add(storedLine);
                            }
                        }
                        Canvas.InvalidateVisual();
                    }
                    else
                    {
                        return;
                    }
                }
            }

            public class Undirected : CanvasEvents, ICanvasEvents
            {
                Window owner;
                Canvas canvas;
                AdjacenceList _adjacenceList;
                ColorPicker _colorpicker;
                
                public Canvas Canvas { get => canvas; set => canvas=value; }
                
                public AdjacenceList AdjacenceList { get => _adjacenceList; set => _adjacenceList=value; }
                public ColorPicker ColorPicker { get => _colorpicker; set => _colorpicker = value; }

                public Window ClassOwner {get => owner; set => owner = value; }
                public void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
                {
                    AdjacenceMatrix win = WindowsInstances.AdjacenceMatrixWindowInst(ClassOwner);

                    IncidenceMatrix inc_win = WindowsInstances.MatrixIncidenceWindowInst(ClassOwner);

                    var mouseWasDownOn = e.Source as FrameworkElement;

                    string elementName = mouseWasDownOn.Name;


                    if (color == true && (e.OriginalSource is Ellipse || e.OriginalSource is Line))
                    {
                        if (e.Source is Ellipse)
                        {
                            var elem = e.Source as Ellipse;

                            if (ColorPicker.SelectedColor.HasValue)
                            {
                                // Set the fill color of the ellipse to the selected color from the ColorPicker
                                elem.Fill = new SolidColorBrush(ColorPicker.SelectedColor.Value);
                            }
                        }
                        else if (e.Source is Line)
                        {
                            var elem = e.Source as Line;

                            if (ColorPicker.SelectedColor.HasValue)
                            {
                                // Set the fill color of the ellipse to the selected color from the ColorPicker
                                elem.Stroke = new SolidColorBrush(ColorPicker.SelectedColor.Value);
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

                                bool shapeExistsEllips1 = canvas.Children.OfType<Shape>().Any(shape => shape.Name == firstEllipseName);

                                bool shapeExistsEllips2 = canvas.Children.OfType<Shape>().Any(shape => shape.Name == secondEllipseName);
                                if (shapeExistsEllips1 && shapeExistsEllips2)
                                {

                                    Ellipse sEllipse = canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == secondEllipseName);

                                    // Find the first ellipse
                                    Ellipse fEllipse = canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == firstEllipseName);

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


                                    if (ContainingChecker.Edge(_adjacenceList, Convert.ToInt32(f_node), Convert.ToInt32(s_node)) == false && f_node != s_node)
                                    {
                                        _adjacenceList.AddEdge(Convert.ToInt32(f_node), Convert.ToInt32(s_node));
                                        canvas.Children.Add(line);
                                    }



                                    firstEllipseName = null;
                                    secondEllipseName = null;
                                    canvas.InvalidateVisual();
                                }
                            }
                            else
                            {
                                firstEllipseName = null;
                                secondEllipseName = null;
                            }
                        }

                        DragStartPoint.X = e.GetPosition(canvas).X;
                        DragStartPoint.Y = e.GetPosition(canvas).Y;


                        ObjectStartLocation.X = Canvas.GetLeft(c);
                        ObjectStartLocation.Y = Canvas.GetTop(c);

                        ClickedObject = c;
                    }
                    else if (remove == true || add_ellipse == true)
                    {
                        if (e.OriginalSource is Ellipse && remove == true) // remove
                        {
                            Ellipse clickedEllips = (Ellipse)e.OriginalSource;
                            canvas.Children.Remove(clickedEllips);
                            var list_lines = DataFromGraph.GetConnectedEdges(ref canvas, _adjacenceList, clickedEllips.Name.SingleNodeName(), GraphType.Undirected);


                            _adjacenceList.RemoveNode(clickedEllips.Name.SingleNodeName());

                            string textBoxName = "Text" + clickedEllips.Name;  // Name of the TextBox to remove

                            TextBlock textBoxToRemove = canvas.Children.OfType<TextBlock>()
                                                        .FirstOrDefault(tb => tb.Name == textBoxName);

                            if (textBoxToRemove != null)
                            {
                                canvas.Children.Remove(textBoxToRemove);
                            }


                            foreach (string lineName in list_lines)
                            {
                                // Retrieve the line by its name
                                Line storedLine = canvas.Children.OfType<Line>().FirstOrDefault(l => l.Name == lineName);


                                if (storedLine != null)
                                {
                                    string name_line = storedLine.Name;

                                    name_line.EdgesNames(out int f_node, out int s_node);

                                    if (f_node == clickedEllips.Name.SingleNodeName() || s_node == clickedEllips.Name.SingleNodeName())
                                    {
                                        canvas.Children.Remove(storedLine);
                                    }
                                }
                            }

                            NamesUpdate updating = new NamesUpdate();
                            updating.UpdateNodes(_adjacenceList, clickedEllips.Name.SingleNodeName());


                            updating.UpdateCanvas(ref canvas, clickedEllips.Name.SingleNodeName());


                        }
                        else if (e.OriginalSource is Line && remove == true) //remove
                        {
                            Line clickedLine = (Line)e.OriginalSource;
                            clickedLine.Name.EdgesNames(out int f_node, out int s_node);
                            _adjacenceList.RemoveEdge(f_node, s_node);
                            canvas.Children.Remove(clickedLine);



                        }
                        else if (add_ellipse == true) //add
                        {


                            Ellipse AAACircle = new Ellipse()
                            {
                                Name = $"Ellipse_{_adjacenceList.CountNodes + 1}",
                                Height = 50,
                                Width = 50,
                                Stroke = Brushes.Black,
                                StrokeThickness = 1,
                                Fill = Brushes.White,

                            };

                            TextBlock textBlock = new TextBlock()
                            {
                                Name = "Text" + AAACircle.Name,
                                Text = (_adjacenceList.CountNodes + 1).ToString(),
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                FontSize = 16,
                                FontFamily = new FontFamily("Arial"),

                            };

                            Canvas.SetLeft(AAACircle, Mouse.GetPosition(canvas).X);
                            Canvas.SetTop(AAACircle, Mouse.GetPosition(canvas).Y);


                            Point center1_for_text = DataFromGraph.AllignOfText(AAACircle, (_adjacenceList.CountNodes + 1).ToString());



                            Canvas.SetLeft(textBlock, center1_for_text.X);
                            Canvas.SetTop(textBlock, center1_for_text.Y);

                            canvas.Children.Add(AAACircle);
                            canvas.Children.Add(textBlock);
                            _adjacenceList.AddNode(AAACircle.Name.SingleNodeName());


                        }
                    }
                    if (win.Matrix != null)
                    {
                        win.UpdateMatrix();
                    }
                    if (inc_win.Matrix != null)
                    {
                        inc_win.UpdateMatrix();
                    }
                }


                public void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
                {
                    ClickedObject = null;
                }

                public void PreviewMouseMove(object sender, MouseEventArgs e)
                {
                    if (ClickedObject == null)
                        return;

                    DragEndPoint.X = e.GetPosition(canvas).X;
                    DragEndPoint.Y = e.GetPosition(canvas).Y;

                    double deltaX = DragEndPoint.X - DragStartPoint.X;
                    double deltaY = DragEndPoint.Y - DragStartPoint.Y;



                    if (move == true && ClickedObject is Ellipse)
                    {
                        Ellipse c = ClickedObject as Ellipse;
                        Canvas.SetLeft(c, ObjectStartLocation.X + deltaX);
                        Canvas.SetTop(c, ObjectStartLocation.Y + deltaY);

                        Point center1_for_text = DataFromGraph.AllignOfText(c, c.Name);



                        string textBoxName = "Text" + c.Name;  // Name of the TextBox to remove

                        TextBlock textBlock = canvas.Children.OfType<TextBlock>()
                                                    .FirstOrDefault(tb => tb.Name == textBoxName);

                        if (textBlock != null)
                        {
                            Canvas.SetLeft(textBlock, center1_for_text.X);
                            Canvas.SetTop(textBlock, center1_for_text.Y);
                        }

                        var lineNamesUndirected = DataFromGraph.GetConnectedEdges(ref canvas, _adjacenceList, c.Name.SingleNodeName(), GraphType.Undirected);
                        foreach (string lineName in lineNamesUndirected)
                        {
                            // Retrieve the line by its name
                            Line storedLine = canvas.Children.OfType<Line>().FirstOrDefault(l => l.Name == lineName);

                            if (storedLine != null)
                            {
                                string name_line = storedLine.Name;

                                string name_of_first_ellipse = name_line.Substring(name_line.IndexOf("_") + 1, name_line.LastIndexOf("_") - name_line.IndexOf("_") - 1);
                                string name_of_second_ellipse = name_line.Substring(name_line.LastIndexOf("_") + 1);

                                Ellipse sEllipse = canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == "Ellipse_" + name_of_second_ellipse);

                                // Find the first ellipse
                                Ellipse fEllipse = canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == "Ellipse_" + name_of_first_ellipse);

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
                        canvas.InvalidateVisual();
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }
}
