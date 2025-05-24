using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Language
{

    /// <summary>
    ///     使用方式：
    ///     1、引入命名空间 xmlns:localString="clr-namespace:....;assembly=..."
    ///     2、在需要本地化的属性调用StringResource：TargetProperty="{localString:SRT key,DefaultValue=defaultValue}"
    ///     例:<TextBlock Text="{localString:StringResource test1,DefaultValue=测试一}"></TextBlock>
    /// </summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class SRT : MarkupExtension, INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs
            ValueChangedEventArgs = new PropertyChangedEventArgs("Value");

        private string _value;

        public SRT(string key)
            : this()
        {
            Key = key;
        }


        public SRT(string key, string DefaultValue)
            : this()
        {
            Key = key;
            this.DefaultValue = DefaultValue;
        }

        public SRT()
        {
            //TODO:  如需要在运行时更改界面语言 请在此注册系统文化集更改事件 并调用 NotifyValueChanged（）;
            //SystemCulture.UICultureChanged += SystemCulture_UICultureChanged;
        }


        /// <summary>
        ///     是否在设计模式下
        /// </summary>
        private bool DesignMode
        {
            get
            {
                return (bool)DesignerProperties.IsInDesignModeProperty
                    .GetMetadata(typeof(DependencyObject)).DefaultValue;
            }
        }

        /// <summary>
        ///     本地化字符串所对应的Key
        /// </summary>
        [ConstructorArgument("key")]
        public string Key { get; set; }

        /// <summary>
        ///     默认值，为了使在设计器的情况时把默认值绑到设计器
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        ///     资源的具体内容，通过资源名称也就是上面的Key找到对应内容
        /// </summary>
        public string Value
        {
            get
            {
                if (DesignMode)
                {
                    if (DefaultValue != null) return DefaultValue;
                    if (Key != null) return Key;
                    return string.Empty;
                }
                if (Key != null)
                {
                    string strResault = null;
                    try
                    {
                        strResault = LanguageProxy.GetLanguageWithDefault(Key, DefaultValue);
                    }
                    catch (Exception ex)
                    {
                        // ignored
                        Console.WriteLine(ex);
                    }
                    if (string.IsNullOrEmpty(strResault))
                    {
                        strResault = DefaultValue;
                    }
                    return strResault;
                }
                return _value;
            }
            set { _value = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string GetString(string key)
        {
            try
            {
                return LanguageProxy.GetLanguage(key);
            }
            catch (Exception)
            {
                return key;
            }
        }

        /// <summary>
        ///     每一标记扩展实现的 ProvideValue 方法能在可提供上下文的运行时使用 IServiceProvider。然后会查询此 IServiceProvider 以获取传递信息的特定服务
        ///     当 XAML 处理器在处理一个类型节点和成员值，且该成员值是标记扩展时，它将调用该标记扩展的 ProvideValue 方法并将结果写入到对象关系图或序列化流,XAML 对象编写器将服务环境通过 serviceProvider
        ///     参数传递到每个此类实现。
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (target == null) return null;

            var setter = target.TargetObject as Setter;
            if (setter != null)
            {
                return new Binding("Value") { Source = this, Mode = BindingMode.OneWay };
            }
            var binding = new Binding("Value") { Source = this, Mode = BindingMode.OneWay };
            return binding.ProvideValue(serviceProvider);
        }

        public void NotifyValueChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, ValueChangedEventArgs);
        }
    }

    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Delegate | AttributeTargets.Parameter | AttributeTargets.Interface | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Module | AttributeTargets.Assembly)]
    public sealed class DescriptionExAttribute : DescriptionAttribute
    {
        private readonly string _defaultValue;

        public DescriptionExAttribute(string description, string defaultValue = null)
            : base(description)
        {
            _defaultValue = string.IsNullOrEmpty(defaultValue) ? description : defaultValue;
        }

        public override string Description
        {
            get
            {
                return LanguageProxy.GetLanguageWithDefault(base.Description, _defaultValue);
            }
        }
    }


    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Delegate | AttributeTargets.Parameter | AttributeTargets.Interface | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Module | AttributeTargets.Assembly)]
    public sealed class CategoryExAttribute : CategoryAttribute
    {
        public CategoryExAttribute(string category)
            : base(category)
        {
        }

        //public new string Category
        //{
        //    get
        //    {
        //        return LanguageProxy.GetLanguage(base.Category);
        //    }
        //}

        protected override string GetLocalizedString(string value)
        {
            return LanguageProxy.GetLanguage(this.Category);
        }
    }

    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Delegate | AttributeTargets.Parameter | AttributeTargets.Interface | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Module | AttributeTargets.Assembly)]
    public sealed class DisplayNameExAttribute : DisplayNameAttribute
    {
        private readonly string _prefixDescription = string.Empty;

        public DisplayNameExAttribute(string displayName)
            : base(displayName)
        {
        }

        public DisplayNameExAttribute(string prefixDescription, string description) : base(description)
        {
            _prefixDescription = prefixDescription;
        }

        public override string DisplayName
        {
            get
            {
                return _prefixDescription + LanguageProxy.GetLanguage(base.DisplayName);
            }
        }
    }
}
