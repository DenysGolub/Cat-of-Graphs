using Main.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Main.InstrumentalPart
{
    public class GraphComponents: SearchAlgorithms
    {
        static string comp = "";
        
        // Знаходження компонентів зв'язності
        public static void ConnectedComponents(AdjacenceList adj, int V)
        {
            bool[] visited = new bool[V+1];
            comp = "";
            for (int v = 1; v <= V; ++v)
            {
                if (!visited[v])
                {
                    // Вивести всі вершини, що досяжні з вершини v
                    DFSUtil(adj, v, visited, ref comp);
                    comp += ("\n");
                }
            }
            MessageBox.Show(comp, "Компоненти зв'язності");
        }
    }
}
