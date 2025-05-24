

using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using Core;
using Language;
using word_replace.View;
using Controls;
using BAI;


namespace Model
{
    public class WordListModel : AbstractNotifyPropertyBaseClass
    {
        public WordListModel(WordsReplaceModel wrControl) 
        {
            WordReplaceController = wrControl;
        }
        public WordsReplaceModel WordReplaceController { get; }
        public BindingList<WordListItem> WaitForDealFiles { get; } = new BindingList<WordListItem>();

        private string _TodayIndex; public string TodayIndex { get { return _TodayIndex; } set { _TodayIndex = value; OnPropertyChanged(nameof(TodayIndex)); } }
        private ICommand _AutoTransCommand; public ICommand AutoTransCommand    
        {
            get
            {
                return _AutoTransCommand ?? (_AutoTransCommand = new RelayCommand(() =>
                {
                    WordReplaceController.AutoTransProcess.IsAutoTrans = !WordReplaceController.AutoTransProcess.IsAutoTrans;
                }));
            }
        }

        private ICommand _AddFileManual; public ICommand AddFileManual
        {
            get
            {
                return _AddFileManual ?? (_AddFileManual = new RelayCommand(() =>
                {
                    var loginWindow = new WordItemEditor();
                    var window = loginWindow.CreateWindow(false, false);
                    if (window.ShowDialog() != true)
                    {
                        return;
                    }
                }));
            }
        }

        private ICommand _OpenFloder; public ICommand OpenFloder
        {
            get
            {
                return _OpenFloder ?? (_OpenFloder = new RelayCommand(() =>
                {
                    var m_tempfileDir = ConfigurationManager.AppSettings["AutoTransTempFolder"].Replace('/','\\');
                    if (!Directory.Exists(m_tempfileDir))
                    {
                        Directory.CreateDirectory(m_tempfileDir);
                    }
                    Process.Start("explorer.exe", m_tempfileDir);
                }));
            }
        }

        private ICommand _RemoveCommand; public ICommand RemoveCommand
        {
            get
            {
                return _RemoveCommand ?? (_RemoveCommand = new RelayCommand<WordListItem>((WordListItem layerItem) =>
                {
                    if (MessageBox.Show(LanguageProxy.GetLanguageWithDefault("WordListModel_Are_You_Sure_To_Delete_Files_Text","是否删除文件:{0}", layerItem.FileName),
                        LanguageProxy.GetLanguageWithDefault("WordListModel_Warn_Text","警告"), MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    {
                        return;
                    }

                    try
                    {
                        if (WaitForDealFiles.Contains(layerItem))
                        {
                            WaitForDealFiles.Remove(layerItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"RemoveJob {layerItem.FileName} Exception: {ex.Message}");
                    }

                }));
            }
        }

        private ICommand _RunCommand; public ICommand RunCommand
        {
            get
            {
                return _RunCommand ?? (_RunCommand = new RelayCommand<WordListItem> ((WordListItem wordItem) =>
                {
                    wordItem.DealFinish = false;
                    var m_toolsFileDir = ConfigurationManager.AppSettings["AutoTransToolFolder"];
                    wordItem.SaveToFile(m_toolsFileDir); ;
                    App.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        //等待转换器退出
                        while (true)
                        {
                            Process[] processes = Process.GetProcessesByName("文字替换软件定制版");
                            if (processes.Length > 0)
                            {
                                Thread.Sleep(500);
                            }
                            else
                            { break; }
                        }
                        Process.Start(m_toolsFileDir + "/文字替换软件定制版.exe");
                        //等待转换器退出
                        while (true)
                        {
                            Process[] processes = Process.GetProcessesByName("文字替换软件定制版");
                            if (processes.Length > 0)
                            {
                                Thread.Sleep(500);
                            }
                            else
                            { break; }
                        }
                        wordItem.DealFinish = true;
                    }));

                }));
            }
        }

    }
}