using Main.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Main.InstrumentalPart
{
    public class GraphComponents: SearchAlgorithms
    {
        static string comp;
        
        // Знаходження компонентів зв'язності
        public string ConnectedComponents(AdjacenceList adj, int V)
        {
            bool[] visited = new bool[V+1];
            comp = "";
            for (int v = 1; v <= V; ++v)
            {
                if (!visited[v])
                {
                    // Вивести всі вершини, що досяжні з вершини v
                    DepthFirstSearch(adj, v, visited, ref comp);
                    comp += ("\n");
                }
            }

            return comp;
        }

        private protected override void DepthFirstSearch(AdjacenceList adj, int v, bool[] visited, ref string comp)
        {
            visited[v] = true;
            comp += (v + " ");

            foreach (int i in adj[v])
            {
                if (!visited[i])
                    DepthFirstSearch(adj, i, visited, ref comp);
            }
        }
    }
}
