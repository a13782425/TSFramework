using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace TSFrame.UI
{
    public sealed class UIButton : UIElement<Button>
    {
        public delegate void onClick();
        public delegate void onClickWithObj(GameObject obj);
        /// <summary>
        /// 双击检测协程
        /// </summary>
        private Coroutine _doubleClickCoroutine = null;

        private bool _isFirstClick = false;

        /// <summary>
        /// 双击间隔
        /// </summary>
        public float DoubleClickInterval = 0.1f;

        private float _tempTime = 0;

        internal UIButton(UIView uIView, Button control) : base(uIView, control)
        {
            this.onPointerClick += tempOnClick; ;
        }



        private event Action<GameObject> _onDoubleClick = null;
        /// <summary>
        /// 双击事件
        /// </summary>
        public event Action<GameObject> onDoubleClick { add { AddDoubleClick(value); } remove { RemoveDoubleClick(value); } }
        /// <summary>
        /// 双击事件
        /// </summary>
        public event Action<GameObject> onRightClick;
        private void AddDoubleClick(Action<GameObject> value)
        {
            _onDoubleClick += value;
            if (_doubleClickCoroutine == null)
            {
                _doubleClickCoroutine = GameApp.Instance.StartCoroutine(DoubleClickCheck());
            }
        }

        private IEnumerator DoubleClickCheck()
        {
            while (true)
            {
                if (_isFirstClick)
                {
                    _tempTime += GameApp.Instance.deltaTime;
                    if (_tempTime >= DoubleClickInterval)
                    {
                        OnClick?.Invoke();
                        OnClickWithObj?.Invoke(gameObject);
                        _isFirstClick = false;
                        _tempTime = 0;
                    }
                }
                yield return null;
            }
        }

        private void RemoveDoubleClick(Action<GameObject> value)
        {
            _onDoubleClick -= value;
            if (_onDoubleClick == null && _doubleClickCoroutine != null)
            {
                GameApp.Instance.StopCoroutine(_doubleClickCoroutine);
                _doubleClickCoroutine = null;
            }
        }
        /// <summary>
        /// 点击事件
        /// </summary>
        public event onClick OnClick = null;
        /// <summary>
        /// 带参的点击事件
        /// </summary>
        public event onClickWithObj OnClickWithObj = null;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Element.onClick.RemoveAllListeners();
            OnClickWithObj = null;
            OnClick = null;
            if (_doubleClickCoroutine != null)
            {
                GameApp.Instance.StopCoroutine(_doubleClickCoroutine);
                _doubleClickCoroutine = null;
            }
        }
        private void tempOnClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (eventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Right)
            {
                onRightClick?.Invoke(gameObject);
            }
            else if (eventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left)
            {
                if (_doubleClickCoroutine == null)
                {
                    OnClick?.Invoke();
                    OnClickWithObj?.Invoke(gameObject);
                }
                else
                {
                    //存在双击
                    if (_isFirstClick)
                    {
                        _onDoubleClick?.Invoke(gameObject);
                        _isFirstClick = false;
                        _tempTime = 0;
                    }
                    else
                    {
                        _isFirstClick = true;
                        _tempTime = 0;
                    }
                }
            }
        }

    }
}
