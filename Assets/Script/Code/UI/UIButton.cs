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

        public UIButton(Button control) : base(control)
        {
            Element.onClick.AddListener(tempOnClick);
        }

        private onClick _onClick;

        public onClick OnClick { get => _onClick; set => _onClick = value; }

        private onClickWithObj _onClickWithObj;
        public onClickWithObj OnClickWithObj { get => _onClickWithObj; set => _onClickWithObj = value; }


        public override void Binding<TField>(BindableProperty<TField> property)
        {
            Debug.LogError("Button组件不支持反向绑定ViewModel");
        }

        public override void Freed()
        {
            Element.onClick.RemoveAllListeners();
        }


        private void tempOnClick()
        {
            OnClick?.Invoke();
            OnClickWithObj?.Invoke(gameObject);
        }


    }
}
