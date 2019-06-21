using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame.MVVM
{
    internal interface IBindableProperty
    {
        string name { get; }
        void Bind(Action<object> valueChanged);
        void Unbind(Action<object> valueChanged);
        object GetValue();
        void SetValue(object value);
        void Freed();
    }
}
