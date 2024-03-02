using Infralution.Localization.Wpf;
using Main.Classes;
using Main.Enumerators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
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
        private string culture = "";
        private string _oper =  "";
        public string Operation { get { return _oper; } }
        public OperationResult()
        {
            InitializeComponent();
            Closed += (sender, e) => WindowsInstances.WindowClosed(sender, e);

            DataContext = this;
            WindowsInstances.MainWindowInst.CultureChanged += SetCurrentCulture;


        }

        public void SetCurrentCulture(string code)
        {
            if (code == "en-US")
            {
                _oper += "ENGLISH";
            }
            else if(code == "uk-UA")
            {
                _oper = _oper.Replace("ENGLISH", "");
            }
            UpdateBinding();
        }
        public void SetOperation(CurrentGraphOperation operation)
        {
            switch (operation)
            {
                case CurrentGraphOperation.Unity:
                    _oper = "UnityTitle";
                    break;
                case CurrentGraphOperation.CircleSum:
                    _oper = "CircleSumTitle";
                    break;
                case CurrentGraphOperation.Intersection:
                    _oper = "IntersectionTitle";
                    break;
                case CurrentGraphOperation.CartesianProduct:
                    _oper = "CartesianProductTitle";
                    break;
                default:
                    break;
            }

            UpdateBinding();


        }

        private void UpdateBinding()
        {

            // Create the resource manager.
            Assembly assembly = this.GetType().Assembly;

            //ResFile.Strings -> <Namespace>.<ResourceFileName i.e. Strings.resx> 
            var resman = new ResourceManager("Main.Localization.OperationResult", assembly);

            Binding b = new Binding();
            b.Source = resman;
            Title = resman.GetString(_oper);

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

            Height = c.Height;
            Width = c.Width;
        }
    }
}
