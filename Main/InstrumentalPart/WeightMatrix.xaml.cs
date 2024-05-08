using Gu.Wpf.DataGrid2D;
using Main.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
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
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Main.InstrumentalPart
{
    /// <summary>
    /// Interaction logic for WeightMatrix.xaml
    /// </summary>
    public partial class WeightMatrix : Window
    {
        int[,] table = new int[0, 0];
        public WeightMatrix()
        {
            InitializeComponent();
        }

        public WeightMatrix(AdjacenceList list)
        {
            InitializeComponent();
            table = new int[list.CountNodes, list.CountNodes];

            matrix.SetArray2D(table);

            matrix.SetRowHeadersSource(list.GetList.Keys.ToArray());
            matrix.SetColumnHeadersSource(list.GetList.Keys.ToArray());
        }

        private void matrix_ValueInCellChanged(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGridColumn col1 = e.Column;
            DataGridRow row1 = e.Row;
            int weight = int.Parse(((TextBox)e.EditingElement).Text);

            table[matrix.Items.IndexOf(matrix.CurrentItem), matrix.SelectedCells[0].Column.DisplayIndex] = weight;
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var str = Dijkstra.FindShortestWay(table, int.Parse(source_tt.Text));
            MessageBox.Show(str);
        }
    }
}
