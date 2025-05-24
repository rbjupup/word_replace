using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BAI;
using Core;
using Language;

namespace Controls
{
    public class SunFlowerModel : AbstractNotifyPropertyBaseClass
    {
        public static SunFlowerModel Instance = new SunFlowerModel();
        public SunFlowerModel()
        {
            
        }
        private ResourceDictionary _m_resourcesDictionary; public ResourceDictionary m_resourcesDictionary { 
            get
            {
                if (_m_resourcesDictionary == null)
                    _m_resourcesDictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/Theme/Icons.xaml") }; 
                return _m_resourcesDictionary; 
            } 
            set { _m_resourcesDictionary = value; } }

        private object _CurrentView; public object CurrentView { get { return _CurrentView; } set { _CurrentView = value; OnPropertyChanged(nameof(CurrentView)); } }
        private SunFlowerBtnItem _selectItem; public SunFlowerBtnItem selectItem { get { return _selectItem; } set {
                if (_selectItem != value)
                {
                    // 清除旧值选中状态
                    if (_selectItem != null)
                        _selectItem.IsSelected = false;

                    _selectItem = value;

                    // 设置新值选中状态
                    if (_selectItem != null)
                    {
                        _selectItem.IsSelected = true;
                        CurrentView = _selectItem.page;
                    }
                }
                OnPropertyChanged(nameof(selectItem)); } }
        public BindingList<SunFlowerBtnItem> SunFlowerBtnItems { get; } = new BindingList<SunFlowerBtnItem>();

        public void AddPage(object page,string icoName,string pageName)
        {
            var item = new SunFlowerBtnItem(page, m_resourcesDictionary[icoName] as Geometry, pageName);
            SunFlowerBtnItems.Add(item);
            if (SunFlowerBtnItems.Count == 1)
            {
                selectItem = item;
            }
        }
        private ICommand _CloseCommand; public ICommand CloseCommand
        {
            get
            {
                return _CloseCommand ?? (_CloseCommand = new RelayCommand(() =>
                {
                    var result = MessageBox.Show(Application.Current.MainWindow,
                        LanguageProxy.GetLanguageWithDefault("SunFlowerModel_Sure_To_Exit_Application_Text", "是否关闭软件"),
                        LanguageProxy.GetLanguageWithDefault("SunFlowerModel_Warn_Text", "警告"),
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        Environment.Exit(0);
                    }
                }));
            }
        }

        private ICommand _MiniusCommand; public ICommand MiniusCommand
        {
            get
            {
                return _MiniusCommand ?? (_MiniusCommand = new RelayCommand(() =>
                {
                    App.Current.MainWindow.WindowState = WindowState.Minimized;

                }));
            }
        }

    }

    public class SunFlowerBtnItem:AbstractNotifyPropertyBaseClass
    {
        public SunFlowerBtnItem(object pageIn, Geometry icoNameIn,string pageNameIn)
        {
            page = pageIn;
            icoName = icoNameIn;
            pageName = pageNameIn;
        }
        private object _page; public object page { get { return _page; } set { _page = value; OnPropertyChanged(nameof(page)); } }

        private Geometry _icoName; public Geometry icoName { get { return _icoName; } set { _icoName = value; OnPropertyChanged(nameof(icoName)); } }

        private string _pageName; public string pageName { get { return _pageName; } set { _pageName = value; OnPropertyChanged(nameof(pageName)); } }

        private bool _IsSelected; public bool IsSelected { get { return _IsSelected; } set { _IsSelected = value; OnPropertyChanged(nameof(IsSelected)); } }
    }
}
