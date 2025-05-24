using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WindowStartupLocation = System.Windows.WindowStartupLocation;

namespace Controls
{
    public static class UIElementExtend
    {

        static UIElementExtend()
        {
        }

        public static Window CreateWindow(this UIElement element, bool isFullScreen = false, bool hideTools = false)
        {
            var window = new Window
            {
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = new SolidColorBrush(Colors.Transparent)
            };
            var content = new CommControl { Content = element, HideToolPanel = hideTools };
            window.Content = content;
            if (isFullScreen)
                content.OnMaxWinSizeButtonClick(null, null);
            window.Activate();
            return window;
        }

        public static Window CreateDefaultWindow(this UIElement element, bool isFullScreen = false, bool hideTools = false)
        {
            var window = new Window
            {
                AllowsTransparency = false,
                WindowStyle = WindowStyle.SingleBorderWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = new SolidColorBrush(Colors.Silver)
            };

            window.Content = element;
            window.Activate();
            window.InvalidateVisual();
            return window;
        }


        /// <summary>
        /// 查找应用程序的活动窗口
        /// </summary>
        public static Window GetActiveWindow(this Application application)
        {
            Window result = null;
            foreach (Window item in application.Windows)
            {
                if (item.IsActive) result = item;
            }

            if (result == null) result = application.MainWindow;

            return result;
        }

    }
}