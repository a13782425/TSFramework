using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame.MVVM
{
    public sealed class BindableProperty<T>
    {
        public ValueChangedHandler<T> OnValueChanged;

        private T _value;
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!Equals(_value, value))
                {
                    T old = _value;
                    _value = value;
                    ValueChanged(old, _value);
                }
            }
        }
        private void ValueChanged(T oldValue, T newValue)
        {
            OnValueChanged?.Invoke(oldValue, newValue);
        }
        //internal void OneWayToSource(UIControl uIInputm, Action<object> p)
        //{
        //    throw new NotImplementedException();
        //}



        //public void OneWayToSource(UIControl control)
        //{
        //    control.OneWayToSource((a) =>
        //    {
        //        object obj = a ?? default(T);

        //        Value = (T)Convert.ChangeType(obj, typeof(T)); ;
        //    });
        //}

        public void OneWay()
        {

        }
        public void OneTime()
        {

        }

        public void TwoWay()
        {

        }

        public override string ToString()
        {
            return (Value != null ? Value.ToString() : "null");
        }

        public void Freed()
        {
            OnValueChanged = null;
        }
    }
}
