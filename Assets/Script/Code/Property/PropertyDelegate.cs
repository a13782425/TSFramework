using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame.MVVM
{
    public delegate void ValueChangedHandler<T>(T oldValue, T newValue);
}
