using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Main.TestingPart
{
    public class CustomDataGridTemplateColumn : DataGridTemplateColumn
    {
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            FrameworkElement fe = base.GenerateElement(cell, dataItem);
            if (fe is TextBlock textBlock)
            {
                textBlock.SetBinding(TextBlock.TagProperty, new Binding(TagPropertyName));  //use TagProperty here
            }
            return fe;
        }

        public string TagPropertyName { get; set; }
    }

    public class CustomDataGridTextColumn : DataGridTextColumn
    {
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            FrameworkElement fe = base.GenerateElement(cell, dataItem);
            if (fe is TextBlock textBlock)
            {
                textBlock.SetBinding(TextBlock.TagProperty, new Binding(TagPropertyName));  //use TagProperty here
            }
            return fe;
        }

        public string TagPropertyName { get; set; }
    }
}
