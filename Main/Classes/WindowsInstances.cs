using Main.Enumerators;
using Main.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Main.Classes
{
    static class WindowsInstances
    {
        static MainWindow _mainwin= Application.Current.Windows.OfType<MainWindow>().SingleOrDefault();
        static SecondGraph _sgraph;
        static OperationResult _resgraph;
        static MatrixShow _matrixwin;
        public static MainWindow MainWinInst()
        {
            return _mainwin;
        }
        public static bool ResultWindowExist()
        {
            OperationResult instance = Application.Current.Windows.OfType<OperationResult>().SingleOrDefault();
            if (instance == null)
            {
                return false;
            }
            return true;
        }
        public static bool SecondGraphExist()
        {
            //SecondGraph instance = Application.Current.Windows.OfType<SecondGraph>().SingleOrDefault();
            if (_sgraph == null)
            {
                return false;
            }
            return true;
        }

        public static SecondGraph SecondGraphInst()
        {
            if(!SecondGraphExist())
            {
                return new SecondGraph();
            }
            return Application.Current.Windows.OfType<SecondGraph>().SingleOrDefault();
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
                return new OperationResult();
            }
            return Application.Current.Windows.OfType<OperationResult>().SingleOrDefault();
        }

    }

    static class SetWindowInfo
    {
       /* public static OperationResult SetCurrentOperation(OperationResult win, CurrentGraphOperation oper)
        {

        }*/
    }
}
