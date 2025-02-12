﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame.MVVM
{
    /// <summary>
    /// 绑定数据接口
    /// </summary>
    internal interface IBindingModel
    {
        /// <summary>
        /// 绑定数据前调用
        /// </summary>
        void Initlialize();

        /// <summary>
        /// 取消全部绑定时候调用
        /// </summary>
        void Dispose();
    }
}
