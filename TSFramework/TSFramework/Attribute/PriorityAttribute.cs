using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame
{
    /// <summary>
    /// 启动器，管理类启动顺序，从小到大,如果没有此标签则为0
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PriorityAttribute : Attribute
    {
        public PriorityAttribute(int priority)
        {
            Priority = priority;
        }

        public int Priority { get; set; }
    }
}
