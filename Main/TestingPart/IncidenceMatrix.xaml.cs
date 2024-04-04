using Gu.Wpf.DataGrid2D;
using Main.Classes;
using Main.Enumerators;
using Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Main.TestingPart
{
    /// <summary>
    /// Interaction logic for IncidenceMatrix.xaml
    /// </summary>
    public partial class IncidenceMatrix : Window
    {

        private NamesUpdate update = new NamesUpdate();
        Canvas canv;

        AdjacenceList matrix_array;
        GraphType type;
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
                matrix.SetArray2D(ToIncMatrix(out List<string> lines));
                matrix.SetRowHeadersSource(matrix_array.GetList.Keys.ToArray());
                matrix.SetColumnHeadersSource(lines);
            }
        }

        public IncidenceMatrix()
        {
            InitializeComponent();
            Title = "Матриця інцидентності";
            NodeAdd.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));
            NodeDel.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));
            EdgeAdd.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));
            EdgeDel.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));
        }

        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            matrix_array.AddNode(matrix_array.CountNodes + 1);
        }


        private void UpdateMatrix(object sender, RoutedEventArgs e)
        {
            matrix.SetArray2D(ToIncMatrix(out List<string> lines));
            matrix.SetRowHeadersSource(matrix_array.GetList.Keys.ToArray());
            matrix.SetColumnHeadersSource(lines);
        }

        public void UpdateMatrix()
        {
            try
            {
                matrix.SetArray2D(ToIncMatrix(out List<string>lines));
                matrix.SetRowHeadersSource(matrix_array.GetList.Keys.ToArray());
                matrix.SetColumnHeadersSource(lines);

            }
            catch (Exception ex)
            {

            }
        }

        private void DeleteNode_Click(object sender, RoutedEventArgs e)
        {
            var drv = matrix.Items.IndexOf(matrix.CurrentItem) + 1;

            matrix_array.RemoveNode(int.Parse(drv.ToString()));
            update.UpdateNodes(matrix_array, int.Parse(drv.ToString()));
        }

        private void AddEdge_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Windows.InputBox();
            if (dialog.ShowDialog() == true)
            {
                Match match = Regex.Match(dialog.ResponseText, @"(\d{1,}) (\d{1,})");
                if (match.Success)
                {
                    matrix_array.AddEdge(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                }
            }
        }

        private void DeleteEdge_Click(object sender, RoutedEventArgs e)
        {
            var drv = matrix.CurrentCell.Column.Header.ToString();

            drv.EdgesNames(out int f_node, out int s_node);

            matrix_array.RemoveEdge(f_node, s_node);
        }

        private sbyte[,]ToIncMatrix(out List<string> lineNames)
        {
            lineNames = matrix_array.GetList.GetLinesBasedOnType(type);
            sbyte[,] incidence_matrix = new sbyte[matrix_array.GetList.Keys.Count, lineNames.Count];
            int count = 0;

            switch (type)
            {
                case GraphType.Directed:
                    break;
                case GraphType.Undirected:
                    break;
                default:
                    break;
            }

            switch (type)
            {
                case GraphType.Directed:
                    {
                        foreach (string line in lineNames)
                        {
                            line.EdgesNames(out int f_node, out int s_node);

                            if (f_node == s_node)
                            {
                                incidence_matrix[f_node - 1, count] = 2;
                            }
                            else
                            {
                                incidence_matrix[f_node - 1, count] = -1;
                                incidence_matrix[s_node - 1, count] = 1;
                            }
                            count++;
                        }
                        break;
                    }
                case GraphType.Undirected:
                    {
                        foreach (string line in lineNames)
                        {

                            line.EdgesNames(out int f_node, out int s_node);

                            incidence_matrix[f_node - 1, count] = 1;
                            incidence_matrix[s_node - 1, count] = 1;
                            count++;
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return incidence_matrix;
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            GC.Collect(); // find finalizable objects
            GC.WaitForPendingFinalizers(); // wait until finalizers executed
            GC.Collect(); // collect finalized objects
        }
    }
}
