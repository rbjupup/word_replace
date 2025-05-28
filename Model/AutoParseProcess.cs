
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows;
using BAI;
using Core;
using Language;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;


namespace Model
{
    public class AutoTransProcess : AbstractNotifyPropertyBaseClass
    {
        private bool m_isCopying = false;
        private string m_tempfileDir = "";
        private string m_serverFileDir = "";
        private string m_toolsFileDir = "";
        private string m_copyFileDir = "";
        private string m_logsDir = "";
        private string m_errorFileDir = "";
        private string m_resultDir = "";
        private double m_parseOverTime = 1;
        private WordListModel m_wordListModel;
        public AutoTransProcess(WordsReplaceModel wrControl)
        {
            m_tempfileDir = ConfigurationManager.AppSettings["AutoTransTempFolder"];
            m_serverFileDir = ConfigurationManager.AppSettings["AutoTransServiceFolder"];
            m_copyFileDir = ConfigurationManager.AppSettings["AutoTransCopyFolder"];
            m_errorFileDir = ConfigurationManager.AppSettings["AutoTransErrorFolder"];
            m_toolsFileDir = ConfigurationManager.AppSettings["AutoTransToolFolder"];
            m_logsDir = ConfigurationManager.AppSettings["AutoTransLogFolder"];
            m_resultDir = ConfigurationManager.AppSettings["AutoTransResultFolder"];
            m_parseOverTime = double.Parse(ConfigurationManager.AppSettings["AutoTransOverTime"]);
            if (!Directory.Exists(m_tempfileDir))
            {
                Directory.CreateDirectory(m_tempfileDir);
            }
            if (!Directory.Exists(m_copyFileDir))
            {
                Directory.CreateDirectory(m_copyFileDir);
            }
            if (!Directory.Exists(m_errorFileDir))
            {
                Directory.CreateDirectory(m_errorFileDir);
            }
            if (!Directory.Exists(m_toolsFileDir))
            {
                Directory.CreateDirectory(m_toolsFileDir);
            }
            if (!Directory.Exists(m_logsDir))
            {
                Directory.CreateDirectory(m_logsDir);
            }
            if (!Directory.Exists(m_resultDir))
            {
                Directory.CreateDirectory(m_resultDir);
            }
            m_wordListModel = wrControl.WordListModel;
        }
        private bool _IsAutoTrans; public bool IsAutoTrans { get { return _IsAutoTrans; } set {
                if (_IsAutoTrans == value)
                {
                    return;
                }
                _IsAutoTrans = value;
                if (_IsAutoTrans)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(StartAutoParse));
                    ThreadPool.QueueUserWorkItem(new WaitCallback(StartAutoCopy));
                }
            }
        }

        public void StartAutoParse(object o)//开始自动料号
        {
            while (_IsAutoTrans)
            {
                Thread.Sleep(1000);
                if (m_isCopying)
                {
                    continue;
                }
                var fileList = Directory.GetFiles(m_tempfileDir).ToList();
                if (fileList.Count > 0)
                {
                    var filePath = fileList[0];
                    var wordItem = new WordListItem();
                    try
                    {
                        Excel.Application excelApp = new Excel.Application();
                        Excel.Workbook workbook = excelApp.Workbooks.Open(filePath);
                        this.Info("打开了文夹");
                        try
                        {
                            Excel.Worksheet worksheet = workbook.Sheets[1];
                            Excel.Range range = worksheet.UsedRange;
                            wordItem.FileName = Path.GetFileName(filePath);
                            wordItem.CreateDate = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");
                            string CompanyName_ZHCN = (range.Cells[3, 2] as Excel.Range)?.Value2?.ToString()?.Trim();
                            wordItem.CompanyName_ZHCN = m_wordListModel.TodayIndex + ' ' + CompanyName_ZHCN;
                            string CompanyName = (range.Cells[4, 2] as Excel.Range)?.Value2?.ToString()?.Trim();
                            wordItem.CompanyName = CompanyName;
                            string ComapanyAddreess = (range.Cells[6, 2] as Excel.Range)?.Value2?.ToString()?.Trim();
                            wordItem.ComapanyAddreess = ComapanyAddreess;
                            string CompanyLegalPerson = (range.Cells[10, 2] as Excel.Range)?.Value2?.ToString()?.Trim();
                            wordItem.CompanyLegalPerson = CompanyLegalPerson;
                            string PhoneNumber = (range.Cells[11, 2] as Excel.Range)?.Value2?.ToString()?.Trim();
                            wordItem.PhoneNumber = PhoneNumber;
                            string EmailAddress = (range.Cells[13, 2] as Excel.Range)?.Value2?.ToString()?.Trim();
                            wordItem.EmailAddress = EmailAddress;
                            wordItem.StartTime = DateTime.Now.ToString("yyyy/MM/dd");
                            wordItem.StartTimeDMY = DateTime.Now.AddYears(1).ToString("yyyy/MM/dd");
                            wordItem.EndTime = DateTime.Now.ToString("yyyy/MM/dd");
                        }
                        catch
                        {
                            this.Error(LanguageProxy.GetLanguageWithDefault("AutoParseProcess_Err_readData", "数据读取错误"));
                        }
                        finally
                        {
                            this.Info("开始关闭文件");
                            workbook.Close();
                            excelApp.Quit();
                        }
                        string fileCopy = m_copyFileDir + @"\" + Path.GetFileName(filePath);
                        this.Info("开始复制文件");
                        File.Copy(filePath, fileCopy, true);
                        wordItem.DealFinish = false;
                        this.Info("开始序列化到文件");
                        wordItem.SaveToFile(m_toolsFileDir);
                        //等待转换器退出
                        while(true)
                        {
                            //var processes = Process.GetProcesses();
                            Process[] processes = Process.GetProcessesByName("文字替换软件定制版");
                            if (processes.Length > 0)
                            {
                                Thread.Sleep(500);
                            }
                            else
                                { break; }
                        }
                        Process.Start(m_toolsFileDir + "/文字替换软件定制版.exe");
                        App.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                        {
                            m_wordListModel.WaitForDealFiles.Insert(0, wordItem);
                        }));
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
                    }
                    catch (Exception e)
                    {
                        string errrorFilePath = m_errorFileDir + @"\" + Path.GetFileName(filePath);
                        File.Copy(filePath, errrorFilePath, true);
                        MessageBox.Show(e.Message);
                        Thread.Sleep(3000);
                    }
                    File.Delete(filePath);
                }
            }
        }
        public void StartAutoCopy(object o)
        {
            while (_IsAutoTrans && m_serverFileDir != "")
            {
                try
                {
                    string[] files = Directory.GetFiles(m_serverFileDir);
                    if (files.Length > 0)
                    {
                        foreach (var file in files)
                        {
                            if (!_IsAutoTrans)
                            {
                                break;
                            }
                            m_isCopying = true;
                            string fileDestin = m_tempfileDir + @"\" + Path.GetFileName(file);
                            File.Copy(file, fileDestin, true);
                            File.Delete(file);
                            Thread.Sleep(500);
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(m_serverFileDir + " error:\r\n" + e.Message);
                    return;
                }
                finally
                {
                    m_isCopying = false;
                    Thread.Sleep(200);
                }
            }
        }
    }
}