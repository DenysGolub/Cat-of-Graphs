using Main.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.InstrumentalPart
{
    public class Dijkstra
    {
        private static readonly int NO_PARENT = -1;

        // Function that implements Dijkstra's 
        // single source shortest path 
        // algorithm for a graph represented 
        // using adjacency matrix 
        // representation 
        public static string FindShortestWay(int[,] adjacencyMatrix,
                                            int startVertex)
        {
            startVertex -= 1;
            int nVertices = adjacencyMatrix.GetLength(0);

            // shortestDistances[i] will hold the 
            // shortest distance from src to i 
            int[] shortestDistances = new int[nVertices];

            // added[i] will true if vertex i is 
            // included / in shortest path tree 
            // or shortest distance from src to 
            // i is finalized 
            bool[] added = new bool[nVertices];

            // Initialize all distances as 
            // INFINITE and added[] as false 
            for (int vertexIndex = 0; vertexIndex < nVertices;
                                                vertexIndex++)
            {
                shortestDistances[vertexIndex] = int.MaxValue;
                added[vertexIndex] = false;
            }

            // Distance of source vertex from 
            // itself is always 0 
            shortestDistances[startVertex] = 0;

            // Parent array to store shortest 
            // path tree 
            int[] parents = new int[nVertices];

            // The starting vertex does not 
            // have a parent 
            parents[startVertex] = NO_PARENT;

            // Find shortest path for all 
            // vertices 
            for (int i = 0; i < nVertices; i++)
            {

                // Pick the minimum distance vertex 
                // from the set of vertices not yet 
                // processed. nearestVertex is 
                // always equal to startNode in 
                // first iteration. 
                int nearestVertex = -1;
                int shortestDistance = int.MaxValue;
                for (int vertexIndex = 0;
                        vertexIndex < nVertices;
                        vertexIndex++)
                {
                    if (!added[vertexIndex] &&
                        shortestDistances[vertexIndex] <
                        shortestDistance)
                    {
                        nearestVertex = vertexIndex;
                        shortestDistance = shortestDistances[vertexIndex];
                    }
                }

                // Mark the picked vertex as 
                // processed 
                added[nearestVertex] = true;

                // Update dist value of the 
                // adjacent vertices of the 
                // picked vertex. 
                for (int vertexIndex = 0;
                        vertexIndex < nVertices;
                        vertexIndex++)
                {
                    int edgeDistance = adjacencyMatrix[nearestVertex, vertexIndex];

                    if (edgeDistance > 0
                        && ((shortestDistance + edgeDistance) <
                            shortestDistances[vertexIndex]))
                    {
                        parents[vertexIndex] = nearestVertex;
                        shortestDistances[vertexIndex] = shortestDistance +
                                                        edgeDistance;
                    }
                }
            }

            return printSolution(startVertex, shortestDistances, parents);
        }

        // A utility function to print 
        // the constructed distances 
        // array and shortest paths 
        private static string printSolution(int startVertex,
                                        int[] distances,
                                        int[] parents)
        {
            string str = "";
            int nVertices = distances.Length;
            str += ("Vertex\t\tDistance\t\tPath");

            for (int vertexIndex = 0;
                    vertexIndex < nVertices;
                    vertexIndex++)
            {
                if (vertexIndex != startVertex)
                {
                    str += $"\n{startVertex + 1}->";
                    str += $"{vertexIndex + 1}\t\t";
                    str += $"{distances[vertexIndex]}\t\t";
                    printPath(vertexIndex, parents, ref str);
                }
            }
            return str;
        }

        // Function to print shortest path 
        // from source to currentVertex 
        // using parents array 
        private static void printPath(int currentVertex,
                                    int[] parents, ref string  str)
        {

            // Base case : Source node has 
            // been processed 
            if (currentVertex == NO_PARENT)
            {
                return;
            }
            printPath(parents[currentVertex], parents, ref str);
            str += $"{currentVertex + 1} ";
        }
    }
}
