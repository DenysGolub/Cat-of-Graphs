using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Interfaces
{
    internal interface IMatrix
    {
        public void AddEdge();
        public void AddNode();
        public void RemoveEdge();
        public void RemoveNode();
        public void To2DArray();


    }
}
