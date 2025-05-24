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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Controls
{
    /// <summary>
    /// SunFlowerTab.xaml 的交互逻辑
    /// </summary>
    public partial class SunFlowerTab : UserControl
    {
        public SunFlowerTab()
        {
            InitializeComponent();
            this.DataContext = SunFlowerModel.Instance;
        }
        internal void OnMaxWinSizeButtonClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null) return;
            window.WindowState = WindowState.Maximized;
        }
    }
}
