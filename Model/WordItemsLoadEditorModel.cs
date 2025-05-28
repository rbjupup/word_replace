using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using BAI;
using Core;
using Language;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;

namespace Model
{
    public class WordItemsLoadEditorModel : AbstractNotifyPropertyBaseClass
    {
        public WordItemsLoadEditorModel() { }

        public BindingList<WordListItem> LoadItems { get; } = new BindingList<WordListItem>();
        private string _FileName; public string FileName { get { return _FileName; } set { _FileName = value; OnPropertyChanged(nameof(FileName)); } }
        private int _StartCount; public int StartCount { get { return _StartCount; } set { _StartCount = value; OnPropertyChanged(nameof(StartCount)); } }
        private int _EndCount; public int EndCount { get { return _EndCount; } set { _EndCount = value; OnPropertyChanged(nameof(EndCount)); } }
        private ICommand _SelectFile; public ICommand SelectFile
        {
            get
            {
                return _SelectFile ?? (_SelectFile = new RelayCommand(() =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();

                    // 设置对话框属性
                    openFileDialog.Title = LanguageProxy.GetLanguageWithDefault("WordItemsLoadEditorModel_Select_File_Exist", "选择文件");
                    openFileDialog.Filter = " (*.xlsx)|*.xlsx| (*.*)|*.*";
                    //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    openFileDialog.Multiselect = false;

                    // 显示对话框并处理结果
                    if (openFileDialog.ShowDialog() == true) // WPF 中返回的是可空 bool 类型
                    {
                        FileName = openFileDialog.FileName;
                    }
                }));
            }
        }
        private ICommand _LoadData; public ICommand LoadData
        {
            get
            {
                return _LoadData ?? (_LoadData = new RelayCommand(() =>
                {
                    if(!File.Exists(this.FileName))
                    {
                        MessageBox.Show(LanguageProxy.GetLanguageWithDefault("WordItemsLoadEditorModel_File_Not_Exist", "文件不存在"));
                        return;
                    }
                    if(!(StartCount > 0 && EndCount > 0 && EndCount >= StartCount))
                    {
                        MessageBox.Show(LanguageProxy.GetLanguageWithDefault("WordItemsLoadEditorModel_Index_Error", "序号错误"));
                        return;
                    }
                    try
                    {
                        Excel.Application excelApp = new Excel.Application();
                        Excel.Workbook workbook = excelApp.Workbooks.Open(this.FileName);
                        try
                        {
                            Excel.Worksheet worksheet = workbook.Sheets[1];
                            Excel.Range range = worksheet.UsedRange;
                            var rows = range.Rows.Count;
                            for (int i = StartCount; i <= Math.Min(EndCount, rows); i++)
                            {
                                var wordItem = new WordListItem();
                                wordItem.FileName = Path.GetFileName(this.FileName);
                                wordItem.CreateDate = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");
                                string CompanyName_ZHCN = (range.Cells[i, 10] as Excel.Range)?.Value2?.ToString()?.Trim();
                                wordItem.CompanyName_ZHCN = CompanyName_ZHCN.Replace("Contract欧代协议-","");
                                string CompanyName = (range.Cells[i, 5] as Excel.Range)?.Value2?.ToString()?.Trim();
                                wordItem.CompanyName = CompanyName;
                                string ComapanyAddreess = (range.Cells[i, 6] as Excel.Range)?.Value2?.ToString()?.Trim();
                                wordItem.ComapanyAddreess = ComapanyAddreess;
                                string CompanyLegalPerson = (range.Cells[i, 9] as Excel.Range)?.Value2?.ToString()?.Trim();
                                wordItem.CompanyLegalPerson = CompanyLegalPerson;
                                string PhoneNumber = (range.Cells[i, 8] as Excel.Range)?.Value2?.ToString()?.Trim();
                                wordItem.PhoneNumber = PhoneNumber;
                                string EmailAddress = (range.Cells[i, 7] as Excel.Range)?.Value2?.ToString()?.Trim();
                                wordItem.EmailAddress = EmailAddress;
                                wordItem.StartTime = DateTime.FromOADate(int.Parse((range.Cells[i, 13] as Excel.Range)?.Value2?.ToString()?.Trim())).ToString("yyyy-MM-dd");
                                wordItem.StartTimeDMY = DateTime.FromOADate(int.Parse((range.Cells[i, 13] as Excel.Range)?.Value2?.ToString()?.Trim())).ToString("yyyy-MM-dd");
                                wordItem.EndTime = DateTime.FromOADate(int.Parse((range.Cells[i, 14] as Excel.Range)?.Value2?.ToString()?.Trim())).ToString("yyyy-MM-dd");
                                wordItem.DealFinish = false;
                                LoadItems.Add(wordItem);

                            }
                        }
                        catch
                        {
                            this.Error(LanguageProxy.GetLanguageWithDefault("WordItemsLoadEditorModel_Read_Data_Error", "数据读取错误"));
                        }
                        finally
                        {
                            this.Info("开始关闭文件");
                            workbook.Close();
                            excelApp.Quit();
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        Thread.Sleep(3000);
                    }
                }));
            }
        }
    }
}
