using Gu.Wpf.DataGrid2D;
using Main.Classes;
using Main.Enumerators;
using Main.Interfaces;
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

namespace Main.Windows
{
    /// <summary>
    /// Interaction logic for MatrixShow.xaml
    /// </summary>
    public partial class MatrixShow : Window
    {
        public MatrixShow() 
        {
            InitializeComponent();
        }

        public MatrixShow(AdjacenceList list)
        {
            InitializeComponent();
            var array = list.ToAdjacenceMatrix();
            matrix.SetArray2D(array);
            
            

            matrix.SetRowHeadersSource(list.GetList.Keys.ToArray());
            matrix.SetColumnHeadersSource(list.GetList.Keys.ToArray());
            Title = "Матриця суміжності";

        }

        public MatrixShow(AdjacenceList list, Canvas c, GraphType g_type)
        {
            InitializeComponent();
            var array = list.ToIncidenceMatrix(g_type, ref c, out List<string> lines);
            matrix.SetArray2D(array);



            matrix.SetRowHeadersSource(list.GetList.Keys.ToArray());

            matrix.SetColumnHeadersSource(lines);
            Title = "Матриця інцидентності";
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
