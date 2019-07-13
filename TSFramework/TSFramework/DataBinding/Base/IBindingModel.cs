using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame.MVVM
{
    /// <summary>
    /// 绑定数据接口
    /// </summary>
    public interface IBindingModel
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Init();
        /// <summary>
        /// 释放
        /// </summary>
        void Free();
    }
}
