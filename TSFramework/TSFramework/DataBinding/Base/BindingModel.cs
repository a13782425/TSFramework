using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame.MVVM
{
    /// <summary>
    /// 绑定数据模型基类
    /// </summary>
    public abstract class BindingModel
    {
        /// <summary>
        /// 绑定数据前调用
        /// </summary>
        protected internal virtual void Initlialize() { }

        /// <summary>
        /// 取消全部绑定时候调用
        /// </summary>
        protected internal virtual void Dispose() { }

    }
}
