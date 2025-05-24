using System;
using System.ComponentModel;
using System.Windows.Input;

//添加变量
//private int _paramName;public int ParamName{get { return _paramName; }set{_paramName = value;OnPropertyChanged(nameof(ParamName));}}

namespace Core
{
    [Serializable]
    public abstract class AbstractNotifyPropertyBaseClass : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null) return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
