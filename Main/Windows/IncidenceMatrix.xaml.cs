using Gu.Wpf.DataGrid2D;
using Main.Classes;
using Main.Enumerators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Main.Windows
{
   

    /// <summary>
    /// Interaction logic for IncidenceMatrix.xaml
    /// </summary>
    public partial class IncidenceMatrix : Window
    {

        public event AddNode AddNodeDelegate;
        public event Edge AddEdgeDelegate;

        public event DelNode DeleteNodeDelegate;
        public event DelEdge DeleteEdgeDelegate;


        public event UpdateMatrix Update;
        private NamesUpdate update = new NamesUpdate();
        Canvas canv;

        AdjacenceList matrix_array;
        GraphType type;
        public GraphType TypeGraph { get { return type; } set { type = value; } }

        public Canvas Canvas
        {
            get
            {
                return canv;
            }
            set
            {
                canv = value;
            }
        }
        public AdjacenceList Matrix
        {
            get
            {
                return matrix_array;
            }
            set
            {
                matrix.SetArray2D(matrix_array.ToIncidenceMatrix(type, ref canv, out List<string> lines));
                matrix.SetRowHeadersSource(matrix_array.GetList.Keys.ToArray());
                matrix.SetColumnHeadersSource(lines);
            }
        }

        public IncidenceMatrix()
        {

        }

        public IncidenceMatrix(AdjacenceList list, Canvas c, GraphType g_type)
        {
            InitializeComponent();
            var array = list.ToIncidenceMatrix(g_type, ref c, out List<string> lines);
            matrix_array = list;
            matrix.SetArray2D(array);



            matrix.SetRowHeadersSource(list.GetList.Keys.ToArray());

            matrix.SetColumnHeadersSource(lines);
            type = g_type;
            Title = "Матриця інцидентності";
            NodeAdd.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));
            NodeDel.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));
            EdgeAdd.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));
            EdgeDel.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));



            canv = c;

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
            matrix.SetArray2D(matrix_array.ToIncidenceMatrix(type, ref canv, out List<string>lines));
            matrix.SetRowHeadersSource(matrix_array.GetList.Keys.ToArray());
            matrix.SetColumnHeadersSource(lines);

        }

        public void UpdateMatrix()
        {
            try
            {
                matrix.SetArray2D(matrix_array.ToIncidenceMatrix(type, ref canv, out List<string> lines));
                matrix.SetRowHeadersSource(matrix_array.GetList.Keys.ToArray());
                matrix.SetColumnHeadersSource(lines);
            }
            catch (Exception ex)
            {

            }
        }
        private void DeleteNode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var canv = type == GraphType.Undirected ? WindowsInstances.MainWinInst().DrawingCanvas_Undirected : WindowsInstances.MainWinInst().DrawingCanvas_Directed;
                var drv = matrix.Items.IndexOf(matrix.CurrentItem)+1;

                var lines = DataFromGraph.GetConnectedEdges(ref canv, matrix_array, int.Parse(drv.ToString()), type);
                matrix_array.RemoveNode(int.Parse(drv.ToString()));
                update.UpdateNodes(matrix_array, int.Parse(drv.ToString()));

                if (DeleteNodeDelegate != null)
                {
                    DeleteNodeDelegate(int.Parse(drv.ToString()), lines);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void AddEdge_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InputBox();
            if (dialog.ShowDialog() == true)
            {
                Match match = Regex.Match(dialog.ResponseText, @"(\d{1,}) (\d{1,})");
                if(match.Success)
                {
                    if (AddEdgeDelegate != null)
                    {
                        AddEdgeDelegate(match.Groups[1].Value, match.Groups[2].Value);
                    }
                }
            }
        }

        private void DeleteEdge_Click(object sender, RoutedEventArgs e)
        {
            var drv = matrix.CurrentCell.Column.Header.ToString();

            drv.EdgesNames(out int f_node, out int s_node);
            if (DeleteEdgeDelegate != null)
            {
                DeleteEdgeDelegate(f_node.ToString(), s_node.ToString());
            }
        }
    }
}
