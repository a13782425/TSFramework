using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.UI;

namespace TSFrame
{
    /// <summary>
    /// 自动注册
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class BindingAttribute : Attribute
    {
        public Type View { get; private set; }
        public BindingAttribute(Type panelType)
        {
            View = panelType;
        }
    }

}
