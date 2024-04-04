using Gu.Wpf.DataGrid2D;
using Main.Classes;
using Main.Enumerators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace Main.TestingPart.QuestionsAnsweringWindows
{
    /// <summary>
    /// Interaction logic for ToAdjMatrixFromGraphWin.xaml
    /// </summary>
    public partial class ToAdjMatrixFromGraphWin : Window
    {
        AdjacenceList matrix_array=new AdjacenceList();
        Question _quest;
        GraphType type;

        private NamesUpdate update = new NamesUpdate();
        public GraphType TypeGraph { get { return type; } set { type = value; } }
        public ToAdjMatrixFromGraphWin(Question quest)
        {
            InitializeComponent();
            NodeAdd.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));
            NodeDel.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(UpdateMatrix));
            _quest = quest;
            GraphImage.Source = LoadImage(quest.ImageSource);
            type = quest.GraphType;
            matrix_array.Type = type;

            if(quest.Description != "")
            {
                Description.Text = quest.Description;
            }
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
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

        bool AreMatricesEqual(sbyte[,] matrix1, sbyte[,] matrix2)
        {
            // Check dimensions
            if (matrix1.GetLength(0) != matrix2.GetLength(0) || matrix1.GetLength(1) != matrix2.GetLength(1))
                return false;

            // Check each element
            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix1.GetLength(1); j++)
                {
                    if (matrix1[i, j] != matrix2[i, j])
                        return false;
                }
            }

            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var answer_matrix = matrix_array.ToAdjacenceMatrix();

            var quest_matrix = (_quest.AdjMatrix.ToAdjacenceMatrix());
            if (AreMatricesEqual((sbyte[,])answer_matrix, quest_matrix))
            {
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
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
