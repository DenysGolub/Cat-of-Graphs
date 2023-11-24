using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Main.Interfaces
{
    internal interface ICanvasEvents
    {
        void PreviewMouseMove(object sender, RoutedEventArgs e);
    }
}
