using Gu.Wpf.DataGrid2D;
using Main.Classes;
using Main.Enumerators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class ToGraphFromAdjMatrixWin : Window
    {
        Question _quest;


        AdjacenceList adjacenceListUndirected = new AdjacenceList(GraphType.Undirected);
        AdjacenceList adjacenceListDirected = new AdjacenceList(GraphType.Directed);
        GraphOperationsCanvas graph_operations = new GraphOperationsCanvas();

        Events.CanvasEvents.Undirected undirected_events = new Events.CanvasEvents.Undirected();
        Events.CanvasEvents.Directed directed_events = new Events.CanvasEvents.Directed();
        Events events = new Events();
        GraphType _type;
        public AdjacenceList GraphAdjacenceList
        {
            get
            {
                if (DrawingCanvas_Undirected.Visibility == Visibility.Visible)
                {
                    return adjacenceListUndirected;
                }
                return adjacenceListDirected;
            }
        }

        public ToGraphFromAdjMatrixWin()
        {
            InitializeComponent();
        }

        public ToGraphFromAdjMatrixWin(Question quest)
        {
            InitializeComponent();
            _quest = quest;

            if(quest.GraphType == GraphType.Undirected)
            {
                DrawingCanvas_Directed.Visibility = Visibility.Collapsed;
                DrawingCanvas_Undirected.Visibility = Visibility.Visible;
            }
            else if(quest.GraphType == GraphType.Directed)
            {
                DrawingCanvas_Directed.Visibility = Visibility.Visible;
                DrawingCanvas_Undirected.Visibility = Visibility.Collapsed;
            }

            matrix.SetArray2D(_quest.AdjMatrix.ToAdjacenceMatrix());
            matrix.SetColumnHeadersSource(_quest.AdjMatrix.GetList.Keys.ToArray());
            matrix.SetRowHeadersSource(_quest.AdjMatrix.GetList.Keys.ToArray());

            if (quest.Description != "")
            {
                Description.Text = quest.Description;
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

            var answer_matrix = (GraphAdjacenceList.ToAdjacenceMatrix());
            PrintMatrix(answer_matrix);
            Debug.WriteLine(" ");

            var quest_matrix = (_quest.AdjMatrix.ToAdjacenceMatrix());
            PrintMatrix(quest_matrix);
            if (AreMatricesEqual(answer_matrix, quest_matrix))
            {
                DialogResult = true;
            }
            else
            {
                DialogResult= false;
            }
        }

        private void PrintMatrix(sbyte[,] m)
        {
            for(int i =0; i<m.GetLength(0); i++)
            {
                for(int j = 0; j<m.GetLength(1); j++)
                {
                    Debug.Write(m[i,j] + " ");
                }
                Debug.WriteLine("");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawingCanvas_Undirected.PreviewMouseLeftButtonUp += (sender, e) => undirected_events.PreviewMouseLeftButtonUp(sender, e);
            DrawingCanvas_Undirected.PreviewMouseLeftButtonDown += (sender, e) => undirected_events.PreviewMouseLeftButtonDown(sender, e);
            DrawingCanvas_Undirected.PreviewMouseMove += (sender, e) => undirected_events.PreviewMouseMove(sender, e);

            DrawingCanvas_Directed.PreviewMouseLeftButtonDown += (sender, e) => directed_events.PreviewMouseLeftButtonDown(sender, e);
            DrawingCanvas_Directed.PreviewMouseLeftButtonUp += (sender, e) => directed_events.PreviewMouseLeftButtonUp(sender, e);
            DrawingCanvas_Directed.PreviewMouseMove += (sender, e) => directed_events.PreviewMouseMove(sender, e);

            undirected_events.Canvas = DrawingCanvas_Undirected;
            undirected_events.AdjacenceList = adjacenceListUndirected;

            directed_events.Canvas = DrawingCanvas_Directed;
            directed_events.AdjacenceList = adjacenceListDirected;

            undirected_events.SubscribeToChangesQuest(events);
            directed_events.SubscribeToChanges(events);

            Delete.Click += (sender, e) => events.RemoveMode();
            MoveEllipse.Click += (sender, e) => events.MoveMode();
            AddEllipse.Click += (sender, e) => events.AddMode();

        }
    }
}
