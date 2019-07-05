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
            Element.onClick.AddListener(tempOnClick);
        }

        private onClick _onClick;

        public onClick OnClick { get => _onClick; set => _onClick = value; }

        private onClickWithObj _onClickWithObj;
        public onClickWithObj OnClickWithObj { get => _onClickWithObj; set => _onClickWithObj = value; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Element.onClick.RemoveAllListeners();
        }

        private void tempOnClick()
        {
            OnClick?.Invoke();
            OnClickWithObj?.Invoke(gameObject);
        }


    }
}
