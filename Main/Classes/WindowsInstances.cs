using Main.Enumerators;
using Main.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Main.Classes
{
    static class WindowsInstances
    {
        static MainWindow _mainwin= Application.Current.Windows.OfType<MainWindow>().SingleOrDefault();
        static SecondGraph _sgraph;
        static OperationResult _resgraph;

        public static MainWindow MainWindowInst => _mainwin;

        public static SecondGraph SecGraphInst => SecondGraphInst();
        public static OperationResult ResGraphInst => ResultWindowInst();
        
        private static bool ResultWindowExist()
        {
            OperationResult instance = Application.Current.Windows.OfType<OperationResult>().SingleOrDefault();
            if (instance == null)
            {
                return false;
            }
            return true;
        }
        private static bool SecondGraphExist()
        {
            //SecondGraph instance = Application.Current.Windows.OfType<SecondGraph>().SingleOrDefault();
            if (_sgraph == null)
            {
                return false;
            }
            return true;
        }

        private static SecondGraph SecondGraphInst()
        {
            if(!SecondGraphExist())
            {
                _sgraph = new SecondGraph();
                return _sgraph;
            }
            _sgraph = Application.Current.Windows.OfType<SecondGraph>().SingleOrDefault();
            return _sgraph;
        }


        static public void WindowClosed(object sender, EventArgs e)
        {
            switch (sender)
            {
                case OperationResult:
                    {
                        _resgraph = null;
                        break;
                    }
                case SecondGraph:
                    {
                        _sgraph = null;
                        break;
                    }
                default:
                    {
                        break;
                    }
                   
            }
        }

        static public OperationResult ResultWindowInst()
        {
            if (!ResultWindowExist())
            {
                _resgraph = new OperationResult();
                return _resgraph;
            }
            _resgraph = Application.Current.Windows.OfType<OperationResult>().SingleOrDefault();
            return _resgraph;
        }

        static public bool AdjacenceMatrixWindowExist(Window wnd, out int win_index)
        {
            try
            {
                for (int i = 0; i < wnd.OwnedWindows.Count; i++)
                {
                    if (wnd.OwnedWindows[i] is AdjacenceMatrix)
                    {
                        win_index = i;
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {

            }
            win_index = -1;
            return false;

        }
        static public AdjacenceMatrix AdjacenceMatrixWindowInst(Window wnd)
        {

            if(!AdjacenceMatrixWindowExist(wnd, out int index))
            {
                return new AdjacenceMatrix();
            }

            return (AdjacenceMatrix)wnd.OwnedWindows[index];
           
        }


        static public bool MatrixIncidenceWindowExist(Window wnd, out int win_index)
        {
            try
            {
                for (int i = 0; i < wnd.OwnedWindows.Count; i++)
                {
                    if (wnd.OwnedWindows[i] is IncidenceMatrix)
                    {
                        win_index = i;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            win_index = -1;
            return false;

        }
        static public IncidenceMatrix MatrixIncidenceWindowInst(Window wnd)
        {

            if (!MatrixIncidenceWindowExist(wnd, out int index))
            {
                return new IncidenceMatrix();
            }

            return (IncidenceMatrix)wnd.OwnedWindows[index];

        }

    }



    static class SetWindowInfo
    {
        public static void SecGraphWindow(CurrentGraphOperation oper, GraphType type)
        {
            SecondGraph graph = WindowsInstances.SecGraphInst;
            graph.Type = type;
            graph.Owner = WindowsInstances.MainWindowInst;
            graph.CurrentOperation = oper;
            switch (oper)
            {
                case CurrentGraphOperation.Unity:
                    graph.Title = "Режим об'єднання графів";
                    break;
                case CurrentGraphOperation.CircleSum:
                    graph.Title = "Режим кільцевої суми графів";
                    break;
                case CurrentGraphOperation.Intersection:
                    graph.Title = "Режим перетину графів";
                    break;
                case CurrentGraphOperation.CartesianProduct:
                    graph.Title = "Режим декартового добутку графів";
                    break;
                default:
                    break;
            }
            graph.Show();
        }

        public static void ResultWindow(CurrentGraphOperation oper, GraphType type, Canvas bigger_canvas, Canvas smaller_canvas, AdjacenceList list1, AdjacenceList list2) 
        {
            OperationResult window = WindowsInstances.ResGraphInst;
            window.Owner = WindowsInstances.MainWindowInst;
            GraphOperationsCanvas graph = new GraphOperationsCanvas();
            switch (oper)
            {

                case CurrentGraphOperation.Unity:
                    window.SetCanvas(graph.Unity(bigger_canvas, list1.GetList, list2.GetList, type, out AdjacenceList unity));
                    break;
                case CurrentGraphOperation.CircleSum:
                    //window.SetResourceReference(Window.TitleProperty, "Resx CircleSumTitle");
                    window.SetCanvas(graph.CircleSum(bigger_canvas, list1.GetList, list2.GetList, type, out AdjacenceList circleSum));
                    break;
                case CurrentGraphOperation.Intersection:
                    //window.SetResourceReference(Window.TitleProperty, "Resx IntersectionTitle");
                    window.SetCanvas(graph.Intersection(bigger_canvas, list1.GetList, list2.GetList, type, out AdjacenceList intersection));
                    break;
                case CurrentGraphOperation.CartesianProduct:
                    
                    //window.SetResourceReference(Window.TitleProperty, "Resx CartesianProductTitle");
                    window.SetCanvas(graph.CartesianProduct(list1.GetList, list2.GetList, type));
                    break;
                default:
                    break;
            }
            window.SetOperation(oper);
            window.Show();
        }
        public static void AdjacenceMatrixWindow(Window win, GraphType type)
        {
            AdjacenceMatrix adj = WindowsInstances.AdjacenceMatrixWindowInst(win);

            adj.TypeGraph = type;
        }
    }

}


    

