using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TSFrame.UI;

namespace TSFrame.MVVM
{
    public sealed class Binding
    {
        #region variable

        private IBindingModel _sourceData = null;
        /// <summary>
        /// 当前绑定的数据
        /// </summary>
        public IBindingModel SourceData { get => _sourceData; set { SourceDataChange(value); } }

        /// <summary>
        /// 绑定数据对应绑定模式缓存
        /// </summary>
        private readonly Dictionary<string, BindPropertyData> _bindPropertyDic = null;

        //private Dictionary<IBindingElement, BindElementData> _bindElementDic = null;
        private readonly Dictionary<int, BindElementData> _bindElementDic = null;

        private readonly UIView _view = null;

        private bool _active = true;
        internal bool active
        {
            get => _active; set
            {
                bool b = _active;
                _active = value;
                if (b != value)
                {
                    if (value)
                    {
                        PushValue();
                    }
                }
            }
        }

        #endregion

        public Binding(UIView view)
        {
            if (view == null)
            {
                GameApp.Instance.LogError("Binding 必须依托于一个view");
                return;
            }
            _bindPropertyDic = new Dictionary<string, BindPropertyData>();
            _bindElementDic = new Dictionary<int, BindElementData>();
            _view = view;
            _active = true;
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="prop"></param>
        /// <param name="element"></param>
        /// <param name="bindingMode"></param>
        public void Bind<TField>(BindableProperty<TField> prop, IBindingElement element, BindingMode bindingMode = BindingMode.OneWay)
        {
            if (string.IsNullOrWhiteSpace(prop.name))
            {
                GameApp.Instance.LogError("BindableProperty name is null");
                return;
            }
            Bind(prop.name, element, bindingMode);
        }
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="element"></param>
        /// <param name="fieldName"></param>
        /// <param name="bindingMode"></param>
        public void Bind(IBindingElement element, string fieldName, BindingMode bindingMode = BindingMode.OneWay)
        {
            Bind(fieldName, element, bindingMode);
        }
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="element"></param>
        /// <param name="bindingMode"></param>
        public void Bind(string fieldName, IBindingElement element, BindingMode bindingMode = BindingMode.OneWay)
        {
            if (element == null)
            {
                GameApp.Instance.LogError("绑定的组件为null");
                return;
            }
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                GameApp.Instance.LogError("绑定的属性名称为空");
                return;
            }
            if (!_view.CheckElementBelongView(element))
            {
                GameApp.Instance.LogError("UI元素不是当前视图");
                return;
            }
            switch (bindingMode)
            {
                case BindingMode.OneWayToSource:
                    BindElement(fieldName, element, bindingMode);
                    break;
                case BindingMode.TwoWay:
                    BindProp(fieldName, element, bindingMode);
                    BindElement(fieldName, element, bindingMode);
                    break;
                case BindingMode.OneWay:
                case BindingMode.OnTime:
                default:
                    BindProp(fieldName, element, bindingMode);
                    break;
            }
        }

        private void BindElement(string fieldName, IBindingElement element, BindingMode bindingMode)
        {
            bool isNew = false;
            if (!_bindElementDic.ContainsKey(element.InstanceId))
            {
                _bindElementDic.Add(element.InstanceId, new BindElementData(this, element));
                isNew = true;
            }
            BindElementData bindData = _bindElementDic[element.InstanceId];
            bindData.Add(fieldName, bindingMode);
            if (isNew)
            {
                if (SourceData != null)
                {
                    Type type = SourceData.GetType();
                    List<FieldInfo> fieldInfos = BindingCacheData.GetFieldInfos(type);
                    foreach (var item in fieldInfos)
                    {
                        if (item.Name == fieldName)
                        {
                            object value = item.GetValue(SourceData);
                            if (value is IBindableProperty bindableProperty)
                            {
                                bindData.SetProp(bindableProperty);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void BindProp(string fieldName, IBindingElement element, BindingMode bindingMode = BindingMode.OneWay)
        {
            bool isNew = false;
            if (!_bindPropertyDic.ContainsKey(fieldName))
            {
                _bindPropertyDic.Add(fieldName, new BindPropertyData(this, fieldName));
                isNew = true;
            }
            BindPropertyData bindData = _bindPropertyDic[fieldName];
            bindData.Add(element, bindingMode);
            if (isNew)
            {
                if (SourceData != null)
                {
                    Type type = SourceData.GetType();
                    List<FieldInfo> fieldInfos = BindingCacheData.GetFieldInfos(type);
                    foreach (var item in fieldInfos)
                    {
                        if (item.Name == fieldName)
                        {
                            object value = item.GetValue(SourceData);
                            if (value is IBindableProperty bindableProperty)
                            {
                                bindData.Reset();
                                bindData.BindProp(bindableProperty);
                                break;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 取消绑定
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="element"></param>
        /// <param name="bindingMode"></param>
        public void Unbind(string fieldName, IBindingElement element, BindingMode bindingMode)
        {
            if (element == null)
            {
                GameApp.Instance.LogError("绑定的组件为null");
                return;
            }
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                GameApp.Instance.LogError("绑定的属性名称为空");
                return;
            }
            switch (bindingMode)
            {
                case BindingMode.OneWayToSource:
                    UnbindElement(fieldName, element, bindingMode);
                    break;
                case BindingMode.TwoWay:
                    UnbindProp(fieldName, element, bindingMode);
                    UnbindElement(fieldName, element, bindingMode);
                    break;
                case BindingMode.OneWay:
                case BindingMode.OnTime:
                default:
                    UnbindProp(fieldName, element, bindingMode);
                    break;
            }
        }
        public void Unbind(string fieldName, BindingMode bindingMode)
        {
            if (_bindPropertyDic.ContainsKey(fieldName))
            {
                _bindPropertyDic[fieldName].Unbind(bindingMode);
            }
            foreach (KeyValuePair<int, BindElementData> item in _bindElementDic)
            {
                item.Value.Unbind(fieldName, bindingMode);
            }
        }
        public void Unbind(IBindingElement element, BindingMode bindingMode)
        {
            if (_bindElementDic.ContainsKey(element.InstanceId))
            {
                _bindElementDic[element.InstanceId].Unbind(bindingMode);
            }
            foreach (KeyValuePair<string, BindPropertyData> item in _bindPropertyDic)
            {
                item.Value.Unbind(element, bindingMode);
            }
        }

        private void UnbindProp(string fieldName, IBindingElement element, BindingMode bindingMode)
        {
            if (_bindPropertyDic.ContainsKey(fieldName))
            {
                _bindPropertyDic[fieldName].Unbind(element, bindingMode);
            }
        }

        private void UnbindElement(string fieldName, IBindingElement element, BindingMode bindingMode)
        {
            if (_bindElementDic.ContainsKey(element.InstanceId))
            {
                _bindElementDic[element.InstanceId].Unbind(fieldName, bindingMode);
            }
        }
        /// <summary>
        /// 取消全部绑定
        /// </summary>
        public void UnbindAll()
        {
            foreach (KeyValuePair<string, BindPropertyData> item in _bindPropertyDic)
            {
                item.Value.UnbindAll();
            }
            foreach (KeyValuePair<int, BindElementData> item in _bindElementDic)
            {
                item.Value.UnbindAll();
            }
            if (this.SourceData != null)
            {
                this.SourceData.Dispose();
            }
        }

        /// <summary>
        /// 源数据改变回调
        /// </summary>
        private void SourceDataChange(IBindingModel data)
        {
            if (data == null)
            {
                GameApp.Instance.LogError("绑定的数据为空！");
                return;
            }
            Type type = data.GetType();
            List<FieldInfo> fieldInfos = BindingCacheData.GetFieldInfos(type);
            foreach (var item in fieldInfos)
            {
                object value = item.GetValue(data);
                if (value is IBindableProperty bindableProperty)
                {
                    if (_bindPropertyDic.ContainsKey(item.Name))
                    {
                        BindPropertyData bindData = _bindPropertyDic[item.Name];
                        bindData.Reset();
                        bindData.BindProp(bindableProperty);
                    }
                    foreach (KeyValuePair<int, BindElementData> temp in _bindElementDic)
                    {
                        temp.Value.SetProp(bindableProperty);
                    }
                }
            }
            _sourceData = data;
            foreach (KeyValuePair<string, BindPropertyData> item in _bindPropertyDic)
            {
                item.Value.SetValue(BindingMode.OnTime);
                item.Value.SetValue(BindingMode.OneWay);
            }
        }

        /// <summary>
        /// 数据源从隐藏到显示推数据
        /// </summary>
        private void PushValue()
        {
            foreach (KeyValuePair<string, BindPropertyData> item in _bindPropertyDic)
            {
                item.Value.SetValue(BindingMode.OneWay);
                item.Value.SetValue(BindingMode.TwoWay);
            }
            foreach (KeyValuePair<int, BindElementData> item in _bindElementDic)
            {
                item.Value.SetLastValue();
            }
        }

        private class BindPropertyData
        {
            internal string FieldName { get; private set; }
            internal Dictionary<BindingMode, List<IBindingElement>> _bindingElementDic = null;
            private Binding _currentBinding = null;
            private IBindableProperty _bindableProperty = null;
            internal BindPropertyData(Binding binding, string fieldName)
            {
                _currentBinding = binding;
                FieldName = fieldName;
                _bindingElementDic = new Dictionary<BindingMode, List<IBindingElement>>
                {
                    { BindingMode.OneWay, new List<IBindingElement>() },
                    { BindingMode.OnTime, new List<IBindingElement>() },
                    { BindingMode.TwoWay, new List<IBindingElement>() }
                };
            }
            internal void Add(IBindingElement bindingElement, BindingMode bindingMode)
            {
                this._bindingElementDic[bindingMode].Add(bindingElement);
            }
            internal bool Contains(IBindingElement bindingElement, BindingMode bindingMode)
            {
                return this._bindingElementDic[bindingMode].Contains(bindingElement);
            }
            internal bool Remove(IBindingElement bindingElement, BindingMode bindingMode)
            {
                if (this._bindingElementDic.ContainsKey(bindingMode))
                {
                    return this._bindingElementDic[bindingMode].Remove(bindingElement);
                }
                return false;
            }

            public override string ToString()
            {
                return $"{FieldName}:{_bindingElementDic.Count}";
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }
                if (obj is BindPropertyData bind)
                {
                    return bind.FieldName == this.FieldName;

                }
                return false;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            internal void BindProp(IBindableProperty value)
            {
                _bindableProperty = value;
                _bindableProperty.Subscribe(OnValueChanged);
            }

            private void OnValueChanged(object oldValue, object newValue)
            {
                SetValue(BindingMode.OneWay, newValue);
                SetValue(BindingMode.TwoWay, newValue);
            }
            /// <summary>
            /// 设置Value
            /// </summary>
            /// <param name="bindingMode"></param>
            public void SetValue(BindingMode bindingMode)
            {
                try
                {
                    SetValue(bindingMode, _bindableProperty.GetValue());
                }
                catch (Exception ex)
                {
                    if (_currentBinding.SourceData == null)
                    {
                        GameApp.Instance.LogError($"UIView :{_currentBinding._view.name} 中数据源为空！！！");
                    }
                    else
                    {
                        GameApp.Instance.LogError(ex.Message);
                        GameApp.Instance.LogError($"猜测错误：{_currentBinding.SourceData.GetType().Name} 中绑定数据字段：{FieldName} 不存在！");
                    }
                }
            }
            /// <summary>
            /// 设置Value
            /// </summary>
            /// <param name="bindingMode"></param>
            /// <param name="value"></param>
            public void SetValue(BindingMode bindingMode, object o)
            {
                if (!_currentBinding.active)
                {
                    return;
                }
                List<IBindingElement> bindingElements = _bindingElementDic[bindingMode];
                if (bindingElements.Count > 0)
                {
                    foreach (var item in bindingElements)
                    {
                        item.SetValue(o);
                    }
                }
            }
            /// <summary>
            /// 重置绑定
            /// </summary>
            internal void Reset()
            {
                if (_bindableProperty != null)
                {
                    _bindableProperty.Unsubscribe(OnValueChanged);
                }
                _bindableProperty = null;
            }
            /// <summary>
            /// 取消绑定
            /// </summary>
            /// <param name="element"></param>
            /// <param name="bindingMode"></param>
            internal void Unbind(IBindingElement element, BindingMode bindingMode)
            {
                Remove(element, bindingMode);
            }
            /// <summary>
            /// 解绑全部
            /// </summary>
            internal void UnbindAll()
            {
                foreach (KeyValuePair<BindingMode, List<IBindingElement>> item in _bindingElementDic)
                {
                    item.Value.Clear();
                }
                Reset();
            }

            internal void Unbind(BindingMode bindingMode)
            {
                if (_bindingElementDic.ContainsKey(bindingMode))
                {
                    _bindingElementDic[bindingMode].Clear();
                }
            }
        }

        private class BindElementData
        {
            internal IBindingElement BindingElement { get; private set; }
            private Binding _currentBinding = null;
            private object _lastValue = null;
            private bool _isChange = false;
            internal Dictionary<BindingMode, Dictionary<string, IBindableProperty>> _bindingProertyDic = null;
            internal BindElementData(Binding binding, IBindingElement bindingElement)
            {
                _currentBinding = binding;
                BindingElement = bindingElement;
                _bindingProertyDic = new Dictionary<BindingMode, Dictionary<string, IBindableProperty>>
                {
                    { BindingMode.OneWayToSource, new Dictionary<string, IBindableProperty>() },
                    { BindingMode.TwoWay, new Dictionary<string, IBindableProperty>() }
                };
                BindingElement.ValueChanged += BindingElement_ValueChanged;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }
                if (obj is BindElementData bind)
                {
                    return bind.BindingElement == this.BindingElement;

                }
                return false;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            internal void Add(string propName, BindingMode bindingMode)
            {
                if (this._bindingProertyDic[bindingMode].ContainsKey(propName))
                {
                    GameApp.Instance.LogError($"重复添加Key:{propName}!");
                    return;
                }
                this._bindingProertyDic[bindingMode].Add(propName, null);
            }
            internal bool Contains(string propName, BindingMode bindingMode)
            {
                return this._bindingProertyDic[bindingMode].ContainsKey(propName);
            }
            internal bool Remove(string propName, BindingMode bindingMode)
            {
                if (this._bindingProertyDic[bindingMode].ContainsKey(propName))
                {
                    return this._bindingProertyDic[bindingMode].Remove(propName);
                }
                return false;
            }

            /// <summary>
            /// 设置属性
            /// </summary>
            /// <param name="bindableProperty"></param>
            internal void SetProp(IBindableProperty bindableProperty)
            {
                string propName = bindableProperty.name;
                if (string.IsNullOrWhiteSpace(propName))
                {
                    GameApp.Instance.LogError($"属性名字为空!");
                    return;
                }
                foreach (KeyValuePair<BindingMode, Dictionary<string, IBindableProperty>> item in _bindingProertyDic)
                {
                    if (item.Value.ContainsKey(propName))
                    {
                        item.Value[propName] = bindableProperty;
                    }
                }
            }

            private void BindingElement_ValueChanged(object value)
            {
                SetValue(BindingMode.OneWayToSource, value);
                SetValue(BindingMode.TwoWay, value);
            }
            /// <summary>
            /// 内部设定最后一次Value
            /// </summary>
            public void SetLastValue()
            {
                if (_isChange)
                {
                    _isChange = false;
                    SetValue(BindingMode.OneWayToSource, _lastValue);
                    SetValue(BindingMode.TwoWay, _lastValue);
                }
            }
            /// <summary>
            /// 设置Value
            /// </summary>
            /// <param name="bindingMode"></param>
            /// <param name="value"></param>
            private void SetValue(BindingMode bindingMode, object value)
            {
                if (!_currentBinding.active)
                {
                    _lastValue = value;
                    _isChange = true;
                    return;
                }
                Dictionary<string, IBindableProperty> dic = _bindingProertyDic[bindingMode];
                if (dic.Count > 0)
                {
                    foreach (KeyValuePair<string, IBindableProperty> item in dic)
                    {
                        item.Value.SetValue(value);
                    }
                }
            }
            /// <summary>
            /// 重置
            /// </summary>
            internal void Reset()
            {
                BindingElement.ValueChanged -= BindingElement_ValueChanged;
            }
            internal void Unbind(string propName, BindingMode bindingMode)
            {
                Remove(propName, bindingMode);
            }
            /// <summary>
            /// 解绑全部
            /// </summary>
            internal void UnbindAll()
            {
                foreach (KeyValuePair<BindingMode, Dictionary<string, IBindableProperty>> item in _bindingProertyDic)
                {
                    item.Value.Clear();
                }
                Reset();
            }

            internal void Unbind(BindingMode bindingMode)
            {
                if (_bindingProertyDic.ContainsKey(bindingMode))
                {
                    _bindingProertyDic[bindingMode].Clear();
                }
            }
        }
    }

}
