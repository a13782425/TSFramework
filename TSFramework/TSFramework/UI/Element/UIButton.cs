using System;
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

        internal UIButton(UIView uIView, Button control) : base(uIView, control)
        {
            this.onPointerClick += tempOnClick; ;
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
        }
        private void tempOnClick(UnityEngine.EventSystems.PointerEventData obj)
        {
            OnClick?.Invoke();
            OnClickWithObj?.Invoke(gameObject);
        }

    }
}
