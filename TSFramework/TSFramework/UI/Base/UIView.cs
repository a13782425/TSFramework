﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;
using UnityEngine;

namespace TSFrame.UI
{

    public abstract class UIView : UIElement
    {
        /// <summary>
        /// UI路径
        /// </summary>
        public abstract string UIPath { get; }


        public virtual int DefaultSiblingIndex => 0;

        /// <summary>
        /// 设置UI层级
        /// </summary>
        public int SiblingIndex { get { return this.rectTransform.GetSiblingIndex(); } set { this.rectTransform.SetSiblingIndex(value); } }

        protected Dictionary<int, UIElement> UIElementDic { get => _uiElementDic; }

        private readonly Dictionary<int, UIElement> _uiElementDic;
        private Binding _bindingContext = null;

        public Binding BindingContext { get => _bindingContext; }

        protected UIView() : base()
        {
            _uiElementDic = new Dictionary<int, UIElement>();

            _bindingContext = new Binding(this);
        }

        #region public

        public void SetAsFirstSibling()
        {
            this.rectTransform.SetAsFirstSibling();
        }

        public void SetAsLastSibling()
        {
            this.rectTransform.SetAsLastSibling();
        }

        #endregion

        #region override


        /// <summary>
        /// 初始化组件
        /// </summary>
        protected virtual void InitializeElement()
        {

        }

        /// <summary>
        /// UI创建
        /// </summary>
        protected virtual void OnCreate()
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _bindingContext.UnbindAll();
            _bindingContext = null;
        }

        #endregion

        #region 内部函数

        /// <summary>
        /// 检测元素是否属于界面
        /// </summary>
        /// <returns></returns>
        internal bool CheckElementBelongView(IElement uIElement)
        {
            if (uIElement == null)
            {
                return false;
            }
            return _uiElementDic.ContainsKey(uIElement.InstanceId);
        }

        protected void BindingElement<T>(UnityEngine.EventSystems.UIBehaviour behaviour, out T element)
            where T : UIElement
        {
            element = UIUtil.New<T>(this, behaviour);
            this.UIElementDic.Add(element.InstanceId, element);
        }

        internal void RemoveElement(UIElement uIElement)
        {
            if (UIElementDic.ContainsKey(uIElement.InstanceId))
            {
                UIElementDic.Remove(uIElement.InstanceId);
            }
        }
        internal IBindingElement GetBindingElement(int instanceId)
        {
            if (UIElementDic.ContainsKey(instanceId))
            {
                return UIElementDic[instanceId] as IBindingElement;
            }
            return null;
        }

        private void Initialize(Transform parent)
        {
            GameObject obj = GameApp.Instance.ResourcesLoader.Load<GameObject>(UIPath);
            _gameObject = GameObject.Instantiate(obj, parent);
            _rectTransform = _gameObject.GetComponent<RectTransform>();
        }
        #endregion

        #region Static

        /// <summary>
        /// 创建一个View
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        public static T1 CreateView<T1>(Transform parent = null) where T1 : UIView, new()
        {
            T1 t = new T1();
            if (parent == null)
            {
                if (t is UIPanel uIPanel)
                {
                    parent = GameApp.Instance.UILoader.GetParent(uIPanel.LayerEnum);
                }
            }
            t.Initialize(parent);
            t.InitializeElement();
            t.OnCreate();
            if (t.DefaultSiblingIndex != 0)
            {
                t.SiblingIndex = t.DefaultSiblingIndex;
            }
            t.OnEnable();
            return t;
        }


        #endregion
    }
}
