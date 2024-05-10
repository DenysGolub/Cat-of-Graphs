using Main.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows.Documents;

namespace Main.InstrumentalPart
{
    public class Euler:SearchAlgorithms
    {
        public bool isEuler(Dictionary<int, int> degree)
        {
            foreach(var key in degree)
            {
                if(key.Value % 2 !=0)
                {
                    return false;
                }
            }
            return true;
        }

        public virtual List<int> FindEulerianPathCircuit(AdjacenceList list)
        {
            return new List<int>();
        }


        private protected virtual bool IsConnectedOrHasEulerianCircuit(AdjacenceList graph)
        {
            return true;
        }

        private protected virtual int FindStartNode(AdjacenceList graph) { return 0; }
    }

    public class EulerDirected:Euler
    {
        public override List<int> FindEulerianPathCircuit(AdjacenceList list)
        {

            AdjacenceList graph = new AdjacenceList();


            foreach (var item in list.GetList)
            {
                graph.GetList.Add(item.Key, new HashSet<int>(item.Value));
            }
            if (graph.GetList.Count == 0) { return new List<int>(); }


            // If the graph is not connected or it has more than 2 nodes with different in-degree and out-degree, return null
            if (!IsConnectedOrHasEulerianCircuit(graph))
            {
                Console.WriteLine("Graph does not have an Eulerian path or circuit.");
                return null;
            }

            // Find the starting node for the Eulerian path or circuit
            int startNode = FindStartNode(graph);

            // Create a stack to store the path
            Stack<int> stack = new Stack<int>();
            List<int> circuit = new List<int>();

            // Create a dictionary to keep track of visited edges
            Dictionary<int, HashSet<int>> visitedEdges = new Dictionary<int, HashSet<int>>();

            foreach (var nodeEdges in graph.GetList)
            {
                visitedEdges[nodeEdges.Key] = new HashSet<int>();
            }

            // Push the starting node onto the stack
            stack.Push(startNode);
            int currentNode = startNode;

            while (stack.Count > 0)
            {
                // If there's an unvisited edge from the current node
                if (graph[currentNode].Count > 0)
                {
                    // Push the current node onto the stack
                    stack.Push(currentNode);

                    // Get the next node
                    int nextNode = graph[currentNode].First();

                    // Mark the edge as visited
                    visitedEdges[currentNode].Add(nextNode);

                    // Remove the edge from the graph
                    graph[currentNode].Remove(nextNode);

                    // Move to the next node
                    currentNode = nextNode;
                }
                else
                {
                    // If there's no unvisited edge from the current node, backtrack
                    circuit.Add(currentNode);

                    // Pop the top node from the stack and set it as the current node
                    currentNode = stack.Pop();
                }
            }

            // Reverse the circuit to get the correct order
            circuit.Reverse();

            return circuit;
        }

        private protected override bool IsConnectedOrHasEulerianCircuit(AdjacenceList graph)
        {
            int inOutDegreeDiffCount = 0;

            // Count nodes with difference in-degree and out-degree
            foreach (var nodeEdges in graph.GetList)
            {
                int inDegree = 0, outDegree = nodeEdges.Value.Count;

                foreach (var edges in graph.GetList)
                {
                    if (edges.Value.Contains(nodeEdges.Key))
                        inDegree++;
                }

                if (inDegree != outDegree)
                    inOutDegreeDiffCount++;
            }

            // For a directed graph to have an Eulerian path, it must have at most one node with (out-degree - in-degree = 1) and one node with (in-degree - out-degree = 1),
            // and all other nodes must have equal in-degree and out-degree. For a circuit, all nodes must have equal in-degree and out-degree.
            if (inOutDegreeDiffCount == 0 || inOutDegreeDiffCount == 2)
                return true;
            else
                return false;
        }

        private override protected int FindStartNode(AdjacenceList graph)
        {
            foreach (var nodeEdges in graph.GetList)
            {
                int inDegree = 0, outDegree = nodeEdges.Value.Count;

                foreach (var edges in graph.GetList)
                {
                    if (edges.Value.Contains(nodeEdges.Key))
                        inDegree++;
                }

                if (outDegree - inDegree == 1)
                    return nodeEdges.Key;
            }

            return graph.GetList.Keys.First();
        }
    }

    public class EulerUndirected: Euler
    {
        public override List<int> FindEulerianPathCircuit(AdjacenceList list)
        {
            AdjacenceList graph = new AdjacenceList();


            foreach (var item in list.GetList)
            {
                graph.GetList.Add(item.Key, new HashSet<int>(item.Value));
            }
            if (graph.GetList.Count == 0) { return new List<int>(); }


            // If the graph is not connected or it has more than 2 nodes with odd degree, return null
            if (!IsConnectedOrHasEulerianCircuit(graph))
            {
                Console.WriteLine("Graph does not have an Eulerian path or circuit.");
                return null;
            }

            // Find the starting point for the Eulerian path or circuit
            int startNode = FindStartNode(graph);

            // Create a stack to store the path
            Stack<int> stack = new Stack<int>();
            List<int> circuit = new List<int>();

            // Create a dictionary to keep track of visited edges
            Dictionary<int, HashSet<int>> visitedEdges = new Dictionary<int, HashSet<int>>();

            foreach (var nodeEdges in graph.GetList)
            {
                visitedEdges[nodeEdges.Key] = new HashSet<int>();
            }

            // Push the starting node onto the stack
            stack.Push(startNode);
            int currentNode = startNode;

            while (stack.Count > 0)
            {
                // If there's an unvisited edge from the current node
                if (graph[currentNode].Count > 0)
                {
                    // Push the current node onto the stack
                    stack.Push(currentNode);

                    // Get the next node
                    int nextNode = graph[currentNode].First();

                    // Mark the edge as visited
                    visitedEdges[currentNode].Add(nextNode);
                    visitedEdges[nextNode].Add(currentNode);

                    // Remove the edge from the graph
                    graph[currentNode].Remove(nextNode);
                    graph[nextNode].Remove(currentNode);

                    // Move to the next node
                    currentNode = nextNode;
                }
                else
                {
                    // If there's no unvisited edge from the current node, backtrack
                    circuit.Add(currentNode);

                    // Pop the top node from the stack and set it as the current node
                    currentNode = stack.Pop();
                }
            }

            // Reverse the circuit to get the correct order
            circuit.Reverse();

            return circuit;
        }

        private protected override bool IsConnectedOrHasEulerianCircuit(AdjacenceList graph)
        {
            int oddDegreeCount = 0;

            // Count nodes with odd degree
            foreach (var nodeEdges in graph.GetList)
            {
                if (nodeEdges.Value.Count % 2 != 0)
                    oddDegreeCount++;
            }

            // For a graph to have an Eulerian path:
            // 1. If it's undirected, it must have exactly 0 or 2 nodes with odd degree.
            // 2. If it's directed, it must have exactly 0 or 2 nodes with out-degree - in-degree = 1,
            //    and 0 or 2 nodes with in-degree - out-degree = 1.
            if (oddDegreeCount == 0 || oddDegreeCount == 2)
                return true;
            else
                return false;
        }

        private protected override int FindStartNode(AdjacenceList graph)
        {
            // If there's a node with odd degree, start from it; otherwise start from any node
            foreach (var nodeEdges in graph.GetList)
            {
                if (nodeEdges.Value.Count % 2 != 0)
                    return nodeEdges.Key;
            }
            return graph.GetList.Keys.First();
        }
    }
}

