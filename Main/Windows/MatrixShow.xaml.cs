using Gu.Wpf.DataGrid2D;
using Main.Classes;
using Main.Enumerators;
using Main.Interfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public delegate void AddNode();
    public delegate void DelNode(int node, HashSet<string> lines);

    public delegate void Edge(string x, string y);
    public delegate void DelEdge(string x, string y);
    public delegate void UpdateMatrix();


    /// <summary>
    /// Interaction logic for MatrixShow.xaml
    /// </summary>
    public partial class MatrixShow : Window
    {
        public event AddNode AddNodeDelegate;
        public event Edge AddEdgeDelegate;

        public event DelNode DeleteNodeDelegate;
        public event DelEdge DeleteEdgeDelegate;


        public event UpdateMatrix Update;

        AdjacenceList matrix_array;
        GraphType type;

        private NamesUpdate update = new NamesUpdate();
        public GraphType TypeGraph { get { return type; } set { type = value; } } 
        public AdjacenceList Matrix
        {
            get
            {
                return matrix_array;
            }
            set
            {
                matrix_array = value;
                matrix.SetArray2D(matrix_array.ToAdjacenceMatrix());
                matrix.SetRowHeadersSource(matrix_array.GetList.Keys.ToArray());
                matrix.SetColumnHeadersSource(matrix_array.GetList.Keys.ToArray());
            }
        }
        public MatrixShow() 
        {
            InitializeComponent();
            NodeAdd.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));
            NodeDel.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));

        }

        public MatrixShow(AdjacenceList list)
        {
            InitializeComponent();
            //var array = list.ToAdjacenceMatrix();
           

        }

        public MatrixShow(GraphType type)
        {
            this.type = type;
        }

        public MatrixShow(AdjacenceList list, Canvas c, GraphType g_type)
        {
            InitializeComponent();
            var array = list.ToIncidenceMatrix(g_type, ref c, out List<string> lines);
            matrix.SetArray2D(array);



            matrix.SetRowHeadersSource(list.GetList.Keys.ToArray());

            matrix.SetColumnHeadersSource(lines);
            type = g_type;
            Title = "Матриця інцидентності";
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        
        private void matrix_ValueInCellChanged(object sender, DataGridCellEditEndingEventArgs e)
        {
            

            DataGridColumn col1 = e.Column;
            DataGridRow row1 = e.Row;
            if (((TextBox)e.EditingElement).Text == "1")
            {

                /*            var b = new MatrixChangesControl();
                            b.Change(col1, row1, this);*/



                if (AddEdgeDelegate != null)
                {
                    AddEdgeDelegate(e.Row.Header.ToString(), e.Column.Header.ToString());
                }
            }
            else if (((TextBox)e.EditingElement).Text == "0")
            {
                if (DeleteNodeDelegate != null)
                {
                    DeleteEdgeDelegate(e.Row.Header.ToString(), e.Column.Header.ToString());
                }
            }
        }

        
        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            matrix_array.AddNode(matrix_array.CountNodes + 1);

            if (AddNodeDelegate != null)
            {
                AddNodeDelegate();
            }
        }

        private void UpdateMatrix(object sender, RoutedEventArgs e)
        {
            matrix.SetArray2D(matrix_array.ToAdjacenceMatrix());
            matrix.SetRowHeadersSource(matrix_array.GetList.Keys.ToArray());
            matrix.SetColumnHeadersSource(matrix_array.GetList.Keys.ToArray());

        }
        public void UpdateMatrix()
        {
            try
            {
                matrix.SetArray2D(matrix_array.ToAdjacenceMatrix());
                matrix.SetRowHeadersSource(matrix_array.GetList.Keys.ToArray());
                matrix.SetColumnHeadersSource(matrix_array.GetList.Keys.ToArray());
            }
            catch(Exception ex)
            {

            }
        }

        private void DeleteNode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var canv = type == GraphType.Undirected ? WindowsInstances.MainWinInst().DrawingCanvas_Undirected : WindowsInstances.MainWinInst().DrawingCanvas_Directed;
                var drv = matrix.CurrentCell.Column.Header;
                var lines = DataFromGraph.GetConnectedEdges(ref canv, matrix_array, int.Parse(drv.ToString()), type);
                matrix_array.RemoveNode(int.Parse(drv.ToString()));
                update.UpdateNodes(matrix_array, int.Parse(drv.ToString()));

                if (DeleteNodeDelegate != null)
                {
                    DeleteNodeDelegate(int.Parse(drv.ToString()), lines);
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
