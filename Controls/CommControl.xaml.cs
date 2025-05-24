using System;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Controls
{
    /// <summary>
    ///     CommControl.xaml 的交互逻辑
    /// </summary>
    public partial class CommControl : UserControl
    {
        private static readonly ResourceDictionary m_resourcesDictionary;
        private int m_clickCount;

        static CommControl()
        {
        }


        public CommControl()
        {
            InitializeComponent();
        }

        public new object Content
        {
            get { return ContentPressenter.Content; }
            set { ContentPressenter.Content = value; }
        }

        public bool HideToolPanel
        {
            get { return ButtonPanel.Visibility != Visibility.Visible; }
            set { ButtonPanel.Visibility = value ? Visibility.Collapsed : Visibility.Visible; }
        }


        private void OnBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null) return;
            m_clickCount += 1;

            var timer = new DispatcherTimer();

            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);

            timer.Tick += (s, e1) => {
                timer.IsEnabled = false;
                m_clickCount = 0;
            };

            timer.IsEnabled = true;

            if (m_clickCount % 2 == 0)
            {
                timer.IsEnabled = false;

                m_clickCount = 0;

                switch (window.WindowState)
                {
                    case WindowState.Maximized:
                        OnNormalWinButtonClick(null, null);
                        break;
                    case WindowState.Normal:
                        OnMaxWinSizeButtonClick(null, null);
                        break;
                }
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                window.DragMove();
            }
        }

        private void OnMiniWinButtonClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null) return;
            window.WindowState = WindowState.Minimized;
        }

        internal void OnMaxWinSizeButtonClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null) return;
            window.WindowState = WindowState.Maximized;
            BtnMaxWin.Visibility = Visibility.Collapsed;
            BtnNormalWin.Visibility = Visibility.Visible;
        }

        private void OnNormalWinButtonClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null) return;
            window.WindowState = WindowState.Normal;
            BtnMaxWin.Visibility = Visibility.Visible;
            BtnNormalWin.Visibility = Visibility.Collapsed;
        }

        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null) return;
            //window.DialogResult = false;
            window.Close();
        }
    }
}