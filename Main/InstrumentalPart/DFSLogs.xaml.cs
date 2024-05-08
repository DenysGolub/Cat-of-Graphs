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

namespace Main.InstrumentalPart
{
    /// <summary>
    /// Interaction logic for DFSLogs.xaml
    /// </summary>
    public partial class DFSLogs : Window
    {
        string str;
        public string Text { get { return str; } set { tb.Text = value; str = value; tb.ScrollToEnd(); } }
        public DFSLogs()
        {
            InitializeComponent();
        }
    }
}
