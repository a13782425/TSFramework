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

        public bool active
        {
            get => gameObject.activeInHierarchy;
            set
            {
                bool b = active;
                gameObject.SetActive(value);
                if (b != value)
                {
                    if (b)
                    {
                        OnEnable();
                    }
                    else
                    {
                        OnDisable();
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

        public void DontDestroyOnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
        }
        public void Close()
        {
            Destroy(this);
        }

        #endregion

        #region virtual

        protected virtual void OnEnable()
        {

        }
        //protected virtual void Update()
        //{

        //}
        protected virtual void OnDisable()
        {

        }
        protected virtual void OnDestroy()
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



        internal UIElement(UIView uIView, T element) : base(element.gameObject)
        {
            if (element == null)
            {
                throw new Exception($"空间初始化为空：{typeof(T).GetType().FullName}");
            }
            _uiView = uIView;
            _element = element;
            UIMono uIMono = this.gameObject.AddComponent<UIMono>();
            uIMono.Init(this);
        }
    }
    class UIMono : MonoBehaviour
    {
        private UIElement _uIElement = null;
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
