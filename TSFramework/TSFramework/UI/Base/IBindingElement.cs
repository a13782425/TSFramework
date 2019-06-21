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

    public interface IBindingElement
    {
        //
        // 摘要:
        //     Occurs when a property value changes.
        event ValueChangedEvent ValueChanged;

        void SetValue(object value);
    }
}
