using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Interfaces
{
    internal interface IAdjacenceList
    {
        public void AddEdge(int first_node, int second_node);
        public void AddNode(int node);
        public void RemoveEdge(int first_node, int second_node);
        public void RemoveNode(int node);
    }
}
