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

    public abstract class UIElement : IElement
    {
        #region prop

        private int _instanceId = 0;
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
        public GameObject gameObject => _gameObject;
        public Transform transform => _gameObject.transform;

        private protected RectTransform _rectTransform = null;
        public RectTransform rectTransform => _rectTransform;

        private UIElement _parent = null;
        /// <summary>
        /// 父亲
        /// </summary>
        public UIElement parent { get => _parent; set { _parent = value; this.transform.SetParent(_parent); } }
        private protected UIView _uiView = null;
        public UIView UIView => _uiView;
        //private List<UIElement> _childs = null;
        //protected List<UIElement> Childs => _childs;
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
            _instanceId = UIUtil.GetInstanceId();
        }

        protected UIElement(GameObject obj)
        {
            _instanceId = UIUtil.GetInstanceId();
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
            obj.OnDisable();
            obj.OnDestroy();
            UnityEngine.Object.Destroy(obj.gameObject);
            obj._gameObject = null;
            UIUtil.RecoverInstanceId(obj.InstanceId);
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
            UIUtil.RecoverInstanceId(obj.InstanceId);
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
        /// <summary>
        /// 内部显示调用方法
        /// </summary>
        internal virtual void Internal_OnEnable()
        {

        }
        /// <summary>
        /// 内部隐藏调用方法
        /// </summary>
        internal virtual void Internal_OnDisable()
        {

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

    public abstract class UIElement<T> : UIElement where T : UIBehaviour
    {
        private T _element = null;

        public T Element => _element;

        private UIMono _currentMono = null;

        private bool _enableDrag = false;
        /// <summary>
        /// 是否可以拖拽
        /// </summary>
        public bool EnableDrag { get { return _enableDrag; } set { _enableDrag = value; _currentMono.enableDrag = value; } }
        /// <summary>
        /// 开始拖拽
        /// </summary>
        public event Action<PointerEventData> onBeginDrag { add { _currentMono.onBeginDrag += value; } remove { _currentMono.onBeginDrag -= value; } }
        /// <summary>
        /// 拖拽中
        /// </summary>
        public event Action<PointerEventData> onDrag { add { _currentMono.onDrag += value; } remove { _currentMono.onDrag -= value; } }
        /// <summary>
        /// 停止拖拽
        /// </summary>
        public event Action<PointerEventData> onEndDrag { add { _currentMono.onEndDrag += value; } remove { _currentMono.onBeginDrag -= value; } }


        private bool _enableEnter = false;
        /// <summary>
        /// 是否启用进入退出
        /// </summary>
        public bool EnableEnter { get { return _enableEnter; } set { _enableEnter = value; _currentMono.enableEnter = value; } }
        /// <summary>
        /// 光标进入
        /// </summary>
        public event Action<PointerEventData> onEnter { add { _currentMono.onEndDrag += value; } remove { _currentMono.onBeginDrag -= value; } }
        /// <summary>
        /// 光标退出
        /// </summary>
        public event Action<PointerEventData> onExit { add { _currentMono.onEndDrag += value; } remove { _currentMono.onBeginDrag -= value; } }

        internal UIElement(UIView uIView, T element) : base(element.gameObject)
        {
            if (element == null)
            {
                throw new Exception($"空间初始化为空：{typeof(T).GetType().FullName}");
            }
            _uiView = uIView;
            _element = element;
            _currentMono = this.gameObject.AddComponent<UIMono>();
            _currentMono.Init(this);
        }
    }

    class UIMono : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private UIElement _uIElement = null;

        internal bool enableDrag = false;
        internal bool enableEnter = false;
        internal event Action<PointerEventData> onBeginDrag;
        internal event Action<PointerEventData> onDrag;
        internal event Action<PointerEventData> onEndDrag;
        internal event Action<PointerEventData> onEnter;
        internal event Action<PointerEventData> onExit;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!enableDrag)
                return;
            onBeginDrag?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!enableDrag)
                return;
            onDrag?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!enableDrag)
                return;
            onEndDrag?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!enableEnter)
                return;
            onEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!enableEnter)
                return;
            onExit?.Invoke(eventData);
        }

        internal void Init<T>(UIElement<T> uIElement) where T : UIBehaviour
        {
            _uIElement = uIElement;
        }

        void OnDestroy()
        {
            if (_uIElement is IBindingElement bindingElement)
            {
                if (_uIElement.UIView != null)
                {
                    _uIElement.UIView.RemoveElement(_uIElement);
                    if (_uIElement.UIView.BindingContext != null)
                    {
                        _uIElement.UIView.BindingContext.Unbind(bindingElement, BindingMode.OneWayToSource);
                        _uIElement.UIView.BindingContext.Unbind(bindingElement, BindingMode.TwoWay);
                    }
                }
            }
        }
    }
}
