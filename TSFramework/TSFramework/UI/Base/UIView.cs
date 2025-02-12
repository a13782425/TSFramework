﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        /// <summary>
        /// 绑定上下文
        /// </summary>
        public Binding BindingContext { get => _bindingContext; }

        private Dictionary<int, Coroutine> _coroutineDic;

        /// <summary>
        /// 任务工厂
        /// </summary>
        private TaskFactory _taskFactory = null;
        private CancellationTokenSource _cancellationTokenSource = null;

        protected UIView() : base()
        {
            _uiElementDic = new Dictionary<int, UIElement>();
            _coroutineDic = new Dictionary<int, Coroutine>();
            _bindingContext = new Binding(this);
        }



        #region public

        /// <summary>
        /// 添加Item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddSubItem<T>() where T : UIItem, new()
        {
            return AddSubItem<T>(null);
        }

        /// <summary>
        /// 添加Item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">默认父亲</param>
        /// <returns></returns>
        public T AddSubItem<T>(Transform parent) where T : UIItem, new()
        {
            T t = UIView.CreateView<T>(parent);
            t.BelongView = this;
            BindingElement(t);
            return t;
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

        /// <summary>
        /// 销毁的时候调用
        /// </summary>
        protected override void OnDestroy()
        {

        }
        internal override void Internal_OnDestroy()
        {
            if (_taskFactory != null)
            {
                _cancellationTokenSource.Cancel();
            }
            List<int> keys = UIElementDic.Keys.ToList();
            foreach (var item in keys)
            {
                if (UIElementDic.ContainsKey(item))
                {
                    UIElementDic[item].Internal_OnDestroy();
                }
            }
            this.OnDestroy();
            UIElementDic.Clear();
            _bindingContext.UnbindAll();
            _bindingContext = null;
        }
        internal override void Internal_OnEnable()
        {
            foreach (var item in UIElementDic)
            {
                //处于激活状态才会相应事件
                if (item.Value.active)
                {
                    item.Value.Internal_OnEnable();
                }
            }
            this.OnEnable();
            this.BindingContext.active = true;
        }
        internal override void Internal_OnDisable()
        {
            this.BindingContext.active = false;
            foreach (var item in UIElementDic)
            {
                //处于激活状态才会相应事件
                if (item.Value.active)
                {
                    item.Value.Internal_OnDisable();
                }
            }
            this.OnDisable();
        }

        #endregion

        #region internal



        #endregion

        #region protected

        /// <summary>
        /// 启动一个任务
        /// </summary>
        protected void StartTask(Action action)
        {
            CheckTaskFactory();
            _taskFactory.StartNew(action, _cancellationTokenSource.Token);
        }
        /// <summary>
        /// 启动一个任务
        /// </summary>
        protected void StartTask(Action<object> action, object data)
        {
            CheckTaskFactory();
            _taskFactory.StartNew(action, data, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// 启动协程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            Coroutine coroutine = GameApp.Instance.StartCoroutine(routine);
            _coroutineDic.Add(coroutine.GetHashCode(), coroutine);
            return coroutine;
        }
        /// <summary>
        /// 停止协程
        /// </summary>
        /// <param name="coroutine"></param>
        public void StopCoroutine(Coroutine coroutine)
        {
            GameApp.Instance.StopCoroutine(coroutine);
            int hashCode = coroutine.GetHashCode();
            if (_coroutineDic.ContainsKey(hashCode))
            {
                _coroutineDic.Remove(hashCode);
            }
        }
        /// <summary>
        /// 停止协程
        /// </summary>
        public void StopAllCoroutines()
        {
            foreach (var item in _coroutineDic)
            {
                GameApp.Instance.StopCoroutine(item.Value);
            }
            _coroutineDic.Clear();
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
        /// <summary>
        /// 绑定组件到View
        /// </summary>
        /// <param name="behaviour"></param>
        /// <param name="element"></param>
        protected void BindingElement<T>(UnityEngine.EventSystems.UIBehaviour behaviour, out T element)
            where T : UIElement
        {
            element = UIUtil.New<T>(this, behaviour);
            this.UIElementDic.Add(element.InstanceId, element);
        }
        /// <summary>
        /// 绑定组件到View
        /// </summary>
        /// <param name="element"></param>
        internal void BindingElement(UIElement element)
        {
            this.UIElementDic.Add(element.InstanceId, element);
        }
        /// <summary>
        /// 删除一个组件
        /// </summary>
        /// <param name="uIElement"></param>
        internal void RemoveElement(UIElement uIElement)
        {
            if (UIElementDic.ContainsKey(uIElement.InstanceId))
            {
                UIElementDic.Remove(uIElement.InstanceId);
            }
        }
        /// <summary>
        /// 获取一个组件
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
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

        private void CheckTaskFactory()
        {
            if (_taskFactory == null)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _taskFactory = new TaskFactory(_cancellationTokenSource.Token);
            }
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
