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
using Controls;
using Model;

namespace word_replace.View
{
    /// <summary>
    /// MainControl.xaml 的交互逻辑
    /// </summary>
    public partial class MainControl : UserControl
    {
        public MainControl()
        {
            InitializeComponent();
        }

        private void BtnAutoParse_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            WordsReplaceModel jobListModel = (WordsReplaceModel)this.DataContext;

            btn.Background = jobListModel.AutoTransProcess.IsAutoTrans == false ? System.Windows.Media.Brushes.Green : (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#222222");

        }

        private void OnDataGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (sender is DataGridRow row
                && row.DataContext is WordListItem layer)
            {
                var layerTmp = layer.Clone();
                var window = new WordItemEditor().CreateWindow();
                window.DataContext = layerTmp;
                window.Owner = Window.GetWindow(this);
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if (window.ShowDialog() == true)
                {
                    layerTmp.CloneTo(layer);
                    layer.DealFinish = false;
                }
            }
        }
    }
}
