using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

//添加命令绑定
//private ICommand _newCommand;public ICommand NewCommand{get{return _newCommand ?? (_newCommand = new RelayCommand(() =>{
        
//}));}}
namespace Core
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Predicate<T> m_canExecute;
        private readonly Action<T> m_execute;

        /// <summary>
        /// 创建新命令
        /// </summary>
        /// <param name="execute">执行逻辑</param>
        public RelayCommand(Action<T> execute)
          : this(execute, null) { }

        /// <summary>
        /// 创建新命令
        /// </summary>
        /// <param name="execute">执行逻辑</param>
        /// <param name="canExecute">执行状态.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            m_execute = execute;
            m_canExecute = canExecute;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return m_canExecute == null ? true : m_canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (m_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (m_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public void Execute(object parameter)
        {
            m_execute((T)parameter);
        }

        #endregion
    }

    /// <summary>
    /// 该命令的目的只是函数的中继作用，其他对象调用通过委托调用函数功能。
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields

        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// 创建新命令
        /// </summary>
        /// <param name="execute">执行逻辑</param>
        public RelayCommand(Action execute)
          : this(execute, null) { }

        /// <summary>
        /// 创建新命令
        /// </summary>
        /// <param name="execute">执行逻辑</param>
        /// <param name="canExecute">执行状态.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        #endregion
    }
}
