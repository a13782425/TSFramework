using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TSFrame.UI;

namespace TSFrame.MVVM
{
    public abstract class ViewModelBase
    {
        //public ViewModelBase ParentViewModel { get; set; }

        //private bool _isFreed = false;

        //private List<BindableData> _bindableDataList;

        //public ViewModelBase()
        //{
        //    _isFreed = false;
        //    CacheBindableProperty();
        //    OnInitialize();
        //}
        //protected virtual void OnInitialize()
        //{

        //}

        //private void CacheBindableProperty()
        //{
        //    _bindableDataList = new List<BindableData>();
        //    Type ibindableType = typeof(IBindableProperty);
        //    FieldInfo[] fieldInfos = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
        //    foreach (var item in fieldInfos)
        //    {
        //        if (ibindableType.IsAssignableFrom(item.FieldType))
        //        {
        //            BindableData bindableData = new BindableData(item.GetValue(this) as IBindableProperty);
        //            _bindableDataList.Add(bindableData);
        //        }
        //    }
        //}


        //public void Binding<TField>(UIElement control, BindableProperty<TField> field, BindingMode bindingMode = BindingMode.OneWay)
        //{
        //    BindableData bindableData = null;
        //    foreach (var item in _bindableDataList)
        //    {
        //        if (item.BindableProperty == field)
        //        {
        //            bindableData = item;
        //            break;
        //        }
        //    }
        //    if (bindableData == null)
        //    {
        //        UnityEngine.Debug.LogError("只允许注册当前类的成员");
        //    }
        //    if (!bindableData.IsBind)
        //    {
        //        field.Binding(bindableData.OnValueChange);
        //        bindableData.IsBind = true;
        //    }
        //    switch (bindingMode)
        //    {
        //        case BindingMode.OneWay:
        //        case BindingMode.OnTime:
        //            bindableData.Binding(control, bindingMode);
        //            break;
        //        case BindingMode.OneWayToSource:
        //            control.Binding(field);
        //            break;
        //        case BindingMode.TwoWay:
        //            bindableData.Binding(control, bindingMode);
        //            control.Binding(field);
        //            break;
        //        default:
        //            throw new Exception($"{bindingMode.ToString()}绑定模式不支持！！！");
        //    }
        //}
        //public void Unbinding<TField>(UIElement control, BindableProperty<TField> field, BindingMode bindingMode)
        //{
        //    BindableData data = null;
        //    foreach (var item in _bindableDataList)
        //    {
        //        if (item.BindableProperty == field)
        //        {
        //            data = item;
        //            break;
        //        }
        //    }
        //    if (data != null)
        //    {
        //        data.Unbinding(control, bindingMode);
        //    }
        //}
        //public void Unbinding<TField>(UIElement control, BindableProperty<TField> field)
        //{
        //    BindableData data = null;
        //    foreach (var item in _bindableDataList)
        //    {
        //        if (item.BindableProperty == field)
        //        {
        //            data = item;
        //            break;
        //        }
        //    }
        //    if (data != null)
        //    {
        //        data.Unbinding(control);
        //    }
        //}
        //public void Freed()
        //{
        //    if (_isFreed)
        //    {
        //        return;
        //    }
        //    //清理所有字段
        //}

        //~ViewModelBase()
        //{
        //    Freed();
        //}


        //private class BindableData
        //{
        //    public IBindableProperty BindableProperty { get; private set; }
        //    public bool IsBind { get; set; }

        //    public Dictionary<BindingMode, List<UIElement>> UIControlDic { get; private set; }

        //    public BindableData(IBindableProperty bindableProperty)
        //    {
        //        if (bindableProperty == null)
        //        {
        //            UnityEngine.Debug.LogError("绑定变量为Null");
        //        }
        //        UIControlDic = new Dictionary<BindingMode, List<UIElement>>();
        //        UIControlDic.Add(BindingMode.OneWay, new List<UIElement>());
        //        UIControlDic.Add(BindingMode.OnTime, new List<UIElement>());
        //        UIControlDic.Add(BindingMode.OneWayToSource, new List<UIElement>());
        //        UIControlDic.Add(BindingMode.OnTime, new List<UIElement>());
        //        BindableProperty = bindableProperty;
        //        IsBind = false;
        //    }

        //    public void Binding(UIElement control, BindingMode bindingMode)
        //    {
        //        if (!UIControlDic[bindingMode].Contains(control))
        //        {
        //            UIControlDic[bindingMode].Add(control);
        //        }
        //    }

        //    public void Unbinding(UIElement control, BindingMode bindingMode)
        //    {
        //        if (UIControlDic[bindingMode].Contains(control))
        //        {
        //            UIControlDic[bindingMode].Remove(control);
        //        }

        //    }
        //    public void Unbinding(UIElement control)
        //    {
        //        foreach (KeyValuePair<BindingMode, List<UIElement>> item in UIControlDic)
        //        {
        //            if (item.Value.Contains(control))
        //            {
        //                item.Value.Remove(control);
        //            }
        //        }
        //    }
        //    public void OnValueChange<T>(T oldValue, T newValue)
        //    {
        //        List<UIElement> uIControls = UIControlDic[BindingMode.OneWay];
        //        foreach (var item in uIControls)
        //        {
        //            item.SetValue(newValue);
        //        }
        //        uIControls = UIControlDic[BindingMode.TwoWay];
        //        foreach (var item in uIControls)
        //        {
        //            item.SetValue(newValue);
        //        }
        //    }

        //}

    }
}
