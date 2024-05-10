using Main.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.InstrumentalPart
{
    public class Tree: SearchAlgorithms
    {
        public bool isTree(AdjacenceList adj, int count_of_edges, int count_of_nodes)
        {
            return isConnected(adj, count_of_nodes) && count_of_edges == count_of_nodes - 1;
        }

        private protected override void DepthFirstSearch(AdjacenceList adj, int v, bool[] visited, ref string parent)
        {
         
            if(adj.GetList.Count == 0) return;
            visited[v] = true;
            string v_str = v.ToString();
            foreach (var i in adj[v])
            {
                if (!visited[i])
                {
                    DepthFirstSearch(adj, i, visited, ref v_str);
                }
            }
        }

        public bool isConnected(AdjacenceList adj, int V)
        {
            bool[] visited = new bool[V+1];
            for (int i = 0; i <= V; i++)
                visited[i] = false;
            string v_str = (-1).ToString();
            DepthFirstSearch(adj, 1, visited, ref v_str);

            for (int u = 1; u <= V; u++)
                if (!visited[u])
                    return false;

            return true;
        }

    }
}
