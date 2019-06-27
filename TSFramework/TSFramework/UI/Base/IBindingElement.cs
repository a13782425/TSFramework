using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame.UI
{
    public delegate void ValueChangedEvent(object value);

    public interface IElement
    {
        /// <summary>
        /// 实例ID
        /// </summary>
        int InstanceId { get; }
    }

    public interface IBindingElement : IElement
    {
        /// <summary>
        /// 自己变化调用其他
        /// </summary>
        event ValueChangedEvent ValueChanged;
        /// <summary>
        /// 别人变化调用自身
        /// </summary>
        /// <param name="value"></param>
        void SetValue(object value);

        ///// <summary>
        ///// 检测绑定条件
        ///// </summary>
        ///// <returns></returns>
        //bool DetectBindCondition();
    }
}
