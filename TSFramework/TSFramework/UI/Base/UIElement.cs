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

        private Transform _parent = null;
        /// <summary>
        /// 父亲
        /// </summary>
        public Transform parent { get => _parent; set { _parent = value; this.transform.SetParent(_parent); } }
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

        public UIElement(GameObject obj)
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
            UnityEngine.Object.Destroy(obj.gameObject);
            UIUtil.RecoverInstanceId(obj.InstanceId);
            obj.OnDestroy();
            obj = null;
        }

        /// <summary>
        /// 删除Object
        /// </summary>
        /// <param name="obj"></param>
        public static void DestroyImmediate(UIElement obj)
        {
            UnityEngine.Object.DestroyImmediate(obj.gameObject);
            UIUtil.RecoverInstanceId(obj.InstanceId);
            obj.OnDestroy();
            obj = null;
        }
        #endregion

        #region public

        public void DontDestroyOnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
        }

        #endregion

        #region virtual

        protected virtual void OnEnable()
        {

        }
        protected virtual void Update()
        {

        }
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

        public UIElement(T element) : base(element.gameObject)
        {
            if (element == null)
            {
                throw new Exception($"空间初始化为空：{typeof(T).GetType().FullName}");
            }
            _element = element;
            _rectTransform = _element.GetComponent<RectTransform>();
        }

    }

}
