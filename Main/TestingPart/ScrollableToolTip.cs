using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Main.TestingPart
{
    public class ScrollableToolTip:ToolTip
    {
        public ScrollableToolTip()
        {
            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            scrollViewer.Content = Content;
            Content = scrollViewer;
        }

        protected override void OnOpened(RoutedEventArgs e)
        {
            base.OnOpened(e);
            MouseWheel += ScrollableToolTip_MouseWheel;
        }

        protected override void OnClosed(RoutedEventArgs e)
        {
            base.OnClosed(e);
            MouseWheel -= ScrollableToolTip_MouseWheel;
        }

        private void ScrollableToolTip_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)((ScrollableToolTip)sender).Content;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
