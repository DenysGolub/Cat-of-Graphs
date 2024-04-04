using Main.Enumerators;
using Main.TestingPart;
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
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Main.Classes
{
    static class WindowsInstances
    {
        static MainWindow _mainwin = Application.Current.Windows.OfType<MainWindow>().SingleOrDefault();
        static SecondGraph _sgraph;
        static OperationResult _resgraph;
        static About _about;
        static ImageWindow _imgwin;

        public static MainWindow MainWindowInst => _mainwin;

        public static SecondGraph SecGraphInst => SecondGraphInst();
        public static OperationResult ResGraphInst => ResultWindowInst();
        public static About AboutInst => AboutWindowInst();
        
        private static bool ResultWindowExist()
        {
            OperationResult instance = Application.Current.Windows.OfType<OperationResult>().SingleOrDefault();
            if (instance == null)
            {
                return false;
            }
            return true;
        }

        private static bool ImgWindowExist()
        {
            ImageWindow instance = Application.Current.Windows.OfType<ImageWindow>().SingleOrDefault();
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

        private static About AboutWindowInst()
        {
            if (!IsAboutWindowExist(MainWindowInst, out int index))
            {
                _about = new About();
                return _about;
            }
            _about = Application.Current.Windows.OfType<About>().SingleOrDefault();
            _about.Activate();
            return _about;
        }

        private static bool IsAboutWindowExist(Window wnd, out int win_index)
        {
            try
            {
                for (int i = 0; i < wnd.OwnedWindows.Count; i++)
                {
                    if (wnd.OwnedWindows[i] is About)
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
                case About:
                    {
                        _about = null;
                        break;
                    }
                case ImageWindow:
                {
                        _imgwin = null;
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
            win_index = -1;
            try
            {
                if (wnd==null)
                {
                    return false;
                }
                for (int i = 0; i < wnd.OwnedWindows.Count; i++)
                {
                    if (wnd.OwnedWindows[i] is Main.Windows.AdjacenceMatrix)
                    {
                        win_index = i;
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return false;

        }
        static public Main.Windows.AdjacenceMatrix AdjacenceMatrixWindowInst(Window wnd)
        {

            if(!AdjacenceMatrixWindowExist(wnd, out int index))
            {
                return new Main.Windows.AdjacenceMatrix();
            }

            return (Main.Windows.AdjacenceMatrix)wnd.OwnedWindows[index];
           
        }


        static public bool MatrixIncidenceWindowExist(Window wnd, out int win_index)
        {
            win_index = -1;
            try
            {   
                if(wnd==null)
                {
                    return false;
                }
                for (int i = 0; i < wnd.OwnedWindows.Count; i++)
                {
                    if (wnd.OwnedWindows[i] is Main.Windows.IncidenceMatrix)
                    {
                        win_index = i;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return false;

        }
        static public Main.Windows.IncidenceMatrix MatrixIncidenceWindowInst(Window wnd)
        {

            if (!MatrixIncidenceWindowExist(wnd, out int index))
            {
                return new Main.Windows.IncidenceMatrix();
            }

            return (Main.Windows.IncidenceMatrix)wnd.OwnedWindows[index];

        }

        static public ImageWindow ImageWindowInst(ImageSource source)
        {
            if (!ImgWindowExist())
            {
                _imgwin = new ImageWindow(source);
                _imgwin.Show();
                _imgwin.Owner = MainWindowInst;
                return _imgwin;
            }
            _imgwin = Application.Current.Windows.OfType<ImageWindow>().SingleOrDefault();
            _imgwin.Graph.Source = source;
            return _imgwin;
        }

    }



    static class SetWindowInfo
    {
        public static void AboutWindow()
        {
            About about = WindowsInstances.AboutInst;

            about.Owner = WindowsInstances.MainWindowInst;
            about.Show();
        }
        public static void SecGraphWindow(CurrentGraphOperation oper, GraphType type)
        {
            SecondGraph graph = WindowsInstances.SecGraphInst;
            graph.Type = type;
            graph.Owner = WindowsInstances.MainWindowInst;
            graph.CurrentOperation = oper;
            
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
            Main.Windows.AdjacenceMatrix adj = WindowsInstances.AdjacenceMatrixWindowInst(win);

            adj.TypeGraph = type;
        }
    }

}


    

