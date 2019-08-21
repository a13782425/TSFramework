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
        void Subscribe(Action<object, object> action);
        void Unsubscribe(Action<object, object> action);
        object GetValue();
        void SetValue(object value);
        void Freed();
    }
}
