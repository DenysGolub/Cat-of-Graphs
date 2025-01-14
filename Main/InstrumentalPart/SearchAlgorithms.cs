﻿using Main.Classes;
using Main.Enumerators;
using Main.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Main.InstrumentalPart
{
    public class SearchAlgorithms
    {
        static async Task SetAnimation(Canvas graph, int vertex)
        {
            Ellipse node = graph.Children.OfType<Ellipse>().FirstOrDefault(l => l.Name == $"Ellipse_{vertex}");

            Storyboard sb = new Storyboard();

            ColorAnimation da = new ColorAnimation(((SolidColorBrush)node.Fill).Color, (Color)(ColorConverter.ConvertFromString("Lime")), new TimeSpan(0, 0, 1));

            PropertyPath colorTargetPath = new PropertyPath("(0).(1)", Ellipse.FillProperty, SolidColorBrush.ColorProperty);

            Storyboard.SetTarget(da, node);
            Storyboard.SetTargetProperty(da, colorTargetPath);

            sb.Children.Add(da);
            sb.Duration = TimeSpan.FromSeconds(1);
            sb.FillBehavior = FillBehavior.HoldEnd;
            sb.AutoReverse = true;

            node.BeginStoryboard(sb);

            //node.Fill = Brushes.Red;
            graph.InvalidateVisual();
            await Task.Delay(1000); // Adjust the delay duration as needed

        }

        static public async Task DFS(AdjacenceList list, Canvas graph, int startVertex, Dictionary<int, int> visited)
        {
            Stack<int> stack = new Stack<int>();
            stack.Push(startVertex);
            DFSLogs dFSLogs = Application.Current.Windows.OfType<DFSLogs>().SingleOrDefault();
            while (stack.Count > 0)
            {
                int vertex = stack.Pop();

                if (visited[vertex] == 0)
                {
                    dFSLogs.Text += $"DFS({vertex})\n";
                    visited[vertex] = 1;
                    await SetAnimation(graph, vertex); 

                    // Get all adjacent vertices of the popped vertex s
                    // If an adjacent has not been visited, then push it
                    // to the stack.
                    foreach (int adjacentVertex in list[vertex])
                    {
                        if (visited[adjacentVertex] == 0)
                        {
                            stack.Push(adjacentVertex);
                        }
                        else
                        {
                            dFSLogs.Text += $"{adjacentVertex} відвідана\n";
                        }
                    }
                }
            }
        }


        static public bool IsCycleExistInDirectedGraph(AdjacenceList list, int startVertex, HashSet<int> visited_gray, HashSet<int> visited_black, bool IsCyclic=false)
        {

            if(IsCyclic)
            {
                return IsCyclic;
            }
            

            visited_gray.Add(startVertex);
            foreach(var  adjacentVertex in list[startVertex])
            {
                if(visited_gray.Contains(adjacentVertex))
                {
                    IsCyclic = true;
                    return IsCyclic;
                }
                else
                {
                    IsCyclic = IsCycleExistInDirectedGraph(list, adjacentVertex, visited_gray, visited_black, IsCyclic);
                }

            }
            visited_gray.Remove(startVertex);

            visited_black.Add(startVertex);


            return IsCyclic;

        }

        public static bool IsCycleExistInUndirectedGraph(AdjacenceList list, int vertex, int parent, HashSet<int> visited, ref bool isCyclic)
        {
            visited.Add(vertex);

            foreach (var adjacentVertex in list[vertex])
            {
                if (!visited.Contains(adjacentVertex))
                {
                    if (IsCycleExistInUndirectedGraph(list, adjacentVertex, vertex, visited, ref isCyclic))
                        return true;
                }
                else if (adjacentVertex != parent)
                {
                    isCyclic = true;
                    return true;
                }
            }

            return false;
        }

        static public void BFS(AdjacenceList list, Canvas graph)
        {

        }

        private protected virtual void DepthFirstSearch(AdjacenceList adj, int v, bool[] visited, ref string comp)
        {
            
        }

    }
}
