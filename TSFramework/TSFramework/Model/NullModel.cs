
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;

namespace TSFrame
{
    /// <summary>
    /// 空绑定Model
    /// </summary>
    public sealed class NullModel : IBindingModel
    {
        public void Dispose() { }

        public void Initlialize() { }
    }
}
