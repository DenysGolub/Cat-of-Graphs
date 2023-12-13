using Main.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Main.Windows
{
    /// <summary>
    /// Interaction logic for OperationResult.xaml
    /// </summary>
    public partial class OperationResult : Window
    {
        public OperationResult()
        {
            InitializeComponent();
            Closed += (sender, e) => WindowsInstances.WindowClosed(sender, e);
        }

        public void SetCanvas(Canvas canvas)
        {
            DisplayingData.Children.Clear();
            Canvas c = canvas;
            while (c.Children.Count > 0)
            {
                UIElement obj = c.Children[0];
                c.Children.Remove(obj);
                DisplayingData.Children.Add(obj);
            }
            DisplayingData.InvalidateVisual();
        }
    }
}
