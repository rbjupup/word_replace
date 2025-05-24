using System;
using System.Data;
using System.Globalization;
using System.IO;
using Core;

namespace Model
{
    public class WordListItem : AbstractNotifyPropertyBaseClass
    {
        public WordChangeRules rules { get; set; }
        public WordListItem() 
        {
            rules = new WordChangeRules();
        }
        private string _FileName; public string FileName { get { return _FileName; } set { _FileName = value; OnPropertyChanged(nameof(FileName)); } }
        private string _CreateDate; public string CreateDate { get { return _CreateDate; } set { _CreateDate = value; OnPropertyChanged(nameof(CreateDate)); } }
        private bool _DealFinish; public bool DealFinish { get { return _DealFinish; } set { _DealFinish = value; OnPropertyChanged(nameof(DealFinish)); } }
        private string _PhoneNumber; public string PhoneNumber { get { return _PhoneNumber; } set { _PhoneNumber = value; rules.Replace_TelePhone = value; OnPropertyChanged(nameof(PhoneNumber)); } }
        private string _EmailAddress; public string EmailAddress { get { return _EmailAddress; } set { _EmailAddress = value; rules.Replace_EMail = value; OnPropertyChanged(nameof(EmailAddress)); } }
        private string _ComapanyAddreess; public string ComapanyAddreess { get { return _ComapanyAddreess; } set { _ComapanyAddreess = value; rules.Replace_CompanyAddress = value; OnPropertyChanged(nameof(ComapanyAddreess)); } }
        private string _CompanyName; public string CompanyName { get { return _CompanyName; } set { _CompanyName = value; rules.Replace_CompanyName = value; OnPropertyChanged(nameof(CompanyName)); } }
        private string _CompanyName_ZHCN; public string CompanyName_ZHCN { get { return _CompanyName_ZHCN; } set { _CompanyName_ZHCN = value; rules.Replace_CompanyName_ZHCN = value;OnPropertyChanged(nameof(CompanyName_ZHCN)); } }
        private string _CompanyLegalPerson; public string CompanyLegalPerson { get { return _CompanyLegalPerson; } set { _CompanyLegalPerson = value; rules.Replace_LegalPersonName = value; OnPropertyChanged(nameof(CompanyLegalPerson)); } }
        private string _StartTime; public string StartTime { get { return _StartTime; } set { 
                _StartTime = value;
                DateTime dateTime = DateTime.Now;
                DateTime.TryParse(value, out dateTime);
                rules.ReplaceStartTime = dateTime.ToString("yyyy-MM-dd");
                OnPropertyChanged(nameof(StartTime));
            } }
        private string _StartTimeDMY; public string StartTimeDMY { get { return _StartTimeDMY; } set { 
                _StartTimeDMY = value;
                DateTime dateTime = DateTime.Now;
                DateTime.TryParse(value, out dateTime);
                rules.Replace_StartDMY = dateTime.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                OnPropertyChanged(nameof(StartTimeDMY)); 
            } }
        private string _EndTime; public string EndTime { get { return _EndTime; } set { 
                _EndTime = value;
                DateTime dateTime = DateTime.Now;
                DateTime.TryParse(value,out dateTime);
                rules.ReplaceEndTime = dateTime.ToString("yyyy-MM-dd");
                OnPropertyChanged(nameof(EndTime));
            } }
        public void SaveToFile(string toolsDir)
        {
            rules.Replace_CompanyAddress = rules.Replace_CompanyAddress.Replace(' ', ' ');
            string jsonSave = rules.SerializeToJson();
            File.WriteAllText(toolsDir+ "/replacements.json", jsonSave);
        }
        public WordListItem Clone()
        {
            WordListItem wordListItem = new WordListItem();
            CloneTo(wordListItem);
            return wordListItem;
        }
        public void CloneTo(WordListItem wordListItem)
        {
            wordListItem.FileName = this.FileName;
            wordListItem.CreateDate = this.CreateDate;
            wordListItem.DealFinish = this.DealFinish;
            wordListItem.PhoneNumber = this.PhoneNumber;
            wordListItem.EmailAddress = this.EmailAddress;
            wordListItem.ComapanyAddreess = this.ComapanyAddreess;
            wordListItem.CompanyName = this.CompanyName;
            wordListItem.CompanyName_ZHCN = this.CompanyName_ZHCN;
            wordListItem.CompanyLegalPerson = this.CompanyLegalPerson;
        }
    }
}