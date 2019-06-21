using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TSFrame.UI
{

    public abstract class UIElement
    {
        private int _instanceId = 0;
        public int InstanceId { get => _instanceId; }

        protected Func<bool> checkBinding;
        protected Action<object> _onValueChanged;
        /// <summary>
        /// 元素名
        /// </summary>
        public virtual string name { get; set; }
        public UIElement()
        {
            _instanceId = UIUtil.GetInstanceId();
        }

        public virtual void SetValue(object value)
        {

        }
        public virtual void Binding<TField>(BindableProperty<TField> property)
        {

        }
        public virtual void Unbinding<TField>(BindableProperty<TField> property)
        {

        }
    }

    public abstract partial class UIElement<T> : UIElement where T : UIBehaviour
    {
        private T _element = null;

        public T Element => _element;

        //protected List<BindableData> _bindableDatasList;

        protected static Type[] _supportType;
        public UIElement(T element)
        {
            if (element == null)
            {
                throw new Exception($"空间初始化为空：{typeof(T).GetType().FullName}");
            }
            _element = element;
            //_bindableDatasList = new List<BindableData>();
            _rectTransform = _element.GetComponent<RectTransform>();
        }
        public override string name
        {
            get { return Element?.name; }
            set { Element.name = value; }
        }
        public HideFlags hideFlags { get => gameObject.hideFlags; set => gameObject.hideFlags = value; }
        public bool IsActive { get => gameObject.activeInHierarchy; set => gameObject.SetActive(value); }

        public GameObject gameObject => Element.gameObject;
        public Transform transform => Element.transform;

        private RectTransform _rectTransform = null;
        public RectTransform rectTransform => _rectTransform;

        ///// <summary>
        ///// 绑定字段，组件变化带动组件变化
        ///// </summary>
        ///// <typeparam name="TField"></typeparam>
        ///// <param name="property"></param>
        //public override void Binding<TField>(BindableProperty<TField> property)
        //{
        //    Type type = typeof(TField);
        //    if (_supportType == null)
        //    {
        //        _bindableDatasList.Add(new BindableData(type, property));
        //        return;
        //    }
        //    Type supportType = null;
        //    foreach (var item in _supportType)
        //    {
        //        if (type == item)
        //        {
        //            supportType = item;
        //            break;
        //        }
        //    }
        //    if (supportType == null)
        //    {
        //        UnityEngine.Debug.LogError($"暂不支持{type.Name}的绑定，请联系管理员");
        //        return;
        //    }
        //    _bindableDatasList.Add(new BindableData(type, property));
        //}
        //public override void Unbinding<TField>(BindableProperty<TField> property)
        //{
        //    BindableData data = null;
        //    foreach (var item in _bindableDatasList)
        //    {
        //        if (item.BindableProperty == property)
        //        {
        //            data = item;
        //            break;
        //        }
        //    }
        //    if (data != null)
        //    {
        //        _bindableDatasList.Remove(data);
        //    }
        //}

        //public void UnbindingAll()
        //{
        //    _bindableDatasList.Clear();
        //}

        public virtual void Freed()
        {
            //_bindableDatasList.Clear();
        }
    }

    public partial class UIElement<T>
    {
       
    }
}
