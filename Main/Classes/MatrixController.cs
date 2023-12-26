using Main.Enumerators;
using Main.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Main.Classes
{
    internal class MatrixController
    {
        public static void Adjacence(MainWindow main)
        {
            AdjacenceMatrix win = WindowsInstances.AdjacenceMatrixWindowInst(main);
            win.Owner = main;
            win.AddNodeDelegate += (win_del) => DelegateCanvasEvents.AddNodeToCanvas(main);
            win.AddEdgeDelegate += (win_del, x, y) => DelegateCanvasEvents.AddEdgeToCanvas(main, x.ToString(), y.ToString());
            win.DeleteNodeDelegate += (win_del, node, lines) => DelegateCanvasEvents.DeleteNodeCanvas(main, node, lines);
            win.DeleteEdgeDelegate += (win_del, x, y) => DelegateCanvasEvents.DeleteEdgeCanvas(main, x.ToString(), y.ToString());

            win.TypeGraph = main.graphType;
            win.Matrix = main.GraphAdjacenceList;
            win.Show();
        }

        public static void Adjacence(SecondGraph second_gr)
        {
            AdjacenceMatrix win = WindowsInstances.AdjacenceMatrixWindowInst(second_gr);
            win.Owner = second_gr;
            win.AddNodeDelegate += (main) => DelegateCanvasEvents.AddNodeToCanvas(second_gr);
            win.AddEdgeDelegate += (main, x, y) => DelegateCanvasEvents.AddEdgeToCanvas(second_gr, x.ToString(), y.ToString());
            win.DeleteNodeDelegate += (main, node, lines) => DelegateCanvasEvents.DeleteNodeCanvas(second_gr, node, lines);
            win.DeleteEdgeDelegate += (main, x, y) => DelegateCanvasEvents.DeleteEdgeCanvas(second_gr, x.ToString(), y.ToString());

            win.TypeGraph = second_gr.Type;
            win.Matrix = second_gr.SecondGraphAdjacenceList;
            win.Show();
        }

        public static void Incidence(MainWindow main)
        {
            IncidenceMatrix win = WindowsInstances.MatrixIncidenceWindowInst(main);
            win.Owner = main;
            win.AddNodeDelegate += (win_del) => DelegateCanvasEvents.AddNodeToCanvas(main);
            win.AddEdgeDelegate += (win_del, x, y) => DelegateCanvasEvents.AddEdgeToCanvas(main, x.ToString(), y.ToString());
            win.DeleteNodeDelegate += (win_del, node, lines) => DelegateCanvasEvents.DeleteNodeCanvas(main, node, lines);
            win.DeleteEdgeDelegate += (win_del, x, y) => DelegateCanvasEvents.DeleteEdgeCanvas(main, x.ToString(), y.ToString());

            win.Canvas = main.GraphCanvas;
            win.TypeGraph = main.graphType;
            win.Matrix = main.GraphAdjacenceList;
            win.Show();
        }

        public static void Incidence(SecondGraph second_gr)
        {
            IncidenceMatrix win = WindowsInstances.MatrixIncidenceWindowInst(second_gr);
            win.Owner = second_gr;
            win.AddNodeDelegate += (main) => DelegateCanvasEvents.AddNodeToCanvas(second_gr);
            win.AddEdgeDelegate += (main, x, y) => DelegateCanvasEvents.AddEdgeToCanvas(second_gr, x.ToString(), y.ToString());
            win.DeleteNodeDelegate += (main, node, lines) => DelegateCanvasEvents.DeleteNodeCanvas(second_gr, node, lines);
            win.DeleteEdgeDelegate += (main, x, y) => DelegateCanvasEvents.DeleteEdgeCanvas(second_gr, x.ToString(), y.ToString());

            win.Canvas = second_gr.SecondGraphCanvas;
            win.TypeGraph = second_gr.Type;
            win.Matrix = second_gr.SecondGraphAdjacenceList;
            win.Show();
        }

    }
}
