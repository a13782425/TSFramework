using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.UI;

namespace TSFrame.MVVM
{
    /// <summary>
    /// 绑定数据类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class BindableProperty<T> : IBindableProperty
    {
        private Action<T, T> _onValueChanged;

        private Action<object, object> _onObjValueChanged;

        private bool _isEnable = true;
        /// <summary>
        /// 是否启动绑定数据
        /// </summary>
        public bool IsEnable { get => _isEnable; set => _isEnable = value; }
        private bool _isForce = false;
        /// <summary>
        /// 强行刷新
        /// </summary>
        public bool IsForce { get => _isForce; set => _isForce = value; }

        private T _value;
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (IsForce || !Equals(_value, value))
                {
                    T old = _value;
                    _value = value;
                    if (IsEnable)
                    {
                        ValueChanged(old, _value);
                    }
                }
            }
        }

        public string name { get; private set; }

        public BindableProperty()
        : this(null)
        {
        }

        public BindableProperty(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return (Value != null ? Value.ToString() : "null");
        }


        public void Freed()
        {
            _onValueChanged = null;
            _onObjValueChanged = null;
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="action">Action(先前的值，现在的值)</param>
        /// <returns></returns>
        public BindableProperty<T> Subscribe(Action<T, T> action)
        {
            _onValueChanged += action;
            return this;
        }
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="action">Action(先前的值，现在的值)</param>
        /// <returns></returns>
        void IBindableProperty.Subscribe(Action<object, object> valueChanged)
        {
            _onObjValueChanged += valueChanged;
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="action">Action(先前的值，现在的值)</param>
        /// <returns></returns>
        public BindableProperty<T> Unsubscribe(Action<T, T> action)
        {
            _onValueChanged -= action;
            return this;
        }
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="action">Action(先前的值，现在的值)</param>
        /// <returns></returns>
        void IBindableProperty.Unsubscribe(Action<object, object> valueChanged)
        {
            _onObjValueChanged -= valueChanged;
        }
        void IBindableProperty.SetValue(object value)
        {
            try
            {
                object obj = ChangeType(value, typeof(T));
                this.Value = (T)obj;
            }
            catch (Exception ex)
            {
                if (string.IsNullOrWhiteSpace(value.ToString()))
                {
                    this.Value = default;
                }

                GameApp.Instance.LogError(ex.Message);
            }
        }

        object IBindableProperty.GetValue()
        {
            return Value;
        }
        #region private

        private void ValueChanged(T oldValue, T newValue)
        {
            _onObjValueChanged?.Invoke(oldValue, newValue);
            _onValueChanged?.Invoke(oldValue, newValue);
        }

        #endregion

        #region operator

        public static implicit operator T(BindableProperty<T> prop)
        {
            return prop.Value;
        }

        #endregion

        private static object ChangeType(object value, Type type)
        {
            if (value == null) return null;
            else if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            else if (type == value.GetType()) return value;
            else if (type.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(type, value as string);
                else
                    return Enum.ToObject(type, value);
            }
            else if (!type.IsInterface && type.IsGenericType)
            {
                Type innerType = type.GetGenericArguments()[0];
                object innerValue = ChangeType(value, type);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }
            else if (value is string && type == typeof(Guid)) return new Guid(value as string);
            else if (value is string && type == typeof(Version)) return new Version(value as string);
            else if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type);
        }
    }
}
