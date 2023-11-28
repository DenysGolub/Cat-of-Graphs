using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Main.Interfaces
{
    /// <summary>
    /// Interface for exceptions that may appear during runtime
    /// </summary>
    internal interface IExceptions
    {
        void MatrixEmpty();
        void WrongMatrixValue();
    }
}
