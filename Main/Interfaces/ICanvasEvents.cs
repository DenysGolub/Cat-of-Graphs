using Main.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace Main.Interfaces
{
    interface ICanvasEvents
    {
        void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e);
        void PreviewMouseMove(object sender, MouseEventArgs e);
        void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e);
        Canvas Canvas { get; set; }
        AdjacenceList AdjacenceList { get; set; }
        ColorPicker ColorPicker { get; set; }
    }
}
