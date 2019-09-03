using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame.UI
{
    /// <summary>
    /// UI层级
    /// </summary>
    [Flags]
    public enum UILayerEnum : int
    {
        /// <summary>
        /// 初始层
        /// </summary>
        None = 1 << 0,
        /// <summary>
        /// 底部层级
        /// </summary>
        Bottom = 1 << 1,
        /// <summary>
        /// Layer_2
        /// </summary>
        Layer_2 = 1 << 2,
        /// <summary>
        /// Layer_3
        /// </summary>
        Layer_3 = 1 << 3,
        /// <summary>
        /// 普通层级
        /// </summary>
        Normal = 1 << 4,
        /// <summary>
        /// Layer_5
        /// </summary>
        Layer_5 = 1 << 5,
        /// <summary>
        /// Layer_6
        /// </summary>
        Layer_6 = 1 << 6,
        /// <summary>
        /// Layer_7
        /// </summary>
        Layer_7 = 1 << 7,
        /// <summary>
        /// 固定层级
        /// </summary>
        Fixed = 1 << 8,
        /// <summary>
        /// Layer_9
        /// </summary>
        Layer_9 = 1 << 9,
        /// <summary>
        /// Layer_10
        /// </summary>
        Layer_10 = 1 << 10,
        /// <summary>
        /// Layer_11
        /// </summary>
        Layer_11 = 1 << 11,
        /// <summary>
        /// 提示层级
        /// </summary>
        Tooltip = 1 << 12,
        /// <summary>
        /// Layer_13
        /// </summary>
        Layer_13 = 1 << 13,
        /// <summary>
        /// Layer_14
        /// </summary>
        Layer_14 = 1 << 14,
        /// <summary>
        /// Layer_15
        /// </summary>
        Layer_15 = 1 << 15,
        /// <summary>
        /// 弹窗层级
        /// </summary>
        Dialog = 1 << 16,
        /// <summary>
        /// Layer_17
        /// </summary>
        Layer_17 = 1 << 17,
        /// <summary>
        /// Layer_18
        /// </summary>
        Layer_18 = 1 << 18,
        /// <summary>
        /// Layer_19
        /// </summary>
        Layer_19 = 1 << 19,
        /// <summary>
        /// 公告层级
        /// </summary>
        Notice = 1 << 20,
        /// <summary>
        /// Layer_21
        /// </summary>
        Layer_21 = 1 << 21,
        /// <summary>
        /// Layer_22
        /// </summary>
        Layer_22 = 1 << 22,
        /// <summary>
        /// Layer_23
        /// </summary>
        Layer_23 = 1 << 23,
        /// <summary>
        /// (最高层级)
        /// </summary>
        Max = 1 << 24,
    }
}
