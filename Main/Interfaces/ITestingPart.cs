using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Main.Interfaces
{
    public interface ITestingPart
    {
        /*void CreateQuestion();
        bool BuildGraph { get; set; }
        bool BuildAdjacenceMatrix { get; set; }
        bool BuildIncidenceMatrix { get; set; }*/


        public string Description { get; set; }
        public Canvas GraphImage { get; set; }
    }
}
