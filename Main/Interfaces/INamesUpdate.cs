using Main.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Main.Interfaces
{
    interface INamesUpdate
    {
        void UpdateNodes(AdjacenceList adj, int node);
        void UpdateCanvas(ref Canvas canvas, int node);
    }
}
