using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame.MVVM
{
    /// <summary>
    /// 绑定类型
    /// </summary>
    [Flags]
    public enum BindingMode
    {
        /// <summary>
        /// Model变更新UI
        /// </summary>
        OneWay = 1,
        /// <summary>
        /// Model更新UI
        /// 绑定时候更新一次
        /// </summary>
        OnTime = 2,
        /// <summary>
        /// UI更新Model
        /// </summary>
        OneWayToSource = 4,
        /// <summary>
        /// UI和Model互相更新(慎用，容易变成递归操作)
        /// </summary>
        TwoWay = 8,
    }
}
