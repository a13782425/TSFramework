﻿using System;
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

        private object _sourceData = null;
        /// <summary>
        /// 当前绑定的数据
        /// </summary>
        public object SourceData { get => _sourceData; set { _sourceData = value; SourceDataChange(value); } }

        /// <summary>
        /// 绑定数据对应绑定模式缓存
        /// </summary>
        private Dictionary<string, BindPropertyData> _bindPropertyDic = null;

        private Dictionary<IBindingElement, BindElementData> _bindElementDic = null;

        #endregion

        public Binding()
        {
            _bindPropertyDic = new Dictionary<string, BindPropertyData>();
            _bindElementDic = new Dictionary<IBindingElement, BindElementData>();
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
            if (!_bindElementDic.ContainsKey(element))
            {
                _bindElementDic.Add(element, new BindElementData(element));
            }
            BindElementData bindData = _bindElementDic[element];
            bindData.Add(fieldName, bindingMode);
        }

        private void BindProp(string fieldName, IBindingElement element, BindingMode bindingMode = BindingMode.OneWay)
        {
            if (!_bindPropertyDic.ContainsKey(fieldName))
            {
                _bindPropertyDic.Add(fieldName, new BindPropertyData(fieldName));
            }
            BindPropertyData bindData = _bindPropertyDic[fieldName];
            bindData.Add(element, bindingMode);
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

        private void UnbindProp(string fieldName, IBindingElement element, BindingMode bindingMode)
        {
            if (_bindPropertyDic.ContainsKey(fieldName))
            {
                _bindPropertyDic[fieldName].Unbind(element, bindingMode);
            }
        }

        private void UnbindElement(string fieldName, IBindingElement element, BindingMode bindingMode)
        {
            if (_bindElementDic.ContainsKey(element))
            {
                _bindElementDic[element].Unbind(fieldName, bindingMode);
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
            foreach (KeyValuePair<IBindingElement, BindElementData> item in _bindElementDic)
            {
                item.Value.UnbindAll();
            }
        }

        /// <summary>
        /// 源数据改变回调
        /// </summary>
        private void SourceDataChange(object data)
        {
            Type type = data.GetType();
            List<FieldInfo> fieldInfos = BindingCacheData.GetFieldInfos(type);
            foreach (var item in fieldInfos)
            {
                if (_bindPropertyDic.ContainsKey(item.Name))
                {
                    BindPropertyData bindData = _bindPropertyDic[item.Name];
                    object value = item.GetValue(data);
                    if (value is IBindableProperty bindableProperty)
                    {
                        bindData.Reset();
                        bindData.BindProp(bindableProperty);
                        foreach (KeyValuePair<IBindingElement, BindElementData> temp in _bindElementDic)
                        {
                            temp.Value.SetProp(bindableProperty);
                        }
                    }
                }
            }
            _sourceData = data;
            foreach (KeyValuePair<IBindingElement, BindElementData> temp in _bindElementDic)
            {
                temp.Value.Reset();
                temp.Value.BindElement();
            }
            foreach (KeyValuePair<string, BindPropertyData> item in _bindPropertyDic)
            {
                item.Value.SetValue(BindingMode.OnTime);
            }
        }

        private class BindPropertyData
        {
            internal string FieldName { get; private set; }
            internal Dictionary<BindingMode, List<IBindingElement>> _bindingElementDic = null;

            private IBindableProperty _bindableProperty = null;
            internal BindPropertyData(string fieldName)
            {
                FieldName = fieldName;
                _bindingElementDic = new Dictionary<BindingMode, List<IBindingElement>>();
                _bindingElementDic.Add(BindingMode.OneWay, new List<IBindingElement>());
                _bindingElementDic.Add(BindingMode.OnTime, new List<IBindingElement>());
                _bindingElementDic.Add(BindingMode.TwoWay, new List<IBindingElement>());
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
                return this._bindingElementDic[bindingMode].Remove(bindingElement);
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
            /// 重置绑定
            /// </summary>
            internal void Reset()
            {
                if (_bindableProperty != null)
                {
                    _bindableProperty.Unbind(onValueChanged);
                }
                _bindableProperty = null;
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
                _bindableProperty.Bind(onValueChanged);
            }


            private void onValueChanged(object o)
            {
                SetValue(BindingMode.OneWay, o);
                SetValue(BindingMode.TwoWay, o);
            }
            /// <summary>
            /// 设置Value
            /// </summary>
            /// <param name="bindingMode"></param>
            public void SetValue(BindingMode bindingMode)
            {
                SetValue(bindingMode, _bindableProperty.GetValue());
            }
            /// <summary>
            /// 设置Value
            /// </summary>
            /// <param name="bindingMode"></param>
            /// <param name="value"></param>
            public void SetValue(BindingMode bindingMode, object o)
            {
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
        }

        private class BindElementData
        {
            internal IBindingElement BindingElement { get; private set; }
            internal Dictionary<BindingMode, Dictionary<string, IBindableProperty>> _bindingProertyDic = null;
            internal BindElementData(IBindingElement bindingElement)
            {
                BindingElement = bindingElement;
                _bindingProertyDic = new Dictionary<BindingMode, Dictionary<string, IBindableProperty>>();
                _bindingProertyDic.Add(BindingMode.OneWayToSource, new Dictionary<string, IBindableProperty>());
                _bindingProertyDic.Add(BindingMode.TwoWay, new Dictionary<string, IBindableProperty>());
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
            internal void Unbind(string propName, BindingMode bindingMode)
            {
                Remove(propName, bindingMode);
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

            /// <summary>
            /// 绑定组件
            /// </summary>
            internal void BindElement()
            {
                BindingElement.ValueChanged += bindingElement_ValueChanged;
            }

            private void bindingElement_ValueChanged(object value)
            {
                SetValue(BindingMode.OneWayToSource, value);
                SetValue(BindingMode.TwoWay, value);
            }
            /// <summary>
            /// 设置Value
            /// </summary>
            /// <param name="bindingMode"></param>
            /// <param name="value"></param>
            private void SetValue(BindingMode bindingMode, object value)
            {
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
                BindingElement.ValueChanged -= bindingElement_ValueChanged;
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
        }
    }

}