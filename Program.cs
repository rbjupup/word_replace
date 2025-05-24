using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Language;
using System.Windows;
using word_replace.View;
using word_replace;
using Controls;
using System.Windows.Media;
using System.Xml.Linq;
using System.Threading;
using Model;

namespace BAI
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new App();
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CN");
            //创建主程序窗口
            var window = new Window
            {
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = new SolidColorBrush(Colors.Transparent)
            };
            var content = new SunFlowerTab();
            window.Content = content;
            SunFlowerModel.Instance.AddPage(new MainControl() {DataContext=new WordsReplaceModel() }, "Home", "主界面");
            SunFlowerModel.Instance.AddPage(new ParamSet(), "Wave", "参数设置");
            content.OnMaxWinSizeButtonClick(null, null);
            window.Activate();
            app.Run(window);

        }
    }
}
