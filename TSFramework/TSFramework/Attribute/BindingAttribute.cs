using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;
using TSFrame.UI;

namespace TSFrame
{
    /// <summary>
    /// 自动注册
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class BindingAttribute : Attribute
    {
        public BindingMode Mode { get; private set; }
        public BindingAttribute(BindingMode bindingMode = BindingMode.OneWay)
        {
            Mode = bindingMode;
        }
    }
}
