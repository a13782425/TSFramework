using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TSFrame.UI
{
    /// <summary>
    /// 组件基类(非泛型)
    /// </summary>
    public abstract class UIElement : IElement
    {
        #region prop

        private int _instanceId = 0;
        /// <summary>
        /// 实例ID
        /// </summary>
        public int InstanceId { get => _instanceId; }

        /// <summary>
        /// 元素名
        /// </summary>
        public virtual string name
        {
            get { return gameObject.name; }
            set { gameObject.name = value; }
        }

        private protected GameObject _gameObject = null;
        /// <summary>
        /// 游戏物体
        /// </summary>
        public GameObject gameObject => _gameObject;
        /// <summary>
        /// 游戏物体的Transform
        /// </summary>
        public Transform transform => _gameObject.transform;

        private protected RectTransform _rectTransform = null;
        /// <summary>
        /// 游戏物体的RectTransform
        /// </summary>
        public RectTransform rectTransform => _rectTransform;

        private UIElement _parent = null;
        /// <summary>
        /// 父亲
        /// </summary>
        public UIElement parent { get => _parent; set { _parent = value; this.transform.SetParent(_parent); } }
        private protected UIView _uiView = null;
        /// <summary>
        /// 属于哪个视图
        /// </summary>
        public UIView UIView => _uiView;
        /// <summary>
        /// 隐藏标识符
        /// </summary>
        public HideFlags hideFlags { get => gameObject.hideFlags; set => gameObject.hideFlags = value; }

        /// <summary>
        /// 是否处于激活状态
        /// </summary>
        public bool active
        {
            get => gameObject.activeInHierarchy;
            set
            {
                bool b = active;
                gameObject.SetActive(value);
                if (b != value)
                {
                    if (value)
                    {
                        Internal_OnEnable();
                        //OnEnable();
                    }
                    else
                    {
                        Internal_OnDisable();
                        //OnDisable();
                    }
                }
            }
        }

        #endregion

        protected UIElement()
        {
            _instanceId = FrameTool.GetInstanceId();
            _eventDic = new Dictionary<EventTriggerType, UIEventBase>();
        }

        protected UIElement(GameObject obj)
        {
            _instanceId = FrameTool.GetInstanceId();
            _eventDic = new Dictionary<EventTriggerType, UIEventBase>();
            _gameObject = obj;
            if (_gameObject != null)
            {
                _rectTransform = gameObject.GetComponent<RectTransform>();
                if (active)
                {
                    OnEnable();
                }
            }

        }

        #region static
        /// <summary>
        /// 删除Object
        /// </summary>
        /// <param name="obj"></param>
        public static void Destroy(UIElement obj)
        {
            obj.active = false;
            obj.Internal_OnDestroy();
            UnityEngine.Object.Destroy(obj.gameObject);
            obj._gameObject = null;
            FrameTool.RecoverInstanceId(obj.InstanceId);
            obj = null;
        }

        /// <summary>
        /// 删除Object
        /// </summary>
        /// <param name="obj"></param>
        public static void DestroyImmediate(UIElement obj)
        {
            obj.OnDestroy();
            UnityEngine.Object.DestroyImmediate(obj.gameObject);
            obj._gameObject = null;
            FrameTool.RecoverInstanceId(obj.InstanceId);
            obj = null;
        }
        #endregion

        #region public
        /// <summary>
        /// 加载场景时候不删除
        /// </summary>
        public void DontDestroyOnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
        }
        /// <summary>
        /// 关闭这个组件
        /// </summary>
        public void Close()
        {
            Destroy(this);
        }

        #endregion

        #region virtual

        protected virtual void OnEnable()
        {

        }
        protected virtual void OnDisable()
        {

        }
        protected virtual void OnDestroy()
        {

        }

        internal virtual void Internal_OnDestroy()
        {
            this.OnDestroy();
        }
        /// <summary>
        /// 内部显示调用方法
        /// </summary>
        internal virtual void Internal_OnEnable()
        {
            this.OnEnable();
        }
        /// <summary>
        /// 内部隐藏调用方法
        /// </summary>
        internal virtual void Internal_OnDisable()
        {
            this.OnDisable();
        }

        #endregion

        #region override

        public override int GetHashCode()
        {
            return this.InstanceId;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        #endregion

        #region event
        /// <summary>
        /// 事件字典
        /// </summary>
        private Dictionary<EventTriggerType, UIEventBase> _eventDic = null;

        

        /// <summary>
        /// 拖动开始事件
        /// </summary>
        public event Action<PointerEventData> onBeginDrag { add { AddEvent(EventTriggerType.BeginDrag, value); } remove { RemoveEvent(EventTriggerType.BeginDrag, value); } }
        /// <summary>
        /// 拖动中事件
        /// </summary>
        public event Action<PointerEventData> onDrag { add { AddEvent(EventTriggerType.Drag, value); } remove { RemoveEvent(EventTriggerType.Drag, value); } }
        /// <summary>
        /// 拖动结束事件
        /// </summary>
        public event Action<PointerEventData> onEndDrag { add { AddEvent(EventTriggerType.EndDrag, value); } remove { RemoveEvent(EventTriggerType.EndDrag, value); } }
        /// <summary>
        /// 指针进入事件
        /// </summary>
        public event Action<PointerEventData> onPointerEnter { add { AddEvent(EventTriggerType.PointerEnter, value); } remove { RemoveEvent(EventTriggerType.PointerEnter, value); } }
        /// <summary>
        /// 指针离开事件
        /// </summary>
        public event Action<PointerEventData> onPointerExit { add { AddEvent(EventTriggerType.PointerExit, value); } remove { RemoveEvent(EventTriggerType.PointerExit, value); } }
        /// <summary>
        /// 指针按下事件
        /// </summary>
        public event Action<PointerEventData> onPointDown { add { AddEvent(EventTriggerType.PointerDown, value); } remove { RemoveEvent(EventTriggerType.PointerDown, value); } }
        /// <summary>
        /// 接收拖动事件
        /// *接受实现IDragHandler接口的组件拖动到本组件上面松开时触发
        /// </summary>
        public event Action<PointerEventData> onDrop { add { AddEvent(EventTriggerType.Drop, value); } remove { RemoveEvent(EventTriggerType.Drop, value); } }
        /// <summary>
        /// 指针抬起事件
        /// </summary>
        public event Action<PointerEventData> onPointerUp { add { AddEvent(EventTriggerType.PointerUp, value); } remove { RemoveEvent(EventTriggerType.PointerUp, value); } }
        /// <summary>
        /// 指针点击事件
        /// 在组件可视的区域按下且抬起时指针处于区域内(按下离开区域后抬起不会触发)
        /// </summary>
        public event Action<PointerEventData> onPointerClick { add { AddEvent(EventTriggerType.PointerClick, value); } remove { RemoveEvent(EventTriggerType.PointerClick, value); } }
        /// <summary>
        /// 初始化潜在的拖动事件
        /// *(必须实现IDragHandler接口)与IPointerDownHandler事件触发条件大致相同
        /// </summary>
        public event Action<PointerEventData> onInitializePotentialDrag { add { AddEvent(EventTriggerType.InitializePotentialDrag, value); } remove { RemoveEvent(EventTriggerType.InitializePotentialDrag, value); } }
        /// <summary>
        /// 滚动事件
        /// *滑轮在上面滚动时触发
        /// </summary>
        public event Action<PointerEventData> onScroll { add { AddEvent(EventTriggerType.Scroll, value); } remove { RemoveEvent(EventTriggerType.Scroll, value); } }
        /// <summary>
        /// 选中物体每帧触发事件
        /// </summary>
        public event Action<BaseEventData> onUpdateSelected { add { AddEvent(EventTriggerType.UpdateSelected, value); } remove { RemoveEvent(EventTriggerType.UpdateSelected, value); } }
        /// <summary>
        /// 选择事件
        /// </summary>
        public event Action<BaseEventData> onSelect { add { AddEvent(EventTriggerType.Select, value); } remove { RemoveEvent(EventTriggerType.Select, value); } }
        /// <summary>
        /// 取消选择事件
        /// </summary>
        public event Action<BaseEventData> onDeselect { add { AddEvent(EventTriggerType.Deselect, value); } remove { RemoveEvent(EventTriggerType.Deselect, value); } }
        /// <summary>
        /// 提交事件
        /// *按下InputManager里的Submit对应的按键(PC、Mac默认:Enter键)。Input.GetButtonDown
        /// </summary>
        public event Action<BaseEventData> onSubmit { add { AddEvent(EventTriggerType.Submit, value); } remove { RemoveEvent(EventTriggerType.Submit, value); } }
        /// <summary>
        /// 取消事件
        ///  *按下InputManager里的Cancel对应的按键(PC、Mac默认:Esc键)。Input.GetButtonDown
        /// </summary>
        public event Action<BaseEventData> onCancel { add { AddEvent(EventTriggerType.Cancel, value); } remove { RemoveEvent(EventTriggerType.Cancel, value); } }
        /// <summary>
        /// 移动事件(上下左右)
        /// *与InputManager里的Horizontal和Vertical按键相对应。Input.GetAxisRaw
        /// </summary>
        public event Action<AxisEventData> onMove { add { AddEvent(EventTriggerType.Move, value); } remove { RemoveEvent(EventTriggerType.Move, value); } }

        private void AddEvent(EventTriggerType triggerType, Action<PointerEventData> value)
        {
            if (_eventDic.ContainsKey(triggerType))
            {
                _eventDic[triggerType].pointerCallback += value;
            }
            else
            {
                UIEventBase uIEventBase = UIUtil.GetUIEventBase(this.gameObject, triggerType);
                if (uIEventBase == null)
                {
                    throw new Exception($"{triggerType}的事件类型没有找到！");
                }
                uIEventBase.pointerCallback += value;
            }
        }
        private void AddEvent(EventTriggerType triggerType, Action<BaseEventData> value)
        {
            if (_eventDic.ContainsKey(triggerType))
            {
                _eventDic[triggerType].baseCallback += value;
            }
            else
            {
                UIEventBase uIEventBase = UIUtil.GetUIEventBase(this.gameObject, triggerType);
                if (uIEventBase == null)
                {
                    throw new Exception($"{triggerType}的事件类型没有找到！");
                }
                uIEventBase.baseCallback += value;
            }
        }
        private void AddEvent(EventTriggerType triggerType, Action<AxisEventData> value)
        {
            if (_eventDic.ContainsKey(triggerType))
            {
                _eventDic[triggerType].axisCallback += value;
            }
            else
            {
                UIEventBase uIEventBase = UIUtil.GetUIEventBase(this.gameObject, triggerType);
                if (uIEventBase == null)
                {
                    throw new Exception($"{triggerType}的事件类型没有找到！");
                }
                uIEventBase.axisCallback += value;
            }
        }

        private void RemoveEvent(EventTriggerType triggerType, Action<PointerEventData> value)
        {
            if (_eventDic.ContainsKey(triggerType))
            {
                _eventDic[triggerType].pointerCallback -= value;
            }
        }
        private void RemoveEvent(EventTriggerType triggerType, Action<BaseEventData> value)
        {
            if (_eventDic.ContainsKey(triggerType))
            {
                _eventDic[triggerType].baseCallback -= value;
            }
        }
        private void RemoveEvent(EventTriggerType triggerType, Action<AxisEventData> value)
        {
            if (_eventDic.ContainsKey(triggerType))
            {
                _eventDic[triggerType].axisCallback -= value;
            }
        }

        #endregion

        #region operator

        public static implicit operator GameObject(UIElement element)
        {
            return element.gameObject;
        }
        public static implicit operator Transform(UIElement element)
        {
            return element.transform;
        }

        #endregion
    }
    /// <summary>
    /// 泛型组件基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UIElement<T> : UIElement where T : UIBehaviour
    {
        private T _element = null;
        /// <summary>
        /// Unity组件
        /// </summary>
        public T Element => _element;

        internal UIElement(UIView uIView, T element) : base(element.gameObject)
        {
            if (element == null)
            {
                throw new Exception($"空间初始化为空：{typeof(T).GetType().FullName}");
            }
            _uiView = uIView;
            _element = element;
        }

        internal override void Internal_OnDestroy()
        {
            if (this is IBindingElement bindingElement)
            {
                if (this.UIView != null)
                {
                    this.UIView.RemoveElement(this);
                    if (this.UIView.BindingContext != null)
                    {
                        this.UIView.BindingContext.Unbind(bindingElement, BindingMode.OneWayToSource);
                        this.UIView.BindingContext.Unbind(bindingElement, BindingMode.TwoWay);
                    }
                }
            }
            this.OnDestroy();
        }
    }

}
