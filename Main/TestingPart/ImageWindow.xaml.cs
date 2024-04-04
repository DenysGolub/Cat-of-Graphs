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

namespace Main.TestingPart
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        public ImageWindow()
        {
            InitializeComponent();
        }

        public ImageWindow(ImageSource img)
        {
            InitializeComponent();
            Graph.Source = img;
            if(img != null)
            {
                Width = img.Width; Height = img.Height+15;
            }
        }

        private void Graph_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            Width = Graph.Source.Width; Height = Graph.Source.Height + 15;
        }
    }
}
