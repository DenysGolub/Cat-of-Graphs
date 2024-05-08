using Main.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.InstrumentalPart
{
    public class Tree:SearchAlgorithms
    {
        public static bool isTree(AdjacenceList adj, int count_of_edges, int count_of_nodes)
        {
            // as we proved earlier if a graph is connected and
            // has V - 1 edges then it is a tree i.e. E = V - 1
            return isConnected(adj, count_of_nodes) && count_of_edges == count_of_nodes - 1;
        }

        private static void DFSUtil(AdjacenceList adj, int v, bool[] visited, int parent)
        {
           
            if(adj.GetList.Count == 0) return;
            // Mark the current node as visited
            visited[v] = true;

            // Recur for all the vertices adjacent to this
            // vertex
            foreach (var i in adj[v])
            {
                // If an adjacent is not visited, then recur for
                // that adjacent
                if (!visited[i])
                {
                    DFSUtil(adj, i, visited, v);
                }
            }
        }

        // Returns true if the graph is connected, else false.
        public static bool isConnected(AdjacenceList adj, int V)
        {
            // Mark all the vertices as not visited and not part
            // of recursion stack
            bool[] visited = new bool[V+1];
            for (int i = 0; i <= V; i++)
                visited[i] = false;

            // Performing DFS traversal of the graph and marking
            // reachable vertices from 0 to true
            DFSUtil(adj, 1, visited, -1);

            // If we find a vertex which is not reachable from 0
            // (not marked by dfsTraversal(), then we return
            // false since graph is not connected
            for (int u = 1; u <= V; u++)
                if (!visited[u])
                    return false;

            // since all nodes were reachable so we returned
            // true and hence graph is connected
            return true;
        }

    }
}
