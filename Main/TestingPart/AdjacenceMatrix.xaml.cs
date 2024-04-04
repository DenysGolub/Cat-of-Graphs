using Gu.Wpf.DataGrid2D;
using Main.Classes;
using Main.Enumerators;
using System.Windows;
using System.Windows.Controls;

namespace Main.TestingPart
{
    /// <summary>
    /// Interaction logic for MatrixShow.xaml
    /// </summary>
    public partial class AdjacenceMatrix : Window
    {
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
        public AdjacenceMatrix()
        {
            InitializeComponent();
            NodeAdd.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));
            NodeDel.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            GC.Collect(); // find finalizable objects
            GC.WaitForPendingFinalizers(); // wait until finalizers executed
            GC.Collect(); // collect finalized objects
           
        }

        private void matrix_ValueInCellChanged(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGridColumn col1 = e.Column;
            DataGridRow row1 = e.Row;

            if (((TextBox)e.EditingElement).Text != "0" && type == GraphType.Undirected && e.Column.Header.ToString() == e.Row.Header.ToString())
            {
                System.Windows.MessageBox.Show("У неорієнтованому графі неможливі петлі!");
                ((TextBox)e.EditingElement).Text = "0";
                e.Cancel = true;
            }
            else if (((TextBox)e.EditingElement).Text != "0" && ((TextBox)e.EditingElement).Text != "1")
            {
                System.Windows.MessageBox.Show("Неправильне значення! Введіть 0 або 1");
                e.Cancel = true;
            }
            else
            {
                matrix_array.AddEdge(int.Parse(col1.Header.ToString()), int.Parse(row1.Header.ToString()));
            }
        }


        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            matrix_array.AddNode(matrix_array.CountNodes + 1);
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
            catch (Exception ex)
            {

            }
        }

        private void DeleteNode_Click(object sender, RoutedEventArgs e)
        {
            var drv = matrix.CurrentCell.Column.Header;
            matrix_array.RemoveNode(int.Parse(drv.ToString()));
            update.UpdateNodes(matrix_array, int.Parse(drv.ToString()));
        }
    }
}
